// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Xml.Serialization;
using System.Text.Json.Serialization;

namespace MTConnect.Devices
{
    /// <summary>
    /// ComponentRelationship describes the association between two components within a piece of equipment that function independently but together perform a capability or service within a piece of equipment.
    /// </summary>
    public class ComponentRelationship : Relationship, IComponentRelationship
    {
        /// <summary>
        /// Defines the authority that this component element has relative to the associated component element.
        /// </summary>
        [XmlAttribute("type")]
        [JsonPropertyName("type")]
        public ComponentRelationshipType Type { get; set; }
    }
}
