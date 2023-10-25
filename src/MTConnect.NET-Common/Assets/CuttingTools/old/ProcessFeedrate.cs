// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.CuttingTools
{
    public class ProcessFeedrate
    {
        /// <summary>
        /// The upper bound for the tool’s process target feed rate
        /// </summary>
        [XmlAttribute("maximum")]
        [JsonPropertyName("maximum")]
        public int Maximum { get; set; }

        /// <summary>
        /// The lower bound for the tools feed rate.
        /// </summary>
        [XmlAttribute("minimum")]
        [JsonPropertyName("minimum")]
        public int Minimum { get; set; }

        /// <summary>
        /// The nominal feed rate the tool is designed to operate at.
        /// </summary>
        [XmlText]
        [XmlAttribute("nominal")]
        [JsonPropertyName("nominal")]
        public int Nominal { get; set; }
    }
}