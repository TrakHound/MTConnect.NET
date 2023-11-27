// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    public enum MotionActuationType
    {
        /// <summary>
        /// Movement is initiated by the component.
        /// </summary>
        DIRECT,
        
        /// <summary>
        /// No actuation of this axis.> Note: Actuation of `NONE` can be either a derived `REVOLUTE` or `PRISMATIC` motion or static `FIXED` relationship.
        /// </summary>
        NONE,
        
        /// <summary>
        /// Motion is computed and is used for expressing an imaginary movement.
        /// </summary>
        VIRTUAL
    }
}