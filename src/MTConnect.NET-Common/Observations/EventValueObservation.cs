// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;

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
        [XmlText]
        [JsonPropertyName("result")]
        public string Result
        {
            get => GetValue(ValueKeys.Result);
            set => AddValue(new ObservationValue(ValueKeys.Result, value));
        }

        internal bool ResultOutput => false;
    }
}
