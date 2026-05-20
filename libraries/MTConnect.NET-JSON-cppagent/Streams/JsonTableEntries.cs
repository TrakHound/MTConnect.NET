// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations;
using System;
using System.Collections.Generic;
using System.Text.Json;
#if NET6_0_OR_GREATER
using System.Text.Json.Nodes;
#endif
using System.Text.Json.Serialization;

namespace MTConnect.NET_JSON_cppagent.Streams
{
    /// <summary>
    /// JSON serialization surrogate for the keyed table of cell-maps
    /// carried by a TABLE observation in the cppagent-compatible shape.
    /// Each row is a JSON object whose properties are the cell values
    /// (numeric or string, inferred per cell), keyed by row identifier on
    /// the outer object. The unavailable state collapses to the string
    /// <c>UNAVAILABLE</c>.
    /// </summary>
    [JsonConverter(typeof(JsonTableEntriesConverter))]
    public class JsonTableEntries
    {
        /// <summary>
        /// The table rows keyed by row identifier; each row is a map of
        /// cell name to cell value.
        /// </summary>
        public Dictionary<string, Dictionary<string, object>> Entries { get; set; }

        /// <summary>
        /// The number of rows, captured at construction for round-trip
        /// and inspection convenience.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// True when the underlying table observation reported the
        /// <c>UNAVAILABLE</c> sentinel rather than a set of rows.
        /// </summary>
        public bool IsUnavailable { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonTableEntries() { }

        /// <summary>
        /// Initializes an unavailable-marker instance that will be
        /// serialized as the string <c>UNAVAILABLE</c>.
        /// </summary>
        public JsonTableEntries(bool isUnavailable)
        {
            IsUnavailable = isUnavailable;
        }

        /// <summary>
        /// Initializes the surrogate from a nested dictionary of rows,
        /// also caching the row count.
        /// </summary>
        public JsonTableEntries(Dictionary<string, Dictionary<string, object>> entries)
        {
            Entries = entries;
            Count = entries != null ? entries.Count : 0;
        }


        /// <summary>
        /// <see cref="JsonConverter{T}"/> that reads and writes
        /// <see cref="JsonTableEntries"/> as either a nested JSON object
        /// or the <c>UNAVAILABLE</c> string sentinel.
        /// </summary>
        public class JsonTableEntriesConverter : JsonConverter<JsonTableEntries>
        {
#if NET5_0_OR_GREATER
            /// <summary>
            /// Returns <c>true</c> so the converter is invoked for null
            /// or unavailable JSON values.
            /// </summary>
            public override bool HandleNull => true;
#endif

            /// <summary>
            /// Reads a nested JSON object into a
            /// <see cref="JsonTableEntries"/>, returning <c>null</c> when
            /// the token is not an object (the unavailable sentinel case).
            /// </summary>
            public override JsonTableEntries Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.StartObject)
                {
#if NET6_0_OR_GREATER
                    var obj = JsonObject.Parse(ref reader);
                    var entries = obj.Deserialize<Dictionary<string, Dictionary<string, object>>>();
                    return new JsonTableEntries(entries);
#endif
                }
                else
                {
                    reader.TrySkip(); // Unavailable
                }

                return null;
            }

            /// <summary>
            /// Writes the surrogate as a nested JSON object whose row
            /// cell values are inferred per cell (number vs. string), or
            /// as the <c>UNAVAILABLE</c> string sentinel when no rows are
            /// available.
            /// </summary>
            public override void Write(Utf8JsonWriter writer, JsonTableEntries value, JsonSerializerOptions options)
            {
                if (value != null && !value.IsUnavailable)
                {
                    writer.WriteStartObject();

                    if (!value.Entries.IsNullOrEmpty())
                    {
                        foreach (var entry in value.Entries)
                        {
                            writer.WritePropertyName(entry.Key);

                            if (!entry.Value.IsNullOrEmpty())
                            {
                                writer.WriteStartObject();

                                foreach (var cell in entry.Value)
                                {
                                    if (cell.Value.IsNumeric())
                                    {
                                        writer.WriteNumber(cell.Key, cell.Value.ToDouble());
                                    }
                                    else
                                    {
                                        writer.WriteString(cell.Key, cell.Value?.ToString());
                                    }
                                }

                                writer.WriteEndObject();
                            }
                        }
                    }

                    writer.WriteEndObject();
                }
                else
                {
                    writer.WriteStringValue(Observation.Unavailable);
                }
            }
        }
    }
}
