// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate that partitions a component's
    /// configuration relationships by kind into typed sibling lists.
    /// The cppagent shape keys each list by the relationship element
    /// name (<c>AssetRelationship</c>, <c>DataItemRelationship</c>,
    /// <c>ComponentRelationship</c>, <c>DeviceRelationship</c>,
    /// <c>SpecificationRelationship</c>), so this container exposes one
    /// list per kind and only the lists with content are populated.
    /// </summary>
    public class JsonRelationshipContainer
    {
        /// <summary>
        /// Relationships to assets.
        /// </summary>
        [JsonPropertyName("AssetRelationship")]
        public List<JsonRelationship> AssetRelationships { get; set; }

        /// <summary>
        /// Relationships to data items.
        /// </summary>
        [JsonPropertyName("DataItemRelationship")]
        public List<JsonRelationship> DataItemRelationships { get; set; }

        /// <summary>
        /// Relationships to other components.
        /// </summary>
        [JsonPropertyName("ComponentRelationship")]
        public List<JsonRelationship> ComponentRelationships { get; set; }

        /// <summary>
        /// Relationships to other devices.
        /// </summary>
        [JsonPropertyName("DeviceRelationship")]
        public List<JsonRelationship> DeviceRelationships { get; set; }

        /// <summary>
        /// Relationships to specifications.
        /// </summary>
        [JsonPropertyName("SpecificationRelationship")]
        public List<JsonRelationship> SpecificationRelationships { get; set; }
    }
}