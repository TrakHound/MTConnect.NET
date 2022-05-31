// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.DataItems
{
    public static class DataItemCategoryDescriptions
    {
        /// <summary>
        /// A CONDITION category data item communicates information about the health of a device and its ability to function.
        /// </summary>
        public const string CONDITION = "A CONDITION category data item communicates information about the health of a device and its ability to function.";

        /// <summary>
        /// An EVENT category data item represents a discrete piece of information from a device.
        /// </summary>
        public const string EVENT = "An EVENT category data item represents a discrete piece of information from a device.";

        /// <summary>
        /// A SAMPLE category data item provides the reading of the value of a continuously variable or analog data value.
        /// </summary>
        public const string SAMPLE = "A SAMPLE category data item provides the reading of the value of a continuously variable or analog data value.";


        public static string Get(DataItemCategory dataItemCategory)
        {
            switch (dataItemCategory)
            {
                case DataItemCategory.CONDITION: return CONDITION;
                case DataItemCategory.EVENT: return EVENT;
                case DataItemCategory.SAMPLE: return SAMPLE;
            }

            return "";
        }
    }
}
