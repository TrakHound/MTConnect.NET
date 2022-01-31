// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;

namespace MTConnect.Observations
{
    /// <summary>
    /// An Information Model that describes Streaming Data reported by a piece of equipment
    /// where multiple values are reported at fixed intervals in a single observation
    /// </summary>
    public interface ITimeSeriesObservation
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
        /// The frequency at which the values were observed at
        /// </summary>
        double SampleRate { get; set; }

        /// <summary>
        /// The values that were reported during the Observation
        /// </summary>
        IEnumerable<double> Samples { get; set; }

        /// <summary>
        /// The timestamp (UnixTime in Milliseconds) that the observation was recorded at
        /// </summary>
        long Timestamp { get; set; }
    }
}
