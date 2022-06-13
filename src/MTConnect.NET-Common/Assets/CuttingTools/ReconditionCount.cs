// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.CuttingTools
{
    public class ReconditionCount
    {
        /// <summary>
        /// The maximum number of times this tool may be reconditioned
        /// </summary>
        [XmlAttribute("maximumCount")]
        [JsonPropertyName("maximumCount")]
        public int MaximumCount { get; set; }

        /// <summary>
        /// Value that represents the number of times the cutter has been reconditioned.
        /// </summary>
        [XmlText]
        [JsonPropertyName("value")]
        public int Value { get; set; }
    }
}
