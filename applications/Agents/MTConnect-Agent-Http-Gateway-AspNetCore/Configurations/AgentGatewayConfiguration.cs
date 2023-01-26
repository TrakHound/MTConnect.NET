// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MTConnect.Configurations
{
    public class AgentGatewayConfiguration : HttpAgentConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("clients")]
        public List<HttpClientConfiguration> Clients { get; set; }


        public new static AgentGatewayConfiguration Read(string path = null)
        {
            var configurationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, JsonFilename);
            if (path != null) configurationPath = path;

            if (!string.IsNullOrEmpty(configurationPath))
            {
                try
                {
                    var text = File.ReadAllText(configurationPath);
                    if (!string.IsNullOrEmpty(text))
                    {
                        return JsonSerializer.Deserialize<AgentGatewayConfiguration>(text);
                    }
                }
                catch { }
            }

            return null;
        }
    }
}
