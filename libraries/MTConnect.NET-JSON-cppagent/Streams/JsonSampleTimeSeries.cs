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
    /// <summary>
    /// JSON serialization surrogate for a SAMPLE observation carrying a
    /// TIME_SERIES representation in the cppagent-compatible Streams
    /// shape. The series of sample values is emitted as a numeric JSON
    /// array on the <c>value</c> property (via
    /// <see cref="JsonTimeSeriesSamples"/>), with the sample count and
    /// rate carried alongside.
    /// </summary>
    public class JsonSampleTimeSeries : JsonObservation
    {
        /// <summary>
        /// The series of sample values, emitted as a numeric JSON array
        /// or as the <c>UNAVAILABLE</c> string sentinel.
        /// </summary>
        [JsonPropertyName("value")]
        public JsonTimeSeriesSamples Samples { get; set; }

        /// <summary>
        /// The number of samples in the series.
        /// </summary>
        [JsonPropertyName("sampleCount")]
        public long? SampleCount { get; set; }

        /// <summary>
        /// The sample rate, in Hz.
        /// </summary>
        [JsonPropertyName("sampleRate")]
        public double? SampleRate { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonSampleTimeSeries() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed time-series
        /// <see cref="IObservation"/>, optionally surfacing category and
        /// instance-id. Emits an unavailable marker when the source has
        /// no samples.
        /// </summary>
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

        /// <summary>
        /// Initializes the surrogate from a streaming
        /// <see cref="IObservationOutput"/>, rehydrating the time-series
        /// samples from the observation's value bag.
        /// </summary>
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

        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="ISampleTimeSeriesObservation"/>, restoring the
        /// data-item type from the supplied dictionary key and writing
        /// each sample into the observation's keyed value bag, or
        /// emitting the <c>UNAVAILABLE</c> result when the series is
        /// marked unavailable.
        /// </summary>
        public ISampleTimeSeriesObservation ToObservation(string type)
        {
            // Route construction through the typed factory so the runtime
            // type discriminator survives the envelope read path.
            var observation = SampleObservation.Create(type, DataItemRepresentation.TIME_SERIES) as SampleTimeSeriesObservation;
            if (observation == null) observation = new SampleTimeSeriesObservation();
            observation.DataItemId = DataItemId;
            observation.Timestamp = Timestamp;
            observation.Name = Name;
            observation.InstanceId = InstanceId;
            observation.Sequence = Sequence;
            //observation.Category = Category.ConvertEnum<DataItemCategory>();
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