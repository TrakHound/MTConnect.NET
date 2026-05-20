// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using System;

namespace MTConnect.Observations.Output
{
    /// <summary>
    /// A read-only projection of an Observation prepared for serialization into an MTConnectStreams response.
    /// </summary>
    public interface IObservationOutput
    {
        /// <summary>
        /// The UUID of the Device the Observation belongs to.
        /// </summary>
        string DeviceUuid { get; }

        /// <summary>
        /// The DataItem definition the Observation was produced for.
        /// </summary>
        IDataItem DataItem { get; }

        /// <summary>
        /// The Id of the DataItem the Observation was produced for.
        /// </summary>
        string DataItemId { get; }

        /// <summary>
        /// The category (SAMPLE, EVENT, or CONDITION) of the DataItem.
        /// </summary>
        DataItemCategory Category { get; }

        /// <summary>
        /// The type of the DataItem.
        /// </summary>
        string Type { get; }

        /// <summary>
        /// The subtype of the DataItem, when present.
        /// </summary>
        string SubType { get; }

        /// <summary>
        /// The name of the DataItem, when present.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The instance identifier of the Agent that produced the Observation.
        /// </summary>
        ulong InstanceId { get; }

        /// <summary>
        /// The sequence number assigned to the Observation in the Agent's buffer.
        /// </summary>
        ulong Sequence { get; }

        /// <summary>
        /// The UTC timestamp at which the Observation was recorded.
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// The Observation timestamp expressed with the configured output time-zone offset.
        /// </summary>
        DateTimeOffset TimeZoneTimestamp { get; }

        /// <summary>
        /// The Id of the Composition the Observation is associated with, when present.
        /// </summary>
        string CompositionId { get; }

        /// <summary>
        /// The representation (VALUE, DATA_SET, TABLE, or TIME_SERIES) of the DataItem.
        /// </summary>
        DataItemRepresentation Representation { get; }

        /// <summary>
        /// The quality of the reported data.
        /// </summary>
        Quality Quality { get; }

        /// <summary>
        /// Whether the DataItem is deprecated in the active MTConnect version.
        /// </summary>
        bool Deprecated { get; }

        /// <summary>
        /// Whether the Observation carries values beyond the standard MTConnect schema.
        /// </summary>
        bool Extended { get; }

        /// <summary>
        /// The ValueKey and value pairs that make up the Observation.
        /// </summary>
        ObservationValue[] Values { get; }


        /// <summary>
        /// Gets the value recorded for the specified ValueKey.
        /// </summary>
        /// <param name="valueKey">The ValueKey identifying the Representation component to retrieve.</param>
        /// <returns>The recorded value, or <c>null</c> when no matching value exists.</returns>
        string GetValue(string valueKey);
    }
}