// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace MTConnect.Devices
{
    /// <summary>
    /// Motion defines the movement of the Component relative to a coordinate system. Motion specifies the kinematic chain of the Components.
    /// </summary>
    public class Motion
    {
        /// <summary>
        /// The unique identifier for this element.     
        /// </summary>
        [XmlAttribute("id")]
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// A pointer to the id attribute of the parent Motion.
        /// </summary>
        [XmlAttribute("parentIdRef")]
        [JsonPropertyName("parentIdRef")]
        public string ParentIdRef { get; set; }

        /// <summary>
        /// The coordinate system within which the kinematic motion occurs.
        /// </summary>
        [XmlAttribute("coordinateSystemIdRef")]
        [JsonPropertyName("coordinateSystemIdRef")]
        public string CoordinateSystemIdRef { get; set; }

        /// <summary>
        /// Describes the type of motion.    
        /// </summary>
        [XmlAttribute("type")]
        [JsonPropertyName("type")]
        public MotionType Type { get; set; }

        /// <summary>
        /// Describes if this Component is actuated directly or indirectly as a result of other motion.
        /// </summary>
        [XmlAttribute("actuation")]
        [JsonPropertyName("actuation")]
        public MotionActuationType Actuation { get; set; }

        /// <summary>
        /// An element that can contain any descriptive content.
        /// </summary>
        [XmlElement("Description")]
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// Axis defines the axis along or around which the Component moves relative to a coordinate system.
        /// </summary>
        [XmlElement("Axis")]
        [JsonPropertyName("axis")]
        public string Axis { get; set; }

        /// <summary>
        /// A fixed point from which measurement or motion commences.
        /// </summary>
        [XmlElement("Origin")]
        [JsonPropertyName("origin")]
        public string Origin { get; set; }

        /// <summary>
        /// The Transformation of the parent Origin or Transformation using Translation and Rotation.
        /// </summary>
        [XmlElement("Transformation")]
        [JsonPropertyName("transformation")]
        public Transformation Transformation { get; set; }
    }
}
