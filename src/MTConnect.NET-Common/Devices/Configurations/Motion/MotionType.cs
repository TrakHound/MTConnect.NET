// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Configurations.Motion
{
    /// <summary>
    /// Describes the type of motion.    
    /// </summary>
    public enum MotionType
    {
        /// <summary>
        /// Rotates around an axis with a fixed range of motion.
        /// </summary>
        REVOLUTE,

        /// <summary>
        /// Revolves around an axis with a continuous range of motion.
        /// </summary>
        CONTINUOUS,

        /// <summary>
        /// Sliding linear motion along an axis with a fixed range of motion.
        /// </summary>
        PRISMATIC,

        /// <summary>
        /// The axis does not move.
        /// </summary>
        FIXED
    }
}
