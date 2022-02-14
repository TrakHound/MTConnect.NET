// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Collections.Generic;
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
        [JsonPropertyName("coordinateSystem")]
        public List<CoordinateSystem> CoordinateSystems { get; set; }

        /// <summary>
        /// Motion defines the movement of the Component relative to a coordinate system.
        /// </summary>
        [JsonPropertyName("motion")]
        public Motion Motion { get; set; }

        /// <summary>
        /// Relationships organizes Relationship elements for a Component.
        /// </summary>
        [JsonPropertyName("definition")]
        public List<Relationship> Relationships { get; set; }

        /// <summary>
        /// SensorConfiguration contains configuration information about a Sensor.
        /// </summary>
        [JsonPropertyName("sensorConfiguration")]
        public SensorConfiguration SensorConfiguration { get; set; }

        /// <summary>
        /// SolidModel references a file with the three-dimensional geometry of the Component or Composition.
        /// </summary>
        [JsonPropertyName("solidModel")]
        public SolidModel SolidModel { get; set; }

        /// <summary>
        /// Specifications organizes Specification elements for a Component. 
        /// </summary>
        [JsonPropertyName("specifications")]
        public List<AbstractSpecification> Specifications { get; set; }
    }
}
