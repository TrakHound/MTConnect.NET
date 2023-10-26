// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// Interpretation of `PART_COUNT`.
    /// </summary>
    public enum PartCountType
    {
        /// <summary>
        /// Count is of individual items.
        /// </summary>
        EACH,
        
        /// <summary>
        /// Pre-specified group of items.
        /// </summary>
        BATCH
    }
}