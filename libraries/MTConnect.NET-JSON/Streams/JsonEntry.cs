// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Streams.Json
{
    /// <summary>
    /// JSON serialization surrogate for a single entry of a DATA_SET or TABLE
    /// observation. A data-set entry carries a scalar value, while a table
    /// entry instead carries a collection of cells.
    /// </summary>
    public class JsonEntry
    {
        /// <summary>
        /// The key identifying the entry within its data set or table.
        /// </summary>
        [JsonPropertyName("key")]
        public string Key { get; set; }

        /// <summary>
        /// Whether the entry has been removed from the data set or table.
        /// </summary>
        [JsonPropertyName("removed")]
        public bool Removed { get; set; }

        /// <summary>
        /// The entry value for a data-set entry; unused for table entries.
        /// </summary>
        [JsonPropertyName("value")]
        public string Value { get; set; }

        /// <summary>
        /// The cells of a table entry; unused for data-set entries.
        /// </summary>
        [JsonPropertyName("cells")]
        public IEnumerable<JsonCell> Cells { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonEntry() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed data-set entry.
        /// </summary>
        public JsonEntry(IDataSetEntry entry)
        {
            if (entry != null)
            {
                Key = entry.Key;
                Value = entry.Value;
                Removed = entry.Removed;
            }
        }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed table entry,
        /// converting each of its cells.
        /// </summary>
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