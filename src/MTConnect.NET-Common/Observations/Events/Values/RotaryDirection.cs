// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
