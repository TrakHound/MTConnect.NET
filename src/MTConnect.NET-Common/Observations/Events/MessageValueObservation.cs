// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// Any text string of information to be transferred from a piece of equipment to a client software application.
    /// </summary>
    public class MessageValueObservation : EventValueObservation, IEventValueObservation
    {
        /// <summary>
        /// Used to describe a value (text or data) published as part of an XML element.
        /// </summary>
        public string NativeCode
        {
            get => GetValue(ValueKeys.NativeCode);
            set => AddValue(new ObservationValue(ValueKeys.NativeCode, value));
        }
    }
}
