// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Streams.Json
{
    public class JsonObservation
    {
        [JsonPropertyName("dataItemId")]
        public string DataItemId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; }

        [JsonPropertyName("representation")]
        public string Representation { get; set; }

        //[JsonIgnore]
        //public string Type { get; set; }

        [JsonPropertyName("subType")]
        public string SubType { get; set; }

        [JsonPropertyName("compositionId")]
        public string CompositionId { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("sequence")]
        public long Sequence { get; set; }

        [JsonPropertyName("instanceId")]
        public long InstanceId { get; set; }

        [JsonPropertyName("resetTriggered")]
        public string ResetTriggered { get; set; }

        //[JsonPropertyName("value")]
        //public string Value { get; set; }

        //[JsonPropertyName("samples")]
        //public IEnumerable<string> Samples { get; set; }

        //[JsonPropertyName("value")]
        //public IEnumerable<JsonEntry> Entries { get; set; }

        //[JsonPropertyName("count")]
        //public long? Count { get; set; }

        [JsonPropertyName("nativeCode")]
        public string NativeCode { get; set; }

        [JsonPropertyName("assetType")]
        public string AssetType { get; set; }


        public static Dictionary<string, object> CreateDataSetEntries(IEnumerable<IDataSetEntry> entries)
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
                return jsonEntries;
            }

            return null;
        }

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


        public static Dictionary<string, Dictionary<string, object>> CreateTableEntries(IEnumerable<ITableEntry> entries)
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
                return jsonEntries;
            }

            return null;
        }

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
    }
}