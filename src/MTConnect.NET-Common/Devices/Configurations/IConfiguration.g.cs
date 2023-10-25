// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Technical information about an entity describing its physical layout, functional characteristics, and relationships with other entities.
    /// </summary>
    public interface IConfiguration
    {
        /// <summary>
        /// Reference system that associates a unique set of n parameters with each point in an n-dimensional space. ISO 10303-218:2004
        /// </summary>
        System.Collections.Generic.IEnumerable<MTConnect.Devices.Configurations.ICoordinateSystem> CoordinateSystems { get; }
        
        /// <summary>
        /// Reference to a file containing an image of the Component.
        /// </summary>
        System.Collections.Generic.IEnumerable<MTConnect.Devices.Configurations.IImageFile> ImageFiles { get; }
        
        /// <summary>
        /// Movement of the component relative to a coordinate system.
        /// </summary>
        MTConnect.Devices.Configurations.IMotion Motion { get; }
        
        /// <summary>
        /// Association between two pieces of equipment that function independently but together perform a manufacturing operation.
        /// </summary>
        System.Collections.Generic.IEnumerable<MTConnect.Devices.Configurations.IRelationship> Relationships { get; }
        
        /// <summary>
        /// Configuration for a Sensor.
        /// </summary>
        MTConnect.Devices.Configurations.ISensorConfiguration SensorConfiguration { get; }
        
        /// <summary>
        /// References to a file with the three-dimensional geometry of the Component or Composition.
        /// </summary>
        MTConnect.Devices.Configurations.ISolidModel SolidModel { get; }
        
        /// <summary>
        /// Design characteristics for a piece of equipment.
        /// </summary>
        System.Collections.Generic.IEnumerable<MTConnect.Devices.Configurations.ISpecification> Specifications { get; }
    }
}