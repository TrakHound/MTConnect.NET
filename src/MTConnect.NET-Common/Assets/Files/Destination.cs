// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Files
{
    /// <summary>
    /// The Destination is a reference to the target Device for this File.
    /// </summary>
    public class Destination
    {
        /// <summary>
        /// uuid of the target device or application.
        /// </summary>
        [XmlAttribute("deviceUuid")]
        [JsonPropertyName("deviceUuid")]
        public string DeviceUuid { get; set; }
    }
}
