// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace MTConnect.Observations
{
    /// <summary>
    /// An XML element which provides the information and data reported from a piece of equipment for those DataItem elements defined with a category attribute of EVENT in the MTConnectDevices document.
    /// </summary>
    public class EventTableObservation : EventObservation, IEventTableObservation
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


        public EventTableObservation() : base()
        {
            _representation = Devices.DataItemRepresentation.TABLE;
        }
    }
}