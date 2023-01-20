// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// The direction of motion.
    /// </summary>
    public enum LinearDirection
    {
        /// <summary>
        /// No direction
        /// </summary>
        NONE,

        /// <summary>
        /// Linear position is decreasing.
        /// </summary>
        NEGATIVE,

        /// <summary>
        /// Linear position is increasing.
        /// </summary>
        POSITIVE
    }
}