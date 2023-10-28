// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    public static class DataItemFilterTypeDescriptions
    {
        /// <summary>
        /// New value **MUST NOT** be reported for a data item unless the measured value has changed from the last reported value by at least the delta given as the value of this element.The value of Filter **MUST** be an absolute value using the same units as the reported data.
        /// </summary>
        public const string MINIMUM_DELTA = "New value **MUST NOT** be reported for a data item unless the measured value has changed from the last reported value by at least the delta given as the value of this element.The value of Filter **MUST** be an absolute value using the same units as the reported data.";
        
        /// <summary>
        /// Data reported for a data item is provided on a periodic basis. The `PERIOD` for reporting data is defined in the value of the Filter.The value of Filter **MUST** be an absolute value reported in seconds representing the time between reported samples of the value of the data item.
        /// </summary>
        public const string PERIOD = "Data reported for a data item is provided on a periodic basis. The `PERIOD` for reporting data is defined in the value of the Filter.The value of Filter **MUST** be an absolute value reported in seconds representing the time between reported samples of the value of the data item.";


        public static string Get(DataItemFilterType value)
        {
            switch (value)
            {
                case DataItemFilterType.MINIMUM_DELTA: return "New value **MUST NOT** be reported for a data item unless the measured value has changed from the last reported value by at least the delta given as the value of this element.The value of Filter **MUST** be an absolute value using the same units as the reported data.";
                case DataItemFilterType.PERIOD: return "Data reported for a data item is provided on a periodic basis. The `PERIOD` for reporting data is defined in the value of the Filter.The value of Filter **MUST** be an absolute value reported in seconds representing the time between reported samples of the value of the data item.";
            }

            return null;
        }
    }
}