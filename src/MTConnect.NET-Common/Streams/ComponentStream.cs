// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices;
using MTConnect.Observations;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Streams
{
    /// <summary>
    /// ComponentStream is a XML container that organizes the data associated with each Structural Element defined for that piece of equipment in the associated MTConnectDevices XML document
    /// </summary>
    public class ComponentStream : IComponentStream
    {
        private IEnumerable<IObservation> _observations;


        /// <summary>
        /// Component identifies the Structural Element associated with the ComponentStream element.
        /// </summary>
        public IComponent Component { get; set; }

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
        public IEnumerable<IObservation> Samples => _observations != null ? _observations.Where(o => o.Category == Devices.DataItems.DataItemCategory.SAMPLE) : null;

        /// <summary>
        /// Returns only the Observations associated with DataItems with a Category of SAMPLE and a Representation of VALUE
        /// </summary>
        public IEnumerable<SampleValueObservation> SampleValues => GetObservations<SampleValueObservation>(Samples);

        /// <summary>
        /// Returns only the Observations associated with DataItems with a Category of SAMPLE and a Representation of TIME_SERIES
        /// </summary>
        public IEnumerable<SampleTimeSeriesObservation> SampleTimeSeries => GetObservations<SampleTimeSeriesObservation>(Samples);

        /// <summary>
        /// Returns only the Observations associated with DataItems with a Category of SAMPLE and a Representation of DATA_SET
        /// </summary>
        public IEnumerable<SampleDataSetObservation> SampleDataSets => GetObservations<SampleDataSetObservation>(Samples);

        /// <summary>
        /// Returns only the Observations associated with DataItems with a Category of SAMPLE and a Representation of TABLE
        /// </summary>
        public IEnumerable<SampleTableObservation> SampleTables => GetObservations<SampleTableObservation>(Samples);

        /// <summary>
        /// Returns only the Observations associated with DataItems with a Category of EVENT
        /// </summary>
        public IEnumerable<IObservation> Events => _observations != null ? _observations.Where(o => o.Category == Devices.DataItems.DataItemCategory.EVENT) : null;

        /// <summary>
        /// Returns only the Observations associated with DataItems with a Category of EVENT and a Representation of VALUE
        /// </summary>
        public IEnumerable<EventValueObservation> EventValues => GetObservations<EventValueObservation>(Events);

        /// <summary>
        /// Returns only the Observations associated with DataItems with a Category of EVENT and a Representation of DATA_SET
        /// </summary>
        public IEnumerable<EventDataSetObservation> EventDataSets => GetObservations<EventDataSetObservation>(Events);

        /// <summary>
        /// Returns only the Observations associated with DataItems with a Category of EVENT and a Representation of TABLE
        /// </summary>
        public IEnumerable<EventTableObservation> EventTables => GetObservations<EventTableObservation>(Events);

        /// <summary>
        /// Returns only the Observations associated with DataItems with a Category of CONDITION
        /// </summary>
        public IEnumerable<IObservation> Conditions => _observations != null ? _observations.Where(o => o.Category == Devices.DataItems.DataItemCategory.CONDITION) : null;


        private IEnumerable<T> GetObservations<T>(IEnumerable<IObservation> observations) where T : Observation
        {
            var l = new List<T>();
            if (!observations.IsNullOrEmpty())
            {
                var x = observations.Where(o => o.GetType().IsAssignableFrom(typeof(T)));
                if (!x.IsNullOrEmpty())
                {
                    foreach (var y in x) l.Add((T)y);
                }
            }
            return l;
        }
    }  
}
