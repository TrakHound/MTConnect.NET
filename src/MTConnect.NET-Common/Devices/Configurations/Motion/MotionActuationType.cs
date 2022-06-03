// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Configurations.Motion
{
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
