// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.Text.Json.Serialization;

namespace MTConnect.Streams
{
    /// <summary>
    /// A key-value pair published as part of a Data Set observation.
    /// </summary>
    public class Entry
    {
        /// <summary>
        /// A unique identifier for each key-value pair.
        /// </summary>
        [XmlAttribute("key")]
        [JsonPropertyName("key")]
        public string Key { get; set; }

        /// <summary>
        /// Boolean removal indicator of a key-value pair that MUST be true or false.
        /// </summary>
        [XmlAttribute("removed")]
        [JsonPropertyName("removed")]
        public bool Removed { get; set; }

        [XmlText]
        [JsonPropertyName("cdata")]
        public string CDATA { get; set; }

        [XmlElement("Cells")]
        [JsonPropertyName("cells")]
        public List<Cell> Cells { get; set; }


        public Entry() { }

        public Entry(string key, object value, bool removed = false)
        {
            Key = key;
            if (value != null) CDATA = value.ToString();
            //CDATA = value != null ? value.ToString() : string.Empty;
            Removed = removed;
        }
    }
}
