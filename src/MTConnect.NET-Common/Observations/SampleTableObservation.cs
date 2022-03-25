// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;

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
        [XmlAttribute("count")]
        [JsonPropertyName("count")]
        public long Count => GetValue(ValueKeys.Count).ToLong();

        internal bool CountOutput => Count > 0;

        /// <summary>
        /// The key-value pairs published as part of the Table observation.
        /// </summary>
        [XmlElement("Entry")]
        [JsonPropertyName("entries")]
        public IEnumerable<ITableEntry> Entries => TableObservation.GetEntries(Values);

        internal bool EntriesOutput => false;
    }
}
