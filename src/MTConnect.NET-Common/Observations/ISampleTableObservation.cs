// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace MTConnect.Observations
{
    /// <summary>
    /// An abstract XML Element. Replaced in the XML document by type(s) of Sample XML elements representing SAMPLE category data items defined for a Device in the Device Information Model.
    /// There can be multiple types of Sample XML Elements in a Samples container.
    /// </summary>
    public interface ISampleTableObservation : ISampleObservation
    {
        /// <summary>
        /// The number of Entry elements for the observation.
        /// </summary>
        long Count { get; }

        /// <summary>
        /// The key-value pairs published as part of the Table observation.
        /// </summary>
        IEnumerable<ITableEntry> Entries { get; }
    }
}