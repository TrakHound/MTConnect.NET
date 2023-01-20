// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Headers;
using MTConnect.Observations.Output;
using System;
using System.Collections.Generic;

namespace MTConnect.Streams.Output
{
    /// <summary>
    /// The Streams Information Model provides a representation of the data reported by a piece of equipment used for a manufacturing process, or used for any other purpose.
    /// </summary>
    public interface IStreamsResponseOutputDocument
    {
        /// <summary>
        /// Contains the Header information in an MTConnect Streams XML document
        /// </summary>
        IMTConnectStreamsHeader Header { get; }

        /// <summary>
        /// Streams is a container type XML element used to group the data reported from one or more pieces of equipment into a single XML document.
        /// </summary>
        IDeviceStreamOutput[] Streams { get; }

        /// <summary>
        /// The Version of the MTConnect Response Document
        /// </summary>
        Version Version { get; }

        /// <summary>
        /// Gets All Observations (Samples, Events, & Conditions)
        /// </summary>
        IEnumerable<IObservationOutput> GetObservations();

        long FirstObservationSequence { get; }

        long LastObservationSequence { get; }

        int ObservationCount { get; }
    }
}
