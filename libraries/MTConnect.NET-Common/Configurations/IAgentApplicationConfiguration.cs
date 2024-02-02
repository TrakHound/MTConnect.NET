// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace MTConnect.Configurations
{
    /// <summary>
    /// Configuration for an MTConnect Shdr > Http Agent
    /// </summary>
    public interface IAgentApplicationConfiguration : IAgentConfiguration
    {
        /// <summary>
        /// The Path to look for the file(s) that represent the Device Information Models to load into the Agent.
        /// The path can either be a single file or a directory. The path can be absolute or relative to the executable's directory
        /// </summary>
        string Devices { get; set; }


        /// <summary>
        /// Changes the service name when installing or removing the service. This allows multiple agents to run as services on the same machine.
        /// </summary>
        string ServiceName { get; set; }

        /// <summary>
        /// Sets the Service Start Type. True = Auto | False = Manual
        /// </summary>
        bool ServiceAutoStart { get; set; }


        /// <summary>
        /// Gets or Sets whether the Agent buffers are durable and retain state after restart
        /// </summary>
        bool Durable { get; set; }

        /// <summary>
        /// Gets or Sets whether the durable Agent buffers use Compression
        /// </summary>
        bool UseBufferCompression { get; set; }


        /// <summary>
        /// Gets or Sets whether Configuration files are monitored. If enabled and a configuration file is changed, the Agent will restart
        /// </summary>
        bool MonitorConfigurationFiles { get; set; }

        /// <summary>
        /// Gets or Sets the minimum time (in seconds) between Agent restarts when MonitorConfigurationFiles is enabled
        /// </summary>
        int ConfigurationFileRestartInterval { get; set; }


        IEnumerable<object> Modules { get; set; }

        IEnumerable<object> Processors { get; set; }


        Dictionary<object, object> GetModules();

        IEnumerable<object> GetModules(string key);

        IEnumerable<TConfiguration> GetModules<TConfiguration>(string key);

        bool IsModuleConfigured(string key);


		Dictionary<object, object> GetProcessors();

        IEnumerable<object> GetProcessors(string key);

        IEnumerable<TConfiguration> GetProcessors<TConfiguration>(string key);
    }
}