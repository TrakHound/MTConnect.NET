// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// State of the emergency stop signal for a piece of equipment, controller path, or any other component or subsystem of a piece of equipment.
    /// </summary>
    public enum EmergencyStop
    {
        /// <summary>
        /// Emergency stop circuit is complete and the piece of equipment, component, or composition is allowed to operate.
        /// </summary>
        ARMED,
        
        /// <summary>
        /// Operation of the piece of equipment, component, or composition is inhibited.
        /// </summary>
        TRIGGERED
    }
}