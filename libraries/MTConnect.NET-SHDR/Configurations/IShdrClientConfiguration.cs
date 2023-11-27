// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Configurations
{
    /// <summary>
    /// Configuration for an MTConnect SHDR Client
    /// </summary>
    public interface IShdrClientConfiguration
    {
        /// <summary>
        /// The unique identifier for the Adapter
        /// </summary>
        string Id { get; }

        /// <summary>
        /// The UUID or Name of the device that corresponds to the name of the device in the Devices file.
        /// </summary>
        string DeviceKey { get; }

        /// <summary>
        /// The host the adapter is located on.
        /// </summary>
        string Hostname { get; }

        /// <summary>
        /// The port to connect to the adapter.
        /// </summary>
        int Port { get; }

        /// <summary>
        /// The Heartbeat interval (in milliseconds) that the TCP Connection will use to maintain a connection when no new data has been sent
        /// </summary>
        int Heartbeat { get; }

        /// <summary>
        /// The amount of time (in milliseconds) an adapter can be silent before it is disconnected. This is only for legacy adapters that do not support heartbeats. 
        /// If heartbeats are present, this will be ignored.
        /// </summary>
        int ConnectionTimeout { get; }

        /// <summary>
        /// The amount of time (in milliseconds) between adapter reconnection attempts. 
        /// This is useful for implementation of high performance adapters where availability needs to be tracked in near-real-time. 
        /// </summary>
        int ReconnectInterval { get; }
    }
}