// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace MTConnect.Configurations
{
    public class StyleConfiguration : IStyleConfiguration
    {
        [JsonPropertyName("location")]
        public string Location { get; set; }

        [JsonPropertyName("path")]
        public string Path { get; set; }
    }
}