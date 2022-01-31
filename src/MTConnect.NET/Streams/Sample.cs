// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Streams
{
    /// <summary>
    /// An abstract XML Element. Replaced in the XML document by type(s) of Sample XML elements representing SAMPLE category data items defined for a Device in the Device Information Model.
    /// There can be multiple types of Sample XML Elements in a Samples container.
    /// </summary>
    public class Sample : DataItem
    {
        public Sample()
        {
            Category = Devices.DataItemCategory.SAMPLE;
        }

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
        public double SampleRate { get; set; }

        /// <summary>
        /// The type of statistical calculation specified in the Device Information Model that this Sample element represents.
        /// </summary>
        [XmlAttribute("statistic")]
        [JsonPropertyName("statistic")]
        public Devices.DataItemStatistic Statistic { get; set; }

        /// <summary>
        /// The time-period over which the data was collected.
        /// </summary>
        [XmlAttribute("duration")]
        [JsonPropertyName("duration")]
        public double Duration { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public bool IsTimeSeries { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public bool IsDataSet { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public bool IsTable { get; set; }
    }
}
