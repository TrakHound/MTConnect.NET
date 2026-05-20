// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// Description text for each <see cref="DirectionRotary"/> value as defined by the MTConnect Standard.
    /// </summary>
    public static class DirectionRotaryDescriptions
    {
        /// <summary>
        /// Clockwise rotation using the right-hand rule.
        /// </summary>
        public const string CLOCKWISE = "Clockwise rotation using the right-hand rule.";
        
        /// <summary>
        /// Counter-clockwise rotation using the right-hand rule.
        /// </summary>
        public const string COUNTER_CLOCKWISE = "Counter-clockwise rotation using the right-hand rule.";
        
        /// <summary>
        /// No direction.
        /// </summary>
        public const string NONE = "No direction.";


        /// <summary>
        /// Returns the MTConnect Standard description text for the specified <see cref="DirectionRotary"/> value, or <c>null</c> when none is defined.
        /// </summary>
        public static string Get(DirectionRotary value)
        {
            switch (value)
            {
                case DirectionRotary.CLOCKWISE: return "Clockwise rotation using the right-hand rule.";
                case DirectionRotary.COUNTER_CLOCKWISE: return "Counter-clockwise rotation using the right-hand rule.";
                case DirectionRotary.NONE: return "No direction.";
            }

            return null;
        }
    }
}