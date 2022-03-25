// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices;
using System;
using System.Collections.Generic;

namespace MTConnect.Observations
{
    /// <summary>
    /// An Information Model Input that describes Streaming Data reported by a piece of equipment.
    /// </summary>
    public interface IObservation
    {
        /// <summary>
        /// The UUID of the Device that the Observation is associated with
        /// </summary>
        string DeviceUuid { get; }

        /// <summary>
        /// The DataItem that the Observation is associated with
        /// </summary>
        IDataItem DataItem { get; }

        /// <summary>
        /// Category of DataItem (Condition, Event, or Sample)
        /// </summary>
        DataItemCategory Category { get; }

        /// <summary>
        /// Type associated with the DataItem
        /// </summary>
        string Type { get; }

        /// <summary>
        /// The subtype of the DataItem defined in the Device Information Model that this DataItem element represents
        /// </summary>
        string SubType { get; }

        /// <summary>
        /// The unique identifier for the DataItem. 
        /// The DataItemID MUST match the id attribute of the data item defined in the Device Information Model that this DataItem element represents.
        /// </summary>
        string DataItemId { get; }

        /// <summary>
        /// THe name of the DataItem.
        /// The name MUST match the name of the data item defined in the Device Information Model that this DataItem represents.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// A number representing the sequential position of an occurence of the DataItem in the data buffer of the Agent.
        /// The value MUST be represented as an unsigned 64 bit with valid values from 1 to 2^64-1.
        /// </summary>
        long Sequence { get; }

        /// <summary>
        /// The time the data for the DataItem was reported or the statistics for the DataItem was computed.
        /// The timestamp MUST always represent the end of the collection interval when a duration or a TIME_SERIES is provided.
        /// The most accurate time available to the device MUST be used for the timestamp.
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// The identifier of the Composition element defined in the MTConnectDevices document associated with the data reported for the DataItem.
        /// </summary>
        string CompositionId { get; }

        /// <summary>
        /// Data consisting of multiple data points or samples or a file presented as a single DataItem.
        /// Each representation will have a unique format defined for each representation. 
        /// Examples or representation are VALUE, TIME_SERIES, DISCRETE, MP3, WAV, etc.
        /// Initially, the represenation for TIME_SERIES, DISCRETE, and VALUE are defined.
        /// If a representation is not specified, it MUST be determined to be a VALUE.
        /// </summary>
        DataItemRepresentation Representation { get; }

        /// <summary>
        /// Gets the Values associated with this Observation. These values represent data recorded during an Observation.
        /// </summary>
        IEnumerable<ObservationValue> Values { get; }

        /// <summary>
        /// Returns whether the Observation is Unavailable meaning a valid value cannot be determined
        /// </summary>
        bool IsUnavailable { get; }

        /// <summary>
        /// Gets the Value with the specified ValueTy
        /// </summary>
        string GetValue(string valueKey);
    }
}
