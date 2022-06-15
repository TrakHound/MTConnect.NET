// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Configurations
{
    /// <summary>
    /// Configuration for an MTConnect Http Agent
    /// </summary>
    public class HttpAgentConfiguration : AgentConfiguration
    {
        /// <summary>
        /// The port number the agent binds to for requests.
        /// </summary>
        [JsonPropertyName("port")]
        public int Port { get; set; }

        /// <summary>
        /// The server IP Address to bind to. Can be used to select the interface in IPV4 or IPV6.
        /// </summary>
        [JsonPropertyName("serverIp")]
        public string ServerIp { get; set; }

        /// <summary>
        /// Allow HTTP PUT or POST of data item values or assets.
        /// </summary>
        [JsonPropertyName("allowPut")]
        public bool AllowPut { get; set; }

        /// <summary>
        /// Allow HTTP PUT or POST from a specific host or list of hosts. 
        /// Lists are comma (,) separated and the host names will be validated by translating them into IP addresses.
        /// </summary>
        [JsonPropertyName("allowPutFrom")]
        public List<string> AllowPutFrom { get; set; }


        public HttpAgentConfiguration()
        {
            ServerIp = "127.0.0.1";
            Port = 5000;
            AllowPut = false;
            AllowPutFrom = null;
        }
    }
}
