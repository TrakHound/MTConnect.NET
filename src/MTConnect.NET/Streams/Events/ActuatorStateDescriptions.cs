// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Events
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
