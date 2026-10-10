// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Input;
using MTConnect.Logging;
using MTConnect.Observations;
using MTConnect.Observations.Events;
using System;

namespace MTConnect.Haas
{
    /// <summary>
    /// The DataItem keys (ID, Name, or Source) that <see cref="HaasDataSource"/> reports observations for.
    /// </summary>
    public static class HaasDataItemKeys
    {
        /// <summary>AVAILABILITY event.</summary>
        public const string Availability = "avail";

        /// <summary>CONTROLLER_MODE event.</summary>
        public const string ControllerMode = "mode";

        /// <summary>EXECUTION event.</summary>
        public const string Execution = "execution";

        /// <summary>PROGRAM event.</summary>
        public const string Program = "program";

        /// <summary>PART_COUNT event.</summary>
        public const string PartCount = "partCount";

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
    }

    /// <summary>
    /// Reads a Haas control using Q-commands and reports the results as observations.
    /// </summary>
    public abstract class HaasDataSource : MTConnectDataSource
    {
        // Macro variables read using ?Q600
        private const int XMachinePositionVariable = 5021;
        private const int YMachinePositionVariable = 5022;
        private const int ZMachinePositionVariable = 5023;
        private const int SpindleSpeedVariable = 3027;

        private const double MillimetersPerInch = 25.4;

        private readonly HaasConnection _connection;
        private readonly HaasConfiguration _configuration;
        private bool? _available;


        /// <summary>
        /// Initializes a new data source that reads using the specified connection.
        /// </summary>
        /// <param name="connection">The connection to the Haas control.</param>
        /// <param name="configuration">The shared Haas settings.</param>
        protected HaasDataSource(HaasConnection connection, HaasConfiguration configuration)
        {
            _connection = connection;
            _configuration = configuration ?? new HaasConfiguration();
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
            var serialNumber = SendCommand("?Q100");
            if (HaasResponse.Clean(serialNumber) == null)
            {
                SetUnavailable();
                return;
            }

            if (_available != true)
            {
                Log(MTConnectLogLevel.Information, $"Haas Control Available : {HaasResponse.Clean(serialNumber)}");
                _available = true;
            }

            AddObservation(HaasDataItemKeys.Availability, Availability.AVAILABLE);

            ProcessMode(SendCommand("?Q104"));
            ProcessStatus(SendCommand("?Q500"));
            ProcessPosition(HaasDataItemKeys.XPosition, XMachinePositionVariable);
            ProcessPosition(HaasDataItemKeys.YPosition, YMachinePositionVariable);
            ProcessPosition(HaasDataItemKeys.ZPosition, ZMachinePositionVariable);
            ProcessSpindleSpeed();
        }


        private string SendCommand(string command)
        {
            var reply = _connection.SendCommand(command);
            Log(MTConnectLogLevel.Trace, $"{command} : {reply}");
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
                Log(MTConnectLogLevel.Debug, $"Unrecognized ?Q104 Reply : {reply}");
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

        private void ProcessPosition(string dataItemKey, int variable)
        {
            var value = ReadMacro(variable);
            if (value.HasValue)
            {
                var position = _configuration.IsInch ? value.Value * MillimetersPerInch : value.Value;
                AddObservation(dataItemKey, Math.Round(position, 4));
            }
        }

        private void ProcessSpindleSpeed()
        {
            var value = ReadMacro(SpindleSpeedVariable);
            if (value.HasValue) AddObservation(HaasDataItemKeys.SpindleSpeed, value.Value);
        }

        private double? ReadMacro(int variable)
        {
            return HaasResponse.ParseMacro(SendCommand($"?Q600 {variable}"));
        }

        private void SetUnavailable()
        {
            if (_available == false) return;

            if (_available == true) Log(MTConnectLogLevel.Warning, "Haas Control Unavailable");
            _available = false;

            AddObservation(HaasDataItemKeys.Availability, Availability.UNAVAILABLE);
            AddObservation(HaasDataItemKeys.ControllerMode, Observation.Unavailable);
            AddObservation(HaasDataItemKeys.Execution, Observation.Unavailable);
            AddObservation(HaasDataItemKeys.Program, Observation.Unavailable);
            AddObservation(HaasDataItemKeys.PartCount, Observation.Unavailable);
            AddObservation(HaasDataItemKeys.XPosition, Observation.Unavailable);
            AddObservation(HaasDataItemKeys.YPosition, Observation.Unavailable);
            AddObservation(HaasDataItemKeys.ZPosition, Observation.Unavailable);
            AddObservation(HaasDataItemKeys.SpindleSpeed, Observation.Unavailable);
            AddObservation(new ConditionFaultStateObservationInput(HaasDataItemKeys.System, ConditionLevel.UNAVAILABLE));
        }
    }
}
