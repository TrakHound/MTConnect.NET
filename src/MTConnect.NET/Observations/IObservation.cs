// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;

namespace MTConnect.Observations
{
    /// <summary>
    /// An Information Model that describes Streaming Data reported by a piece of equipment.
    /// </summary>
    public interface IObservation
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
        /// The Values recorded during the Observation
        /// </summary>
        IEnumerable<ObservationValue> Values { get; set; }

        ///// <summary>
        ///// The recorded value of the Observation
        ///// </summary>
        //object Value { get; set; }

        ///// <summary>
        ///// The ValueType of the recorded value of the Observation
        ///// </summary>
        //string ValueType { get; set; }

        /// <summary>
        /// The timestamp (UnixTime in Milliseconds) that the observation was recorded at
        /// </summary>
        long Timestamp { get; set; }


        string GetValue(string valueType);
    }
}
