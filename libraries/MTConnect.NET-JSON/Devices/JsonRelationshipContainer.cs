// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate that groups the related entities of a
    /// component, composition, or data item by relationship kind. Each kind is
    /// serialized as a separate JSON array, populated only when there is at
    /// least one relationship of that kind.
    /// </summary>
    public class JsonRelationshipContainer
    {
        /// <summary>
        /// The relationships pointing at related assets.
        /// </summary>
        [JsonPropertyName("assetRelationships")]
        public List<JsonRelationship> AssetRelationships { get; set; }

        /// <summary>
        /// The relationships pointing at related data items.
        /// </summary>
        [JsonPropertyName("dataItemRelationships")]
        public List<JsonRelationship> DataItemRelationships { get; set; }

        /// <summary>
        /// The relationships pointing at related components.
        /// </summary>
        [JsonPropertyName("componentRelationships")]
        public List<JsonRelationship> ComponentRelationships { get; set; }

        /// <summary>
        /// The relationships pointing at related devices.
        /// </summary>
        [JsonPropertyName("deviceRelationships")]
        public List<JsonRelationship> DeviceRelationships { get; set; }

        /// <summary>
        /// The relationships pointing at related specifications.
        /// </summary>
        [JsonPropertyName("specificationRelationships")]
        public List<JsonRelationship> SpecificationRelationships { get; set; }


        /// <summary>
        /// Initializes an empty container.
        /// </summary>
        public JsonRelationshipContainer()
        {
            //DataItemRelationships = new List<JsonRelationship>();
            //ComponentRelationships = new List<JsonRelationship>();
            //DeviceRelationships = new List<JsonRelationship>();
            //SpecificationRelationships = new List<JsonRelationship>();
        }
    }
}