// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// State of the axis lockout function when power has been removed and the axis is allowed to move freely.
    /// </summary>
    public enum AxisInterlock
    {
        /// <summary>
        /// Axis lockout function is activated, power has been removed from the axis, and the axis is allowed to move freely.
        /// </summary>
        ACTIVE,
        
        /// <summary>
        /// Axis lockout function has not been activated, the axis may be powered, and the axis is capable of being controlled by another component.
        /// </summary>
        INACTIVE
    }
}