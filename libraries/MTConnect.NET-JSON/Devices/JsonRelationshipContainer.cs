// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonRelationshipContainer
    {
        [JsonPropertyName("assetRelationships")]
        public List<JsonRelationship> AssetRelationships { get; set; }

        [JsonPropertyName("dataItemRelationships")]
        public List<JsonRelationship> DataItemRelationships { get; set; }

        [JsonPropertyName("componentRelationships")]
        public List<JsonRelationship> ComponentRelationships { get; set; }

        [JsonPropertyName("deviceRelationships")]
        public List<JsonRelationship> DeviceRelationships { get; set; }

        [JsonPropertyName("specificationRelationships")]
        public List<JsonRelationship> SpecificationRelationships { get; set; }


        public JsonRelationshipContainer()
        {
            //DataItemRelationships = new List<JsonRelationship>();
            //ComponentRelationships = new List<JsonRelationship>();
            //DeviceRelationships = new List<JsonRelationship>();
            //SpecificationRelationships = new List<JsonRelationship>();
        }
    }
}