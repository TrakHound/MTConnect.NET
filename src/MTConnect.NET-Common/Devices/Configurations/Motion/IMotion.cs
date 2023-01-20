// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations.Motion
{
    /// <summary>
    /// Motion defines the movement of the Component relative to a coordinate system. Motion specifies the kinematic chain of the Components.
    /// </summary>
    public interface IMotion
    {
        /// <summary>
        /// The unique identifier for this element.     
        /// </summary>
        string Id { get; }

        /// <summary>
        /// A pointer to the id attribute of the parent Motion.
        /// </summary>
        string ParentIdRef { get; }

        /// <summary>
        /// The coordinate system within which the kinematic motion occurs.
        /// </summary>
        string CoordinateSystemIdRef { get; }

        /// <summary>
        /// Describes the type of motion.    
        /// </summary>
        MotionType Type { get; }

        /// <summary>
        /// Describes if this Component is actuated directly or indirectly as a result of other motion.
        /// </summary>
        MotionActuationType Actuation { get; }

        /// <summary>
        /// An element that can contain any descriptive content.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Axis defines the axis along or around which the Component moves relative to a coordinate system.
        /// </summary>
        string Axis { get; }

        /// <summary>
        /// A fixed point from which measurement or motion commences.
        /// </summary>
        string Origin { get; }

        /// <summary>
        /// The Transformation of the parent Origin or Transformation using Translation and Rotation.
        /// </summary>
        ITransformation Transformation { get; }
    }
}