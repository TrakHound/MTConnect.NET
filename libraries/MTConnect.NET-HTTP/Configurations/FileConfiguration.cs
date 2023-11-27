// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace MTConnect.Configurations
{
    /// <summary>
    /// Configuration for Static Files that can served from an Http Server
    /// </summary>
    public class FileConfiguration : IFileConfiguration
    {
        /// <summary>
        /// The location of the files on the server (where the Agent is running)
        /// </summary>
        [JsonPropertyName("path")]
        public string Path { get; set; }

        /// <summary>
        /// The path to match in the requested URL
        /// </summary>
        [JsonPropertyName("location")]
        public string Location { get; set; }
    }
}