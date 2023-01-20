// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
