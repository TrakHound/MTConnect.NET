// Copyright (c) 2017 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.MTConnectAssets.CuttingTools
{
    public class ReconditionCount
    {
        /// <summary>
        /// The maximum number of times this tool may be reconditioned
        /// </summary>
        [XmlAttribute("maximumCount")]
        public int MaximumCount { get; set; }

        /// <summary>
        /// CDATA that represents the number of times the cutter has been reconditioned.
        /// </summary>
        [XmlText]
        public int CDATA { get; set; }
    }
}
