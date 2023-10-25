// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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