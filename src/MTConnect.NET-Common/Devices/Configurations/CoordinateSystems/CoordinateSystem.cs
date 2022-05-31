// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Xml.Serialization;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Configurations.CoordinateSystems
{
    /// <summary>
    /// A CoordinateSystem is a reference system that associates a unique set of n parameters with each point in an n-dimensional space.
    /// </summary>
    public class CoordinateSystem : ICoordinateSystem
    {
        /// <summary>
        /// The unique identifier for this element.    
        /// </summary>
        [XmlAttribute("id")]
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The name of the coordinate system.
        /// </summary>
        [XmlAttribute("name")]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The manufacturer’s name or users name for the coordinate system.
        /// </summary>
        [XmlAttribute("nativeName")]
        [JsonPropertyName("nativeName")]
        public string NativeName { get; set; }

        /// <summary>
        /// A pointer to the id attribute of the parent CoordinateSystem.
        /// </summary>
        [XmlAttribute("parentIdRef")]
        [JsonPropertyName("parentIdRef")]
        public string ParentIdRef { get; set; }

        /// <summary>
        /// The type of coordinate system.
        /// </summary>
        [XmlAttribute("type")]
        [JsonPropertyName("type")]
        public CoordinateSystemType Type { get; set; }

        /// <summary>
        /// The type of coordinate system.
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public string TypeDescription => CoordinateSystemTypeDescriptions.Get(Type);

        /// <summary>
        /// The coordinates of the origin position of a coordinate system.
        /// </summary>
        [XmlElement("Origin")]
        [JsonPropertyName("origin")]
        public string Origin { get; set; }

        /// <summary>
        /// The process of transforming to the origin position of the coordinate system from a parent coordinate system using Translation and Rotation.
        /// </summary>
        [XmlElement("Transformation")]
        [JsonPropertyName("transformation")]
        public Transformation Transformation { get; set; }

        /// <summary>
        /// The natural language description of the CoordinateSystem.
        /// </summary>
        [XmlElement("Description")]
        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}
