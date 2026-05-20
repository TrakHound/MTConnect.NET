// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Streams.Json
{
    /// <summary>
    /// JSON serialization surrogate for an MTConnect observation reported in a
    /// component stream. Carries the shape common to samples, events, and
    /// conditions across all representations (VALUE, DATA_SET, TABLE, and
    /// TIME_SERIES), with the representation-specific payload exposed through
    /// <see cref="Result"/>, <see cref="Samples"/>, or <see cref="Entries"/>.
    /// </summary>
    public class JsonObservation
    {
        /// <summary>
        /// Reference to the <c>id</c> of the data item the observation
        /// reports.
        /// </summary>
        [JsonPropertyName("dataItemId")]
        public string DataItemId { get; set; }

        /// <summary>
        /// The name of the data item the observation reports.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The category of the observation (SAMPLE, EVENT, or CONDITION).
        /// </summary>
        [JsonPropertyName("category")]
        public string Category { get; set; }

        /// <summary>
        /// The representation of the observation (for example VALUE, DATA_SET,
        /// TABLE, or TIME_SERIES).
        /// </summary>
        [JsonPropertyName("representation")]
        public string Representation { get; set; }

        /// <summary>
        /// The MTConnect type of the data item the observation reports.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The subtype further qualifying <see cref="Type"/>.
        /// </summary>
        [JsonPropertyName("subType")]
        public string SubType { get; set; }

        /// <summary>
        /// Reference to the <c>id</c> of the composition the observation
        /// originates from.
        /// </summary>
        [JsonPropertyName("compositionId")]
        public string CompositionId { get; set; }

        /// <summary>
        /// The timestamp at which the observation was recorded.
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The sequence number assigned to the observation by the agent.
        /// </summary>
        [JsonPropertyName("sequence")]
        public ulong Sequence { get; set; }

        /// <summary>
        /// The instance identifier of the agent that produced the observation.
        /// </summary>
        [JsonPropertyName("instanceId")]
        public ulong InstanceId { get; set; }

        /// <summary>
        /// Indicates whether a resettable observation's accumulated value was
        /// reset on this observation.
        /// </summary>
        [JsonPropertyName("resetTriggered")]
        public string ResetTriggered { get; set; }

        /// <summary>
        /// The reported value, for a VALUE representation.
        /// </summary>
        [JsonPropertyName("result")]
        public string Result { get; set; }

        /// <summary>
        /// The reported samples, for a TIME_SERIES representation.
        /// </summary>
        [JsonPropertyName("samples")]
        public IEnumerable<string> Samples { get; set; }

        /// <summary>
        /// The reported entries, for a DATA_SET or TABLE representation.
        /// </summary>
        [JsonPropertyName("entries")]
        public IEnumerable<JsonEntry> Entries { get; set; }

        /// <summary>
        /// The number of entries or samples reported, for a DATA_SET, TABLE,
        /// or TIME_SERIES representation.
        /// </summary>
        [JsonPropertyName("count")]
        public long? Count { get; set; }

        /// <summary>
        /// The native code of a condition observation as reported by the data
        /// source.
        /// </summary>
        [JsonPropertyName("nativeCode")]
        public string NativeCode { get; set; }

        /// <summary>
        /// The asset type, for an asset-changed or asset-removed observation.
        /// </summary>
        [JsonPropertyName("assetType")]
        public string AssetType { get; set; }


        /// <summary>
        /// Creates the surrogate entries for a DATA_SET observation from its
        /// strongly-typed entries, or null when there are none.
        /// </summary>
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

        /// <summary>
        /// Creates the surrogate entries for a TABLE observation from its
        /// strongly-typed entries, or null when there are none.
        /// </summary>
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

        /// <summary>
        /// Creates the string-formatted samples for a TIME_SERIES observation
        /// from its strongly-typed sample values, or null when there are none.
        /// </summary>
        public static IEnumerable<string> CreateTimeSeriesSamples(IEnumerable<double> samples)
        {
            if (!samples.IsNullOrEmpty())
            {
                var jsonResults = new List<string>();
                foreach (var sample in samples)
                {
                    jsonResults.Add(sample.ToString());
                }
                return jsonResults;
            }

            return null;
        }
    }
}