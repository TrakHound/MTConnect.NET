// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

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
