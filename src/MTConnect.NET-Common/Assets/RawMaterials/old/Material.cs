// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.RawMaterials
{
    /// <summary>
    /// Material used as the raw material.
    /// </summary>
    public class Material
    {
        /// <summary>
        /// The unique identifier for the material.
        /// </summary>
        [XmlAttribute("id")]
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The name of the material. 
        /// Examples: ULTM9085, ABS, 4140.
        /// </summary>
        [XmlAttribute("name")]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The type of material. 
        /// Examples: Metal, Polymer, Wood, 4140, Recycled, Prestine and Used.
        /// </summary>
        [XmlAttribute("type")]
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The manufacturer’s lot code of the material.
        /// </summary>
        [XmlElement("Lot")]
        [JsonPropertyName("lot")]
        public string Lot { get; set; }

        /// <summary>
        /// The name of the material manufacturer.
        /// </summary>
        [XmlElement("Manufacturer")]
        [JsonPropertyName("manufacturer")]
        public string Manufacturer { get; set; }

        /// <summary>
        /// The manufacturing date of the material from the material manufacturer.
        /// </summary>
        [XmlElement("ManufacturingDate")]
        [JsonPropertyName("manufacturingDate")]
        public DateTime ManufacturingDate { get; set; }

        /// <summary>
        /// The lot code of the raw feed stock for the material, from the feed stock manufacturer.
        /// </summary>
        [XmlElement("ManufacturingCode")]
        [JsonPropertyName("manufacturingCode")]
        public string ManufacturingCode { get; set; }

        /// <summary>
        /// The ASTM standard code that the material complies with.
        /// </summary>
        [XmlElement("MaterialCode")]
        [JsonPropertyName("materialCode")]
        public string MaterialCode { get; set; }
    }
}