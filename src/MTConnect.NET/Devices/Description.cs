// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Xml.Serialization;
using System.Text.Json.Serialization;

namespace MTConnect.Devices
{
    /// <summary>
    /// An element that can contain any description content.
    /// </summary>
    public class Description
    {
        /// <summary>
        /// The name of the manufacturer of the Component
        /// </summary>
        [XmlAttribute("manufacturer")]
        [JsonPropertyName("manufacturer")]
        public string Manufacturer { get; set; }

        /// <summary>
        /// The model description of the Component
        /// </summary>
        [XmlAttribute("model")]
        [JsonPropertyName("model")]
        public string Model { get; set; }

        /// <summary>
        /// The component's serial number
        /// </summary>
        [XmlAttribute("serialNumber")]
        [JsonPropertyName("serialNumber")]
        public string SerialNumber { get; set; }

        /// <summary>
        /// The station where the Component is located when a component is part of a manufacturing unit or cell with multiple stations that share the same physical controller.
        /// </summary>
        [XmlAttribute("station")]
        [JsonPropertyName("station")]
        public string Station { get; set; }

        /// <summary>
        /// Any additional descriptive information the implementer chooses to include regarding the Component.
        /// </summary>
        [XmlText]
        [JsonPropertyName("cdata")]
        public string CDATA { get; set; }
    }
}
