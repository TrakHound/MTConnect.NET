// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    public static class MotionDescriptions
    {
        /// <summary>
        /// Describes if this component is actuated directly or indirectly as a result of other motion.
        /// </summary>
        public const string Actuation = "Describes if this component is actuated directly or indirectly as a result of other motion.";
        
        /// <summary>
        /// Axis along or around which the Component moves relative to a coordinate system.
        /// </summary>
        public const string Axis = "Axis along or around which the Component moves relative to a coordinate system.";
        
        /// <summary>
        /// Coordinate system within which the kinematic motion occurs.
        /// </summary>
        public const string CoordinateSystemIdRef = "Coordinate system within which the kinematic motion occurs.";
        
        /// <summary>
        /// Descriptive content.
        /// </summary>
        public const string Description = "Descriptive content.";
        
        /// <summary>
        /// Unique identifier for this element.
        /// </summary>
        public const string Id = "Unique identifier for this element.";
        
        /// <summary>
        /// Coordinates of the origin position of a coordinate system.
        /// </summary>
        public const string Origin = "Coordinates of the origin position of a coordinate system.";
        
        /// <summary>
        /// Pointer to the id attribute of the parent Motion.The kinematic chain connects all components using the parent relations. All motion is connected to the motion of the parent. The first node in the chain will not have a parent.
        /// </summary>
        public const string ParentIdRef = "Pointer to the id attribute of the parent Motion.The kinematic chain connects all components using the parent relations. All motion is connected to the motion of the parent. The first node in the chain will not have a parent.";
        
        /// <summary>
        /// Process of transforming to the origin position of the coordinate system from a parent coordinate system using Translation and Rotation.
        /// </summary>
        public const string Transformation = "Process of transforming to the origin position of the coordinate system from a parent coordinate system using Translation and Rotation.";
        
        /// <summary>
        /// Type of motion.
        /// </summary>
        public const string Type = "Type of motion.";
    }
}