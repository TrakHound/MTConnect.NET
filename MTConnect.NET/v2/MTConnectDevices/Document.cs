// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using System.Xml.Serialization;

namespace MTConnect.MTConnectDevices
{
    [XmlRoot("MTConnectDevices", Namespace= "urn:mtconnect.org:MTConnectDevices:1.3")]
    public class Document
    {
        [XmlElement("Header")]
        public Headers.MTConnectDevicesHeader Header { get; set; }

        [XmlArray("Devices")]
        public List<Device> Devices { get; set; }
    }
}
