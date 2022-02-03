// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices;
using System.Collections.Generic;

namespace MTConnect.Observations
{
    /// <summary>
    /// An Information Model that describes Streaming Data reported by a piece of equipment
    /// where the reported value(s) are represented as a set of key-value pairs.
    /// </summary>
    public interface IDataSetObservation
    {
        /// <summary>
        /// The name of the Device that the Observation is associated with
        /// </summary>
        string DeviceName { get; set; }

        /// <summary>
        /// The (ID, Name, or Source) of the DataItem that the Observation is associated with
        /// </summary>
        string Key { get; set; }

        /// <summary>
        /// Key-value pairs published as part of a Data Set observation
        /// </summary>
        IEnumerable<DataSetEntry> Entries { get; set; }

        /// <summary>
        /// The timestamp (UnixTime in Milliseconds) that the observation was recorded at
        /// </summary>
        long Timestamp { get; set; }

        /// <summary>
        /// For those DataItem elements that report data that may be periodically reset to an initial value, 
        /// resetTriggered identifies when a reported value has been reset and what has caused that reset to occur.
        /// </summary>
        DataItemResetTrigger ResetTrigger { get; set; }
    }
}
