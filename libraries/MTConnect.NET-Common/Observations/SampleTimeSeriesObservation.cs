// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Observations
{
    /// <summary>
    /// An XML element which provides the information and data reported from a piece of equipment for those DataItem elements defined with a category attribute of SAMPLE in the MTConnectDevices document.
    /// </summary>
    public class SampleTimeSeriesObservation : SampleObservation, ISampleTimeSeriesObservation
    {
        private IEnumerable<double> _entries;


        /// <summary>
        /// The number of readings of the value of a data item provided in the data returned when the representation attribute for teh data item is TIME_SERIES.
        /// SampleCount is not provided for data items unless the representation attribute is TIME_SERIES and it MUST be specified when the attribute is TIME_SERIES.
        /// </summary>
        public int SampleCount => Samples.Count();

        /// <summary>
        /// Time Series observation MUST report multiple values at fixed intervals in a single observation. 
        /// At minimum, one of DataItem or observation MUST specify the sampleRate in hertz (values/second); fractional rates are permitted.
        /// When the observation and the DataItem specify the sampleRate, the observation sampleRate supersedes the DataItem.
        /// </summary>
        public IEnumerable<double> Samples
        {
            get
            {
                if (_entries == null) _entries = TimeSeriesObservation.GetSamples(Values);
                return _entries;
            }
        }


        public SampleTimeSeriesObservation() : base()
        {
            _representation = Devices.DataItemRepresentation.TIME_SERIES;
        }


        protected override void OnValueAdded(ObservationValue observationValue)
        {
            _entries = null;
        }
    }
}