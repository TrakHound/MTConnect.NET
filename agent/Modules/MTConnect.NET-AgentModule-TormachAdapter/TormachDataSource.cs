// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Input;
using MTConnect.Logging;
using MTConnect.Observations;
using MTConnect.Observations.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Tormach
{
    /// <summary>
    /// The DataItem keys (ID, Name, or Source) that <see cref="TormachDataSource"/> reports observations for.
    /// </summary>
    public static class TormachDataItemKeys
    {
        /// <summary>AVAILABILITY event.</summary>
        public const string Availability = "avail";
        /// <summary>EMERGENCY_STOP event.</summary>
        public const string EmergencyStop = "estop";
        /// <summary>CONTROLLER_MODE event.</summary>
        public const string ControllerMode = "mode";
        /// <summary>EXECUTION event.</summary>
        public const string Execution = "execution";
        /// <summary>PROGRAM event: the loaded program file name.</summary>
        public const string Program = "program";
        /// <summary>LINE_NUMBER (ABSOLUTE) event: the program line being executed.</summary>
        public const string Line = "line";
        /// <summary>TOOL_NUMBER event: the tool in the spindle.</summary>
        public const string ToolNumber = "tool";
        /// <summary>WORK_OFFSET event: the active work offset (ex. 54).</summary>
        public const string WorkOffset = "workOffset";
        /// <summary>CONTROLLER_MODE_OVERRIDE (OPTIONAL_STOP) event.</summary>
        public const string OptionalStop = "optionalStop";
        /// <summary>PATH_FEEDRATE (ACTUAL) sample, in millimeters per second.</summary>
        public const string Feedrate = "feedrate";
        /// <summary>PATH_FEEDRATE_OVERRIDE (PROGRAMMED) event, in percent.</summary>
        public const string FeedrateOverride = "feedOverride";
        /// <summary>PATH_FEEDRATE_OVERRIDE (RAPID) event, in percent.</summary>
        public const string RapidOverride = "rapidOverride";
        /// <summary>Spindle ROTARY_VELOCITY (COMMANDED) sample, in revolutions per minute.</summary>
        public const string SpindleSpeed = "speed";
        /// <summary>Spindle ROTARY_VELOCITY_OVERRIDE event, in percent.</summary>
        public const string SpindleOverride = "spindleOverride";

        /// <summary>
        /// Returns the key of the machine position of an axis (ex. <c>x_act</c>).
        /// </summary>
        /// <param name="axis">The axis letter.</param>
        /// <returns>The DataItem key.</returns>
        public static string GetPositionKey(string axis) => $"{axis.ToLowerInvariant()}_act";

        /// <summary>
        /// Returns the key of the work coordinate position of an axis (ex. <c>x_work</c>).
        /// </summary>
        /// <param name="axis">The axis letter.</param>
        /// <returns>The DataItem key.</returns>
        public static string GetWorkPositionKey(string axis) => $"{axis.ToLowerInvariant()}_work";
    }

    /// <summary>
    /// Reads a Tormach PathPilot controller through the PathPilot status server and reports the
    /// results as observations. The status server sends a snapshot every interval; the most recent
    /// snapshot is processed every read cycle.
    /// </summary>
    public class TormachDataSource : MTConnectDataSource
    {
        private readonly TormachClient _client = new TormachClient();
        private readonly TormachConfiguration _configuration;
        private TormachInfo _info;
        private bool _connectFailed;
        private bool? _available;


        /// <summary>
        /// Gets the machine configuration received from the status server, or <c>null</c> if none has been received.
        /// </summary>
        public TormachInfo MachineInfo => _info;

        /// <summary>
        /// Raised when the machine configuration (machine name, axes) changes.
        /// </summary>
        public event EventHandler<TormachInfo> MachineInfoChanged;


        /// <summary>
        /// Initializes a new data source.
        /// </summary>
        /// <param name="configuration">The status server settings.</param>
        public TormachDataSource(TormachConfiguration configuration)
        {
            _configuration = configuration ?? new TormachConfiguration();
        }


        /// <inheritdoc/>
        protected override void OnStop()
        {
            _client.Close();
            SetUnavailable();
        }

        /// <inheritdoc/>
        protected override void OnRead()
        {
            if (!_client.IsConnected && !Connect())
            {
                SetUnavailable();
                return;
            }

            try
            {
                TormachStatus status = null;
                foreach (var message in _client.ReadMessages())
                {
                    var info = message as TormachInfo;
                    if (info != null) UpdateInfo(info);
                    else status = message as TormachStatus ?? status;
                }

                if (status != null) ProcessStatus(status);

                if (_client.MillisecondsSinceData > _configuration.DataTimeout)
                {
                    throw new TimeoutException($"No data received for {_configuration.DataTimeout} ms");
                }
            }
            catch (Exception ex)
            {
                Log(MTConnectLogLevel.Warning, $"Connection Lost : {ex.Message}");
                _client.Close();
                SetUnavailable();
            }
        }


        private bool Connect()
        {
            try
            {
                _client.Connect(_configuration.Server, _configuration.Port, _configuration.ConnectionTimeout);
                Log(MTConnectLogLevel.Debug, $"Connected to {_configuration.Server}:{_configuration.Port}");
                _connectFailed = false;
                return true;
            }
            catch (Exception ex)
            {
                // Only log the first failure so an offline machine doesn't flood the log
                Log(_connectFailed ? MTConnectLogLevel.Debug : MTConnectLogLevel.Warning, $"Connection to {_configuration.Server}:{_configuration.Port} Failed : {ex.Message}");
                _connectFailed = true;
                return false;
            }
        }

        private void UpdateInfo(TormachInfo info)
        {
            var changed = _info == null || _info.GetSignature() != info.GetSignature();
            _info = info;

            if (changed)
            {
                Log(MTConnectLogLevel.Information, $"Machine : {info.Machine} : Axes {string.Join(" ", info.GetAxes())}");
                if (MachineInfoChanged != null) MachineInfoChanged.Invoke(this, info);
            }
        }

        private void ProcessStatus(TormachStatus status)
        {
            if (!status.Available)
            {
                // The status server is running but PathPilot (LinuxCNC) is not
                SetUnavailable();
                return;
            }

            if (_available != true)
            {
                Log(MTConnectLogLevel.Information, "PathPilot Available");
                _available = true;
            }

            AddObservation(TormachDataItemKeys.Availability, Availability.AVAILABLE);
            AddObservation(TormachDataItemKeys.EmergencyStop, status.Estop ? EmergencyStop.TRIGGERED : EmergencyStop.ARMED);

            var mode = TormachMapping.MapControllerMode(status.TaskMode);
            if (mode.HasValue) AddObservation(TormachDataItemKeys.ControllerMode, mode.Value);

            var execution = TormachMapping.MapExecution(status);
            if (execution.HasValue) AddObservation(TormachDataItemKeys.Execution, execution.Value);

            AddObservation(TormachDataItemKeys.Program, TormachMapping.GetProgramName(status.File));
            AddObservation(TormachDataItemKeys.Line, status.Line);
            AddObservation(TormachDataItemKeys.ToolNumber, status.Tool);
            AddObservation(TormachDataItemKeys.OptionalStop, status.OptionalStop ? ControllerModeOverride.ON : ControllerModeOverride.OFF);

            var workOffset = TormachMapping.GetWorkOffset(status.G5xIndex);
            if (workOffset != null) AddObservation(TormachDataItemKeys.WorkOffset, workOffset);

            var linearUnits = status.LinearUnits ?? _info?.LinearUnits;

            if (status.CurrentVelocity.HasValue)
            {
                AddObservation(TormachDataItemKeys.Feedrate, Math.Round(TormachMapping.ToMillimeters(status.CurrentVelocity.Value, linearUnits), 4));
            }

            if (status.Feedrate.HasValue) AddObservation(TormachDataItemKeys.FeedrateOverride, TormachMapping.ToPercent(status.Feedrate.Value));
            if (status.Rapidrate.HasValue) AddObservation(TormachDataItemKeys.RapidOverride, TormachMapping.ToPercent(status.Rapidrate.Value));
            if (status.SpindleOverride.HasValue) AddObservation(TormachDataItemKeys.SpindleOverride, TormachMapping.ToPercent(status.SpindleOverride.Value));

            if (status.SpindleSpeed.HasValue)
            {
                AddObservation(TormachDataItemKeys.SpindleSpeed, status.SpindleEnabled ? Math.Abs(status.SpindleSpeed.Value) : 0);
            }

            foreach (var axis in GetAxes())
            {
                var index = TormachInfo.AxisLetters.IndexOf(axis[0]);
                var rotary = TormachInfo.IsRotary(axis);

                var position = TormachMapping.Get(status.ActualPosition, index);
                if (position.HasValue)
                {
                    AddObservation(TormachDataItemKeys.GetPositionKey(axis), Math.Round(rotary ? position.Value : TormachMapping.ToMillimeters(position.Value, linearUnits), 4));
                }

                if (_configuration.WorkPositions)
                {
                    var work = TormachMapping.GetWorkPosition(status, index);
                    if (work.HasValue)
                    {
                        AddObservation(TormachDataItemKeys.GetWorkPositionKey(axis), Math.Round(rotary ? work.Value : TormachMapping.ToMillimeters(work.Value, linearUnits), 4));
                    }
                }
            }
        }

        private IEnumerable<string> GetAxes() => _info != null ? _info.GetAxes() : new[] { "X", "Y", "Z" };

        private void SetUnavailable()
        {
            if (_available == false) return;

            if (_available == true) Log(MTConnectLogLevel.Warning, "PathPilot Unavailable");
            _available = false;

            AddObservation(TormachDataItemKeys.Availability, Availability.UNAVAILABLE);
            foreach (var key in GetValueKeys(GetAxes(), _configuration)) AddObservation(key, Observation.Unavailable);
        }

        internal static IEnumerable<string> GetValueKeys(IEnumerable<string> axes, TormachConfiguration configuration)
        {
            var keys = new List<string>
            {
                TormachDataItemKeys.EmergencyStop,
                TormachDataItemKeys.ControllerMode,
                TormachDataItemKeys.Execution,
                TormachDataItemKeys.Program,
                TormachDataItemKeys.Line,
                TormachDataItemKeys.ToolNumber,
                TormachDataItemKeys.WorkOffset,
                TormachDataItemKeys.OptionalStop,
                TormachDataItemKeys.Feedrate,
                TormachDataItemKeys.FeedrateOverride,
                TormachDataItemKeys.RapidOverride,
                TormachDataItemKeys.SpindleSpeed,
                TormachDataItemKeys.SpindleOverride
            };

            foreach (var axis in axes)
            {
                keys.Add(TormachDataItemKeys.GetPositionKey(axis));
                if (configuration.WorkPositions) keys.Add(TormachDataItemKeys.GetWorkPositionKey(axis));
            }

            return keys;
        }
    }
}
