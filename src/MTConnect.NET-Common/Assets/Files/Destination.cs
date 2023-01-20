// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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