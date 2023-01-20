// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Collections.Generic;

namespace MTConnect.Observations
{
    /// <summary>
    /// An XML element which provides the information and data reported from a piece of equipment for those DataItem elements defined with a category attribute of SAMPLE in the MTConnectDevices document.
    /// </summary>
    public class SampleTableObservation : SampleObservation, ISampleTableObservation
    {
        /// <summary>
        /// The number of Entry elements for the observation.
        /// </summary>
        public long Count => GetValue(ValueKeys.Count).ToLong();

        /// <summary>
        /// The key-value pairs published as part of the Table observation.
        /// </summary>
        public IEnumerable<ITableEntry> Entries
        {
            get => TableObservation.GetEntries(Values);
            set => AddValues(TableObservation.SetEntries(value));
        }


        public SampleTableObservation() : base()
        {
            _representation = Devices.DataItems.DataItemRepresentation.TABLE;
        }
    }
}
