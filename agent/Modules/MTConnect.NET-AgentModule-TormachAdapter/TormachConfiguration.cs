// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Tormach
{
    /// <summary>
    /// Settings for reading a Tormach PathPilot controller through the PathPilot status server.
    /// </summary>
    public class TormachConfiguration
    {
        /// <summary>
        /// The IP address or hostname of the PathPilot controller.
        /// </summary>
        public string Server { get; set; } = "localhost";

        /// <summary>
        /// The TCP port of the PathPilot status server. Default is 7890.
        /// </summary>
        public int Port { get; set; } = 7890;

        /// <summary>
        /// Maximum time (in milliseconds) to wait for the TCP connection to open. Default is 5000.
        /// </summary>
        public int ConnectionTimeout { get; set; } = 5000;

        /// <summary>
        /// The time (in milliseconds) without receiving data after which the connection is considered lost. Default is 5000.
        /// </summary>
        public int DataTimeout { get; set; } = 5000;

        /// <summary>
        /// When <c>true</c>, work coordinate positions are reported in addition to machine positions. Default is <c>false</c>.
        /// </summary>
        public bool WorkPositions { get; set; }
    }
}
