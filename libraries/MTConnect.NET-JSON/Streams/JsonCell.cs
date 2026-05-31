// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations;
using System.Text.Json.Serialization;

namespace MTConnect.Streams.Json
{
    /// <summary>
    /// JSON serialization surrogate for a TABLE-observation <c>Cell</c>, a
    /// single column value within a table entry. Constructs from a
    /// strongly-typed <see cref="ITableCell"/>.
    /// </summary>
    public class JsonCell
    {
        /// <summary>
        /// The column key.
        /// </summary>
        [JsonPropertyName("key")]
        public string Key { get; set; }

        /// <summary>
        /// The cell value.
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