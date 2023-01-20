// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations.CoordinateSystems
{
    /// <summary>
    /// A CoordinateSystem is a reference system that associates a unique set of n parameters with each point in an n-dimensional space.
    /// </summary>
    public class CoordinateSystem : ICoordinateSystem
    {
        /// <summary>
        /// The unique identifier for this element.    
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The name of the coordinate system.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The manufacturer’s name or users name for the coordinate system.
        /// </summary>
        public string NativeName { get; set; }

        /// <summary>
        /// A pointer to the id attribute of the parent CoordinateSystem.
        /// </summary>
        public string ParentIdRef { get; set; }

        /// <summary>
        /// The type of coordinate system.
        /// </summary>
        public CoordinateSystemType Type { get; set; }

        /// <summary>
        /// The type of coordinate system.
        /// </summary>
        public string TypeDescription => CoordinateSystemTypeDescriptions.Get(Type);

        /// <summary>
        /// The coordinates of the origin position of a coordinate system.
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// The process of transforming to the origin position of the coordinate system from a parent coordinate system using Translation and Rotation.
        /// </summary>
        public ITransformation Transformation { get; set; }

        /// <summary>
        /// The natural language description of the CoordinateSystem.
        /// </summary>
        public string Description { get; set; }
    }
}