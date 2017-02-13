// Copyright (c) 2017 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.MTConnectAssets.CuttingTools
{
    public class Location
    {
        [XmlText]
        public int CDATA { get; set; }

        /// <summary>
        /// The type of location being identified. Current MUST be one of POT, STATION, or CRIB.
        /// </summary>
        [XmlAttribute("type")]
        public LocationType Type { get; set; }

        /// <summary>
        /// The number of locations at higher index value from this location.
        /// </summary>
        [XmlAttribute("positiveOverlap")]
        public int PositiveOverlap { get; set; }

        /// <summary>
        /// The number of location at lower index values from this location.
        /// </summary>
        [XmlAttribute("negativeOverlap")]
        public int NegativeOverlap { get; set; }
    }
}
