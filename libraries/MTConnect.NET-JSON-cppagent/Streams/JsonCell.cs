// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations;
using System.Text.Json.Serialization;

namespace MTConnect.Streams.Json
{
    /// <summary>
    /// JSON serialization surrogate for a single cell of a table-entry
    /// observation. Carried inside a <see cref="JsonEntry"/>'s cell
    /// list when expressed as discrete cell objects, before the parent
    /// <see cref="MTConnect.NET_JSON_cppagent.Streams.JsonTableEntries"/> converter collapses cells into
    /// the row dictionary on emission.
    /// </summary>
    public class JsonCell
    {
        /// <summary>
        /// The cell key.
        /// </summary>
        [JsonPropertyName("key")]
        public string Key { get; set; }

        /// <summary>
        /// The cell value as a string.
        /// </summary>
        [JsonPropertyName("value")]
        public string Value { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonCell() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="ITableCell"/>.
        /// </summary>
        public JsonCell(ITableCell cell)
        {
            if (cell != null)
            {
                Key = cell.Key;
                Value = cell.Value;
            }
        }
    }
}