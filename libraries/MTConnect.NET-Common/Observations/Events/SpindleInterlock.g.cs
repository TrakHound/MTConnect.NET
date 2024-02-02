// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// Indication of the status of the spindle for a piece of equipment when power has been removed and it is free to rotate.
    /// </summary>
    public enum SpindleInterlock
    {
        /// <summary>
        /// Power has been removed and the spindle cannot be operated.
        /// </summary>
        ACTIVE,
        
        /// <summary>
        /// Spindle has not been deactivated.
        /// </summary>
        INACTIVE
    }
}