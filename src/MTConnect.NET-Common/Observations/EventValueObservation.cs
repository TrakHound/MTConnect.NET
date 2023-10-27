// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations
{
    /// <summary>
    /// An XML element which provides the information and data reported from a piece of equipment for those DataItem elements defined with a category attribute of EVENT in the MTConnectDevices document.
    /// </summary>
    public class EventValueObservation : EventObservation, IEventValueObservation
    {
        /// <summary>
        /// Used to describe a value (text or data) published as part of an Observation.
        /// </summary>
        public string Result
        {
            get => GetValue(ValueKeys.Result);
            set => AddValue(new ObservationValue(ValueKeys.Result, value));
        }


        public EventValueObservation() : base()
        {
            _representation = Devices.DataItemRepresentation.VALUE;
        }
    }
}