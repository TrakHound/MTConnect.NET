// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Streams.Json
{
    public class JsonEntry
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("removed")]
        public bool Removed { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("cells")]
        public IEnumerable<JsonCell> Cells { get; set; }


        public JsonEntry() { }

        public JsonEntry(string key, object value)
        {
            Key = key;
            Value = value?.ToString();
        }

        public JsonEntry(IDataSetEntry entry)
        {
            if (entry != null)
            {
                Key = entry.Key;
                Value = entry.Value;
                Removed = entry.Removed;
            }
        }

        public JsonEntry(ITableEntry entry)
        {
            if (entry != null)
            {
                Key = entry.Key;
                Removed = entry.Removed;

                if (!entry.Cells.IsNullOrEmpty())
                {
                    var cells = new List<JsonCell>();
                    foreach (var cell in entry.Cells)
                    {
                        cells.Add(new JsonCell(cell));
                    }
                    Cells = cells;
                }
            }
        }
    }
}