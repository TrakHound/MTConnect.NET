// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.NET_JSON_cppagent.Streams;
using MTConnect.Observations;
using MTConnect.Observations.Output;
using System.Text.Json.Serialization;

namespace MTConnect.Streams.Json
{
    /// <summary>
    /// JSON serialization surrogate for a SAMPLE observation carrying a
    /// scalar value in the cppagent-compatible Streams shape. The
    /// numeric value is emitted under the <c>value</c> property and
    /// routed through <see cref="JsonSampleValueConverter"/> so that the
    /// <c>UNAVAILABLE</c> sentinel can be round-tripped as a string while
    /// regular readings stay numeric.
    /// </summary>
    public class JsonSampleValue : JsonObservation
    {
        /// <summary>
        /// The sample rate the observation was reported at, in Hz.
        /// </summary>
        [JsonPropertyName("sampleRate")]
        public double? SampleRate { get; set; }

        /// <summary>
        /// The statistical function applied to the underlying data (for
        /// example AVERAGE, MAXIMUM).
        /// </summary>
        [JsonPropertyName("statistic")]
        public string Statistic { get; set; }

        /// <summary>
        /// The time-window duration the statistic was computed over, in
        /// seconds.
        /// </summary>
        [JsonPropertyName("duration")]
        public double? Duration { get; set; }

        /// <summary>
        /// The scalar value of the sample, written as a number when
        /// available and as the <c>UNAVAILABLE</c> string sentinel
        /// otherwise.
        /// </summary>
        [JsonPropertyName("value")]
        [JsonConverter(typeof(JsonSampleValueConverter))]
        public object Value { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonSampleValue() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed sample
        /// <see cref="IObservation"/>, optionally surfacing category and
        /// instance-id, and pulling the value, reset trigger, sample
        /// rate, duration, and statistic from the observation's value
        /// bag.
        /// </summary>
        public JsonSampleValue(IObservation observation, bool categoryOutput = false, bool instanceIdOutput = false)
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

                Value = observation.GetValue(ValueKeys.Result);
                ResetTriggered = observation.GetValue(ValueKeys.ResetTriggered);
                SampleRate = observation.GetValue(ValueKeys.SampleRate)?.ToDouble();
                Duration = observation.GetValue(ValueKeys.Duration)?.ToDouble();

                var statistic = observation.GetValue(ValueKeys.Statistic);
                if (statistic != null) Statistic = statistic;
            }
        }

        /// <summary>
        /// Initializes the surrogate from a streaming
        /// <see cref="IObservationOutput"/>, pulling the same value-bag
        /// fields as the model-level constructor.
        /// </summary>
        public JsonSampleValue(IObservationOutput observation)
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

                Value = observation.GetValue(ValueKeys.Result);
                ResetTriggered = observation.GetValue(ValueKeys.ResetTriggered);
                SampleRate = observation.GetValue(ValueKeys.SampleRate)?.ToDouble();
                Duration = observation.GetValue(ValueKeys.Duration)?.ToDouble();

                var statistic = observation.GetValue(ValueKeys.Statistic);
                if (statistic != null) Statistic = statistic;
            }
        }

        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="ISampleValueObservation"/>, restoring the data-item
        /// type from the supplied dictionary key and guarding the optional
        /// reset-trigger/statistic fields so an absent JSON property does
        /// not stamp a stray default enumeration value.
        /// </summary>
        public ISampleValueObservation ToObservation(string type)
        {
            // Route construction through the typed factory so the runtime
            // type discriminator survives the envelope read path. A naked
            // `new SampleValueObservation()` collapses typed sample
            // subclasses back to the abstract value carrier.
            var sample = SampleObservation.Create(type, DataItemRepresentation.VALUE) as SampleValueObservation;
            if (sample == null) sample = new SampleValueObservation();
            sample.DataItemId = DataItemId;
            sample.Timestamp = Timestamp;
            sample.Name = Name;
            sample.InstanceId = InstanceId;
            sample.Sequence = Sequence;
            //sample.Category = Category.ConvertEnum<DataItemCategory>();
            sample.Type = type;
            sample.SubType = SubType;
            sample.CompositionId = CompositionId;
            // Null-guard ResetTriggered/Statistic so omitting the JSON
            // property does not stamp the observation with a stray
            // default-enum value (mirrors JsonDataItem.ToDataItem).
            if (!string.IsNullOrEmpty(ResetTriggered)) sample.ResetTriggered = ResetTriggered.ConvertEnum<ResetTriggered>();
            if (Value != null) sample.Result = Value.ToString();
            sample.SampleRate = SampleRate.HasValue ? SampleRate.Value : 0;
            if (!string.IsNullOrEmpty(Statistic)) sample.Statistic = Statistic.ConvertEnum<DataItemStatistic>();
            sample.Duration = Duration.HasValue ? Duration.Value : 0;
            return sample;
        }
    }
}