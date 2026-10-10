// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Haas
{
    /// <summary>
    /// Settings for connecting to a Haas control over Ethernet.
    /// </summary>
    public class HaasEthernetConfiguration : HaasConfiguration
    {
        /// <summary>
        /// The hostname or IP address of the Haas control.
        /// </summary>
        public string Server { get; set; } = "localhost";

        /// <summary>
        /// The TCP port of the Haas control (Setting 143). Default is 5051.
        /// </summary>
        public int Port { get; set; } = 5051;

        /// <summary>
        /// Maximum time (in milliseconds) to wait for the TCP connection to open. Default is 5000.
        /// </summary>
        public int ConnectionTimeout { get; set; } = 5000;
    }
}
