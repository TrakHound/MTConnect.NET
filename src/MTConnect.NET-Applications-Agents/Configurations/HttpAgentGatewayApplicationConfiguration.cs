// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Configurations
{
    /// <summary>
    /// Configuration for an MTConnect Http Gateway Agent Application
    /// </summary>
    public class HttpAgentGatewayApplicationConfiguration : HttpAgentConfiguration
    {
        /// <summary>
        /// List of MTConnect Http Clients to read from
        /// </summary>
        [JsonPropertyName("clients")]
        public List<HttpClientConfiguration> Clients { get; set; }


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
        /// Gets or Sets whether Configuration files are monitored. If enabled and a configuration file is changed, the Agent will restart
        /// </summary>
        [JsonPropertyName("monitorConfigurationFiles")]
        public bool MonitorConfigurationFiles { get; set; }

        /// <summary>
        /// Gets or Sets the minimum time (in seconds) between Agent restarts when MonitorConfigurationFiles is enabled
        /// </summary>
        [JsonPropertyName("configurationFileRestartInterval")]
        public int ConfigurationFileRestartInterval { get; set; }


        public HttpAgentGatewayApplicationConfiguration() : base()
        {
            Clients = null;
            ServiceName = null;
            ServiceAutoStart = true;
            MonitorConfigurationFiles = true;
            ConfigurationFileRestartInterval = 2;
        }
    }
}
