// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.Observations;
using System.Collections.Generic;

namespace MTConnect.Streams
{
    /// <summary>
    /// Organizes the data associated with each Component entity defined for a Device in the associated MTConnectDevices Response Document.
    /// </summary>
    public interface IComponentStream
    {
        /// <summary>
        /// Component identifies the Structural Element associated with the ComponentStream element.
        /// </summary>
        IComponent Component { get; }

        /// <summary>
        /// The type of the Component that the ComponentStream is associated with
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
        /// Uuid of the Component Associated with the ComponentStream
        /// </summary>
        string Uuid { get; }

        /// <summary>
        /// Gets All Observations (Samples, Events, & Conditions)
        /// </summary>
        IEnumerable<IObservation> Observations { get; }

        /// <summary>
        /// Gets only the Observations associated with DataItems with a Category of SAMPLE
        /// </summary>
        IEnumerable<ISampleObservation> Samples { get; }

        /// <summary>
        /// Gets only the Observations associated with DataItems with a Category of SAMPLE and a Representation of VALUE
        /// </summary>
        IEnumerable<ISampleValueObservation> SampleValues { get; }

        /// <summary>
        /// Gets only the Observations associated with DataItems with a Category of SAMPLE and a Representation of TIME_SERIES
        /// </summary>
        IEnumerable<ISampleTimeSeriesObservation> SampleTimeSeries { get; }

        /// <summary>
        /// Gets only the Observations associated with DataItems with a Category of SAMPLE and a Representation of DATA_SET
        /// </summary>
        IEnumerable<ISampleDataSetObservation> SampleDataSets { get; }

        /// <summary>
        /// Gets only the Observations associated with DataItems with a Category of SAMPLE and a Representation of TABLE
        /// </summary>
        IEnumerable<ISampleTableObservation> SampleTables { get; }

        /// <summary>
        /// Gets only the Observations associated with DataItems with a Category of EVENT
        /// </summary>
        IEnumerable<IEventObservation> Events { get; }

        /// <summary>
        /// Gets only the Observations associated with DataItems with a Category of EVENT and a Representation of VALUE
        /// </summary>
        IEnumerable<IEventValueObservation> EventValues { get; }

        /// <summary>
        /// Gets only the Observations associated with DataItems with a Category of EVENT and a Representation of DATA_SET
        /// </summary>
        IEnumerable<IEventDataSetObservation> EventDataSets { get; }

        /// <summary>
        /// Gets only the Observations associated with DataItems with a Category of EVENT and a Representation of TABLE
        /// </summary>
        IEnumerable<IEventTableObservation> EventTables { get; }

        /// <summary>
        /// Condition organizes the Data Entities returned in the MTConnectStreams XML document for those DataItem elements defined with a category attribute of CONDITION in the MTConnectDevices document.
        /// </summary>
        IEnumerable<IConditionObservation> Conditions { get; }
    }  
}