// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace MTConnect.Configurations
{
    /// <summary>
    /// Default <see cref="INamespaceConfiguration"/> implementation, also
    /// serializable to JSON so the extended schema namespace can be supplied
    /// through agent configuration files.
    /// </summary>
    public class NamespaceConfiguration : INamespaceConfiguration
    {
        /// <summary>
        /// The alias that will be used to reference the extended schema
        /// </summary>
        [JsonPropertyName("alias")]
        public string Alias { get; set; }

        /// <summary>
        /// The URN that uniquely identifies the extended schema namespace.
        /// </summary>
        [JsonPropertyName("urn")]
        public string Urn { get; set; }

        /// <summary>
        /// The location of the xsd file relative in the agent namespace
        /// </summary>
        [JsonPropertyName("location")]
        public string Location { get; set; }

        /// <summary>
        /// The local file system path to the xsd file when it is served by the agent.
        /// </summary>
        [JsonPropertyName("path")]
        public string Path { get; set; }
    }
}