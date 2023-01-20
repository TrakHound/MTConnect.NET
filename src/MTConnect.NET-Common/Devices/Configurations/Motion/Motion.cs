// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations.Motion
{
    /// <summary>
    /// Motion defines the movement of the Component relative to a coordinate system. Motion specifies the kinematic chain of the Components.
    /// </summary>
    public class Motion : IMotion
    {
        /// <summary>
        /// The unique identifier for this element.     
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// A pointer to the id attribute of the parent Motion.
        /// </summary>
        public string ParentIdRef { get; set; }

        /// <summary>
        /// The coordinate system within which the kinematic motion occurs.
        /// </summary>
        public string CoordinateSystemIdRef { get; set; }

        /// <summary>
        /// Describes the type of motion.    
        /// </summary>
        public MotionType Type { get; set; }

        /// <summary>
        /// Describes if this Component is actuated directly or indirectly as a result of other motion.
        /// </summary>
        public MotionActuationType Actuation { get; set; }

        /// <summary>
        /// An element that can contain any descriptive content.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Axis defines the axis along or around which the Component moves relative to a coordinate system.
        /// </summary>
        public string Axis { get; set; }

        /// <summary>
        /// A fixed point from which measurement or motion commences.
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// The Transformation of the parent Origin or Transformation using Translation and Rotation.
        /// </summary>
        public ITransformation Transformation { get; set; }
    }
}