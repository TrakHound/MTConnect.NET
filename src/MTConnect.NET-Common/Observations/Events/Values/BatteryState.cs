// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// Present status of the battery.
    /// </summary>
    public enum BatteryState
    {
        /// <summary>
        /// Component is at it’s maximum rated charge level.
        /// </summary>
        CHARGED,

        /// <summary>
        /// Component's charge is increasing.
        /// </summary>
        CHARGING,

        /// <summary>
        /// Component is at it’s minimum charge level.
        /// </summary>
        DISCHARGED,

        /// <summary>
        /// Component's charge is decreasing.
        /// </summary>
        DISCHARGING
    }
}