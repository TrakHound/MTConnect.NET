// Copyright (c) 2017 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;
using System.Xml.Serialization;

namespace MTConnect.MTConnectAssets
{
    public class Asset
    {
        [XmlAttribute("assetId")]
        public string AssetId { get; set; }

        /// <summary>
        /// The time this asset was last modified. 
        /// Always given in UTC. 
        /// The timestamp MUST be provided in UTC (Universal Time Coordinate, also known as GMT). 
        /// This is the time the asset data was last modified.
        /// </summary>
        [XmlAttribute("timestamp")]
        public DateTime Timestamp { get; set; }

        [XmlIgnore]
        public string Type { get; set; }

        [XmlIgnore]
        public string Xml { get; set; }
    }
}
