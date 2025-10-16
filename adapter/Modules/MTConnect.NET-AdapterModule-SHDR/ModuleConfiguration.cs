// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Configurations
{
    public class ModuleConfiguration
    {
        /// <summary>
        /// The host the adapter is located on.
        /// </summary>
        public string Hostname { get; set; }

        /// <summary>
        /// The port to connect to the adapter.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// The Heartbeat interval (in milliseconds) that the TCP Connection will use to maintain a connection when no new data has been sent
        /// </summary>
        public int Heartbeat { get; set; }

        /// <summary>
        /// The amount of time (in milliseconds) an adapter can be silent before it is disconnected. This is only for legacy adapters that do not support heartbeats. 
        /// If heartbeats are present, this will be ignored.
        /// </summary>
        public int ConnectionTimeout { get; set; }

        /// <summary>
        /// The amount of time (in milliseconds) between adapter reconnection attempts. 
        /// This is useful for implementation of high performance adapters where availability needs to be tracked in near-real-time. 
        /// </summary>
        public int ReconnectInterval { get; set; }

        /// <summary>
        /// Sets the TimeZone to use when timestamps are output from the Agent
        /// </summary>
        public string TimeZoneOutput { get; set; }


        public ModuleConfiguration()
        {
            Hostname = "localhost";
            Port = 7878;
            Heartbeat = 10000;
            ConnectionTimeout = 5000;
            ReconnectInterval = 10000;
            TimeZoneOutput = "Z";
        }
    }
}