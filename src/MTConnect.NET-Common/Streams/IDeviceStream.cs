// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Observations;
using System.Collections.Generic;

namespace MTConnect.Streams
{
    /// <summary>
    /// DeviceStream is a XML container that organizes data reported from a single piece of equipment.A DeviceStream element MUST be provided for each piece of equipment reporting data in an MTConnectStreams document.
    /// </summary>
    public interface IDeviceStream
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
        IEnumerable<IComponentStream> ComponentStreams { get; }

        /// <summary>
        /// Gets All Observations (Samples, Events, & Conditions)
        /// </summary>
        IEnumerable<IObservation> Observations { get; }

        /// <summary>
        /// Gets only the Observations associated with DataItems with a Category of SAMPLE
        /// </summary>
        IEnumerable<IObservation> Samples { get; }

        /// <summary>
        /// Gets only the Observations associated with DataItems with a Category of SAMPLE and a Representation of VALUE
        /// </summary>
        IEnumerable<SampleValueObservation> SampleValues { get; }

        /// <summary>
        /// Gets only the Observations associated with DataItems with a Category of SAMPLE and a Representation of TIME_SERIES
        /// </summary>
        IEnumerable<SampleTimeSeriesObservation> SampleTimeSeries { get; }

        /// <summary>
        /// Gets only the Observations associated with DataItems with a Category of SAMPLE and a Representation of DATA_SET
        /// </summary>
        IEnumerable<SampleDataSetObservation> SampleDataSets { get; }

        /// <summary>
        /// Gets only the Observations associated with DataItems with a Category of SAMPLE and a Representation of TABLE
        /// </summary>
        IEnumerable<SampleTableObservation> SampleTables { get; }

        /// <summary>
        /// Gets only the Observations associated with DataItems with a Category of EVENT
        /// </summary>
        IEnumerable<IObservation> Events { get; }

        /// <summary>
        /// Gets only the Observations associated with DataItems with a Category of EVENT and a Representation of VALUE
        /// </summary>
        IEnumerable<EventValueObservation> EventValues { get; }

        /// <summary>
        /// Gets only the Observations associated with DataItems with a Category of EVENT and a Representation of DATA_SET
        /// </summary>
        IEnumerable<EventDataSetObservation> EventDataSets { get; }

        /// <summary>
        /// Gets only the Observations associated with DataItems with a Category of EVENT and a Representation of TABLE
        /// </summary>
        IEnumerable<EventTableObservation> EventTables { get; }

        /// <summary>
        /// Condition organizes the Data Entities returned in the MTConnectStreams XML document for those DataItem elements defined with a category attribute of CONDITION in the MTConnectDevices document.
        /// </summary>
        IEnumerable<IObservation> Conditions { get; }
    }
}
