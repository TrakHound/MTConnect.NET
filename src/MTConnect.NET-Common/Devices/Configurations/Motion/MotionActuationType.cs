// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations.Motion
{
    /// <summary>
    /// Describes if this Component is actuated directly or indirectly as a result of other motion.
    /// </summary>
    public enum MotionActuationType
    {
        /// <summary>
        /// The movement is initiated by the Component.
        /// </summary>
        DIRECT,

        /// <summary>
        /// The motion is computed and is used for expressing an imaginary movement.
        /// </summary>
        VIRTUAL,

        /// <summary>
        /// There is no actuation of this Axis.
        /// </summary>
        NONE
    }
}