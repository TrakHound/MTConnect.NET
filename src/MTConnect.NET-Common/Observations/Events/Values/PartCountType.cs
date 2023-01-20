// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// Interpretation of PART_COUNT.
    /// </summary>
    public enum PartCountType
    {
        UNAVAILABLE,

        /// <summary>
        /// Pre-specified group of items.
        /// </summary>
        BATCH,

        /// <summary>
        /// Count is of individual items.
        /// </summary>
        EACH
    }
}
