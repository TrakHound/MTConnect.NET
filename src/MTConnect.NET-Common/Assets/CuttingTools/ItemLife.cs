// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.CuttingTools
{
    public class ItemLife
    {
        /// <summary>
        /// The value of the ItemLife
        /// </summary>
        [XmlText]
        [JsonPropertyName("value")]
        public double CDATA { get; set; }

        /// <summary>
        /// The type of tool life being accumulated. MINUTES, PART_COUNT, or WEAR
        /// </summary>
        [XmlAttribute("type")]
        [JsonPropertyName("type")]
        public ToolLifeType Type { get; set; }

        /// <summary>
        /// Indicates if the tool life counts from zero to maximum or maximum to zero, The values MUST be one of UP or DOWN.
        /// </summary>
        [XmlAttribute("countDirection")]
        [JsonPropertyName("countDirection")]
        public ToolLifeCountDirection CountDirection { get; set; }

        /// <summary>
        /// The point at which a tool life warning will be raised.
        /// </summary>
        [XmlAttribute("warning")]
        [JsonPropertyName("warning")]
        public long Warning { get; set; }

        /// <summary>
        /// The end of life limit for this tool. If the countDirection is DOWN, the point at which this tool should be expired, usually zero. 
        /// If the countDirection is UP, this is the upper limit for which this tool should be expired.
        /// </summary>
        [XmlAttribute("limit")]
        [JsonPropertyName("limit")]
        public long Limit { get; set; }

        /// <summary>
        /// The initial life of the tool when it is new.
        /// </summary>
        [XmlAttribute("initial")]
        [JsonPropertyName("initial")]
        public long Initial { get; set; }
    }
}
