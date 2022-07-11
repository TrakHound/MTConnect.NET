// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Text.Json.Serialization;

namespace MTConnect.Configurations
{
    public class ClientConfiguration
    {
        /// <summary>
        /// The Name or UUID of the device to read from
        /// </summary>
        [JsonPropertyName("deviceKey")]
        public string DeviceKey { get; set; }

        /// <summary>
        /// The URL address the client MTConnect Agent is located at.
        /// </summary>
        [JsonPropertyName("address")]
        public string Address { get; set; }

        /// <summary>
        /// The port to connect to the client MTConnect Agent.
        /// </summary>
        [JsonPropertyName("port")]
        public int Port { get; set; }

        /// <summary>
        ///
        /// </summary>
        [JsonPropertyName("interval")]
        public int Interval { get; set; }

        /// <summary>
        ///
        /// </summary>
        [JsonPropertyName("heartbeat")]
        public int Heartbeat { get; set; }

        /// <summary>
        ///
        /// </summary>
        [JsonPropertyName("useSSL")]
        public bool UseSSL { get; set; }


        public ClientConfiguration()
        {
            Port = 5000;
            Interval = 500;
            Heartbeat = 1000;
        }
    }
}
