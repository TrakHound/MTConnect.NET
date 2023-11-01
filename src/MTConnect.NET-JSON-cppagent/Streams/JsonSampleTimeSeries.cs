// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.NET_JSON_cppagent.Streams;
using MTConnect.Observations;
using MTConnect.Observations.Output;
using System.Linq;
using System.Text.Json.Serialization;

namespace MTConnect.Streams.Json
{
    public class JsonSampleTimeSeries : JsonObservation
    {
        [JsonPropertyName("value")]
        public JsonTimeSeriesSamples Samples { get; set; }

        [JsonPropertyName("sampleCount")]
        public long? SampleCount { get; set; }

        [JsonPropertyName("sampleRate")]
        public double? SampleRate { get; set; }


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

                    if (!timeseriesObservation.Samples.IsNullOrEmpty())
                    {
                        Samples = CreateTimeSeriesSamples(timeseriesObservation.Samples);
                        SampleCount = Samples != null ? Samples.Count : 0;
                        SampleRate = timeseriesObservation.SampleRate;
                    }
                    else
                    {
                        Samples = new JsonTimeSeriesSamples(true);
                        SampleCount = 0;
                    }
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

                // TimeSeries Samples
                if (observation.Representation == DataItemRepresentation.TIME_SERIES)
                {
                    var timeseriesObservation = new SampleTimeSeriesObservation();
                    timeseriesObservation.AddValues(observation.Values);

                    if (!timeseriesObservation.Samples.IsNullOrEmpty())
                    {
                        Samples = CreateTimeSeriesSamples(timeseriesObservation.Samples);
                        SampleCount = Samples != null ? Samples.Count : 0;
                        SampleRate = timeseriesObservation.SampleRate;
                    }
                    else
                    {
                        Samples = new JsonTimeSeriesSamples(true);
                        SampleCount = 0;
                    }
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

            if (SampleRate != null) observation.SampleRate = SampleRate.Value;

            if (Samples != null && !Samples.IsUnavailable)
            {
                if (!Samples.Samples.IsNullOrEmpty())
                {
                    var samples = Samples.Samples.ToArray();
                    for (var i = 0; i < samples.Length; i++)
                    {
                        observation.AddValue(ValueKeys.CreateTimeSeriesValueKey(i), samples[i]);
                    }
                }
            }
            else
            {
                observation.AddValue(ValueKeys.Result, Observation.Unavailable);
            }

            return observation;
        }
    }
}