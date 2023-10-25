// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    public static class CoordinateSystemDescriptions
    {
        /// <summary>
        /// Natural language description of the CoordinateSystem.
        /// </summary>
        public const string Description = "Natural language description of the CoordinateSystem.";
        
        /// <summary>
        /// Unique identifier for the coordinate system.
        /// </summary>
        public const string Id = "Unique identifier for the coordinate system.";
        
        /// <summary>
        /// Name of the coordinate system.
        /// </summary>
        public const string Name = "Name of the coordinate system.";
        
        /// <summary>
        /// Manufacturer's name or users name for the coordinate system.
        /// </summary>
        public const string NativeName = "Manufacturer's name or users name for the coordinate system.";
        
        /// <summary>
        /// Coordinates of the origin position of a coordinate system.
        /// </summary>
        public const string Origin = "Coordinates of the origin position of a coordinate system.";
        
        /// <summary>
        /// Pointer to the id attribute of the parent CoordinateSystem.
        /// </summary>
        public const string ParentIdRef = "Pointer to the id attribute of the parent CoordinateSystem.";
        
        /// <summary>
        /// Process of transforming to the origin position of the coordinate system from a parent coordinate system using Translation and Rotation.
        /// </summary>
        public const string Transformation = "Process of transforming to the origin position of the coordinate system from a parent coordinate system using Translation and Rotation.";
        
        /// <summary>
        /// Type of coordinate system.
        /// </summary>
        public const string Type = "Type of coordinate system.";
        
        /// <summary>
        /// Uuid for the coordinate system.
        /// </summary>
        public const string Uuid = "Uuid for the coordinate system.";
    }
}