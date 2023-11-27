// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Pallets
{
    public class PalletPosition
    {
        [XmlAttribute("id")]
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [XmlAttribute("name")]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [XmlElement("Fixture")]
        [JsonPropertyName("fixture")]
        public Fixture Fixture { get; set; }

        [XmlElement("Part")]
        [JsonPropertyName("part")]
        public Part Part { get; set; }
    }
}