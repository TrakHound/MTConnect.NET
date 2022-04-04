// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Text.Json.Serialization;

namespace MTConnect.Agents.Configuration
{
    public class StyleConfiguration
    {
        [JsonPropertyName("location")]
        public string Location { get; set; }

        [JsonPropertyName("path")]
        public string Path { get; set; }
    }
}
