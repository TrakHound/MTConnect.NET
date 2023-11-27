// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations.Output;
using System.Collections.Generic;

namespace MTConnect.Streams.Output
{
    /// <summary>
    /// DeviceStream is a XML container that organizes data reported from a single piece of equipment.A DeviceStream element MUST be provided for each piece of equipment reporting data in an MTConnectStreams document.
    /// </summary>
    public interface IDeviceStreamOutput
    {
        /// <summary>
        /// The name of an element or a piece of equipment. The name associated with the piece of equipment reporting the data contained in this DeviceStream container.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The uuid associated with the piece of equipment reporting the data contained in this DeviceStream container.
        /// </summary>
        string Uuid { get; }

        /// <summary>
        /// An XML container type element that organizes data returned from an Agent in response to a current or sample HTTP request.
        /// </summary>
        IComponentStreamOutput[] ComponentStreams { get; }

        /// <summary>
        /// Gets All Observations (Samples, Events, & Conditions)
        /// </summary>
        IEnumerable<IObservationOutput> Observations { get; }
    }
}