// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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