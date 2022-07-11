// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Xml.Serialization;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Filter provides a means to control when an Agent records updated information for a data item.
    /// </summary>
    public class Filter : IFilter
    {
        [XmlAttribute("type")]
        [JsonPropertyName("type")]
        public DataItemFilterType Type { get; set; }

        ///// <summary>
        ///// The value associated with each Filter
        ///// </summary>
        //[XmlText]
        //[JsonPropertyName("value")]
        //public string CDATA { get; set; }

        /// <summary>
        /// The value associated with each Filter
        /// </summary>
        [XmlText]
        [JsonPropertyName("value")]
        public double Value { get; set; }
    }
}
