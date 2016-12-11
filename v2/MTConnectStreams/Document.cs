// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.MTConnectStreams
{
    [XmlRoot("MTConnectStreams", Namespace = NAMESPACE)]
    public class Document
    {
        [XmlIgnore]
        public const string NAMESPACE = "urn:mtconnect.org:MTConnectStreams:1.3";

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
                        DeviceStreams = document.DeviceStreams;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        [XmlElement("Header")]
        public Headers.MTConnectStreamsHeader Header { get; set; }

        [XmlArray("Streams")]
        [XmlArrayItem("DeviceStream")]
        public List<DeviceStream> DeviceStreams { get; set; }
    }
}
