// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.CuttingTools
{
    public class Location
    {
        [XmlText]
        [JsonPropertyName("value")]
        public int Value { get; set; }

        /// <summary>
        /// The type of location being identified. Current MUST be one of POT, STATION, or CRIB.
        /// </summary>
        [XmlAttribute("type")]
        [JsonPropertyName("type")]
        public LocationType Type { get; set; }

        /// <summary>
        /// The number of locations at higher index value from this location.
        /// </summary>
        [XmlAttribute("positiveOverlap")]
        [JsonPropertyName("positiveOverlap")]
        public int PositiveOverlap { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public bool PositiveOverlapSpecified => PositiveOverlap > 0;

        /// <summary>
        /// The number of location at lower index values from this location.
        /// </summary>
        [XmlAttribute("negativeOverlap")]
        [JsonPropertyName("negativeOverlap")]
        public int NegativeOverlap { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public bool NegativeOverlapSpecified => NegativeOverlap > 0;
    }
}
