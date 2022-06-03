// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Configurations.Motion
{
    public static class MotionAttributeDescriptions
    {
        /// <summary>
        /// The unique identifier for this element.     
        /// </summary>
        public const string Id = "The unique identifier for this element.  ";

        /// <summary>
        /// A pointer to the id attribute of the parent Motion.
        /// </summary>
        public const string ParentIdRef = "A pointer to the id attribute of the parent Motion.";

        /// <summary>
        /// The coordinate system within which the kinematic motion occurs.
        /// </summary>
        public const string CoordinateSystemIdRef = "The coordinate system within which the kinematic motion occurs.";

        /// <summary>
        /// Describes the type of motion.    
        /// </summary>
        public const string Type = "Describes the type of motion.  ";

        /// <summary>
        /// Describes if this Component is actuated directly or indirectly as a result of other motion.
        /// </summary>
        public const string Actuation = "Describes if this Component is actuated directly or indirectly as a result of other motion.";

        /// <summary>
        /// An element that can contain any descriptive content.
        /// </summary>
        public const string Description = "An element that can contain any descriptive content.";

        /// <summary>
        /// Axis defines the axis along or around which the Component moves relative to a coordinate system.
        /// </summary>
        public const string Axis = "Axis defines the axis along or around which the Component moves relative to a coordinate system.";

        /// <summary>
        /// A fixed point from which measurement or motion commences.
        /// </summary>
        public const string Origin = "A fixed point from which measurement or motion commences.";
    }
}
