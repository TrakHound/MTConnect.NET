// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    public static class ActuatorStateDescriptions
    {
        /// <summary>
        /// Actuator is operating.
        /// </summary>
        public const string ACTIVE = "Actuator is operating.";
        
        /// <summary>
        /// Actuator is not operating.
        /// </summary>
        public const string INACTIVE = "Actuator is not operating.";


        public static string Get(ActuatorState value)
        {
            switch (value)
            {
                case ActuatorState.ACTIVE: return "Actuator is operating.";
                case ActuatorState.INACTIVE: return "Actuator is not operating.";
            }

            return null;
        }
    }
}