// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Configurations
{
    /// <summary>
    /// Configuration for a MTConnect Http Client
    /// </summary>
    public interface IHttpClientConfiguration
    {
        /// <summary>
        /// The unique identifier for the Adapter
        /// </summary>
        string Id { get; }

        /// <summary>
        /// The Name or UUID of the MTConnect Device
        /// </summary>
        string DeviceKey { get; }

        /// <summary>
        /// The URL address the client MTConnect Agent is located at.
        /// </summary>
        string Address { get; }

        /// <summary>
        /// The port to connect to the client MTConnect Agent.
        /// </summary>
         int Port { get; }

        /// <summary>
        ///
        /// </summary>
        int Interval { get; }

        /// <summary>
        ///
        /// </summary>
        int Heartbeat { get; }

        /// <summary>
        ///
        /// </summary>
        bool UseSSL { get; }

        /// <summary>
        /// Gets or Sets whether the Connection Information (Host / Port) is output to the Agent to be collected by a client
        /// </summary>
        bool OutputConnectionInformation { get; }
    }
}