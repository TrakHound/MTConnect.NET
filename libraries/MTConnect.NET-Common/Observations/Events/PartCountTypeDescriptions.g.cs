// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// Description text for each <see cref="PartCountType"/> value as defined by the MTConnect Standard.
    /// </summary>
    public static class PartCountTypeDescriptions
    {
        /// <summary>
        /// Count is of individual items.
        /// </summary>
        public const string EACH = "Count is of individual items.";
        
        /// <summary>
        /// Pre-specified group of items.
        /// </summary>
        public const string BATCH = "Pre-specified group of items.";


        /// <summary>
        /// Returns the MTConnect Standard description text for the specified <see cref="PartCountType"/> value, or <c>null</c> when none is defined.
        /// </summary>
        public static string Get(PartCountType value)
        {
            switch (value)
            {
                case PartCountType.EACH: return "Count is of individual items.";
                case PartCountType.BATCH: return "Pre-specified group of items.";
            }

            return null;
        }
    }
}