// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.Observations;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Streams
{
    /// <summary>
    /// Organizes the data associated with each Component entity defined for a Device in the associated MTConnectDevices Response Document.
    /// </summary>
    public class ComponentStream : IComponentStream
    {
        private IEnumerable<IObservation> _observations;


        /// <summary>
        /// Component identifies the Structural Element associated with the ComponentStream element.
        /// </summary>
        public IComponent Component { get; set; }

        /// <summary>
        /// The type of the Component that the ComponentStream is associated with
        /// </summary>
        public string ComponentType { get; set; }

        /// <summary>
        /// The identifier of the Structural Element as defined by the id attribute of the corresponding Structural Element in the MTConnectDevices XML document.
        /// </summary>
        public string ComponentId { get; set; }

        /// <summary>
        /// The name of the ComponentStream element.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// NativeName identifies the common name normally associated with the ComponentStream element.
        /// </summary>
        public string NativeName { get; set; }

        /// <summary>
        /// Uuid of the ComponentStream element.
        /// </summary>
        public string Uuid { get; set; }

        /// <summary>
        /// Returns All Observations for the ComponentStream
        /// </summary>
        public IEnumerable<IObservation> Observations
        {
            get => _observations;
            set => _observations = value;
        }

        /// <summary>
        /// Returns only the Observations associated with DataItems with a Category of SAMPLE
        /// </summary>
        public IEnumerable<ISampleObservation> Samples => GetObservations<ISampleObservation>(_observations);

        /// <summary>
        /// Returns only the Observations associated with DataItems with a Category of SAMPLE and a Representation of VALUE
        /// </summary>
        public IEnumerable<ISampleValueObservation> SampleValues => GetObservations<ISampleValueObservation>(_observations);

        /// <summary>
        /// Returns only the Observations associated with DataItems with a Category of SAMPLE and a Representation of TIME_SERIES
        /// </summary>
        public IEnumerable<ISampleTimeSeriesObservation> SampleTimeSeries => GetObservations<ISampleTimeSeriesObservation>(_observations);

        /// <summary>
        /// Returns only the Observations associated with DataItems with a Category of SAMPLE and a Representation of DATA_SET
        /// </summary>
        public IEnumerable<ISampleDataSetObservation> SampleDataSets => GetObservations<ISampleDataSetObservation>(_observations);

        /// <summary>
        /// Returns only the Observations associated with DataItems with a Category of SAMPLE and a Representation of TABLE
        /// </summary>
        public IEnumerable<ISampleTableObservation> SampleTables => GetObservations<ISampleTableObservation>(_observations);

        /// <summary>
        /// Returns only the Observations associated with DataItems with a Category of EVENT
        /// </summary>
        public IEnumerable<IEventObservation> Events => GetObservations<IEventObservation>(_observations);

        /// <summary>
        /// Returns only the Observations associated with DataItems with a Category of EVENT and a Representation of VALUE
        /// </summary>
        public IEnumerable<IEventValueObservation> EventValues => GetObservations<IEventValueObservation>(_observations);

        /// <summary>
        /// Returns only the Observations associated with DataItems with a Category of EVENT and a Representation of DATA_SET
        /// </summary>
        public IEnumerable<IEventDataSetObservation> EventDataSets => GetObservations<IEventDataSetObservation>(_observations);

        /// <summary>
        /// Returns only the Observations associated with DataItems with a Category of EVENT and a Representation of TABLE
        /// </summary>
        public IEnumerable<IEventTableObservation> EventTables => GetObservations<IEventTableObservation>(_observations);

        /// <summary>
        /// Returns only the Observations associated with DataItems with a Category of CONDITION
        /// </summary>
        public IEnumerable<IConditionObservation> Conditions => GetObservations<IConditionObservation>(_observations);


        private static IEnumerable<T> GetObservations<T>(IEnumerable<IObservation> observations) where T : IObservation
        {
            var l = new List<T>();
            if (!observations.IsNullOrEmpty())
            {
                var x = observations.Where(o => typeof(T).IsAssignableFrom(o.GetType()));
                if (!x.IsNullOrEmpty())
                {
                    foreach (var y in x) l.Add((T)y);
                }
            }
            return l;
        }
    }  
}