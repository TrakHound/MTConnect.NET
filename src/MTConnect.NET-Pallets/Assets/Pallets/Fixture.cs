// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Pallets
{
    public class Fixture
    {   
        [XmlAttribute("id")]
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [XmlAttribute("group")]
        [JsonPropertyName("group")]
        public string Group { get; set; }
    }
}