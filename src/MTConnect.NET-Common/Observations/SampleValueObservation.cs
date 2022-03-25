// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Observations
{
    /// <summary>
    /// An XML element which provides the information and data reported from a piece of equipment for those DataItem elements defined with a category attribute of SAMPLE in the MTConnectDevices document.
    /// </summary>
    public class SampleValueObservation : SampleObservation, ISampleValueObservation
    {
        /// <summary>
        /// Used to describe a value (text or data) published as part of an XML element.
        /// </summary>
        [XmlText]
        [JsonPropertyName("cdata")]
        public string CDATA
        {
            get => GetValue(ValueKeys.CDATA);
            set => AddValue(new ObservationValue(ValueKeys.CDATA, value));
        }

        internal bool CDATAOutput => false;
    }
}
