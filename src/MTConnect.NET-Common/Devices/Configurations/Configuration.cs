// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices.Configurations.CoordinateSystems;
using MTConnect.Devices.Configurations.Motion;
using MTConnect.Devices.Configurations.Relationships;
using MTConnect.Devices.Configurations.Sensor;
using MTConnect.Devices.Configurations.SolidModel;
using MTConnect.Devices.Configurations.Specifications;
using System.Collections.Generic;

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Configuration contains technical information about a component describing its physical layout,
    /// functional characteristics, and relationships with other components within a piece of equipment.
    /// </summary>
    public class Configuration : IConfiguration
    {
        public const string DescriptionText = "Configuration contains technical information about a component describing its physical layout, functional characteristics, and relationships with other components within a piece of equipment.";


        /// <summary>
        /// CoordinateSystems organizes CoordinateSystem elements for a Component and its children.
        /// </summary>
        public IEnumerable<ICoordinateSystem> CoordinateSystems { get; set; }

        /// <summary>
        /// Motion defines the movement of the Component relative to a coordinate system.
        /// </summary>
        public IMotion Motion { get; set; }

        /// <summary>
        /// Relationships organizes Relationship elements for a Component.
        /// </summary>
        public IEnumerable<IRelationship> Relationships { get; set; }

        /// <summary>
        /// SensorConfiguration contains configuration information about a Sensor.
        /// </summary>
        public ISensorConfiguration SensorConfiguration { get; set; }

        /// <summary>
        /// SolidModel references a file with the three-dimensional geometry of the Component or Composition.
        /// </summary>
        public ISolidModel SolidModel { get; set; }

        /// <summary>
        /// Specifications organizes Specification elements for a Component. 
        /// </summary>
        public IEnumerable<IAbstractSpecification> Specifications { get; set; }
    }
}
