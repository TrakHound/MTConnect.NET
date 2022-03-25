// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

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
        public int Maximum { get; set; }

        /// <summary>
        /// The lower bound for the tools feed rate.
        /// </summary>
        [XmlAttribute("minimum")]
        public int Minimum { get; set; }

        /// <summary>
        /// The nominal feed rate the tool is designed to operate at.
        /// </summary>
        [XmlText]
        [XmlAttribute("nominal")]
        public int Nominal { get; set; }
    }
}
