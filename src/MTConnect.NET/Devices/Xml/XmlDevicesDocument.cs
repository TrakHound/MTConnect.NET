// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Collections.Generic;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    [XmlRoot("MTConnectDevices")]
    public class XmlDevicesDocument
    {
        [XmlElement("Header")]
        public Headers.MTConnectDevicesHeader Header { get; set; }

        [XmlArray("Devices")]
        [XmlArrayItem("Device", typeof(XmlDevice))]
        [XmlArrayItem("Agent", typeof(XmlAgent))]
        public List<XmlDevice> Devices { get; set; }

        [XmlArray("Interfaces")]
        public List<Interfaces.Interface> Interfaces { get; set; }


        public XmlDevicesDocument() { }

        public XmlDevicesDocument(DevicesDocument document)
        {
            if (document != null)
            {
                Header = document.Header;

                if (!document.Devices.IsNullOrEmpty())
                {
                    var devices = new List<XmlDevice>();

                    foreach (var device in document.Devices)
                    {
                        if (device.Type == Device.TypeId) devices.Add(new XmlDevice(device));
                        else if (device.Type == Agent.TypeId) devices.Add(new XmlAgent(device));
                    }

                    Devices = devices;
                }

                Interfaces = document.Interfaces;
                //Version = document.Version;
                //Url = document.Url;
                //UserObject = document.UserObject;
            }
        }


        public DevicesDocument ToDocument()
        {
            var document = new DevicesDocument();

            document.Header = Header;

            if (!Devices.IsNullOrEmpty())
            {
                var devices = new List<Device>();

                foreach (var device in Devices) devices.Add(device.ToDevice());

                document.Devices = devices;
            }

            document.Interfaces = Interfaces;
            //document.Version = Version;
            //document.Url = Url;
            //document.UserObject = UserObject;

            return document;
        }
    }
}
