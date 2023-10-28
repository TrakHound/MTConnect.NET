// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1579100679936_1279_16310

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Reference system that associates a unique set of n parameters with each point in an n-dimensional space. ISO 10303-218:2004
    /// </summary>
    public class CoordinateSystem : ICoordinateSystem
    {
        public const string DescriptionText = "Reference system that associates a unique set of n parameters with each point in an n-dimensional space. ISO 10303-218:2004";


        /// <summary>
        /// Natural language description of the CoordinateSystem.
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Unique identifier for the coordinate system.
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// Name of the coordinate system.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Manufacturer's name or users name for the coordinate system.
        /// </summary>
        public string NativeName { get; set; }
        
        /// <summary>
        /// Coordinates of the origin position of a coordinate system.
        /// </summary>
        public MTConnect.UnitVector3D Origin { get; set; }
        
        /// <summary>
        /// Pointer to the id attribute of the parent CoordinateSystem.
        /// </summary>
        public string ParentIdRef { get; set; }
        
        /// <summary>
        /// Process of transforming to the origin position of the coordinate system from a parent coordinate system using Translation and Rotation.
        /// </summary>
        public MTConnect.Devices.Configurations.ITransformation Transformation { get; set; }
        
        /// <summary>
        /// Type of coordinate system.
        /// </summary>
        public MTConnect.Devices.Configurations.CoordinateSystemType Type { get; set; }
        
        /// <summary>
        /// Uuid for the coordinate system.
        /// </summary>
        public string Uuid { get; set; }
    }
}