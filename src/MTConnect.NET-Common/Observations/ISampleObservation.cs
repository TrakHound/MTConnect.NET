// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices;

namespace MTConnect.Observations
{
    /// <summary>
    /// An abstract XML Element. Replaced in the XML document by type(s) of Sample XML elements representing SAMPLE category data items defined for a Device in the Device Information Model.
    /// There can be multiple types of Sample XML Elements in a Samples container.
    /// </summary>
    public interface ISampleObservation : IObservation
    {
        /// <summary>
        /// The rate at which successive samples of the value of a DataItem are recorded.
        /// SampleRate is expressed in terms of samples per second.
        /// If the SampleRate is smaller than one, the number can be represented as a decimal type floating point number.
        /// For example, a rate of 1 per 10 seconds would be 0.1.
        /// The SampleRate attribute MUST be provided when the representation attribute for the data item is TIME_SERIES.
        /// For data items where the representation attribute for the data item IS NOT TIME_SERIES, it may be assumed that the SampleRate is constant and SampleRate does not need to be reported in the MTConnectStreams document.
        /// </summary>
        double SampleRate { get; }

        /// <summary>
        /// For those DataItem elements that report data that may be periodically reset to an initial value, 
        /// resetTriggered identifies when a reported value has been reset and what has caused that reset to occur.
        /// </summary>
        ResetTriggered ResetTriggered { get; }

        /// <summary>
        /// The type of statistical calculation specified in the Device Information Model that this Sample element represents.
        /// </summary>
        DataItemStatistic Statistic { get; }

        /// <summary>
        /// The time-period over which the data was collected.
        /// </summary>
        double Duration { get; }
    }
}
