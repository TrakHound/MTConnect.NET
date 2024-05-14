// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.Observations;
using MTConnect.Observations.Output;
using System.Text.Json.Serialization;

namespace MTConnect.Streams.Json
{
    public class JsonSampleValue : JsonObservation
    {
        [JsonPropertyName("sampleRate")]
        public double? SampleRate { get; set; }

        [JsonPropertyName("statistic")]
        public string Statistic { get; set; }

        [JsonPropertyName("duration")]
        public double? Duration { get; set; }

        [JsonPropertyName("value")]
        public object Value { get; set; }


        public JsonSampleValue() { }

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

        public ISampleValueObservation ToObservation(string type)
        {
            var sample = new SampleValueObservation();
            sample.DataItemId = DataItemId;
            sample.Timestamp = Timestamp;
            sample.Name = Name;
            sample.InstanceId = InstanceId;
            sample.Sequence = Sequence;
            //sample.Category = Category.ConvertEnum<DataItemCategory>();
            sample.Type = type;
            sample.SubType = SubType;
            sample.CompositionId = CompositionId;
            sample.ResetTriggered = ResetTriggered.ConvertEnum<ResetTriggered>();
            if (Value != null) sample.Result = Value.ToString();
            sample.SampleRate = SampleRate.HasValue ? SampleRate.Value : 0;
            sample.Statistic = Statistic.ConvertEnum<DataItemStatistic>();
            sample.Duration = Duration.HasValue ? Duration.Value : 0;
            return sample;
        }
    }
}