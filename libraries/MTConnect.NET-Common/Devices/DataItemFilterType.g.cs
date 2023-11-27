// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    public enum DataItemFilterType
    {
        /// <summary>
        /// New value **MUST NOT** be reported for a data item unless the measured value has changed from the last reported value by at least the delta given as the value of this element.The value of Filter **MUST** be an absolute value using the same units as the reported data.
        /// </summary>
        MINIMUM_DELTA,
        
        /// <summary>
        /// Data reported for a data item is provided on a periodic basis. The `PERIOD` for reporting data is defined in the value of the Filter.The value of Filter **MUST** be an absolute value reported in seconds representing the time between reported samples of the value of the data item.
        /// </summary>
        PERIOD
    }
}