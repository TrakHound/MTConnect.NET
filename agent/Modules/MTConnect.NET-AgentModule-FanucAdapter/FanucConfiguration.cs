// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Fanuc
{
    /// <summary>
    /// Settings for reading a Fanuc CNC using FOCAS over Ethernet.
    /// </summary>
    public class FanucConfiguration
    {
        /// <summary>
        /// The IP address or hostname of the CNC.
        /// </summary>
        public string Server { get; set; } = "localhost";

        /// <summary>
        /// The FOCAS port of the CNC (set on the Embedded Ethernet / FOCAS2 setting screen). Default is 8193.
        /// </summary>
        public int Port { get; set; } = 8193;

        /// <summary>
        /// The FOCAS communication timeout (in milliseconds, rounded up to whole seconds). Default is 5000.
        /// </summary>
        public int ResponseTimeout { get; set; } = 5000;

        /// <summary>
        /// The interval (in milliseconds) between reads of values that change slowly (timers, work offset). Default is 10000.
        /// </summary>
        public int SlowReadInterval { get; set; } = 10000;

        /// <summary>
        /// When <c>true</c>, absolute (work coordinate) positions are read in addition to machine positions. Default is <c>false</c>.
        /// </summary>
        public bool WorkPositions { get; set; }

        /// <summary>
        /// When <c>true</c>, the servo load of each axis is read. Default is <c>true</c>.
        /// </summary>
        public bool AxisLoads { get; set; } = true;

        /// <summary>
        /// When <c>true</c>, the spindle load is read. Default is <c>true</c>.
        /// </summary>
        public bool SpindleLoad { get; set; } = true;

        /// <summary>
        /// When <c>true</c>, the feedrate, rapid and spindle overrides are read from the CNC interface signals
        /// (G0012, G0014, G0030, G0096). Default is <c>true</c>.
        /// </summary>
        public bool Overrides { get; set; } = true;

        /// <summary>
        /// The custom macro variable read for the tool number. Default is 4120 (the modal T code).
        /// Many machines store the tool in the spindle in another variable or PMC address. Set to 0 to disable.
        /// </summary>
        public int ToolNumberVariable { get; set; } = 4120;

        /// <summary>
        /// When <c>true</c>, the active work offset is read from macro variable #4014 (G code group 14, ex. 54). Default is <c>true</c>.
        /// </summary>
        public bool WorkOffset { get; set; } = true;

        /// <summary>
        /// The parameter read for the part count. Default is 6711 (parts machined). Set to 0 to disable.
        /// </summary>
        public int PartCountParameter { get; set; } = 6711;


        /// <summary>
        /// Gets the FOCAS timeout in whole seconds.
        /// </summary>
        public int TimeoutSeconds => ResponseTimeout <= 0 ? 1 : (ResponseTimeout + 999) / 1000;
    }
}
