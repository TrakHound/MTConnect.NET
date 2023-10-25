// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    public static class ConfigurationDescriptions
    {
        /// <summary>
        /// Reference system that associates a unique set of n parameters with each point in an n-dimensional space. ISO 10303-218:2004
        /// </summary>
        public const string CoordinateSystems = "Reference system that associates a unique set of n parameters with each point in an n-dimensional space. ISO 10303-218:2004";
        
        /// <summary>
        /// Reference to a file containing an image of the Component.
        /// </summary>
        public const string ImageFiles = "Reference to a file containing an image of the Component.";
        
        /// <summary>
        /// Movement of the component relative to a coordinate system.
        /// </summary>
        public const string Motion = "Movement of the component relative to a coordinate system.";
        
        /// <summary>
        /// Association between two pieces of equipment that function independently but together perform a manufacturing operation.
        /// </summary>
        public const string Relationships = "Association between two pieces of equipment that function independently but together perform a manufacturing operation.";
        
        /// <summary>
        /// Configuration for a Sensor.
        /// </summary>
        public const string SensorConfiguration = "Configuration for a Sensor.";
        
        /// <summary>
        /// References to a file with the three-dimensional geometry of the Component or Composition.
        /// </summary>
        public const string SolidModel = "References to a file with the three-dimensional geometry of the Component or Composition.";
        
        /// <summary>
        /// Design characteristics for a piece of equipment.
        /// </summary>
        public const string Specifications = "Design characteristics for a piece of equipment.";
    }
}