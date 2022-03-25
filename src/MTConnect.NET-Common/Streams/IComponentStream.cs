// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices;
using MTConnect.Observations;
using System.Collections.Generic;

namespace MTConnect.Streams
{
    /// <summary>
    /// ComponentStream is a XML container that organizes the data associated with each Structural Element defined for that piece of equipment in the associated MTConnectDevices XML document
    /// </summary>
    public interface IComponentStream
    {
        /// <summary>
        /// Component identifies the Structural Element associated with the ComponentStream element.
        /// </summary>
        IComponent Component { get; }

        /// <summary>
        /// Component identifies the Structural Element associated with the ComponentStream element.
        /// </summary>
        string ComponentType { get; }

        /// <summary>
        /// The identifier of the Structural Element as defined by the id attribute of the corresponding Structural Element in the MTConnectDevices XML document.
        /// </summary>
        string ComponentId { get; }

        /// <summary>
        /// The name of the ComponentStream element.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// NativeName identifies the common name normally associated with the ComponentStream element.
        /// </summary>
        string NativeName { get; }

        /// <summary>
        /// Uuid of the ComponentStream element.
        /// </summary>
        string Uuid { get; }

        /// <summary>
        /// Gets All Observations (Samples, Events, & Conditions)
        /// </summary>
        IEnumerable<Observation> Observations { get; }

        /// <summary>
        /// Gets only the Observations associated with DataItems with a Category of SAMPLE
        /// </summary>
        IEnumerable<SampleObservation> Samples { get; }

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
        IEnumerable<EventObservation> Events { get; }

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
        IEnumerable<ConditionObservation> Conditions { get; }
    }  
}
