// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations
{
    /// <summary>
    /// An abstract XML Element. Replaced in the XML document by type(s) of Sample XML elements representing SAMPLE category data items defined for a Device in the Device Information Model.
    /// There can be multiple types of Sample XML Elements in a Samples container.
    /// </summary>
    public interface ISampleValueObservation : ISampleObservation
    {
        /// <summary>
        /// Used to describe a value (text or data) published as part of an Observation.
        /// </summary>
        string Result { get; }
    }
}