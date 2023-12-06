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
    [JsonConverter(typeof(JsonTableEntriesConverter))]
    public class JsonTableEntries
    {
        public Dictionary<string, Dictionary<string, object>> Entries { get; set; }

        public int Count { get; set; }

        public bool IsUnavailable { get; set; }


        public JsonTableEntries() { }

        public JsonTableEntries(bool isUnavailable) 
        {
            IsUnavailable = isUnavailable;
        }

        public JsonTableEntries(Dictionary<string, Dictionary<string, object>> entries)
        { 
            Entries = entries;
            Count = entries != null ? entries.Count : 0;
        }


        public class JsonTableEntriesConverter : JsonConverter<JsonTableEntries>
        {
#if NET5_0_OR_GREATER
            public override bool HandleNull => true;
#endif

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
                    reader.Skip(); // Unavailable
                }

                return null;
            }

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
