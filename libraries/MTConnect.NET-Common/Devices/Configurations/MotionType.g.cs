// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    public enum MotionType
    {
        /// <summary>
        /// Revolves around an axis with a continuous range of motion.
        /// </summary>
        CONTINUOUS,
        
        /// <summary>
        /// Axis does not move.
        /// </summary>
        FIXED,
        
        /// <summary>
        /// Sliding linear motion along an axis with a fixed range of motion.
        /// </summary>
        PRISMATIC,
        
        /// <summary>
        /// Rotates around an axis with a fixed range of motion.
        /// </summary>
        REVOLUTE
    }
}