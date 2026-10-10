// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace MTConnect.Haas
{
    /// <summary>
    /// Settings for connecting to a Haas control over RS-232. These must match
    /// the RS-232 settings on the control (Settings 11-15).
    /// </summary>
    public class HaasSerialConfiguration : HaasConfiguration
    {
        /// <summary>
        /// The serial port name (ex. <c>COM1</c> or <c>/dev/ttyUSB0</c>).
        /// </summary>
        public string ComPort { get; set; } = "COM1";

        /// <summary>
        /// The baud rate (Setting 11). Default is 19200.
        /// </summary>
        public int BaudRate { get; set; } = 19200;

        /// <summary>
        /// The parity (Setting 12): <c>None</c>, <c>Even</c>, <c>Odd</c>, <c>Mark</c> or <c>Space</c>. Default is <c>None</c>.
        /// </summary>
        public string Parity { get; set; } = "None";

        /// <summary>
        /// The number of data bits (Setting 13). Default is 7.
        /// </summary>
        public int DataBits { get; set; } = 7;

        /// <summary>
        /// The number of stop bits (Setting 13): <c>1</c> or <c>2</c>. Default is 1.
        /// </summary>
        public int StopBits { get; set; } = 1;


        /// <summary>
        /// Returns <see cref="Parity"/> as a <see cref="System.IO.Ports.Parity"/> value.
        /// </summary>
        /// <returns>The parsed parity, or <see cref="System.IO.Ports.Parity.None"/> if the value is not recognized.</returns>
        public System.IO.Ports.Parity GetParity()
        {
            System.IO.Ports.Parity parity;
            if (Enum.TryParse(Parity, true, out parity)) return parity;
            return System.IO.Ports.Parity.None;
        }

        /// <summary>
        /// Returns <see cref="StopBits"/> as a <see cref="System.IO.Ports.StopBits"/> value.
        /// </summary>
        /// <returns><see cref="System.IO.Ports.StopBits.Two"/> when set to 2, otherwise <see cref="System.IO.Ports.StopBits.One"/>.</returns>
        public System.IO.Ports.StopBits GetStopBits()
        {
            return StopBits == 2 ? System.IO.Ports.StopBits.Two : System.IO.Ports.StopBits.One;
        }
    }
}
