// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices.DataItems;
using MTConnect.Observations;
using MTConnect.Observations.Output;
using System.Linq;
using System.Text.Json.Serialization;

namespace MTConnect.Streams.Json
{
    public class JsonSample : JsonObservation
    {
        [JsonPropertyName("sampleRate")]
        public double? SampleRate { get; set; }

        [JsonPropertyName("statistic")]
        public string Statistic { get; set; }

        [JsonPropertyName("duration")]
        public double? Duration { get; set; }


        public JsonSample() { }

        public JsonSample(IObservation observation)
        {
            if (observation != null)
            {
                DataItemId = observation.DataItemId;
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
                if (statistic != DataItemStatistic.NONE.ToString()) Statistic = statistic;

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

        public JsonSample(IObservationOutput observation)
        {
            if (observation != null)
            {
                DataItemId = observation.DataItemId;
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
                if (statistic != DataItemStatistic.NONE.ToString()) Statistic = statistic;

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

        public SampleObservation ToSample()
        {
            var sample = new SampleObservation();
            //sample.DataItemId = DataItemId;
            //sample.Timestamp = Timestamp;
            //sample.Name = Name;
            //sample.Sequence = Sequence;
            //sample.Category = Devices.DataItemCategory.SAMPLE;
            //sample.Type = Type;
            //sample.SubType = SubType;
            //sample.CompositionId = CompositionId;
            //sample.ResetTriggered = ResetTriggered.ConvertEnum<ResetTriggered>();
            //sample.CDATA = CDATA;
            //sample.Entries = Entries;
            //sample.Count = Count.HasValue ? Count.Value : 0;
            //sample.SampleRate = SampleRate.HasValue ? SampleRate.Value : 0;
            //sample.Statistic = Statistic.ConvertEnum<Devices.DataItemStatistic>();
            //sample.Duration = Duration.HasValue ? Duration.Value : 0;
            return sample;
        }
    }
}
