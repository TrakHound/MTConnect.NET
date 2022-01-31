// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Pallets
{
    public class Part
    {
        [XmlAttribute("id")]
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [XmlAttribute("serialNumber")]
        [JsonPropertyName("serialNumber")]
        public string SerialNumber { get; set; }

        [XmlElement("Status")]
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [XmlElement("Program")]
        [JsonPropertyName("program")]
        public Program Program { get; set; }
    }
}
