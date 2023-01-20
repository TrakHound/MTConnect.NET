// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Text.Json.Serialization;

namespace MTConnect.Configurations
{
    /// <summary>
    /// Configuration for an MTConnect SHDR Client
    /// </summary>
    public class ShdrClientConfiguration : IShdrClientConfiguration
    {
        public const string DeviceKeyWildcard = "*";


        /// <summary>
        /// The unique identifier for the Adapter
        /// </summary>
        [JsonIgnore]
        public string Id
        {
            get
            {
                var id = $"{Hostname}:{Port}";
                id = id.ToMD5Hash().Substring(0, 10);
                return $"adapter_shdr_{id}";
            }
        }

        /// <summary>
        /// The UUID or Name of the device that corresponds to the name of the device in the Devices file.
        /// </summary>
        [JsonPropertyName("deviceKey")]
        public string DeviceKey { get; set; }

        /// <summary>
        /// The host the adapter is located on.
        /// </summary>
        [JsonPropertyName("hostname")]
        public string Hostname { get; set; }

        /// <summary>
        /// The port to connect to the adapter.
        /// </summary>
        [JsonPropertyName("port")]
        public int Port { get; set; }

        /// <summary>
        /// The Heartbeat interval (in milliseconds) that the TCP Connection will use to maintain a connection when no new data has been sent
        /// </summary>
        public int Heartbeat { get; set; }

        /// <summary>
        /// The amount of time (in milliseconds) an adapter can be silent before it is disconnected. This is only for legacy adapters that do not support heartbeats. 
        /// If heartbeats are present, this will be ignored.
        /// </summary>
        [JsonPropertyName("connectionTimeout")]
        public int ConnectionTimeout { get; set; }

        /// <summary>
        /// The amount of time (in milliseconds) between adapter reconnection attempts. 
        /// This is useful for implementation of high performance adapters where availability needs to be tracked in near-real-time. 
        /// </summary>
        [JsonPropertyName("reconnectInterval")]
        public int ReconnectInterval { get; set; }


        public ShdrClientConfiguration()
        {
            DeviceKey = DeviceKeyWildcard;
            Hostname = "localhost";
            Port = 7878;
            Heartbeat = 5000;
            ConnectionTimeout = 600;
            ReconnectInterval = 10000;
        }
    }
}
