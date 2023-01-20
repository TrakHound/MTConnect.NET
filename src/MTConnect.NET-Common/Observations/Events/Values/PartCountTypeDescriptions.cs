// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// Interpretation of PART_COUNT.
    /// </summary>
    public static class PartCountTypeDescriptions
    {
        /// <summary>
        /// Pre-specified group of items.
        /// </summary>
        public const string BATCH = "Pre-specified group of items.";

        /// <summary>
        /// Count is of individual items.
        /// </summary>
        public const string EACH = "Count is of individual items.";


        public static string Get(PartCountType value)
        {
            switch (value)
            {
                case PartCountType.BATCH: return BATCH;
                case PartCountType.EACH: return EACH;
            }

            return null;
        }
    }
}
