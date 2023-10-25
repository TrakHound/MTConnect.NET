// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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
        public double Maximum { get; set; }

        /// <summary>
        /// The lower bound for the tools spindle speed.
        /// </summary>
        [XmlAttribute("minimum")]
        [JsonPropertyName("minimum")]
        public double Minimum { get; set; }

        /// <summary>
        /// The nominal speed the tool is designed to operate at.
        /// </summary>
        [XmlAttribute("nominal")]
        [JsonPropertyName("nominal")]
        public double Nominal { get; set; }

        [XmlText]
        [JsonPropertyName("value")]
        public double Value { get; set; }
    }
}