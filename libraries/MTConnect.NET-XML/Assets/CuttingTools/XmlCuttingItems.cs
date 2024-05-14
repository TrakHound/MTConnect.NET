// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml.CuttingTools
{
    public class XmlCuttingItems
    {
        [XmlAttribute("count")]
        public int Count { get; set; }

        [XmlElement("CuttingItem")]
        public List<XmlCuttingItem> CuttingItems { get; set; }
    }
}