// Copyright (c) 2017 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Xml.Serialization;

namespace MTConnect.MTConnectDevices
{
    public class Filter
    {
        [XmlAttribute("type")]
        public string Type { get; set; }
    }
}
