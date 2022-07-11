// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices.DataItems;
using MTConnect.Observations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Streams.Json
{
    /// <summary>
    /// An abstract XML Element. Replaced in the XML document by type(s) of Sample XML elements representing SAMPLE category data items defined for a Device in the Device Information Model.
    /// There can be multiple types of Sample XML Elements in a Samples container.
    /// </summary>
    public class JsonSample : JsonObservation
    {
        /// <summary>
        /// The rate at which successive samples of the value of a DataItem are recorded.
        /// SampleRate is expressed in terms of samples per second.
        /// If the SampleRate is smaller than one, the number can be represented as a decimal type floating point number.
        /// For example, a rate of 1 per 10 seconds would be 0.1.
        /// The SampleRate attribute MUST be provided when the representation attribute for the data item is TIME_SERIES.
        /// For data items where the representation attribute for the data item IS NOT TIME_SERIES, it may be assumed that the SampleRate is constant and SampleRate does not need to be reported in the MTConnectStreams document.
        /// </summary>
        [XmlAttribute("sampleRate")]
        [JsonPropertyName("sampleRate")]
        public double? SampleRate { get; set; }

        /// <summary>
        /// The type of statistical calculation specified in the Device Information Model that this Sample element represents.
        /// </summary>
        [XmlAttribute("statistic")]
        [JsonPropertyName("statistic")]
        public string Statistic { get; set; }

        /// <summary>
        /// The time-period over which the data was collected.
        /// </summary>
        [XmlAttribute("duration")]
        [JsonPropertyName("duration")]
        public double? Duration { get; set; }


        public JsonSample() { }

        public JsonSample(Observation observation)
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
                SampleRate = observation.GetValue(ValueKeys.SampleRate).ToDouble();
                Duration = observation.GetValue(ValueKeys.Duration).ToDouble();

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
