// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace MTConnect.Fanuc
{
    /// <summary>
    /// The FOCAS functions used by <see cref="FanucDataSource"/>. Every method returns the FOCAS
    /// return code (see <see cref="FocasReturnCodes"/>); output values are only valid when it is EW_OK.
    /// </summary>
    public interface IFocasClient : IDisposable
    {
        /// <summary>Gets whether a library handle is open.</summary>
        bool IsConnected { get; }

        /// <summary>Opens a library handle to the CNC (cnc_allclibhndl3).</summary>
        /// <param name="host">The IP address or hostname of the CNC.</param>
        /// <param name="port">The FOCAS port of the CNC (typically 8193).</param>
        /// <param name="timeoutSeconds">The communication timeout in seconds.</param>
        /// <returns>The return code.</returns>
        short Connect(string host, int port, int timeoutSeconds);

        /// <summary>Releases the library handle (cnc_freelibhndl).</summary>
        void Disconnect();

        /// <summary>Reads the CNC system information (cnc_sysinfo).</summary>
        /// <param name="info">The system information.</param>
        /// <returns>The return code.</returns>
        short ReadSystemInfo(out FocasSystemInfo info);

        /// <summary>Reads the current and maximum path numbers (cnc_getpath).</summary>
        /// <param name="path">The current path number.</param>
        /// <param name="maxPath">The number of paths.</param>
        /// <returns>The return code.</returns>
        short GetPath(out short path, out short maxPath);

        /// <summary>Selects the path that subsequent path-specific functions read (cnc_setpath).</summary>
        /// <param name="path">The path number (1 based).</param>
        /// <returns>The return code.</returns>
        short SetPath(short path);

        /// <summary>Reads the CNC status (cnc_statinfo).</summary>
        /// <param name="status">The status.</param>
        /// <returns>The return code.</returns>
        short ReadStatus(out FocasStatus status);

        /// <summary>Reads the name of the executing program (cnc_exeprgname).</summary>
        /// <param name="name">The program name (ex. <c>O1234</c>).</param>
        /// <returns>The return code.</returns>
        short ReadProgramName(out string name);

        /// <summary>Reads the executing and main program numbers (cnc_rdprgnum).</summary>
        /// <param name="running">The executing program number.</param>
        /// <param name="main">The main program number.</param>
        /// <returns>The return code.</returns>
        short ReadProgramNumber(out int running, out int main);

        /// <summary>Reads the sequence number (N number) being executed (cnc_rdseqnum).</summary>
        /// <param name="number">The sequence number.</param>
        /// <returns>The return code.</returns>
        short ReadSequenceNumber(out int number);

        /// <summary>Reads a non-axis 2-word (32 bit) parameter (cnc_rdparam).</summary>
        /// <param name="number">The parameter number.</param>
        /// <param name="value">The parameter value.</param>
        /// <returns>The return code.</returns>
        short ReadParameter(short number, out int value);

        /// <summary>Reads a custom macro variable (cnc_rdmacro).</summary>
        /// <param name="number">The variable number.</param>
        /// <param name="value">The value, or <c>null</c> if the variable is vacant.</param>
        /// <returns>The return code.</returns>
        short ReadMacro(short number, out double? value);

        /// <summary>Reads the axis names of the current path (cnc_rdaxisname).</summary>
        /// <param name="names">The axis names including suffix (ex. <c>X</c>, <c>X1</c>).</param>
        /// <returns>The return code.</returns>
        short ReadAxisNames(out string[] names);

        /// <summary>Reads the positions of every axis of the current path (cnc_rdposition).</summary>
        /// <param name="positions">The positions, in axis order.</param>
        /// <returns>The return code.</returns>
        short ReadPositions(out FocasPosition[] positions);

        /// <summary>Reads the servo load meters of the current path (cnc_rdsvmeter).</summary>
        /// <param name="loads">The loads in percent, in axis order.</param>
        /// <returns>The return code.</returns>
        short ReadServoLoads(out FocasLoad[] loads);

        /// <summary>Reads the spindle names of the current path (cnc_rdspdlname).</summary>
        /// <param name="names">The spindle names (ex. <c>S1</c>).</param>
        /// <returns>The return code.</returns>
        short ReadSpindleNames(out string[] names);

        /// <summary>Reads the spindle load meters of the current path (cnc_rdspmeter).</summary>
        /// <param name="loads">The loads in percent, in spindle order.</param>
        /// <returns>The return code.</returns>
        short ReadSpindleLoads(out FocasLoad[] loads);

        /// <summary>Reads the actual feedrate and spindle speed of the current path (cnc_rdspeed).</summary>
        /// <param name="speed">The feedrate and spindle speed.</param>
        /// <returns>The return code.</returns>
        short ReadSpeed(out FocasSpeed speed);

        /// <summary>Reads the actual spindle speed of the current path (cnc_acts).</summary>
        /// <param name="rpm">The spindle speed in revolutions per minute.</param>
        /// <returns>The return code.</returns>
        short ReadSpindleSpeed(out int rpm);

        /// <summary>Reads a timer (cnc_rdtimer).</summary>
        /// <param name="type">0 = power on, 1 = operating, 2 = cutting, 3 = cycle time.</param>
        /// <param name="seconds">The timer value in seconds.</param>
        /// <returns>The return code.</returns>
        short ReadTimer(short type, out double seconds);

        /// <summary>Reads the active alarms of the current path (cnc_rdalmmsg2).</summary>
        /// <param name="alarms">The active alarms.</param>
        /// <returns>The return code.</returns>
        short ReadAlarms(out FocasAlarm[] alarms);

        /// <summary>Reads a range of PMC bytes (pmc_rdpmcrng).</summary>
        /// <param name="addressType">The PMC address type (0 = G, 1 = F, 2 = Y, 3 = X, ...).</param>
        /// <param name="start">The first address.</param>
        /// <param name="end">The last address.</param>
        /// <param name="data">The bytes from <paramref name="start"/> to <paramref name="end"/>.</param>
        /// <returns>The return code.</returns>
        short ReadPmc(short addressType, ushort start, ushort end, out byte[] data);
    }

    /// <summary>CNC system information (ODBSYS).</summary>
    public class FocasSystemInfo
    {
        /// <summary>Additional information flags (bit 1 = i Series).</summary>
        public short AddInfo { get; set; }
        /// <summary>The maximum number of controlled axes.</summary>
        public short MaxAxis { get; set; }
        /// <summary>The CNC type (ex. <c>30</c>, <c>0</c>).</summary>
        public string CncType { get; set; }
        /// <summary>The machine type (<c>M</c> = machining center, <c>T</c> = lathe, ...).</summary>
        public string MtType { get; set; }
        /// <summary>The software series (ex. <c>G421</c>).</summary>
        public string Series { get; set; }
        /// <summary>The software version (ex. <c>15.0</c>).</summary>
        public string Version { get; set; }

        /// <summary>Gets the CNC model (ex. <c>30i-M</c>).</summary>
        public string Model
        {
            get
            {
                var model = (CncType ?? "").Trim();
                if ((AddInfo & 0x02) != 0) model += "i";
                var mtType = (MtType ?? "").Trim();
                return mtType.Length > 0 ? $"{model}-{mtType}" : model;
            }
        }

        /// <summary>Gets the software series and version (ex. <c>G421 15.0</c>).</summary>
        public string Firmware => $"{(Series ?? "").Trim()} {(Version ?? "").Trim()}".Trim();
    }

    /// <summary>CNC status (ODBST, Series 0i / 16i / 18i / 21i / 30i / 31i / 32i / 35i).</summary>
    public class FocasStatus
    {
        /// <summary>The selected automatic / manual mode (0 = MDI, 1 = MEM, 3 = EDIT, 4 = HANDLE, 5 = JOG, 9 = REF, 10 = RMT, ...).</summary>
        public short Mode { get; set; }
        /// <summary>The automatic operation status (0 = reset, 1 = STOP, 2 = HOLD, 3 = START, 4 = MSTR).</summary>
        public short Run { get; set; }
        /// <summary>The axis motion status (0 = none, 1 = MOTION, 2 = DWELL).</summary>
        public short Motion { get; set; }
        /// <summary>The M, S, T, B status (0 = none, 1 = FIN).</summary>
        public short Mstb { get; set; }
        /// <summary>The emergency status (0 = none, 1 = EMERGENCY, 2 = RESET, 3 = WAIT).</summary>
        public short Emergency { get; set; }
        /// <summary>The alarm status (0 = none, 1 = ALARM, 2 = BATTERY LOW, 3 = FAN, ...).</summary>
        public short Alarm { get; set; }
        /// <summary>The program editing status.</summary>
        public short Edit { get; set; }
    }

    /// <summary>The positions of an axis (ODBPOS). Values are scaled to the axis units.</summary>
    public class FocasPosition
    {
        /// <summary>The axis name including suffix.</summary>
        public string Name { get; set; }
        /// <summary>The absolute (work coordinate) position.</summary>
        public double Absolute { get; set; }
        /// <summary>The machine position.</summary>
        public double Machine { get; set; }
        /// <summary>The relative position.</summary>
        public double Relative { get; set; }
        /// <summary>The distance to go.</summary>
        public double DistanceToGo { get; set; }
        /// <summary>The position units (0 = mm, 1 = inch, 2 = degree).</summary>
        public short Unit { get; set; }
    }

    /// <summary>A load meter value (LOADELM).</summary>
    public class FocasLoad
    {
        /// <summary>The axis or spindle name.</summary>
        public string Name { get; set; }
        /// <summary>The load in percent.</summary>
        public double Value { get; set; }
    }

    /// <summary>The actual feedrate and spindle speed (ODBSPEED).</summary>
    public class FocasSpeed
    {
        /// <summary>The actual feedrate.</summary>
        public double Feedrate { get; set; }
        /// <summary>The feedrate units (0 = mm/min, 1 = inch/min, 3 = mm/rev, 4 = inch/rev).</summary>
        public short FeedrateUnit { get; set; }
        /// <summary>The actual spindle speed in revolutions per minute.</summary>
        public double SpindleSpeed { get; set; }
    }

    /// <summary>An active alarm (ODBALMMSG2).</summary>
    public class FocasAlarm
    {
        /// <summary>The alarm number.</summary>
        public int Number { get; set; }
        /// <summary>The alarm type (3 = PS, 6 = SV, ...).</summary>
        public short Type { get; set; }
        /// <summary>The axis number the alarm applies to (0 = none).</summary>
        public short Axis { get; set; }
        /// <summary>The alarm message.</summary>
        public string Message { get; set; }
    }
}
