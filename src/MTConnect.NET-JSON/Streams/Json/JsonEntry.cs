// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Observations
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
