// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonRelationshipContainer
    {
        [JsonPropertyName("AssetRelationship")]
        public List<JsonRelationship> AssetRelationships { get; set; }

        [JsonPropertyName("DataItemRelationship")]
        public List<JsonRelationship> DataItemRelationships { get; set; }

        [JsonPropertyName("ComponentRelationship")]
        public List<JsonRelationship> ComponentRelationships { get; set; }

        [JsonPropertyName("DeviceRelationship")]
        public List<JsonRelationship> DeviceRelationships { get; set; }

        [JsonPropertyName("SpecificationRelationship")]
        public List<JsonRelationship> SpecificationRelationships { get; set; }
    }
}