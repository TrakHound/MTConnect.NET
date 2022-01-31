// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Streams
{
    public class Cell
    {
        /// <summary>
        /// A unique identifier for each key-value pair.
        /// </summary>
        [XmlAttribute("key")]
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [XmlText]
        [JsonPropertyName("cdata")]
        public string CDATA { get; set; }


        public Cell() { }

        public Cell(string key, object value)
        {
            Key = key;
            CDATA = value != null ? value.ToString() : string.Empty;
        }
    }
}
