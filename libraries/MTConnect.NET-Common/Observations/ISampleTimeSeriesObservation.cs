// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace MTConnect.Observations
{
    /// <summary>
    /// An abstract XML Element. Replaced in the XML document by type(s) of Sample XML elements representing SAMPLE category data items defined for a Device in the Device Information Model.
    /// There can be multiple types of Sample XML Elements in a Samples container.
    /// </summary>
    public interface ISampleTimeSeriesObservation : ISampleObservation
    {
        /// <summary>
        /// The number of readings of the value of a data item provided in the data returned when the representation attribute for teh data item is TIME_SERIES.
        /// SampleCount is not provided for data items unless the representation attribute is TIME_SERIES and it MUST be specified when the attribute is TIME_SERIES.
        /// </summary>
        int SampleCount { get; }

        /// <summary>
        /// Time Series observation MUST report multiple values at fixed intervals in a single observation. 
        /// At minimum, one of DataItem or observation MUST specify the sampleRate in hertz (values/second); fractional rates are permitted.
        /// When the observation and the DataItem specify the sampleRate, the observation sampleRate supersedes the DataItem.
        /// </summary>
        IEnumerable<double> Samples { get; }
    }
}