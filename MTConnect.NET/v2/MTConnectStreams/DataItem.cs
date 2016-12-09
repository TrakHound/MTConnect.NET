// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace MTConnect.MTConnectStreams
{
    public class DataItem
    {
        public string CDATA { get; set; }

        /// <summary>
        /// Full XML XPath address of the DataItem.
        /// (Added for TrakHound)
        /// </summary>
        public string FullAddress { get; set; }

        /// <summary>
        /// Type of DataItem (Condition, Event, or Sample)
        /// </summary>
        public DataItemCategory Category { get; set; }

        /// <summary>
        /// Type associated with the DataItem
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The subtype of the DataItem defined in the Device Information Model that this DataItem element represents
        /// </summary>
        public string SubType { get; set; }

        #region "Required"

        /// <summary>
        /// The unique identifier for the DataItem. 
        /// The DataItemID MUST match the id attribute of the data item defined in the Device Information Model that this DataItem element represents.
        /// </summary>
        public string DataItemId { get; set; }

        /// <summary>
        /// THe name of the DataItem.
        /// The name MUST match the name of the data item defined in the Device Information Model that this DataItem represents.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A number representing the sequential position of an occurence of the DataItem in the data buffer of the Agent.
        /// The value MUST be represented as an unsigned 64 bit with valid values from 1 to 2^64-1.
        /// </summary>
        public long Sequence { get; set; }

        /// <summary>
        /// The time the data for the DataItem was reported or the statistics for the DataItem was computed.
        /// The timestamp MUST always represent the end of the collection interval when a duration or a TIME_SERIES is provided.
        /// The most accurate time available to the device MUST be used for the timestamp.
        /// </summary>
        public DateTime Timestamp { get; set; }
        
        #endregion

    }
}
