// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Streams
{
    public class DataItem : IDataItem
    {
        /// <summary>
        /// If an Agent cannot determine a Valid Data Value for a DataItem, the value returned for the CDATA for the Data Entity MUST be reported as UNAVAILABLE.
        /// </summary>
        public const string Unavailable = "UNAVAILABLE";


        /// <summary>
        /// The unique identifier for the DataItem. 
        /// The DataItemID MUST match the id attribute of the data item defined in the Device Information Model that this DataItem element represents.
        /// </summary>
        [XmlAttribute("dataItemId")]
        [JsonPropertyName("dataItemId")]
        public string DataItemId { get; set; }

        /// <summary>
        /// The time the data for the DataItem was reported or the statistics for the DataItem was computed.
        /// The timestamp MUST always represent the end of the collection interval when a duration or a TIME_SERIES is provided.
        /// The most accurate time available to the device MUST be used for the timestamp.
        /// </summary>
        [XmlAttribute("timestamp")]
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The name of the DataItem.
        /// The name MUST match the name of the data item defined in the Device Information Model that this DataItem represents.
        /// </summary>
        [XmlAttribute("name")]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// A number representing the sequential position of an occurence of the DataItem in the data buffer of the Agent.
        /// The value MUST be represented as an unsigned 64 bit with valid values from 1 to 2^64-1.
        /// </summary>
        [XmlAttribute("sequence")]
        [JsonPropertyName("sequence")]
        public long Sequence { get; set; }

        /// <summary>
        /// Category of DataItem (Condition, Event, or Sample)
        /// </summary>
        [XmlAttribute("category")]
        [JsonPropertyName("category")]
        public DataItemCategory Category { get; set; }

        /// <summary>
        /// Type associated with the DataItem
        /// </summary>
        [XmlIgnore]
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The subtype of the DataItem defined in the Device Information Model that this DataItem element represents
        /// </summary>
        [XmlAttribute("subType")]
        [JsonPropertyName("subType")]
        public string SubType { get; set; }

        /// <summary>
        /// The identifier of the Composition element defined in the MTConnectDevices document associated with the data reported for the DataItem.
        /// </summary>
        [XmlAttribute("compositionId")]
        [JsonPropertyName("compositionId")]
        public string CompositionId { get; set; }

        /// <summary>
        /// For those DataItem elements that report data that may be periodically reset to an initial value, 
        /// resetTriggered identifies when a reported value has been reset and what has caused that reset to occur.
        /// </summary>
        [XmlAttribute("resetTriggered")]
        [JsonPropertyName("resetTriggered")]
        public ResetTriggered ResetTriggered { get; set; }


        [XmlText]
        [JsonPropertyName("cdata")]
        public string CDATA { get; set; }

        [XmlElement("Entry")]
        [JsonPropertyName("entries")]
        public List<Entry> Entries { get; set; }

        /// <summary>
        /// The number of Entry elements for the observation.
        /// </summary>
        [XmlAttribute("count")]
        [JsonPropertyName("count")]
        public long Count { get; set; }
    }
}
