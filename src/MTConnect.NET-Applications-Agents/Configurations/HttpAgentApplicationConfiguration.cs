// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Configurations
{
    /// <summary>
    /// Configuration for an MTConnect Http Agent Application
    /// </summary>
    public class HttpAgentApplicationConfiguration : HttpAgentConfiguration, IHttpAgentApplicationConfiguration
    {
        /// <summary>
        /// The Path to look for the file(s) that represent the Device Information Models to load into the Agent.
        /// The path can either be a single file or a directory. The path can be absolute or relative to the executable's directory
        /// </summary>
        [JsonPropertyName("devices")]
        public string Devices { get; set; }


        /// <summary>
        /// Changes the service name when installing or removing the service. This allows multiple agents to run as services on the same machine.
        /// </summary>
        [JsonPropertyName("serviceName")]
        public string ServiceName { get; set; }

        /// <summary>
        /// Sets the Service Start Type. True = Auto | False = Manual
        /// </summary>
        [JsonPropertyName("serviceAutoStart")]
        public bool ServiceAutoStart { get; set; }


        /// <summary>
        /// Gets or Sets whether the Agent buffers are durable and retain state after restart
        /// </summary>
        [JsonPropertyName("durable")]
        public bool Durable { get; set; }

        /// <summary>
        /// Gets or Sets whether the durable Agent buffers use Compression
        /// </summary>
        [JsonPropertyName("useBufferCompression")]
        public bool UseBufferCompression { get; set; }


        /// <summary>
        /// Gets or Sets whether Configuration files are monitored. If enabled and a configuration file is changed, the Agent will restart
        /// </summary>
        [JsonPropertyName("monitorConfigurationFiles")]
        public bool MonitorConfigurationFiles { get; set; }

        /// <summary>
        /// Gets or Sets the minimum time (in seconds) between Agent restarts when MonitorConfigurationFiles is enabled
        /// </summary>
        [JsonPropertyName("configurationFileRestartInterval")]
        public int ConfigurationFileRestartInterval { get; set; }


        [JsonPropertyName("devicesNamespaces")]
        public IEnumerable<NamespaceConfiguration> DevicesNamespaces { get; set; }

        [JsonPropertyName("streamsNamespaces")]
        public IEnumerable<NamespaceConfiguration> StreamsNamespaces { get; set; }

        [JsonPropertyName("assetsNamespaces")]
        public IEnumerable<NamespaceConfiguration> AssetsNamespaces { get; set; }

        [JsonPropertyName("errorNamespaces")]
        public IEnumerable<NamespaceConfiguration> ErrorNamespaces { get; set; }


        [JsonPropertyName("devicesStyle")]
        public StyleConfiguration DevicesStyle { get; set; }

        [JsonPropertyName("streamsStyle")]
        public StyleConfiguration StreamsStyle { get; set; }

        [JsonPropertyName("assetsStyle")]
        public StyleConfiguration AssetsStyle { get; set; }

        [JsonPropertyName("errorStyle")]
        public StyleConfiguration ErrorStyle { get; set; }


        [JsonPropertyName("controllers")]
        public IEnumerable<object> Controllers { get; set; }


        public HttpAgentApplicationConfiguration() : base()
        {
            Devices = null;
            ServiceName = null;
            ServiceAutoStart = true;
            MonitorConfigurationFiles = true;
            ConfigurationFileRestartInterval = 2;
        }
    }
}