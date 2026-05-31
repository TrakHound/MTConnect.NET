// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace MTConnect.Observations
{
    /// <summary>
    /// An XML element which provides the information and data reported from a piece of equipment for those DataItem elements defined with a category attribute of SAMPLE in the MTConnectDevices document.
    /// </summary>
    public class SampleTableObservation : SampleObservation, ISampleTableObservation
    {
        private IEnumerable<ITableEntry> _entries;


        /// <summary>
        /// The number of Entry elements for the observation.
        /// </summary>
        public long Count => GetValue(ValueKeys.Count).ToLong();

        /// <summary>
        /// The key-value pairs published as part of the Table observation.
        /// </summary>
        public IEnumerable<ITableEntry> Entries
        {
            get
            {
                if (_entries == null) _entries = TableObservation.GetEntries(Values);
                return _entries;
            }
            set => AddValues(TableObservation.SetEntries(value));
        }


        /// <summary>
        /// Initializes a new Sample Observation that reports a TABLE representation.
        /// </summary>
        public SampleTableObservation() : base()
        {
            _representation = Devices.DataItemRepresentation.TABLE;
        }


        /// <summary>
        /// Invalidates the cached Table entries so they are rebuilt the next time they are read.
        /// </summary>
        /// <param name="observationValue">The value that was added.</param>
        protected override void OnValueAdded(ObservationValue observationValue)
        {
            _entries = null;
        }
    }
}