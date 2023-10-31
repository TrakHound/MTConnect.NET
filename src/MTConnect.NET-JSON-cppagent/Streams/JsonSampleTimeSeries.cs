// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.Observations;
using MTConnect.Observations.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace MTConnect.Streams.Json
{
    public class JsonSampleTimeSeries : JsonObservation
    {
        [JsonPropertyName("value")]
        [JsonConverter(typeof(JsonSampleTimeSeriesValueConverter))]
        public IEnumerable<double> Samples { get; set; }

        [JsonPropertyName("count")]
        public long? Count { get; set; }


        public JsonSampleTimeSeries() { }

        public JsonSampleTimeSeries(IObservation observation, bool categoryOutput = false, bool instanceIdOutput = false)
        {
            if (observation != null)
            {
                DataItemId = observation.DataItemId;
                if (categoryOutput) Category = observation.Category.ToString();
                if (instanceIdOutput) InstanceId = observation.InstanceId;
                Timestamp = observation.Timestamp;
                Name = observation.Name;
                Sequence = observation.Sequence;
                SubType = observation.SubType;
                CompositionId = observation.CompositionId;
                ResetTriggered = observation.GetValue(ValueKeys.ResetTriggered);
                NativeCode = observation.GetValue(ValueKeys.NativeCode);
                AssetType = observation.GetValue(ValueKeys.AssetType);

                // TimeSeries Entries
                if (observation is SampleTimeSeriesObservation)
                {
                    var timeseriesObservation = (SampleTimeSeriesObservation)observation;
                    Samples = timeseriesObservation.Samples;
                    Count = timeseriesObservation.SampleCount;
                }
            }
        }

        public JsonSampleTimeSeries(IObservationOutput observation)
        {
            if (observation != null)
            {
                DataItemId = observation.DataItemId;
                Timestamp = observation.Timestamp;
                Name = observation.Name;
                InstanceId = observation.InstanceId;
                Sequence = observation.Sequence;
                SubType = observation.SubType;
                CompositionId = observation.CompositionId;
                ResetTriggered = observation.GetValue(ValueKeys.ResetTriggered);
                NativeCode = observation.GetValue(ValueKeys.NativeCode);
                AssetType = observation.GetValue(ValueKeys.AssetType);

                // DataSet Entries
                if (observation.Representation == DataItemRepresentation.TABLE)
                {
                    var timeseriesObservation = new SampleTimeSeriesObservation();
                    timeseriesObservation.AddValues(observation.Values);
                    Samples = timeseriesObservation.Samples;
                    Count = timeseriesObservation.SampleCount;
                }
            }
        }

        public ISampleTimeSeriesObservation ToObservation(string type)
        {
            var observation = new SampleTimeSeriesObservation();
            observation.DataItemId = DataItemId;
            observation.Timestamp = Timestamp;
            observation.Name = Name;
            observation.InstanceId = InstanceId;
            observation.Sequence = Sequence;
            observation.Category = Category.ConvertEnum<DataItemCategory>();
            observation.Type = type;
            observation.SubType = SubType;
            observation.CompositionId = CompositionId;
            observation.ResetTriggered = ResetTriggered.ConvertEnum<ResetTriggered>();

            if (!Samples.IsNullOrEmpty())
            {
                var samples = Samples.ToArray();
                for (var i = 0; i < samples.Length - 1; i++)
                {
                    observation.AddValue(ValueKeys.CreateTimeSeriesValueKey(i), samples[i]);
                }
            }
            else
            {
                observation.AddValue(ValueKeys.Result, Observation.Unavailable);
            }

            return observation;
        }

        public class JsonSampleTimeSeriesValueConverter : JsonConverter<IEnumerable<double>>
        {
            public override IEnumerable<double> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    var obj = JsonObject.Parse(ref reader);
                    return obj.Deserialize<IEnumerable<double>>();
                }
                else
                {
                    reader.Skip(); // Unavailable
                }

                return null;
            }

            public override void Write(Utf8JsonWriter writer, IEnumerable<double> value, JsonSerializerOptions options)
            {
                //writer.WriteStringValue(dateTimeValue.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
            }
        }
    }
}