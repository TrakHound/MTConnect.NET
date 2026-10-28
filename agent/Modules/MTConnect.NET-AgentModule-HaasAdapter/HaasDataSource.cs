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

namespace MTConnect.Haas
{
    /// <summary>
    /// The DataItem keys (ID, Name, or Source) that <see cref="HaasDataSource"/> reports observations for.
    /// </summary>
    public static class HaasDataItemKeys
    {
        /// <summary>AVAILABILITY event.</summary>
        public const string Availability = "avail";

        /// <summary>FIRMWARE (VERSION) event: the control software version.</summary>
        public const string Firmware = "firmware";

        /// <summary>CONTROLLER_MODE event.</summary>
        public const string ControllerMode = "mode";

        /// <summary>EXECUTION event.</summary>
        public const string Execution = "execution";

        /// <summary>PROGRAM event.</summary>
        public const string Program = "program";

        /// <summary>PART_COUNT event: M30 counter #1.</summary>
        public const string PartCount = "partCount";

        /// <summary>PART_COUNT event: M30 counter #2.</summary>
        public const string PartCount2 = "partCount2";

        /// <summary>TOOL_NUMBER event: the tool in use.</summary>
        public const string ToolNumber = "tool";

        /// <summary>WORK_OFFSET event: the active work offset number (ex. 54).</summary>
        public const string WorkOffset = "workOffset";

        /// <summary>CONTROLLER_MODE_OVERRIDE (SINGLE_BLOCK) event.</summary>
        public const string SingleBlock = "singleBlock";

        /// <summary>CONTROLLER_MODE_OVERRIDE (OPTIONAL_STOP) event.</summary>
        public const string OptionalStop = "optionalStop";

        /// <summary>EQUIPMENT_TIMER (POWERED) sample, in seconds.</summary>
        public const string PowerOnTime = "powerOnTime";

        /// <summary>EQUIPMENT_TIMER (OPERATING) sample: total cycle start (program running) time, in seconds.</summary>
        public const string CycleStartTime = "cycleStartTime";

        /// <summary>PROCESS_TIMER (PROCESS) sample: the last cycle time, in seconds.</summary>
        public const string LastCycleTime = "lastCycleTime";

        /// <summary>PROCESS_TIMER (PROCESS) sample: the previous cycle time, in seconds.</summary>
        public const string PreviousCycleTime = "previousCycleTime";

        /// <summary>SYSTEM condition.</summary>
        public const string System = "system";

        /// <summary>X axis POSITION (ACTUAL) sample, in millimeters.</summary>
        public const string XPosition = "x_act";

        /// <summary>Y axis POSITION (ACTUAL) sample, in millimeters.</summary>
        public const string YPosition = "y_act";

        /// <summary>Z axis POSITION (ACTUAL) sample, in millimeters.</summary>
        public const string ZPosition = "z_act";

        /// <summary>Spindle ROTARY_VELOCITY (ACTUAL) sample, in revolutions per minute.</summary>
        public const string SpindleSpeed = "speed";

        /// <summary>Spindle LOAD sample, in percent.</summary>
        public const string SpindleLoad = "spindleLoad";

        /// <summary>Coolant FILL_LEVEL sample.</summary>
        public const string CoolantLevel = "coolantLevel";


        /// <summary>
        /// Returns the key of the machine position (POSITION for linear axes, ANGLE for rotary axes) of an axis (ex. <c>x_act</c>).
        /// </summary>
        /// <param name="axis">The axis name (ex. <c>X</c>).</param>
        /// <returns>The DataItem key.</returns>
        public static string GetPositionKey(string axis) => $"{axis.ToLowerInvariant()}_act";

        /// <summary>
        /// Returns the key of the work coordinate position of an axis (ex. <c>x_work</c>).
        /// </summary>
        /// <param name="axis">The axis name (ex. <c>X</c>).</param>
        /// <returns>The DataItem key.</returns>
        public static string GetWorkPositionKey(string axis) => $"{axis.ToLowerInvariant()}_work";
    }

    /// <summary>
    /// Identifying information read from a Haas control.
    /// </summary>
    public class HaasMachineInfo
    {
        /// <summary>The machine serial number (<c>?Q100</c>).</summary>
        public string SerialNumber { get; set; }

        /// <summary>The machine model (<c>?Q102</c>).</summary>
        public string Model { get; set; }

        /// <summary>The control software version (<c>?Q101</c>).</summary>
        public string SoftwareVersion { get; set; }
    }

    /// <summary>
    /// Reads a Haas control using Q-commands and reports the results as observations.
    /// Values that change often are read every cycle; slowly changing values (timers, counters,
    /// version) are read every <see cref="HaasConfiguration.SlowReadInterval"/>.
    /// </summary>
    public abstract class HaasDataSource : MTConnectDataSource
    {
        // Macro / system variables read using ?Q600
        private const int MachinePositionVariable = 5021; // #5021 - #5026 (X, Y, Z, A, B, C)
        private const int WorkPositionVariable = 5041;    // #5041 - #5046 (X, Y, Z, A, B, C)
        private const int SpindleSpeedVariable = 3027;
        private const int SpindleLoadVariable = 1098;
        private const int SingleBlockVariable = 3030;
        private const int OptionalStopVariable = 3033;
        private const int WorkOffsetVariable = 4132;
        private const int CoolantLevelVariable = 13013;

        private const double MillimetersPerInch = 25.4;

        private readonly HaasConnection _connection;
        private readonly HaasConfiguration _configuration;
        private readonly string[] _axes;
        private readonly HashSet<string> _unsupportedCommands = new HashSet<string>();
        private readonly HashSet<string> _unknownModes = new HashSet<string>();
        private readonly HaasMachineInfo _machineInfo = new HaasMachineInfo();
        private Stopwatch _slowReadTimer;
        private bool _machineInfoChanged;
        private bool? _available;


        /// <summary>
        /// Raised when the serial number, model or software version read from the control changes.
        /// </summary>
        public event EventHandler<HaasMachineInfo> MachineInfoChanged;


        /// <summary>
        /// Initializes a new data source that reads using the specified connection.
        /// </summary>
        /// <param name="connection">The connection to the Haas control.</param>
        /// <param name="configuration">The shared Haas settings.</param>
        protected HaasDataSource(HaasConnection connection, HaasConfiguration configuration)
        {
            _connection = connection;
            _configuration = configuration ?? new HaasConfiguration();

            // Every value is read each poll; only changes need to go through the agent pipeline
            SuppressUnchangedObservations = true;
            _axes = _configuration.GetAxes().ToArray();
            _connection.ResponseTimeout = _configuration.ResponseTimeout;
            _connection.LogReceived += (s, level, message, id) => Log(level, message);
        }


        /// <inheritdoc/>
        protected override void OnStop()
        {
            _connection.Close();
            SetUnavailable();
        }

        /// <inheritdoc/>
        protected override void OnRead()
        {
            if (!_connection.Connect())
            {
                SetUnavailable();
                return;
            }

            // Q100 (serial number) is used as a heartbeat. No reply means the control is not responding.
            // The control may not answer the first command after connecting, so it is sent twice if needed.
            var serialNumber = HaasResponse.ParseValue(SendCommand("?Q100", 2), "SERIAL NUMBER");
            if (serialNumber == null)
            {
                SetUnavailable();
                return;
            }

            if (_available != true)
            {
                Log(MTConnectLogLevel.Information, $"Haas Control Available : Serial Number {serialNumber}");
                _available = true;
                _unsupportedCommands.Clear();
                _slowReadTimer = null;
            }

            AddObservation(HaasDataItemKeys.Availability, Availability.AVAILABLE);
            UpdateMachineInfo(o => o.SerialNumber, (o, v) => o.SerialNumber = v, serialNumber);

            ReadFast();

            if (_slowReadTimer == null || _slowReadTimer.ElapsedMilliseconds >= _configuration.SlowReadInterval)
            {
                ReadSlow();
                _slowReadTimer = Stopwatch.StartNew();
            }

            if (_machineInfoChanged)
            {
                _machineInfoChanged = false;
                var handler = MachineInfoChanged;
                if (handler != null)
                {
                    handler.Invoke(this, new HaasMachineInfo
                    {
                        SerialNumber = _machineInfo.SerialNumber,
                        Model = _machineInfo.Model,
                        SoftwareVersion = _machineInfo.SoftwareVersion
                    });
                }
            }
        }


        private void ReadFast()
        {
            ProcessMode(SendCommand("?Q104"));
            ProcessStatus(SendCommand("?Q500"));

            var tool = HaasResponse.ParseInteger(SendCommand("?Q201"), "USING TOOL");
            if (tool.HasValue) AddObservation(HaasDataItemKeys.ToolNumber, tool.Value);

            for (var i = 0; i < _axes.Length; i++)
            {
                var axis = _axes[i];
                var offset = Array.IndexOf(HaasConfiguration.SupportedAxes, axis);

                ProcessPosition(HaasDataItemKeys.GetPositionKey(axis), axis, MachinePositionVariable + offset);
                if (_configuration.WorkPositions)
                {
                    ProcessPosition(HaasDataItemKeys.GetWorkPositionKey(axis), axis, WorkPositionVariable + offset);
                }
            }

            var speed = ReadMacro(SpindleSpeedVariable);
            if (speed.HasValue) AddObservation(HaasDataItemKeys.SpindleSpeed, speed.Value);

            if (_configuration.SpindleLoad)
            {
                var load = ReadMacro(SpindleLoadVariable);
                if (load.HasValue) AddObservation(HaasDataItemKeys.SpindleLoad, Math.Round(load.Value, 1));
            }
        }

        private void ReadSlow()
        {
            var version = HaasResponse.ParseValue(SendCommand("?Q101"), "SOFTWARE VERSION");
            if (version != null)
            {
                AddObservation(HaasDataItemKeys.Firmware, version);
                UpdateMachineInfo(o => o.SoftwareVersion, (o, v) => o.SoftwareVersion = v, version);
            }

            var model = HaasResponse.ParseValue(SendCommand("?Q102"), "MODEL");
            if (model != null) UpdateMachineInfo(o => o.Model, (o, v) => o.Model = v, model);

            ProcessDuration("?Q300", "P.O. TIME", HaasDataItemKeys.PowerOnTime);
            ProcessDuration("?Q301", "C.S. TIME", HaasDataItemKeys.CycleStartTime);
            ProcessDuration("?Q303", "LAST CYCLE", HaasDataItemKeys.LastCycleTime);
            ProcessDuration("?Q304", "PREV CYCLE", HaasDataItemKeys.PreviousCycleTime);

            var partCount2 = HaasResponse.ParseInteger(SendCommand("?Q403"), "M30 #2");
            if (partCount2.HasValue) AddObservation(HaasDataItemKeys.PartCount2, partCount2.Value);

            var workOffset = ReadMacro(WorkOffsetVariable);
            if (workOffset.HasValue) AddObservation(HaasDataItemKeys.WorkOffset, FormatNumber(workOffset.Value));

            ProcessOverride(HaasDataItemKeys.SingleBlock, SingleBlockVariable);
            ProcessOverride(HaasDataItemKeys.OptionalStop, OptionalStopVariable);

            if (_configuration.CoolantLevel)
            {
                var level = ReadMacro(CoolantLevelVariable);
                if (level.HasValue) AddObservation(HaasDataItemKeys.CoolantLevel, level.Value);
            }
        }


        private string SendCommand(string command, int attempts = 1)
        {
            if (_unsupportedCommands.Contains(command)) return null;

            var reply = _connection.SendCommand(command, attempts);
            if (IsLogEnabled(MTConnectLogLevel.Trace)) Log(MTConnectLogLevel.Trace, $"{command} : {reply}");

            if (HaasResponse.IsUnsupported(reply))
            {
                _unsupportedCommands.Add(command);
                Log(MTConnectLogLevel.Information, $"'{command}' is not supported by the control and will not be read");
                return null;
            }

            return reply;
        }

        private void ProcessMode(string reply)
        {
            var mode = HaasResponse.ParseMode(reply);
            if (mode.HasValue)
            {
                AddObservation(HaasDataItemKeys.ControllerMode, mode.Value);
            }
            else if (reply != null)
            {
                // Log each unknown mode once so it can be added to the mapping
                var value = HaasResponse.ParseValue(reply, "MODE") ?? reply;
                if (_unknownModes.Add(value)) Log(MTConnectLogLevel.Warning, $"Unrecognized Mode : {value}");
            }
        }

        private void ProcessStatus(string reply)
        {
            var status = HaasResponse.ParseStatus(reply);
            if (status == null)
            {
                if (reply != null) Log(MTConnectLogLevel.Debug, $"Unrecognized ?Q500 Reply : {reply}");
                return;
            }

            if (status.Execution.HasValue) AddObservation(HaasDataItemKeys.Execution, status.Execution.Value);
            if (status.Program != null) AddObservation(HaasDataItemKeys.Program, status.Program);
            if (status.PartCount.HasValue) AddObservation(HaasDataItemKeys.PartCount, status.PartCount.Value);

            if (status.Alarm == true)
            {
                var fault = new ConditionFaultStateObservationInput(HaasDataItemKeys.System, ConditionLevel.FAULT);
                fault.Message = "ALARM ON";
                AddObservation(fault);
            }
            else if (status.Alarm == false)
            {
                AddObservation(new ConditionFaultStateObservationInput(HaasDataItemKeys.System, ConditionLevel.NORMAL));
            }
        }

        private void ProcessPosition(string dataItemKey, string axis, int variable)
        {
            var value = ReadMacro(variable);
            if (value.HasValue)
            {
                // Rotary axes are reported in degrees and are not converted
                var position = value.Value;
                if (_configuration.IsInch && !HaasConfiguration.IsRotaryAxis(axis)) position *= MillimetersPerInch;

                AddObservation(dataItemKey, Math.Round(position, 4));
            }
        }

        private void ProcessDuration(string command, string label, string dataItemKey)
        {
            var seconds = HaasResponse.ParseDuration(SendCommand(command), label);
            if (seconds.HasValue) AddObservation(dataItemKey, seconds.Value);
        }

        private void ProcessOverride(string dataItemKey, int variable)
        {
            var value = ReadMacro(variable);
            if (value.HasValue) AddObservation(dataItemKey, value.Value != 0 ? ControllerModeOverride.ON : ControllerModeOverride.OFF);
        }

        private double? ReadMacro(int variable)
        {
            return HaasResponse.ParseMacro(SendCommand($"?Q600 {variable}"));
        }

        private void UpdateMachineInfo(Func<HaasMachineInfo, string> get, Action<HaasMachineInfo, string> set, string value)
        {
            if (get(_machineInfo) != value)
            {
                set(_machineInfo, value);
                _machineInfoChanged = true;
            }
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

            if (_available == true) Log(MTConnectLogLevel.Warning, "Haas Control Unavailable");
            _available = false;

            AddObservation(HaasDataItemKeys.Availability, Availability.UNAVAILABLE);
            foreach (var key in GetValueKeys()) AddObservation(key, Observation.Unavailable);
            AddObservation(new ConditionFaultStateObservationInput(HaasDataItemKeys.System, ConditionLevel.UNAVAILABLE));
        }

        private IEnumerable<string> GetValueKeys()
        {
            var keys = new List<string>
            {
                HaasDataItemKeys.Firmware,
                HaasDataItemKeys.ControllerMode,
                HaasDataItemKeys.Execution,
                HaasDataItemKeys.Program,
                HaasDataItemKeys.PartCount,
                HaasDataItemKeys.PartCount2,
                HaasDataItemKeys.ToolNumber,
                HaasDataItemKeys.WorkOffset,
                HaasDataItemKeys.SingleBlock,
                HaasDataItemKeys.OptionalStop,
                HaasDataItemKeys.PowerOnTime,
                HaasDataItemKeys.CycleStartTime,
                HaasDataItemKeys.LastCycleTime,
                HaasDataItemKeys.PreviousCycleTime,
                HaasDataItemKeys.SpindleSpeed
            };

            foreach (var axis in _axes)
            {
                keys.Add(HaasDataItemKeys.GetPositionKey(axis));
                if (_configuration.WorkPositions) keys.Add(HaasDataItemKeys.GetWorkPositionKey(axis));
            }

            if (_configuration.SpindleLoad) keys.Add(HaasDataItemKeys.SpindleLoad);
            if (_configuration.CoolantLevel) keys.Add(HaasDataItemKeys.CoolantLevel);

            return keys;
        }
    }
}
