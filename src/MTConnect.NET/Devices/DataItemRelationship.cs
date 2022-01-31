// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Xml.Serialization;
using System.Text.Json.Serialization;

namespace MTConnect.Devices
{
    public class DataItemRelationship : Relationship
    {
        ///// <summary>
        ///// A descriptive name associated with this Relationship.
        ///// </summary>
        //[XmlAttribute("name")]
        //[JsonPropertyName("name")]
        //public string Name { get; set; }

        /// <summary>
        /// Specifies how the DataItem is related.
        /// </summary>
        [XmlAttribute("type")]
        [JsonPropertyName("type")]
        public DataItemRelationshipType Type { get; set; }

        ///// <summary>
        ///// A reference to the related DataItem id.
        ///// </summary>
        //[XmlAttribute("idRef")]
        //[JsonPropertyName("idRef")]
        //public string IdRef { get; set; }
    }
}
