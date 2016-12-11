// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.v13.MTConnectDevices
{
    [XmlRoot("MTConnectDevices", Namespace= "urn:mtconnect.org:MTConnectDevices:1.3")]
    public class Document
    {
        public Document() { }

        public Document(string xml)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(Document));
                using (var textReader = new StringReader(xml))
                using (var xmlReader = XmlReader.Create(textReader))
                {
                    var document = (Document)serializer.Deserialize(xmlReader);
                    if (document != null)
                    {
                        Header = document.Header;
                        Devices = document.Devices;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }      
        }


        [XmlElement("Header")]
        public Headers.MTConnectDevicesHeader Header { get; set; }

        [XmlArray("Devices")]
        public List<Device> Devices { get; set; }
    }
}
