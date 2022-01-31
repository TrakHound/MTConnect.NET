// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Collections.Generic;
using System.Xml.Serialization;
using System.Text.Json.Serialization;

namespace MTConnect.Devices
{
    /// <summary>
    /// Configuration contains technical information about a component describing its physical layout,
    /// functional characteristics, and relationships with other components within a piece of equipment.
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// CoordinateSystems organizes CoordinateSystem elements for a Component and its children.
        /// </summary>
        [XmlArray("CoordinateSystems")]
        [XmlArrayItem("CoordinateSystem", typeof(CoordinateSystem))]
        [JsonPropertyName("coordinateSystem")]
        public List<CoordinateSystem> CoordinateSystems { get; set; }

        ///// <summary>
        ///// CoordinateSystems organizes CoordinateSystem elements for a Component and its children.
        ///// </summary>
        //[XmlElement("CoordinateSystem")]
        //[JsonPropertyName("coordinateSystem")]
        //public CoordinateSystem CoordinateSystem { get; set; }

        /// <summary>
        /// Motion defines the movement of the Component relative to a coordinate system.
        /// </summary>
        [XmlElement("Motion")]
        [JsonPropertyName("motion")]
        public Motion Motion { get; set; }

        /// <summary>
        /// Relationships organizes Relationship elements for a Component.
        /// </summary>
        [XmlArray("Relationships")]
        [XmlArrayItem("DataItemRelationship", typeof(DataItemRelationship))]
        [XmlArrayItem("SpecificationRelationship", typeof(SpecificationRelationship))]
        [JsonPropertyName("definition")]
        public List<object> Relationships { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public bool RelationshipsSpecified => !Relationships.IsNullOrEmpty();

        /// <summary>
        /// SensorConfiguration contains configuration information about a Sensor.
        /// </summary>
        [XmlElement("SensorConfiguration")]
        [JsonPropertyName("sensorConfiguration")]
        public SensorConfiguration SensorConfiguration { get; set; }

        /// <summary>
        /// SolidModel references a file with the three-dimensional geometry of the Component or Composition.
        /// </summary>
        [XmlElement("SolidModel")]
        [JsonPropertyName("solidModel")]
        public SolidModel SolidModel { get; set; }

        /// <summary>
        /// Specifications organizes Specification elements for a Component. 
        /// </summary>
        [XmlArray("Specifications")]
        [XmlArrayItem("Specification", typeof(Specification))]
        [XmlArrayItem("ProcessSpecification", typeof(ProcessSpecification))]
        [JsonPropertyName("specifications")]
        public List<object> Specifications { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public bool SpecificationsSpecified => !Specifications.IsNullOrEmpty();
    }
}
