// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Collections.Generic;
using System.Xml.Serialization;
using System.Text.Json.Serialization;

namespace MTConnect.Streams
{
    public class TimeSeries : Sample
    {
        /// <summary>
        /// The number of readings of the value of a data item provided in the data returned when the representation attribute for teh data item is TIME_SERIES.
        /// SampleCount is not provided for data items unless the representation attribute is TIME_SERIES and it MUST be specified when the attribute is TIME_SERIES.
        /// </summary>
        [XmlAttribute("sampleCount")]
        [JsonPropertyName("sampleCount")]
        public long SampleCount { get; set; }

        /// <summary>
        /// Time Series observation MUST report multiple values at fixed intervals in a single observation. 
        /// At minimum, one of DataItem or observation MUST specify the sampleRate in hertz (values/second); fractional rates are permitted.
        /// When the observation and the DataItem specify the sampleRate, the observation sampleRate supersedes the DataItem.
        /// </summary>
        public IEnumerable<double> Values
        {
            get
            {
                return GetValues(CDATA);
            }
        }


        public TimeSeries()
        {
            IsTimeSeries = true;
        }

        public TimeSeries(Sample sample)
        {
            if (sample != null)
            {
                DataItemId = sample.DataItemId;
                Timestamp = sample.Timestamp;
                Name = sample.Name;
                Sequence = sample.Sequence;
                Category = sample.Category;
                Type = sample.Type;
                SubType = sample.SubType;
                CompositionId = sample.CompositionId;
                CDATA = sample.CDATA;
                Entries = sample.Entries;
                ResetTriggered = sample.ResetTriggered;
                SampleRate = sample.SampleRate;
                Statistic = sample.Statistic;
                Duration = sample.Duration;
                IsTimeSeries = true;
            }
        }


        public static IEnumerable<double> GetValues(string cdata)
        {
            if (!string.IsNullOrEmpty(cdata))
            {
                var segments = cdata.Split(' ');
                if (!segments.IsNullOrEmpty())
                {
                    var values = new List<double>();

                    foreach (var segment in segments)
                    {
                        if (double.TryParse(segment, out var value))
                        {
                            values.Add(value);
                        }
                    }

                    return values;
                }
            }

            return null;
        }
    }
}
