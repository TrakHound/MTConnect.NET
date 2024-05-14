// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
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