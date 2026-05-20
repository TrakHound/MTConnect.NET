// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
#if NET6_0_OR_GREATER
using System.Text.Json.Nodes;
#endif
using System.Text.Json.Serialization;

namespace MTConnect.NET_JSON_cppagent.Streams
{
    /// <summary>
    /// JSON serialization surrogate for the array of samples carried by a
    /// TIME_SERIES observation in the cppagent-compatible shape. The
    /// samples are serialized as a bare JSON array of numbers rather than
    /// an object, with the unavailable state collapsed to the string
    /// <c>UNAVAILABLE</c>.
    /// </summary>
    [JsonConverter(typeof(JsonTimeSeriesSamplesConverter))]
    public class JsonTimeSeriesSamples
    {
        /// <summary>
        /// The series of sample values.
        /// </summary>
        public IEnumerable<double> Samples { get; set; }

        /// <summary>
        /// The number of samples in the series, captured at construction
        /// for round-trip and inspection convenience.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// True when the underlying time-series observation reported the
        /// <c>UNAVAILABLE</c> sentinel rather than a numeric series.
        /// </summary>
        public bool IsUnavailable { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonTimeSeriesSamples() { }

        /// <summary>
        /// Initializes an unavailable-marker instance that will be
        /// serialized as the string <c>UNAVAILABLE</c>.
        /// </summary>
        public JsonTimeSeriesSamples(bool isUnavailable)
        {
            IsUnavailable = isUnavailable;
        }

        /// <summary>
        /// Initializes the surrogate from a sequence of sample values,
        /// also caching the sample count.
        /// </summary>
        public JsonTimeSeriesSamples(IEnumerable<double> samples)
        {
            Samples = samples;
            Count = samples != null ? samples.Count() : 0;
        }


        /// <summary>
        /// <see cref="JsonConverter{T}"/> that reads and writes
        /// <see cref="JsonTimeSeriesSamples"/> as either a numeric JSON
        /// array or the <c>UNAVAILABLE</c> string sentinel.
        /// </summary>
        public class JsonTimeSeriesSamplesConverter : JsonConverter<JsonTimeSeriesSamples>
        {
#if NET5_0_OR_GREATER
            /// <summary>
            /// Returns <c>true</c> so the converter is invoked for null
            /// or unavailable JSON values, allowing it to emit the
            /// <c>UNAVAILABLE</c> sentinel.
            /// </summary>
            public override bool HandleNull => true;
#endif

            /// <summary>
            /// Reads a JSON numeric array into a
            /// <see cref="JsonTimeSeriesSamples"/>, returning
            /// <c>null</c> when the token is not an array (the unavailable
            /// sentinel case).
            /// </summary>
            public override JsonTimeSeriesSamples Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.StartArray)
                {
#if NET6_0_OR_GREATER
                    var obj = JsonObject.Parse(ref reader);
                    var entries = obj.Deserialize<IEnumerable<double>>();
                    return new JsonTimeSeriesSamples(entries);
#endif
                }
                else
                {
                    reader.TrySkip(); // Unavailable
                }

                return null;
            }

            /// <summary>
            /// Writes the surrogate as a JSON numeric array, or as the
            /// <c>UNAVAILABLE</c> string sentinel when no samples are
            /// available.
            /// </summary>
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
