// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Configurations
{
    /// <summary>
    /// Configuration for an MTConnect Mqtt Gateway Agent Application
    /// </summary>
    public class MqttRelayAgentGatewayApplicationConfiguration : MqttRelayAgentApplicationConfiguration
    {
        /// <summary>
        /// List of MTConnect Http Clients to read from
        /// </summary>
        [JsonPropertyName("clients")]
        public List<HttpClientConfiguration> Clients { get; set; }
    }
}