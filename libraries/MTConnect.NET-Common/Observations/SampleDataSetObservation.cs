// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace MTConnect.Observations
{
    /// <summary>
    /// An XML element which provides the information and data reported from a piece of equipment for those DataItem elements defined with a category attribute of SAMPLE in the MTConnectDevices document.
    /// </summary>
    public class SampleDataSetObservation : SampleObservation, ISampleDataSetObservation
    {
        private IEnumerable<IDataSetEntry> _entries;


        /// <summary>
        /// The number of Entry elements for the observation.
        /// </summary>
        public long Count => GetValue(ValueKeys.Count).ToLong();

        /// <summary>
        /// The key-value pairs published as part of the Data Set observation.
        /// </summary>
        public IEnumerable<IDataSetEntry> Entries
        {
            get
            {
                if (_entries == null) _entries = DataSetObservation.GetEntries(Values);
                return _entries;
            }
            set => AddValues(DataSetObservation.SetEntries(value));
        }


        public SampleDataSetObservation() : base()
        {
            _representation = Devices.DataItemRepresentation.DATA_SET;
        }


        protected override void OnValueAdded(ObservationValue observationValue)
        {
            _entries = null;
        }
    }
}