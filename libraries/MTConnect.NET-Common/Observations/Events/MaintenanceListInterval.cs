// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// How a MaintenanceList interval value is interpreted relative to the last maintenance action.
    /// </summary>
    public enum MaintenanceListInterval
    {
        /// <summary>
        /// The interval is measured against an absolute reference point.
        /// </summary>
        ABSOLUTE,

        /// <summary>
        /// The interval is measured incrementally from the last maintenance action.
        /// </summary>
        INCREMENTAL
    }
}
