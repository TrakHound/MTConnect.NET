// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    public static class EmergencyStopDescriptions
    {
        /// <summary>
        /// Emergency stop circuit is complete and the piece of equipment, component, or composition is allowed to operate.
        /// </summary>
        public const string ARMED = "Emergency stop circuit is complete and the piece of equipment, component, or composition is allowed to operate.";
        
        /// <summary>
        /// Operation of the piece of equipment, component, or composition is inhibited.
        /// </summary>
        public const string TRIGGERED = "Operation of the piece of equipment, component, or composition is inhibited.";


        public static string Get(EmergencyStop value)
        {
            switch (value)
            {
                case EmergencyStop.ARMED: return "Emergency stop circuit is complete and the piece of equipment, component, or composition is allowed to operate.";
                case EmergencyStop.TRIGGERED: return "Operation of the piece of equipment, component, or composition is inhibited.";
            }

            return null;
        }
    }
}