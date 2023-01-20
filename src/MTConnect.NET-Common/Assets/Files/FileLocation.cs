// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Files
{
    /// <summary>
    /// The URL reference to the file location.
    /// </summary>
    public class FileLocation
    {
        /// <summary>
        /// A URL reference to the file.
        /// </summary>
        [XmlAttribute("href")]
        [JsonPropertyName("href")]
        public string Href { get; set; }

        /// <summary>
        /// The type of href for the xlink href type. MUST be locator referring to a URL.     
        /// </summary>
        [XmlAttribute("xLinkType")]
        [JsonPropertyName("xLinkType")]
        public string xLinkType { get; set; }


        public FileLocation() { }

        public FileLocation(string href)
        {
            Href = href;
        }
    }
}
