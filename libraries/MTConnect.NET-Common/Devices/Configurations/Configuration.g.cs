// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = EAID_C04DCC77_16E8_4cef_92D4_B777AFC52570

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Technical information about an entity describing its physical layout, functional characteristics, and relationships with other entities.
    /// </summary>
    public class Configuration : IConfiguration
    {
        public const string DescriptionText = "Technical information about an entity describing its physical layout, functional characteristics, and relationships with other entities.";


        /// <summary>
        /// Reference system that associates a unique set of n parameters with each point in an n-dimensional space. ISO 10303-218:2004
        /// </summary>
        public System.Collections.Generic.IEnumerable<MTConnect.Devices.Configurations.ICoordinateSystem> CoordinateSystems { get; set; }
        
        /// <summary>
        /// Reference to a file containing an image of the Component.
        /// </summary>
        public System.Collections.Generic.IEnumerable<MTConnect.Devices.Configurations.IImageFile> ImageFiles { get; set; }
        
        /// <summary>
        /// Movement of the component relative to a coordinate system.
        /// </summary>
        public MTConnect.Devices.Configurations.IMotion Motion { get; set; }
        
        /// <summary>
        /// Potential energy sources for the Component.
        /// </summary>
        public MTConnect.Devices.Configurations.IPowerSource PowerSource { get; set; }
        
        /// <summary>
        /// Association between two pieces of equipment that function independently but together perform a manufacturing operation.
        /// </summary>
        public System.Collections.Generic.IEnumerable<MTConnect.Devices.Configurations.IConfigurationRelationship> Relationships { get; set; }
        
        /// <summary>
        /// Configuration for a Sensor.
        /// </summary>
        public MTConnect.Devices.Configurations.ISensorConfiguration SensorConfiguration { get; set; }
        
        /// <summary>
        /// References to a file with the three-dimensional geometry of the Component or Composition.
        /// </summary>
        public MTConnect.Devices.Configurations.ISolidModel SolidModel { get; set; }
        
        /// <summary>
        /// Design characteristics for a piece of equipment.
        /// </summary>
        public System.Collections.Generic.IEnumerable<MTConnect.Devices.Configurations.ISpecification> Specifications { get; set; }
    }
}