// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Observations
{
    /// <summary>
    /// An XML element which provides the information and data reported from a piece of equipment for those DataItem elements defined with a category attribute of SAMPLE in the MTConnectDevices document.
    /// </summary>
    public class SampleTimeSeriesObservation : SampleObservation, ISampleTimeSeriesObservation
    {
        /// <summary>
        /// The number of readings of the value of a data item provided in the data returned when the representation attribute for teh data item is TIME_SERIES.
        /// SampleCount is not provided for data items unless the representation attribute is TIME_SERIES and it MUST be specified when the attribute is TIME_SERIES.
        /// </summary>
        [XmlAttribute("sampleCount")]
        [JsonPropertyName("sampleCount")]
        public long SampleCount => Samples.Count();

        internal bool SampleCountOutput => SampleCount > 0;

        /// <summary>
        /// Time Series observation MUST report multiple values at fixed intervals in a single observation. 
        /// At minimum, one of DataItem or observation MUST specify the sampleRate in hertz (values/second); fractional rates are permitted.
        /// When the observation and the DataItem specify the sampleRate, the observation sampleRate supersedes the DataItem.
        /// </summary>
        [XmlIgnore]
        [JsonPropertyName("samples")]
        public IEnumerable<double> Samples => TimeSeriesObservation.GetSamples(Values);

        internal bool SamplesOutput => false;
    }
}
