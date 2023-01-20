// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// An indication of the status of the spindle for a piece of equipment when power has been removed and it is free to rotate.
    /// </summary>
    public enum SpindleInterlock
    {
        /// <summary>
        /// Spindle has not been deactivated.
        /// </summary>
        INACTIVE,

        /// <summary>
        /// Power has been removed and the spindle cannot be operated.
        /// </summary>
        ACTIVE
    }
}
