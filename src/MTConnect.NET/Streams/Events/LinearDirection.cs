// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Events
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
