// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// Description text for each <see cref="ActuatorState"/> value as defined by the MTConnect Standard.
    /// </summary>
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


        /// <summary>
        /// Returns the MTConnect Standard description text for the specified <see cref="ActuatorState"/> value, or <c>null</c> when none is defined.
        /// </summary>
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