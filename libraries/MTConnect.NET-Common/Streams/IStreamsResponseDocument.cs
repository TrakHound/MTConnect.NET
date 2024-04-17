// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Headers;
using MTConnect.Observations;
using System;
using System.Collections.Generic;

namespace MTConnect.Streams
{
    /// <summary>
    /// Root entity of an MTConnectStreams Response Document that contains the Observation Information Model of one or more Device entities.
    /// </summary>
    public interface IStreamsResponseDocument
    {
        /// <summary>
        /// Provides information from an agent defining version information, storage capacity, and parameters associated with the data management within the agent.
        /// </summary>
        IMTConnectStreamsHeader Header { get; }

        /// <summary>
        /// Streams groups one or more DeviceStream entities.
        /// </summary>
        IEnumerable<IDeviceStream> Streams { get; }

        /// <summary>
        /// The Version of the MTConnect Response Document
        /// </summary>
        Version Version { get; }

        /// <summary>
        /// Gets All Observations (Samples, Events, & Conditions)
        /// </summary>
        IEnumerable<IObservation> GetObservations();
    }
}