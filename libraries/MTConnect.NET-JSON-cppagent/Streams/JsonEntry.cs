// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Streams.Json
{
    /// <summary>
    /// JSON serialization surrogate for a single entry of a DATA_SET or
    /// TABLE observation. Carries the entry key, removed flag, a scalar
    /// value (for data-set entries) or a list of cell values (for table
    /// entries). The cppagent shape collapses these into keyed
    /// dictionaries on the parent observation, so this type is also used
    /// as a convenience constructor target.
    /// </summary>
    public class JsonEntry
    {
        /// <summary>
        /// The entry key.
        /// </summary>
        [JsonPropertyName("key")]
        public string Key { get; set; }

        /// <summary>
        /// True if the entry was removed in this observation rather than
        /// updated.
        /// </summary>
        [JsonPropertyName("removed")]
        public bool Removed { get; set; }

        /// <summary>
        /// The scalar value of a data-set entry, serialized as a string.
        /// </summary>
        [JsonPropertyName("value")]
        public string Value { get; set; }

        /// <summary>
        /// The cell values of a table entry.
        /// </summary>
        [JsonPropertyName("cells")]
        public IEnumerable<JsonCell> Cells { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonEntry() { }

        /// <summary>
        /// Convenience constructor that captures a key/value pair from
        /// the underlying value bag, coercing the value to its string
        /// representation.
        /// </summary>
        public JsonEntry(string key, object value)
        {
            Key = key;
            Value = value?.ToString();
        }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IDataSetEntry"/>.
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
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="ITableEntry"/>, projecting each cell into a
        /// <see cref="JsonCell"/>.
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