// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Pallets
{
    public class Program
    {
        [XmlElement("Location")]
        [JsonPropertyName("location")]
        public string Location { get; set; }

        [XmlElement("Comment")]
        [JsonPropertyName("comment")]
        public string Comment { get; set; }
    }
}