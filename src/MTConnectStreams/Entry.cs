// Copyright (c) 2020 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.MTConnectStreams
{
    public class Entry
    {
        [XmlAttribute("key")]
        public string Key { get; set; }

        [XmlText]
        public string CDATA { get; set; }
    }
}
