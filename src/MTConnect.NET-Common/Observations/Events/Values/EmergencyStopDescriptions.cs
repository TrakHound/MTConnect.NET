// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// The current state of the emergency stop signal for a piece of equipment, controller path, or any other component or subsystem of a piece of equipment.
    /// </summary>
    public static class EmergencyStopDescriptions
    {
        /// <summary>
        /// The emergency stop circuit is open and the operation of the piece of equipment, component, or composition element is inhibited.
        /// </summary>
        public const string TRIGGERED = "The emergency stop circuit is open and the operation of the piece of equipment, component, or composition element is inhibited.";

        /// <summary>
        /// The emergency stop circuit is complete and the piece of equipment, component, or composition element is allowed to operate.
        /// </summary>
        public const string ARMED = "The emergency stop circuit is complete and the piece of equipment, component, or composition element is allowed to operate.";


        public static string Get(EmergencyStop value)
        {
            switch (value)
            {
                case EmergencyStop.TRIGGERED: return TRIGGERED;
                case EmergencyStop.ARMED: return ARMED;
            }

            return null;
        }
    }
}
