// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Collections.Generic;

namespace MTConnect.Observations
{
    /// <summary>
    /// An XML element which provides the information and data reported from a piece of equipment for those DataItem elements defined with a category attribute of SAMPLE in the MTConnectDevices document.
    /// </summary>
    public class SampleDataSetObservation : SampleObservation, ISampleDataSetObservation
    {
        /// <summary>
        /// The number of Entry elements for the observation.
        /// </summary>
        public long Count => GetValue(ValueKeys.Count).ToLong();

        /// <summary>
        /// The key-value pairs published as part of the Data Set observation.
        /// </summary>
        public IEnumerable<IDataSetEntry> Entries
        {
            get => DataSetObservation.GetEntries(Values);
            set => AddValues(DataSetObservation.SetEntries(value));
        }


        public SampleDataSetObservation() : base()
        {
            _representation = Devices.DataItems.DataItemRepresentation.DATA_SET;
        }
    }
}
