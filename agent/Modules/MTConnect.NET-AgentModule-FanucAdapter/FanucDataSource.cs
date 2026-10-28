// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Input;
using MTConnect.Logging;
using MTConnect.Observations;
using MTConnect.Observations.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace MTConnect.Fanuc
{
    /// <summary>
    /// Reads a Fanuc CNC using FOCAS and reports the results as observations. The structure of the
    /// CNC (paths, axes, spindles) is read when connecting. Values that change often are read every
    /// cycle; timers and the work offset are read every <see cref="FanucConfiguration.SlowReadInterval"/>.
    /// </summary>
    public class FanucDataSource : MTConnectDataSource
    {
        private const short WorkOffsetVariable = 4014; // G code group 14 (G54 - G59)

        private readonly IFocasClient _client;
        private readonly FanucConfiguration _configuration;
        private readonly HashSet<string> _unsupported = new HashSet<string>();
        private readonly Dictionary<int, HashSet<string>> _activeAlarms = new Dictionary<int, HashSet<string>>();
        private FanucMachineModel _machineModel;
        private Stopwatch _slowReadTimer;
        private bool _connectFailed;
        private bool? _available;


        /// <summary>
        /// Gets the structure read from the CNC, or <c>null</c> if it has not been read yet.
        /// </summary>
        public FanucMachineModel MachineModel => _machineModel;

        /// <summary>
        /// Raised when the structure read from the CNC (model, paths, axes, spindles) changes.
        /// </summary>
        public event EventHandler<FanucMachineModel> MachineModelChanged;


        /// <summary>
        /// Initializes a new data source.
        /// </summary>
        /// <param name="client">The FOCAS client used to read the CNC.</param>
        /// <param name="configuration">The CNC settings.</param>
        public FanucDataSource(IFocasClient client, FanucConfiguration configuration)
        {
            _client = client;
            _configuration = configuration ?? new FanucConfiguration();

            // Every value is read each poll; only changes need to go through the agent pipeline
            SuppressUnchangedObservations = true;
        }


        /// <inheritdoc/>
        protected override void OnStop()
        {
            _client.Disconnect();
            SetUnavailable();
        }

        /// <inheritdoc/>
        protected override void OnRead()
        {
            // Run the whole poll on the FOCAS thread: each FOCAS call then executes directly instead of
            // being handed to that thread (and waited on) one call at a time
            if (_client is FocasClient focasClient) focasClient.RunOnFocasThread(ReadAll);
            else ReadAll();
        }

        private void ReadAll()
        {
            try
            {
                if (!_client.IsConnected && !Connect())
                {
                    SetUnavailable();
                    return;
                }

                if (_available != true)
                {
                    Discover();
                    Log(MTConnectLogLevel.Information, $"Fanuc CNC Available : {_machineModel.Model} {_machineModel.Firmware}".Trim());
                    _available = true;
                    _slowReadTimer = null;
                }

                AddObservation(FanucDataItemKeys.Availability, Availability.AVAILABLE);

                var readSlow = _slowReadTimer == null || _slowReadTimer.ElapsedMilliseconds >= _configuration.SlowReadInterval;

                foreach (var path in _machineModel.Paths)
                {
                    SelectPath(path.Number);
                    ReadPath(path, readSlow);
                }

                if (readSlow)
                {
                    SelectPath(1);
                    ReadSlow();
                    _slowReadTimer = Stopwatch.StartNew();
                }
            }
            catch (FocasConnectionException ex)
            {
                Log(MTConnectLogLevel.Warning, $"Connection Lost : {ex.Message}");
                _client.Disconnect();
                SetUnavailable();
            }
        }


        private bool Connect()
        {
            var code = _client.Connect(_configuration.Server, _configuration.Port, _configuration.TimeoutSeconds);
            if (code != FocasReturnCodes.EW_OK)
            {
                // Only log the first failure so an offline machine doesn't flood the log
                Log(_connectFailed ? MTConnectLogLevel.Debug : MTConnectLogLevel.Warning, $"Connection to {_configuration.Server}:{_configuration.Port} Failed : FOCAS return code {code}");
                _connectFailed = true;
                return false;
            }

            Log(MTConnectLogLevel.Debug, $"Connected to {_configuration.Server}:{_configuration.Port}");
            _connectFailed = false;
            _unsupported.Clear();
            _available = null;
            return true;
        }

        private void Discover()
        {
            var model = new FanucMachineModel();

            FocasSystemInfo info;
            if (Call("cnc_sysinfo", _client.ReadSystemInfo(out info)))
            {
                model.Model = info.Model;
                model.Firmware = info.Firmware;
            }

            short currentPath, maxPath;
            var pathCount = Call("cnc_getpath", _client.GetPath(out currentPath, out maxPath)) && maxPath > 1 ? maxPath : 1;

            var axisNames = new HashSet<string>();
            var spindleNames = new HashSet<string>();
            for (var number = 1; number <= pathCount; number++)
            {
                if (pathCount > 1) Call("cnc_setpath", _client.SetPath((short)number));

                var path = new FanucPathModel { Number = number };

                string[] names;
                FocasPosition[] positions;
                if (Call("cnc_rdaxisname", _client.ReadAxisNames(out names)) && names != null)
                {
                    Call("cnc_rdposition", _client.ReadPositions(out positions));
                    for (var i = 0; i < names.Length; i++)
                    {
                        // Axis names must be unique across paths (multi-path controls normally use suffixes, ex. X1, X2)
                        var name = names[i];
                        if (!axisNames.Add(name)) axisNames.Add(name = $"{name}{number}");

                        var isRotary = positions != null && i < positions.Length && positions[i].Unit == 2;
                        path.Axes.Add(new FanucAxisModel { Name = name, IsRotary = isRotary });
                    }
                }

                string[] spindles;
                var spindle = Call("cnc_rdspdlname", _client.ReadSpindleNames(out spindles)) && spindles != null && spindles.Length > 0 ? spindles[0] : "S";
                if (!spindleNames.Add(spindle)) spindleNames.Add(spindle = $"{spindle}{number}");
                path.Spindle = spindle;

                model.Paths.Add(path);
            }

            var changed = _machineModel == null || _machineModel.GetSignature() != model.GetSignature();
            _machineModel = model;

            if (changed && MachineModelChanged != null) MachineModelChanged.Invoke(this, model);
        }

        private void SelectPath(int path)
        {
            if (_machineModel.Paths.Count > 1) Call("cnc_setpath", _client.SetPath((short)path));
        }

        private void ReadPath(FanucPathModel path, bool readSlow)
        {
            var n = path.Number;

            FocasStatus status;
            if (Call("cnc_statinfo", _client.ReadStatus(out status)))
            {
                var mode = FanucMapping.MapControllerMode(status.Mode);
                if (mode.HasValue) AddObservation(FanucDataItemKeys.ForPath(FanucDataItemKeys.ControllerMode, n), mode.Value);

                var execution = FanucMapping.MapExecution(status.Run, status.Emergency);
                if (execution.HasValue) AddObservation(FanucDataItemKeys.ForPath(FanucDataItemKeys.Execution, n), execution.Value);

                if (n == 1) AddObservation(FanucDataItemKeys.EmergencyStop, FanucMapping.MapEmergencyStop(status.Emergency));

                ReadAlarms(n, status.Alarm);
            }

            ReadProgram(n);

            int sequenceNumber;
            if (IsSupported("cnc_rdseqnum") && Call("cnc_rdseqnum", _client.ReadSequenceNumber(out sequenceNumber)))
            {
                AddObservation(FanucDataItemKeys.ForPath(FanucDataItemKeys.Line, n), sequenceNumber);
            }

            if (_configuration.PartCountParameter > 0)
            {
                int partCount;
                var function = $"cnc_rdparam {_configuration.PartCountParameter}";
                if (IsSupported(function) && Call(function, _client.ReadParameter((short)_configuration.PartCountParameter, out partCount)))
                {
                    AddObservation(FanucDataItemKeys.ForPath(FanucDataItemKeys.PartCount, n), partCount);
                }
            }

            ReadSpeed(n);
            ReadPositions(path);
            ReadLoads(path);

            if (_configuration.Overrides) ReadOverrides(n);

            if (_configuration.ToolNumberVariable > 0)
            {
                var tool = ReadMacro((short)_configuration.ToolNumberVariable);
                if (tool.HasValue) AddObservation(FanucDataItemKeys.ForPath(FanucDataItemKeys.ToolNumber, n), (long)tool.Value);
            }

            if (readSlow && _configuration.WorkOffset)
            {
                var workOffset = ReadMacro(WorkOffsetVariable);
                if (workOffset.HasValue) AddObservation(FanucDataItemKeys.ForPath(FanucDataItemKeys.WorkOffset, n), FormatNumber(workOffset.Value));
            }
        }

        private void ReadProgram(int path)
        {
            string name;
            if (IsSupported("cnc_exeprgname") && Call("cnc_exeprgname", _client.ReadProgramName(out name)))
            {
                AddObservation(FanucDataItemKeys.ForPath(FanucDataItemKeys.Program, path), name);
                return;
            }

            int running, main;
            if (IsSupported("cnc_rdprgnum") && Call("cnc_rdprgnum", _client.ReadProgramNumber(out running, out main)))
            {
                AddObservation(FanucDataItemKeys.ForPath(FanucDataItemKeys.Program, path), running > 0 ? $"O{running:0000}" : "");
            }
        }

        private void ReadSpeed(int path)
        {
            FocasSpeed speed;
            if (IsSupported("cnc_rdspeed") && Call("cnc_rdspeed", _client.ReadSpeed(out speed)))
            {
                var feedrate = FanucMapping.ToMillimetersPerSecond(speed.Feedrate, speed.FeedrateUnit, speed.SpindleSpeed);
                if (feedrate.HasValue) AddObservation(FanucDataItemKeys.ForPath(FanucDataItemKeys.Feedrate, path), Math.Round(feedrate.Value, 4));

                AddObservation(FanucDataItemKeys.ForPath(FanucDataItemKeys.SpindleSpeed, path), speed.SpindleSpeed);
                return;
            }

            int rpm;
            if (IsSupported("cnc_acts") && Call("cnc_acts", _client.ReadSpindleSpeed(out rpm)))
            {
                AddObservation(FanucDataItemKeys.ForPath(FanucDataItemKeys.SpindleSpeed, path), rpm);
            }
        }

        private void ReadPositions(FanucPathModel path)
        {
            if (path.Axes.Count == 0) return;

            FocasPosition[] positions;
            if (Call("cnc_rdposition", _client.ReadPositions(out positions)) && positions != null)
            {
                for (var i = 0; i < path.Axes.Count && i < positions.Length; i++)
                {
                    var axis = path.Axes[i];
                    var position = positions[i];

                    AddObservation(FanucDataItemKeys.GetPositionKey(axis.Name), Math.Round(FanucMapping.ToMillimeters(position.Machine, position.Unit), 4));
                    if (_configuration.WorkPositions)
                    {
                        AddObservation(FanucDataItemKeys.GetWorkPositionKey(axis.Name), Math.Round(FanucMapping.ToMillimeters(position.Absolute, position.Unit), 4));
                    }
                }
            }
        }

        private void ReadLoads(FanucPathModel path)
        {
            FocasLoad[] loads;
            if (_configuration.AxisLoads && path.Axes.Count > 0 && IsSupported("cnc_rdsvmeter") && Call("cnc_rdsvmeter", _client.ReadServoLoads(out loads)) && loads != null)
            {
                for (var i = 0; i < path.Axes.Count && i < loads.Length; i++)
                {
                    AddObservation(FanucDataItemKeys.GetLoadKey(path.Axes[i].Name), Math.Round(loads[i].Value, 1));
                }
            }

            if (_configuration.SpindleLoad && IsSupported("cnc_rdspmeter") && Call("cnc_rdspmeter", _client.ReadSpindleLoads(out loads)) && loads != null && loads.Length > 0)
            {
                AddObservation(FanucDataItemKeys.ForPath(FanucDataItemKeys.SpindleLoad, path.Number), Math.Round(loads[0].Value, 1));
            }
        }

        private void ReadOverrides(int path)
        {
            var start = FanucMapping.GetPathAddress(FanucMapping.OverrideStartAddress, path);
            var end = FanucMapping.GetPathAddress(FanucMapping.OverrideEndAddress, path);

            byte[] data;
            if (IsSupported("pmc_rdpmcrng") && Call("pmc_rdpmcrng", _client.ReadPmc(0, start, end, out data)) && data != null && data.Length >= 85)
            {
                // data[0] = G0012, data[2] = G0014, data[18] = G0030, data[84] = G0096 (offset by 1000 per path)
                AddObservation(FanucDataItemKeys.ForPath(FanucDataItemKeys.FeedrateOverride, path), FanucMapping.GetFeedrateOverride(data[0]));
                AddObservation(FanucDataItemKeys.ForPath(FanucDataItemKeys.RapidOverride, path), FanucMapping.GetRapidOverride(data[2], data[84]));
                AddObservation(FanucDataItemKeys.ForPath(FanucDataItemKeys.SpindleOverride, path), FanucMapping.GetSpindleOverride(data[18]));
            }
        }

        private void ReadAlarms(int path, short alarmStatus)
        {
            var alarms = new List<ConditionFaultStateObservationInput>();
            var key = FanucDataItemKeys.ForPath(FanucDataItemKeys.System, path);

            if (alarmStatus == 1)
            {
                FocasAlarm[] focasAlarms;
                if (IsSupported("cnc_rdalmmsg2") && Call("cnc_rdalmmsg2", _client.ReadAlarms(out focasAlarms)) && focasAlarms != null && focasAlarms.Length > 0)
                {
                    foreach (var alarm in focasAlarms)
                    {
                        var code = FanucMapping.GetAlarmCode(alarm);
                        alarms.Add(CreateCondition(key, ConditionLevel.FAULT, code, alarm.Message));
                    }
                }
                else
                {
                    alarms.Add(CreateCondition(key, ConditionLevel.FAULT, "ALARM", "ALARM"));
                }
            }
            else
            {
                var warning = FanucMapping.GetWarningMessage(alarmStatus);
                if (warning != null) alarms.Add(CreateCondition(key, ConditionLevel.WARNING, $"W{alarmStatus}", warning));
            }

            UpdateConditions(path, key, alarms);
        }

        private void UpdateConditions(int path, string key, List<ConditionFaultStateObservationInput> conditions)
        {
            HashSet<string> previous;
            _activeAlarms.TryGetValue(path, out previous);

            var current = new HashSet<string>(conditions.Select(o => o.ConditionId));

            if (current.Count == 0)
            {
                // Clear every active condition
                if (previous == null || previous.Count > 0)
                {
                    AddObservation(new ConditionFaultStateObservationInput(key, ConditionLevel.NORMAL));
                }
            }
            else
            {
                if (previous != null)
                {
                    foreach (var removed in previous.Where(o => !current.Contains(o)))
                    {
                        var normal = new ConditionFaultStateObservationInput(key, ConditionLevel.NORMAL);
                        normal.ConditionId = removed;
                        AddObservation(normal);
                    }
                }

                foreach (var condition in conditions)
                {
                    if (previous == null || !previous.Contains(condition.ConditionId)) AddObservation(condition);
                }
            }

            _activeAlarms[path] = current;
        }

        private static ConditionFaultStateObservationInput CreateCondition(string key, ConditionLevel level, string code, string message)
        {
            var condition = new ConditionFaultStateObservationInput(key, level);
            condition.ConditionId = code;
            condition.NativeCode = code;
            condition.Message = message;
            return condition;
        }

        private void ReadSlow()
        {
            if (!string.IsNullOrEmpty(_machineModel.Firmware)) AddObservation(FanucDataItemKeys.Firmware, _machineModel.Firmware);

            ReadTimer(0, FanucDataItemKeys.PowerOnTime);
            ReadTimer(1, FanucDataItemKeys.OperatingTime);
            ReadTimer(2, FanucDataItemKeys.CuttingTime);
            ReadTimer(3, FanucDataItemKeys.CycleTime);
        }

        private void ReadTimer(short type, string key)
        {
            double seconds;
            var function = $"cnc_rdtimer {type}";
            if (IsSupported(function) && Call(function, _client.ReadTimer(type, out seconds)))
            {
                AddObservation(key, Math.Round(seconds, 3));
            }
        }

        private double? ReadMacro(short variable)
        {
            double? value;
            var function = $"cnc_rdmacro {variable}";
            if (IsSupported(function) && Call(function, _client.ReadMacro(variable, out value))) return value;
            return null;
        }


        private bool IsSupported(string function) => !_unsupported.Contains(function);

        private bool Call(string function, short code)
        {
            if (code == FocasReturnCodes.EW_OK) return true;

            if (FocasReturnCodes.IsConnectionError(code)) throw new FocasConnectionException($"{function} returned {code}");

            if (FocasReturnCodes.IsUnsupported(code))
            {
                if (_unsupported.Add(function)) Log(MTConnectLogLevel.Information, $"{function} is not supported by the CNC (return code {code}) and will not be read");
            }
            else
            {
                Log(MTConnectLogLevel.Debug, $"{function} returned {code}");
            }

            return false;
        }

        private static string FormatNumber(double value)
        {
            return value == Math.Floor(value)
                ? ((long)value).ToString(CultureInfo.InvariantCulture)
                : value.ToString(CultureInfo.InvariantCulture);
        }

        private void SetUnavailable()
        {
            if (_available == false) return;

            if (_available == true) Log(MTConnectLogLevel.Warning, "Fanuc CNC Unavailable");
            _available = false;
            _activeAlarms.Clear();

            var model = _machineModel ?? FanucMachineModel.CreateDefault();

            AddObservation(FanucDataItemKeys.Availability, Availability.UNAVAILABLE);
            foreach (var key in GetValueKeys(model, _configuration)) AddObservation(key, Observation.Unavailable);
            foreach (var path in model.Paths)
            {
                AddObservation(new ConditionFaultStateObservationInput(FanucDataItemKeys.ForPath(FanucDataItemKeys.System, path.Number), ConditionLevel.UNAVAILABLE));
            }
        }

        internal static IEnumerable<string> GetValueKeys(FanucMachineModel model, FanucConfiguration configuration)
        {
            var keys = new List<string>
            {
                FanucDataItemKeys.EmergencyStop,
                FanucDataItemKeys.Firmware,
                FanucDataItemKeys.PowerOnTime,
                FanucDataItemKeys.OperatingTime,
                FanucDataItemKeys.CuttingTime,
                FanucDataItemKeys.CycleTime
            };

            foreach (var path in model.Paths)
            {
                var n = path.Number;
                keys.Add(FanucDataItemKeys.ForPath(FanucDataItemKeys.ControllerMode, n));
                keys.Add(FanucDataItemKeys.ForPath(FanucDataItemKeys.Execution, n));
                keys.Add(FanucDataItemKeys.ForPath(FanucDataItemKeys.Program, n));
                keys.Add(FanucDataItemKeys.ForPath(FanucDataItemKeys.Line, n));
                keys.Add(FanucDataItemKeys.ForPath(FanucDataItemKeys.Feedrate, n));
                keys.Add(FanucDataItemKeys.ForPath(FanucDataItemKeys.SpindleSpeed, n));
                if (configuration.PartCountParameter > 0) keys.Add(FanucDataItemKeys.ForPath(FanucDataItemKeys.PartCount, n));
                if (configuration.ToolNumberVariable > 0) keys.Add(FanucDataItemKeys.ForPath(FanucDataItemKeys.ToolNumber, n));
                if (configuration.WorkOffset) keys.Add(FanucDataItemKeys.ForPath(FanucDataItemKeys.WorkOffset, n));
                if (configuration.SpindleLoad) keys.Add(FanucDataItemKeys.ForPath(FanucDataItemKeys.SpindleLoad, n));
                if (configuration.Overrides)
                {
                    keys.Add(FanucDataItemKeys.ForPath(FanucDataItemKeys.FeedrateOverride, n));
                    keys.Add(FanucDataItemKeys.ForPath(FanucDataItemKeys.RapidOverride, n));
                    keys.Add(FanucDataItemKeys.ForPath(FanucDataItemKeys.SpindleOverride, n));
                }

                foreach (var axis in path.Axes)
                {
                    keys.Add(FanucDataItemKeys.GetPositionKey(axis.Name));
                    if (configuration.WorkPositions) keys.Add(FanucDataItemKeys.GetWorkPositionKey(axis.Name));
                    if (configuration.AxisLoads) keys.Add(FanucDataItemKeys.GetLoadKey(axis.Name));
                }
            }

            return keys;
        }
    }

    /// <summary>
    /// Thrown when a FOCAS function reports that the connection to the CNC was lost.
    /// </summary>
    public class FocasConnectionException : Exception
    {
        /// <summary>
        /// Initializes a new exception.
        /// </summary>
        /// <param name="message">The error message.</param>
        public FocasConnectionException(string message) : base(message) { }
    }
}
