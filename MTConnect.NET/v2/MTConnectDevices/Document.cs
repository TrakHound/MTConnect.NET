using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
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
