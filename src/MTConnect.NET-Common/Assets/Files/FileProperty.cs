// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Files
{
    /// <summary>
    /// A key-value pair providing additional metadata about a File.
    /// </summary>
    public class FileProperty
    {
        /// <summary>
        /// The name of the FileProperty
        /// </summary>
        [XmlAttribute("name")]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [XmlText]
        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}
