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
    [JsonConverter(typeof(JsonDataSetEntriesConverter))]
    public class JsonDataSetEntries
    {
        public Dictionary<string, object> Entries { get; set; }

        public int Count { get; set; }

        public bool IsUnavailable { get; set; }


        public JsonDataSetEntries() { }

        public JsonDataSetEntries(bool isUnavailable) 
        {
            IsUnavailable = isUnavailable;
        }

        public JsonDataSetEntries(Dictionary<string, object> entries)
        { 
            Entries = entries;
            Count = entries != null ? entries.Count : 0;
        }


        public class JsonDataSetEntriesConverter : JsonConverter<JsonDataSetEntries>
        {
#if NET5_0_OR_GREATER
            public override bool HandleNull => true;
#endif

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
                    reader.Skip(); // Unavailable
                }

                return null;
            }

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
