// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.CuttingTools
{
    public class ProcessSpindleSpeed
    {
        /// <summary>
        /// The upper bound for the tool’s target spindle speed
        /// </summary>
        [XmlAttribute("maximum")]
        [JsonPropertyName("maximum")]
        public int Maximum { get; set; }

        /// <summary>
        /// The lower bound for the tools spindle speed.
        /// </summary>
        [XmlAttribute("minimum")]
        [JsonPropertyName("minimum")]
        public int Minimum { get; set; }

        /// <summary>
        /// The nominal speed the tool is designed to operate at.
        /// </summary>
        [XmlText]
        [XmlAttribute("nominal")]
        [JsonPropertyName("nominal")]
        public int Nominal { get; set; }
    }
}
