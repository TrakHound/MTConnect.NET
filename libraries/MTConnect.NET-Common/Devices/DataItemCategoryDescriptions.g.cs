// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    /// <summary>
    /// Description text for each <see cref="DataItemCategory"/> value as defined by the MTConnect Standard.
    /// </summary>
    public static class DataItemCategoryDescriptions
    {
        /// <summary>
        /// Information about the health of a piece of equipment and its ability to function.
        /// </summary>
        public const string CONDITION = "Information about the health of a piece of equipment and its ability to function.";
        
        /// <summary>
        /// Discrete piece of information from the piece of equipment.
        /// </summary>
        public const string EVENT = "Discrete piece of information from the piece of equipment.";
        
        /// <summary>
        /// Continuously variable or analog data value. A continuous value can be measured at any point-in-time and will always produce a result.
        /// </summary>
        public const string SAMPLE = "Continuously variable or analog data value. A continuous value can be measured at any point-in-time and will always produce a result.";


        /// <summary>
        /// Returns the MTConnect Standard description text for the specified <see cref="DataItemCategory"/> value, or <c>null</c> when none is defined.
        /// </summary>
        public static string Get(DataItemCategory value)
        {
            switch (value)
            {
                case DataItemCategory.CONDITION: return "Information about the health of a piece of equipment and its ability to function.";
                case DataItemCategory.EVENT: return "Discrete piece of information from the piece of equipment.";
                case DataItemCategory.SAMPLE: return "Continuously variable or analog data value. A continuous value can be measured at any point-in-time and will always produce a result.";
            }

            return null;
        }
    }
}