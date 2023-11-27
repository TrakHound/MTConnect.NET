// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Configurations
{
    /// <summary>
    /// Configuration for an MTConnect SHDR Agent
    /// </summary>
    public class ShdrAgentConfiguration : HttpAgentConfiguration, IShdrAgentConfiguration
    {
        /// <summary>
        /// Do not overwrite the UUID with the UUID from the adapter, preserve the UUID for the Device. 
        /// This can be overridden on a per adapter basis.
        /// </summary>
        [JsonPropertyName("preserveUuid")]
        public bool PreserveUuid { get; set; }

        /// <summary>
        /// Suppress the Adapter IP Address and port when creating the Agent Device ids and names for 1.7. This applies to all adapters.
        /// </summary>
        [JsonPropertyName("suppressIpAddress")]
        public bool SuppressIpAddress { get; set; }

        /// <summary>
        /// The amount of time (in milliseconds) an adapter can be silent before it is disconnected. 
        /// </summary>
        [JsonPropertyName("timeout")]
        public int Timeout { get; set; }

        /// <summary>
        /// The amount of time (in milliseconds) between adapter reconnection attempts. 
        /// </summary>
        [JsonPropertyName("reconnectInterval")]
        public int ReconnectInterval { get; set; }

        /// <summary>
        /// Gets or Sets whether a Device Model can be sent from an SHDR Adapter
        /// </summary>
        [JsonPropertyName("allowShdrDevice")]
        public bool AllowShdrDevice { get; set; }

        /// <summary>
        /// List of SHDR Adapter connection configurations
        /// </summary>
        [JsonPropertyName("adapters")]
        public IEnumerable<ShdrAdapterClientConfiguration> Adapters { get; set; }
    }
}