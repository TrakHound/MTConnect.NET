// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Configurations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MTConnect.Configurations
{
    public class AgentAdapterConfiguration : HttpAgentConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("clients")]
        public List<ClientConfiguration> Clients { get; set; }



        public new static AgentAdapterConfiguration Read(string path = null)
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
                        return JsonSerializer.Deserialize<AgentAdapterConfiguration>(text);
                    }
                }
                catch { }
            }

            return null;
        }
    }
}