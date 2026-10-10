// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations.Events;

namespace MTConnect.Fanuc
{
    /// <summary>
    /// Converts FOCAS values to MTConnect values.
    /// </summary>
    public static class FanucMapping
    {
        private const double MillimetersPerInch = 25.4;

        /// <summary>The first CNC interface (G) address read for overrides.</summary>
        public const ushort OverrideStartAddress = 12;

        /// <summary>The last CNC interface (G) address read for overrides.</summary>
        public const ushort OverrideEndAddress = 96;


        /// <summary>
        /// Maps the ODBST <c>aut</c> (mode) value.
        /// </summary>
        /// <param name="mode">0 = MDI, 1 = MEM, 2 = none, 3 = EDIT, 4 = HANDLE, 5 = JOG, 6 = TEACH IN JOG,
        /// 7 = TEACH IN HANDLE, 8 = INC, 9 = REF, 10 = RMT.</param>
        /// <returns>The ControllerMode, or <c>null</c> if no mode is selected or the value is unknown.</returns>
        public static ControllerMode? MapControllerMode(short mode)
        {
            switch (mode)
            {
                case 0: return ControllerMode.MANUAL_DATA_INPUT;
                case 1:
                case 10: return ControllerMode.AUTOMATIC;
                case 3: return ControllerMode.EDIT;
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9: return ControllerMode.MANUAL;
                default: return null;
            }
        }

        /// <summary>
        /// Maps the ODBST <c>run</c> and <c>emergency</c> values.
        /// </summary>
        /// <param name="run">0 = reset, 1 = STOP, 2 = HOLD, 3 = START, 4 = MSTR.</param>
        /// <param name="emergency">0 = none, 1 = EMERGENCY, 2 = RESET.</param>
        /// <returns>The Execution, or <c>null</c> if the value is unknown.</returns>
        public static Execution? MapExecution(short run, short emergency)
        {
            if (emergency == 1) return Execution.STOPPED;

            switch (run)
            {
                case 0: return Execution.READY;
                case 1: return Execution.STOPPED;
                case 2: return Execution.FEED_HOLD;
                case 3: return Execution.ACTIVE;
                case 4: return Execution.INTERRUPTED;
                default: return null;
            }
        }

        /// <summary>
        /// Maps the ODBST <c>emergency</c> value.
        /// </summary>
        /// <param name="emergency">0 = none, 1 = EMERGENCY, 2 = RESET.</param>
        /// <returns>TRIGGERED while in emergency stop, otherwise ARMED.</returns>
        public static EmergencyStop MapEmergencyStop(short emergency)
        {
            return emergency == 1 ? EmergencyStop.TRIGGERED : EmergencyStop.ARMED;
        }

        /// <summary>
        /// Returns the message for an ODBST <c>alarm</c> warning value (2 and above).
        /// </summary>
        /// <param name="alarm">The ODBST alarm value.</param>
        /// <returns>The warning message, or <c>null</c> for 0 (none) and 1 (alarm).</returns>
        public static string GetWarningMessage(short alarm)
        {
            switch (alarm)
            {
                case 0:
                case 1: return null;
                case 2: return "BATTERY LOW";
                case 3: return "FAN ALARM";
                case 4: return "PS WARNING";
                case 5: return "FSSB WARNING";
                case 6: return "INSULATION WARNING";
                case 7: return "ENCODER WARNING";
                case 8: return "PMC ALARM";
                default: return $"WARNING {alarm}";
            }
        }

        /// <summary>
        /// Returns the native code of an alarm (ex. <c>PS0010</c>, <c>SV0401</c>).
        /// </summary>
        /// <param name="alarm">The alarm.</param>
        /// <returns>The alarm type prefix followed by the 4 digit alarm number.</returns>
        public static string GetAlarmCode(FocasAlarm alarm)
        {
            return $"{GetAlarmPrefix(alarm.Type)}{alarm.Number:0000}";
        }

        /// <summary>
        /// Returns the prefix displayed on the CNC for an ODBALMMSG2 alarm type.
        /// </summary>
        /// <param name="type">The alarm type.</param>
        /// <returns>The prefix (ex. <c>PS</c>), or <c>AL</c> if the type is unknown.</returns>
        public static string GetAlarmPrefix(short type)
        {
            switch (type)
            {
                case 0: return "SW";
                case 1: return "PW";
                case 2: return "IO";
                case 3: return "PS";
                case 4: return "OT";
                case 5: return "OH";
                case 6: return "SV";
                case 7: return "SR";
                case 8: return "MC";
                case 9: return "SP";
                case 10: return "DS";
                case 11: return "IE";
                case 12: return "BG";
                case 13: return "SN";
                case 15: return "EX";
                case 19: return "PC";
                default: return "AL";
            }
        }

        /// <summary>
        /// Converts a FOCAS feedrate to millimeters per second.
        /// </summary>
        /// <param name="feedrate">The feedrate.</param>
        /// <param name="unit">0 = mm/min, 1 = inch/min, 3 = mm/rev, 4 = inch/rev.</param>
        /// <param name="spindleSpeed">The spindle speed (RPM), used for feed per revolution.</param>
        /// <returns>The feedrate in mm/s, or <c>null</c> if the unit is unknown.</returns>
        public static double? ToMillimetersPerSecond(double feedrate, short unit, double spindleSpeed)
        {
            switch (unit)
            {
                case 0: return feedrate / 60.0;
                case 1: return feedrate * MillimetersPerInch / 60.0;
                case 3: return feedrate * spindleSpeed / 60.0;
                case 4: return feedrate * MillimetersPerInch * spindleSpeed / 60.0;
                default: return null;
            }
        }

        /// <summary>
        /// Converts a FOCAS position to millimeters (linear axes) or degrees (rotary axes).
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="unit">0 = mm, 1 = inch, 2 = degree.</param>
        /// <returns>The converted position.</returns>
        public static double ToMillimeters(double position, short unit)
        {
            return unit == 1 ? position * MillimetersPerInch : position;
        }

        /// <summary>
        /// Returns the G address of a CNC interface signal for a path (path 2 = +1000, path 3 = +2000, ...).
        /// </summary>
        /// <param name="address">The path 1 address (ex. 12 for G0012).</param>
        /// <param name="path">The path number.</param>
        /// <returns>The address for <paramref name="path"/>.</returns>
        public static ushort GetPathAddress(ushort address, int path) => (ushort)(address + ((path - 1) * 1000));

        /// <summary>
        /// Returns the feedrate override (%) from G0012 (*FV0 - *FV7, negative logic).
        /// </summary>
        /// <param name="g0012">The G0012 byte.</param>
        /// <returns>The override in percent (0 - 254).</returns>
        public static int GetFeedrateOverride(byte g0012) => (byte)~g0012;

        /// <summary>
        /// Returns the rapid traverse override (%) from G0014 (ROV1, ROV2) and G0096 (HROV, *HROV0 - *HROV6).
        /// </summary>
        /// <param name="g0014">The G0014 byte.</param>
        /// <param name="g0096">The G0096 byte.</param>
        /// <returns>The override in percent. F0 (ROV1 = ROV2 = 1) is reported as 0.</returns>
        public static int GetRapidOverride(byte g0014, byte g0096)
        {
            // 1% rapid override signals are used when HROV (G0096#7) is on
            if ((g0096 & 0x80) != 0) return (~g0096) & 0x7F;

            switch (g0014 & 0x03)
            {
                case 0: return 100;
                case 1: return 50;
                case 2: return 25;
                default: return 0; // F0
            }
        }

        /// <summary>
        /// Returns the spindle speed override (%) from G0030 (SOV0 - SOV7).
        /// </summary>
        /// <param name="g0030">The G0030 byte.</param>
        /// <returns>The override in percent.</returns>
        public static int GetSpindleOverride(byte g0030) => g0030;
    }
}
