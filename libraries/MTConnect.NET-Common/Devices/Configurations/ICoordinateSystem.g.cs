// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Reference system that associates a unique set of n parameters with each point in an n-dimensional space. ISO 10303-218:2004
    /// </summary>
    public interface ICoordinateSystem
    {
        /// <summary>
        /// Natural language description of the CoordinateSystem.
        /// </summary>
        string Description { get; }
        
        /// <summary>
        /// Unique identifier for the coordinate system.
        /// </summary>
        string Id { get; }
        
        /// <summary>
        /// Name of the coordinate system.
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Manufacturer's name or users name for the coordinate system.
        /// </summary>
        string NativeName { get; }
        
        /// <summary>
        /// Coordinates of the origin position of a coordinate system.
        /// </summary>
        MTConnect.UnitVector3D Origin { get; }
        
        /// <summary>
        /// Id.
        /// </summary>
        string ParentIdRef { get; }
        
        /// <summary>
        /// Process of transforming to the origin position of the coordinate system from a parent coordinate system using Translation and Rotation.
        /// </summary>
        MTConnect.Devices.Configurations.ITransformation Transformation { get; }
        
        /// <summary>
        /// Type of coordinate system.
        /// </summary>
        MTConnect.Devices.Configurations.CoordinateSystemType Type { get; }
        
        /// <summary>
        /// UUID for the coordinate system.
        /// </summary>
        string Uuid { get; }
    }
}