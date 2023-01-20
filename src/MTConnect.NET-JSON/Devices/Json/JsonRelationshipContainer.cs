// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonRelationshipContainer
    {
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
