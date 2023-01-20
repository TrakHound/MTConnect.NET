// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// The direction of motion.
    /// </summary>
    public enum RotaryDirection
    {
        /// <summary>
        /// Counter-clockwise rotation using the right-hand rule
        /// </summary>
        COUNTER_CLOCKWISE,

        /// <summary>
        /// No direction.
        /// </summary>
        NONE,

        /// <summary>
        /// Clockwise rotation using the right-hand rule.
        /// </summary>
        CLOCKWISE
    }
}