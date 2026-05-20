// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// The direction in which a MaintenanceList interval counts toward its limit.
    /// </summary>
    public enum MaintenanceListDirection
    {
        /// <summary>
        /// The interval counts down toward zero from the configured limit.
        /// </summary>
        DOWN,

        /// <summary>
        /// The interval counts up toward the configured limit.
        /// </summary>
        UP
    }
}
