// Copyright (c) 2017 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect
{
    public enum DataItemCategory
    {
        /// <summary>
        /// A CONDITION category data item communicates information about the health of a device and its ability to function.
        /// </summary>
        CONDITION,

        /// <summary>
        /// An EVENT category data item represents a discrete piece of information from a device.
        /// </summary>
        EVENT,

        /// <summary>
        /// A SAMPLE category data item provides the reading of the value of a continuously variable or analog data value.
        /// </summary>
        SAMPLE
    }
}
