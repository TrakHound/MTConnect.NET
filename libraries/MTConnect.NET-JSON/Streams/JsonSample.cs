// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.Observations;
using MTConnect.Observations.Output;
using System.Linq;
using System.Text.Json.Serialization;

namespace MTConnect.Streams.Json
{
    /// <summary>
    /// JSON serialization surrogate for a SAMPLE-category observation,
    /// extending <see cref="JsonObservation"/> with the sample-specific
    /// properties. Converts to and from the strongly-typed
    /// <see cref="SampleValueObservation"/> model.
    /// </summary>
    public class JsonSample : JsonObservation
    {
        /// <summary>
        /// The rate, in samples per second, at which the values were sampled
        /// for a TIME_SERIES representation.
        /// </summary>
        [JsonPropertyName("sampleRate")]
        public double? SampleRate { get; set; }

        /// <summary>
        /// The statistical operation (for example AVERAGE or MAXIMUM) applied
        /// to the reported value.
        /// </summary>
        [JsonPropertyName("statistic")]
        public string Statistic { get; set; }

        /// <summary>
        /// The duration, in seconds, the statistic was computed over.
        /// </summary>
        [JsonPropertyName("duration")]
        public double? Duration { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonSample() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IObservation"/>, optionally emitting the category and
        /// instance id, and projecting data-set, table, or time-series payloads
        /// into the corresponding properties.
        /// </summary>
        public JsonSample(IObservation observation, bool categoryOutput = false, bool instanceIdOutput = false)
        {
            if (observation != null)
            {
                DataItemId = observation.DataItemId;
                if (categoryOutput) Category = observation.Category.ToString();
                if (instanceIdOutput) InstanceId = observation.InstanceId;
                Timestamp = observation.Timestamp;
                Name = observation.Name;
                Sequence = observation.Sequence;
                Type = observation.Type;
                SubType = observation.SubType;
                CompositionId = observation.CompositionId;

                Result = observation.GetValue(ValueKeys.Result);
                ResetTriggered = observation.GetValue(ValueKeys.ResetTriggered);
                SampleRate = observation.GetValue(ValueKeys.SampleRate)?.ToDouble();
                Duration = observation.GetValue(ValueKeys.Duration)?.ToDouble();

                var statistic = observation.GetValue(ValueKeys.Statistic);
                if (statistic != null) Statistic = statistic;

                // DataSet Entries
                if (observation is SampleDataSetObservation)
                {
                    Entries = CreateEntries(((SampleDataSetObservation)observation).Entries);
                    Count = !Entries.IsNullOrEmpty() ? Entries.Count() : 0;
                }

                // Table Entries
                if (observation is SampleTableObservation)
                {
                    Entries = CreateEntries(((SampleTableObservation)observation).Entries);
                    Count = !Entries.IsNullOrEmpty() ? Entries.Count() : 0;
                }

                // TimeSeries
                if (observation is SampleTimeSeriesObservation)
                {
                    Samples = CreateTimeSeriesSamples(((SampleTimeSeriesObservation)observation).Samples);
                }
            }
        }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IObservationOutput"/>, reconstructing the data-set,
        /// table, or time-series payload from the observation's raw values.
        /// </summary>
        public JsonSample(IObservationOutput observation)
        {
            if (observation != null)
            {
                DataItemId = observation.DataItemId;
                Timestamp = observation.Timestamp;
                Name = observation.Name;
                InstanceId = observation.InstanceId;
                Sequence = observation.Sequence;
                Type = observation.Type;
                SubType = observation.SubType;
                CompositionId = observation.CompositionId;

                Result = observation.GetValue(ValueKeys.Result);
                ResetTriggered = observation.GetValue(ValueKeys.ResetTriggered);
                SampleRate = observation.GetValue(ValueKeys.SampleRate)?.ToDouble();
                Duration = observation.GetValue(ValueKeys.Duration)?.ToDouble();

                var statistic = observation.GetValue(ValueKeys.Statistic);
                if (statistic != null) Statistic = statistic;

                // DataSet Entries
                if (observation.Representation == DataItemRepresentation.DATA_SET)
                {
                    var dataSetObservation = new SampleDataSetObservation();
                    dataSetObservation.AddValues(observation.Values);
                    Entries = CreateEntries(dataSetObservation.Entries);
                    Count = !Entries.IsNullOrEmpty() ? Entries.Count() : 0;
                }

                // Table Entries
                if (observation.Representation == DataItemRepresentation.TABLE)
                {
                    var tableObservation = new SampleTableObservation();
                    tableObservation.AddValues(observation.Values);
                    Entries = CreateEntries(tableObservation.Entries);
                    Count = !Entries.IsNullOrEmpty() ? Entries.Count() : 0;
                }

                // TimeSeries
                if (observation.Representation == DataItemRepresentation.TIME_SERIES)
                {
                    var timeSeriesObservation = new SampleTimeSeriesObservation();
                    timeSeriesObservation.AddValues(observation.Values);
                    Samples = CreateTimeSeriesSamples(timeSeriesObservation.Samples);
                }
            }
        }

        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="ISampleObservation"/>; currently delegates to
        /// <see cref="ToSampleValue"/> for all representations.
        /// </summary>
        public ISampleObservation ToSample()
        {
            if (Representation == DataItemRepresentation.DATA_SET.ToString())
            {

            }
            else if (Representation == DataItemRepresentation.TABLE.ToString())
            {

            }
            else if (Representation == DataItemRepresentation.TIME_SERIES.ToString())
            {

            }

            return ToSampleValue();
        }

        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="ISampleValueObservation"/>, parsing the category, reset
        /// trigger, and statistic enumerations.
        /// </summary>
        public ISampleValueObservation ToSampleValue()
        {
            var sample = new SampleValueObservation();
            sample.DataItemId = DataItemId;
            sample.Timestamp = Timestamp;
            sample.Name = Name;
            sample.InstanceId = InstanceId;
            sample.Sequence = Sequence;
            sample.Category = Category.ConvertEnum<DataItemCategory>();
            sample.Type = Type;
            sample.SubType = SubType;
            sample.CompositionId = CompositionId;
            sample.ResetTriggered = ResetTriggered.ConvertEnum<ResetTriggered>();
            sample.Result = Result;
            sample.SampleRate = SampleRate.HasValue ? SampleRate.Value : 0;
            sample.Statistic = Statistic.ConvertEnum<DataItemStatistic>();
            sample.Duration = Duration.HasValue ? Duration.Value : 0;
            return sample;
        }
    }
}