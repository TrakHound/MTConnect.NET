// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations.CoordinateSystems;
using MTConnect.Devices.Configurations.ImageFiles;
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
		/// ImageFiles groups one or more ImageFile entities 
		/// </summary>
		public IEnumerable<IImageFile> ImageFiles { get; set; }

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