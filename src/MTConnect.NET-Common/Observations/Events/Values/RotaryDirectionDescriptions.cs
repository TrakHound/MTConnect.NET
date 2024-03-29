// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// The direction of motion.
    /// </summary>
    public static class RotaryDirectionDescriptions
    {
        /// <summary>
        /// Counter-clockwise rotation using the right-hand rule.
        /// </summary>
        public const string COUNTER_CLOCKWISE = "Counter-clockwise rotation using the right-hand rule.";

        /// <summary>
        /// No direction.
        /// </summary>
        public const string NONE = "No direction.";

        /// <summary>
        /// Clockwise rotation using the right-hand rule.
        /// </summary>
        public const string CLOCKWISE = "Clockwise rotation using the right-hand rule.";


        public static string Get(RotaryDirection value)
        {
            switch (value)
            {
                case RotaryDirection.COUNTER_CLOCKWISE: return COUNTER_CLOCKWISE;
                case RotaryDirection.NONE: return NONE;
                case RotaryDirection.CLOCKWISE: return CLOCKWISE;
            }

            return null;
        }
    }
}