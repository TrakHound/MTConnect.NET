// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// The current state of the emergency stop signal for a piece of equipment, controller path, or any other component or subsystem of a piece of equipment.
    /// </summary>
    public enum EmergencyStop
    {
        UNAVAILABLE,

        /// <summary>
        /// The emergency stop circuit is open and the operation of the piece of equipment, component, or composition element is inhibited.
        /// </summary>
        TRIGGERED,

        /// <summary>
        /// The emergency stop circuit is complete and the piece of equipment, component, or composition element is allowed to operate.
        /// </summary>
        ARMED
    }
}