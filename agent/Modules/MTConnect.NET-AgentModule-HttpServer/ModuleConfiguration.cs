// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Configurations
{
    /// <summary>
    /// Configuration for an MTConnect Http Agent Module
    /// </summary>
    public class ModuleConfiguration : HttpServerConfiguration
    {
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
    }
}