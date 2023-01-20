// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Text.Json.Serialization;

namespace MTConnect.Configurations
{
    public class NamespaceConfiguration : INamespaceConfiguration
    {
        /// <summary>
        /// The alias that will be used to reference the extended schema
        /// </summary>
        [JsonPropertyName("alias")]
        public string Alias { get; set; }

        [JsonPropertyName("urn")]
        public string Urn { get; set; }

        /// <summary>
        /// The location of the xsd file relative in the agent namespace
        /// </summary>
        [JsonPropertyName("location")]
        public string Location { get; set; }

        [JsonPropertyName("path")]
        public string Path { get; set; }
    }
}
