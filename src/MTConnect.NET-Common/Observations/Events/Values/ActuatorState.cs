// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// Represents the operational state of an apparatus for moving or controlling a mechanism or system.
    /// </summary>
    public enum ActuatorState
    {
        /// <summary>
        /// The actuator is not operating
        /// </summary>
        INACTIVE,

        /// <summary>
        /// The actuator is operating
        /// </summary>
        ACTIVE
    }
}
