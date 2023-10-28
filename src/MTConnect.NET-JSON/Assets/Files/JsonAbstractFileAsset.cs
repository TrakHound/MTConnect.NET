// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.Files
{
    public abstract class JsonAbstractFileAsset
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("mediaType")]
        public string MediaType { get; set; }

        [JsonPropertyName("applicationCategory")]
        public string ApplicationCategory { get; set; }

        [JsonPropertyName("applicationType")]
        public string ApplicationType { get; set; }

        [JsonPropertyName("fileProperties")]
        public IEnumerable<JsonFileProperty> FileProperties { get; set; }

        [JsonPropertyName("fileComments")]
        public IEnumerable<JsonFileComment> FileComments { get; set; }
    }
}