// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
            var configurationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Filename);
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
