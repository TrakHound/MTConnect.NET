// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MTConnect.Observations;

namespace MTConnect.Streams.Json
{
    public class JsonObservation
    {
        /// <summary>
        /// The unique identifier for the DataItem. 
        /// The DataItemID MUST match the id attribute of the data item defined in the Device Information Model that this DataItem element represents.
        /// </summary>
        [JsonPropertyName("dataItemId")]
        public string DataItemId { get; set; }

        /// <summary>
        /// The time the data for the DataItem was reported or the statistics for the DataItem was computed.
        /// The timestamp MUST always represent the end of the collection interval when a duration or a TIME_SERIES is provided.
        /// The most accurate time available to the device MUST be used for the timestamp.
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The name of the DataItem.
        /// The name MUST match the name of the data item defined in the Device Information Model that this DataItem represents.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// A number representing the sequential position of an occurence of the DataItem in the data buffer of the Agent.
        /// The value MUST be represented as an unsigned 64 bit with valid values from 1 to 2^64-1.
        /// </summary>
        [JsonPropertyName("sequence")]
        public long Sequence { get; set; }

        /// <summary>
        /// Category of DataItem (Condition, Event, or Sample)
        /// </summary>
        [JsonIgnore]
        public string Category { get; set; }

        /// <summary>
        /// Type associated with the DataItem
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The subtype of the DataItem defined in the Device Information Model that this DataItem element represents
        /// </summary>
        [JsonPropertyName("subType")]
        public string SubType { get; set; }

        /// <summary>
        /// The identifier of the Composition element defined in the MTConnectDevices document associated with the data reported for the DataItem.
        /// </summary>
        [JsonPropertyName("compositionId")]
        public string CompositionId { get; set; }

        /// <summary>
        /// For those DataItem elements that report data that may be periodically reset to an initial value, 
        /// resetTriggered identifies when a reported value has been reset and what has caused that reset to occur.
        /// </summary>
        [JsonPropertyName("resetTriggered")]
        public string ResetTriggered { get; set; }

        [JsonPropertyName("result")]
        public string Result { get; set; }

        [JsonPropertyName("entries")]
        public IEnumerable<JsonEntry> Entries { get; set; }

        /// <summary>
        /// The number of Entry elements for the observation.
        /// </summary>
        [JsonPropertyName("count")]
        public long? Count { get; set; }


        public static IEnumerable<JsonEntry> CreateEntries(IEnumerable<IDataSetEntry> entries)
        {
            if (!entries.IsNullOrEmpty())
            {
                var jsonEntries = new List<JsonEntry>();
                foreach (var entry in entries)
                {
                    jsonEntries.Add(new JsonEntry(entry));
                }
                return jsonEntries;
            }

            return null;
        }

        public static IEnumerable<JsonEntry> CreateEntries(IEnumerable<ITableEntry> entries)
        {
            if (!entries.IsNullOrEmpty())
            {
                var jsonEntries = new List<JsonEntry>();
                foreach (var entry in entries)
                {
                    jsonEntries.Add(new JsonEntry(entry));
                }
                return jsonEntries;
            }

            return null;
        }
    }
}
