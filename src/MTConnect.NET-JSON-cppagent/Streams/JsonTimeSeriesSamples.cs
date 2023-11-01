// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace MTConnect.NET_JSON_cppagent.Streams
{
    [JsonConverter(typeof(JsonTimeSeriesSamplesConverter))]
    public class JsonTimeSeriesSamples
    {
        public IEnumerable<double> Samples { get; set; }

        public int Count { get; set; }

        public bool IsUnavailable { get; set; }


        public JsonTimeSeriesSamples() { }

        public JsonTimeSeriesSamples(bool isUnavailable) 
        {
            IsUnavailable = isUnavailable;
        }

        public JsonTimeSeriesSamples(IEnumerable<double> samples)
        {
            Samples = samples;
            Count = samples != null ? samples.Count() : 0;
        }


        public class JsonTimeSeriesSamplesConverter : JsonConverter<JsonTimeSeriesSamples>
        {
            public override bool HandleNull => true;


            public override JsonTimeSeriesSamples Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.StartArray)
                {
                    var obj = JsonObject.Parse(ref reader);
                    var entries = obj.Deserialize<IEnumerable<double>>();
                    return new JsonTimeSeriesSamples(entries);
                }
                else
                {
                    reader.Skip(); // Unavailable
                }

                return null;
            }

            public override void Write(Utf8JsonWriter writer, JsonTimeSeriesSamples value, JsonSerializerOptions options)
            {
                if (value != null && !value.IsUnavailable)
                {
                    writer.WriteStartArray();

                    if (!value.Samples.IsNullOrEmpty())
                    {
                        foreach (var sample in value.Samples)
                        {
                            writer.WriteNumberValue(sample);
                        }
                    }

                    writer.WriteEndArray();
                }
                else
                {
                    writer.WriteStringValue(Observation.Unavailable);
                }
            }
        }
    }
}
