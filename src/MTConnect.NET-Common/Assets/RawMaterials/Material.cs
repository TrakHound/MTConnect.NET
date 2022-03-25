// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;
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
        public string Id { get; set; }

        /// <summary>
        /// The name of the material. 
        /// Examples: ULTM9085, ABS, 4140.
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// The type of material. 
        /// Examples: Metal, Polymer, Wood, 4140, Recycled, Prestine and Used.
        /// </summary>
        [XmlAttribute("type")]
        public string Type { get; set; }

        /// <summary>
        /// The manufacturer’s lot code of the material.
        /// </summary>
        [XmlElement("Lot")]
        public string Lot { get; set; }

        /// <summary>
        /// The name of the material manufacturer.
        /// </summary>
        [XmlElement("Manufacturer")]
        public string Manufacturer { get; set; }

        /// <summary>
        /// The manufacturing date of the material from the material manufacturer.
        /// </summary>
        [XmlElement("ManufacturingDate")]
        public DateTime ManufacturingDate { get; set; }

        /// <summary>
        /// The lot code of the raw feed stock for the material, from the feed stock manufacturer.
        /// </summary>
        [XmlElement("ManufacturingCode")]
        public string ManufacturingCode { get; set; }

        /// <summary>
        /// The ASTM standard code that the material complies with.
        /// </summary>
        [XmlElement("MaterialCode")]
        public string MaterialCode { get; set; }
    }
}
