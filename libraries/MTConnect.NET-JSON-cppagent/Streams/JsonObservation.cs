// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.NET_JSON_cppagent.Streams;
using MTConnect.Observations;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Streams.Json
{
    /// <summary>
    /// JSON serialization surrogate for an MTConnect observation in the
    /// cppagent-compatible shape. Carries the metadata common to samples,
    /// events, and conditions; subclasses add representation-specific
    /// payloads. Static helpers project data-set, table, and time-series
    /// payloads to and from their wire representations.
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
        /// The representation of the observation.
        /// </summary>
        [JsonPropertyName("representation")]
        public string Representation { get; set; }

        /// <summary>
        /// The subtype further qualifying the data item's type.
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
        /// Converts strongly-typed data set entries to the cppagent key/value
        /// JSON shape, parsing numeric values to double when possible.
        /// </summary>
        public static JsonDataSetEntries CreateDataSetEntries(IEnumerable<IDataSetEntry> entries)
        {
            if (!entries.IsNullOrEmpty())
            {
                var jsonEntries = new Dictionary<string, object>();
                foreach (var entry in entries)
                {
                    if (!jsonEntries.ContainsKey(entry.Key))
                    {
                        if (double.TryParse(entry.Value, out var value))
                        {
                            jsonEntries.Add(entry.Key, value);
                        }
                        else
                        {
                            jsonEntries.Add(entry.Key, entry.Value);
                        }
                    }
                }

                return new JsonDataSetEntries(jsonEntries);
            }

            return null;
        }

        /// <summary>
        /// Converts the cppagent key/value JSON shape back to strongly-typed
        /// data set entries.
        /// </summary>
        public static IEnumerable<IDataSetEntry> CreateDataSetEntries(Dictionary<string, object> entries)
        {
            if (!entries.IsNullOrEmpty())
            {
                var jsonEntries = new List<IDataSetEntry>();
                foreach (var entry in entries)
                {
                    jsonEntries.Add(new DataSetEntry(entry.Key, entry.Value));
                }
                return jsonEntries;
            }

            return null;
        }


        /// <summary>
        /// Converts strongly-typed table entries to the cppagent
        /// row/key/value nested JSON shape, parsing numeric cell values to
        /// double when possible.
        /// </summary>
        public static JsonTableEntries CreateTableEntries(IEnumerable<ITableEntry> entries)
        {
            if (!entries.IsNullOrEmpty())
            {
                var jsonEntries = new Dictionary<string, Dictionary<string, object>>();
                foreach (var entry in entries)
                {
                    if (!jsonEntries.ContainsKey(entry.Key) && !entry.Cells.IsNullOrEmpty())
                    {
                        var cells = new Dictionary<string, object>();

                        foreach (var cell in entry.Cells)
                        {
                            if (!cells.ContainsKey(entry.Key))
                            {
                                if (double.TryParse(cell.Value, out var value))
                                {
                                    cells.Add(cell.Key, value);
                                }
                                else
                                {
                                    cells.Add(cell.Key, cell.Value);
                                }
                            }
                        }

                        jsonEntries.Add(entry.Key, cells);
                    }
                }
                return new JsonTableEntries(jsonEntries);
            }

            return null;
        }

        /// <summary>
        /// Converts the cppagent row/key/value nested JSON shape back to
        /// strongly-typed table entries.
        /// </summary>
        public static IEnumerable<ITableEntry> CreateTableEntries(Dictionary<string, Dictionary<string, object>> entries)
        {
            if (!entries.IsNullOrEmpty())
            {
                var jsonEntries = new List<ITableEntry>();
                foreach (var entry in entries)
                {
                    if (entry.Value != null)
                    {
                        var cells = new List<ITableCell>();
                        foreach (var cell in entry.Value)
                        {
                            cells.Add(new TableCell(cell.Key, cell.Value));
                        }
                        jsonEntries.Add(new TableEntry(entry.Key, cells));
                    }
                }
                return jsonEntries;
            }

            return null;
        }


        /// <summary>
        /// Wraps a sample value sequence as a <see cref="JsonTimeSeriesSamples"/>,
        /// or null when there are none.
        /// </summary>
        public static JsonTimeSeriesSamples CreateTimeSeriesSamples(IEnumerable<double> samples)
        {
            if (!samples.IsNullOrEmpty())
            {
                return new JsonTimeSeriesSamples(samples);
            }

            return null;
        }
    }
}