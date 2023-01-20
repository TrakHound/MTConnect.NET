// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// Represents the operational state of an apparatus for moving or controlling a mechanism or system.
    /// </summary>
    public static class ActuatorStateDescriptions
    {
        /// <summary>
        /// The actuator is not operating
        /// </summary>
        public const string INACTIVE = "The actuator is not operating";

        /// <summary>
        /// The actuator is operating
        /// </summary>
        public const string ACTIVE = "The actuator is operating";


        public static string Get(ActuatorState value)
        {
            switch (value)
            {
                case ActuatorState.INACTIVE: return INACTIVE;
                case ActuatorState.ACTIVE: return ACTIVE;
            }

            return null;
        }
    }
}