// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Configurations
{
    /// <summary>
    /// Configuration for an MTConnect Shdr Adapter
    /// </summary>
    public interface IShdrAdapterApplicationConfiguration
    {
        string ChangeToken { get; }

        string Path { get; }


        /// <summary>
        /// Get a unique identifier for the Adapter
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// The Name or UUID of the Device to create a connection for
        /// </summary>
        string DeviceKey { get; set; }

        /// <summary>
        /// The TCP Port used for communication
        /// </summary>
        int Port { get; set; }

        /// <summary>
        /// The heartbeat used to maintain a connection between the Adapter and the Agent
        /// </summary>
        int Heartbeat { get; set; }

        /// <summary>
        /// The amount of time (in milliseconds) to allow for a connection attempt to the Agent
        /// </summary>
        int Timeout { get; set; }

        /// <summary>
        /// The interval (in milliseconds) at which new data is read from the Data Source
        /// </summary>
        int ReadInterval { get; set; }

        /// <summary>
        /// The interval (in milliseconds) at which new data is sent to the Agent
        /// </summary>
        int WriteInterval { get; set; }

        /// <summary>
        /// Use multiline Assets
        /// </summary>
        bool MultilineAssets { get; set; }

        /// <summary>
        /// Use multiline Devices
        /// </summary>
        bool MultilineDevices { get; set; }

        /// <summary>
        /// Determines whether to filter out duplicate data
        /// </summary>
        bool FilterDuplicates { get; set; }

        /// <summary>
        /// Determines whether to output Timestamps for each SHDR line
        /// </summary>
        bool OutputTimestamps { get; set; }

        /// <summary>
        /// Determines whether to send all changes to data or only most recent at the specified Interval
        /// </summary>
        bool EnableBuffer { get; set; }


        /// <summary>
        /// <summary>
        /// Changes the service name when installing or removing the service. This allows multiple Adapters to run as services on the same machine.
        /// </summary>
        string ServiceName { get; set; }

        /// <summary>
        /// Sets the Service Start Type. True = Auto | False = Manual
        /// </summary>
        bool ServiceAutoStart { get; set; }


        /// <summary>
        /// Gets or Sets whether Configuration files are monitored. If enabled and a configuration file is changed, the Adapter will restart
        /// </summary>
        bool MonitorConfigurationFiles { get; set; }

        /// <summary>
        /// Gets or Sets the minimum time (in seconds) between Adapter restarts when MonitorConfigurationFiles is enabled
        /// </summary>
        int ConfigurationFileRestartInterval { get; set; }
    }
}