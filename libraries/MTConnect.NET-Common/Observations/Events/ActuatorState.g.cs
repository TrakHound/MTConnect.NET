// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// Operational state of an apparatus for moving or controlling a mechanism or system.
    /// </summary>
    public enum ActuatorState
    {
        /// <summary>
        /// Actuator is operating.
        /// </summary>
        ACTIVE,
        
        /// <summary>
        /// Actuator is not operating.
        /// </summary>
        INACTIVE
    }
}