// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace MTConnect.Configurations
{
    /// <summary>
    /// Default <see cref="IStyleConfiguration"/> implementation, also
    /// serializable to JSON so the response document stylesheet can be
    /// supplied through agent configuration files.
    /// </summary>
    public class StyleConfiguration : IStyleConfiguration
    {
        /// <summary>
        /// The URL location of the stylesheet emitted in the document's
        /// stylesheet processing instruction.
        /// </summary>
        [JsonPropertyName("location")]
        public string Location { get; set; }

        /// <summary>
        /// The local file system path to the stylesheet when it is served by the agent.
        /// </summary>
        [JsonPropertyName("path")]
        public string Path { get; set; }
    }
}