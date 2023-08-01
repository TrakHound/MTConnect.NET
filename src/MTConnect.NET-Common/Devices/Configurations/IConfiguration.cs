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
	public interface IConfiguration
    {
        /// <summary>
        /// CoordinateSystems organizes CoordinateSystem elements for a Component and its children.
        /// </summary>
        IEnumerable<ICoordinateSystem> CoordinateSystems { get; }

		/// <summary>
		/// ImageFiles groups one or more ImageFile entities 
		/// </summary>
		IEnumerable<IImageFile> ImageFiles { get; }

		/// <summary>
		/// Motion defines the movement of the Component relative to a coordinate system.
		/// </summary>
		IMotion Motion { get; }

        /// <summary>
        /// Relationships organizes Relationship elements for a Component.
        /// </summary>
        IEnumerable<IRelationship> Relationships { get; }

        /// <summary>
        /// SensorConfiguration contains configuration information about a Sensor.
        /// </summary>
        ISensorConfiguration SensorConfiguration { get; }

        /// <summary>
        /// SolidModel references a file with the three-dimensional geometry of the Component or Composition.
        /// </summary>
        ISolidModel SolidModel { get; }

        /// <summary>
        /// Specifications organizes Specification elements for a Component. 
        /// </summary>
        IEnumerable<IAbstractSpecification> Specifications { get; }
	}
}