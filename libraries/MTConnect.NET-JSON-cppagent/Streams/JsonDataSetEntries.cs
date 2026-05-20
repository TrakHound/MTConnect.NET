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
    /// JSON serialization surrogate for the key/value entries carried by a
    /// DATA_SET observation in the cppagent-compatible shape. Entries are
    /// serialized as a JSON object keyed by entry name with numeric or
    /// string values inferred per entry, and the unavailable state is
    /// collapsed to the string <c>UNAVAILABLE</c>.
    /// </summary>
    [JsonConverter(typeof(JsonDataSetEntriesConverter))]
    public class JsonDataSetEntries
    {
        /// <summary>
        /// The data-set entries keyed by entry name.
        /// </summary>
        public Dictionary<string, object> Entries { get; set; }

        /// <summary>
        /// The number of entries, captured at construction for round-trip
        /// and inspection convenience.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// True when the underlying data-set observation reported the
        /// <c>UNAVAILABLE</c> sentinel rather than a set of entries.
        /// </summary>
        public bool IsUnavailable { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonDataSetEntries() { }

        /// <summary>
        /// Initializes an unavailable-marker instance that will be
        /// serialized as the string <c>UNAVAILABLE</c>.
        /// </summary>
        public JsonDataSetEntries(bool isUnavailable)
        {
            IsUnavailable = isUnavailable;
        }

        /// <summary>
        /// Initializes the surrogate from a dictionary of entries, also
        /// caching the entry count.
        /// </summary>
        public JsonDataSetEntries(Dictionary<string, object> entries)
        {
            Entries = entries;
            Count = entries != null ? entries.Count : 0;
        }


        /// <summary>
        /// <see cref="JsonConverter{T}"/> that reads and writes
        /// <see cref="JsonDataSetEntries"/> as either a keyed JSON object
        /// or the <c>UNAVAILABLE</c> string sentinel.
        /// </summary>
        public class JsonDataSetEntriesConverter : JsonConverter<JsonDataSetEntries>
        {
#if NET5_0_OR_GREATER
            /// <summary>
            /// Returns <c>true</c> so the converter is invoked for null
            /// or unavailable JSON values.
            /// </summary>
            public override bool HandleNull => true;
#endif

            /// <summary>
            /// Reads a JSON object into a
            /// <see cref="JsonDataSetEntries"/>, returning <c>null</c>
            /// when the token is not an object (the unavailable sentinel
            /// case).
            /// </summary>
            public override JsonDataSetEntries Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.StartObject)
                {
#if NET6_0_OR_GREATER
                    var obj = JsonObject.Parse(ref reader);
                    var entries = obj.Deserialize<Dictionary<string, object>>();
                    return new JsonDataSetEntries(entries);
#endif
                }
                else
                {
                    reader.TrySkip(); // Unavailable
                }

                return null;
            }

            /// <summary>
            /// Writes the surrogate as a JSON object whose value types
            /// are inferred per entry (number vs. string), or as the
            /// <c>UNAVAILABLE</c> string sentinel when no entries are
            /// available.
            /// </summary>
            public override void Write(Utf8JsonWriter writer, JsonDataSetEntries value, JsonSerializerOptions options)
            {
                if (value != null && !value.IsUnavailable)
                {
                    writer.WriteStartObject();

                    if (!value.Entries.IsNullOrEmpty())
                    {
                        foreach (var entry in value.Entries)
                        {
                            if (entry.Value.IsNumeric())
                            {
                                writer.WriteNumber(entry.Key, entry.Value.ToDouble());
                            }
                            else
                            {
                                writer.WriteString(entry.Key, entry.Value?.ToString());
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
