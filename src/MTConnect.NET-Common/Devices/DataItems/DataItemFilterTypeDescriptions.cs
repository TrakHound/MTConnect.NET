// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.DataItems
{
    public static class DataItemFilterTypeDescriptions
    {
        /// <summary>
        /// For a MINIMUM_DELTA type Filter, a new value MUST NOT be reported for a data item unless the measured value
        /// has changed from the last reported value by at least the delta given as the CDATA of this element.
        /// </summary>
        public const string MINIMUM_DELTA = "For a MINIMUM_DELTA type Filter, a new value MUST NOT be reported for a data item unless the measured value has changed from the last reported value by at least the delta given as the CDATA of this element.";

        /// <summary>
        /// For a PERIOD type Filter, the data reported for a data item is provided on a periodic basis. The PERIOD for reporting data is defined in the CDATA for the Filter.
        /// </summary>
        public const string PERIOD = "For a PERIOD type Filter, the data reported for a data item is provided on a periodic basis. The PERIOD for reporting data is defined in the CDATA for the Filter.";


        public static string Get(DataItemFilterType type)
        {
            switch (type)
            {
                case DataItemFilterType.MINIMUM_DELTA: return MINIMUM_DELTA;
                case DataItemFilterType.PERIOD: return PERIOD;
            }

            return "";
        }
    }
}
