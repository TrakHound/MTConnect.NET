// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Files
{
    /// <summary>
    /// A remark or interpretation for human interpretation associated with a File or FileArchetype.
    /// </summary>
    public class FileComment
    {
        /// <summary>
        /// The time the comment was made.
        /// </summary>
        [XmlAttribute("timestamp")]
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [XmlText]
        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}