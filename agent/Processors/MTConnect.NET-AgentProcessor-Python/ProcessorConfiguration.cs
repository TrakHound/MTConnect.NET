// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace MTConnect.Processors
{
    public class ProcessorConfiguration
    {
        [JsonPropertyName("directory")]
        public string Directory { get; set; }
    }
}
