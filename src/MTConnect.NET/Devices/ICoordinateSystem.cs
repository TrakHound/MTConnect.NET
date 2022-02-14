// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices
{
    /// <summary>
    /// A CoordinateSystem is a reference system that associates a unique set of n parameters with each point in an n-dimensional space.
    /// </summary>
    public interface ICoordinateSystem
    {
        /// <summary>
        /// The unique identifier for this element.    
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// The name of the coordinate system.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The manufacturer’s name or users name for the coordinate system.
        /// </summary>
        string NativeName { get; set; }

        /// <summary>
        /// A pointer to the id attribute of the parent CoordinateSystem.
        /// </summary>
        string ParentIdRef { get; set; }

        /// <summary>
        /// The type of coordinate system.
        /// </summary>
        CoordinateSystemType Type { get; set; }

        /// <summary>
        /// The coordinates of the origin position of a coordinate system.
        /// </summary>
        string Origin { get; set; }

        /// <summary>
        /// The process of transforming to the origin position of the coordinate system from a parent coordinate system using Translation and Rotation.
        /// </summary>
        Transformation Transformation { get; set; }

        /// <summary>
        /// The natural language description of the CoordinateSystem.
        /// </summary>
        string Description { get; set; }
    }
}
