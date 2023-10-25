// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
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
    }
}