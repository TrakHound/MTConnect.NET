// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Streams.Xml
{
    /// <summary>
    /// The Streams Information Model provides a representation of the data reported by a piece of equipment used for a manufacturing process, or used for any other purpose.
    /// </summary>
    [XmlRoot("MTConnectStreams")]
    public class XmlStreamsDocument
    {
        /// <summary>
        /// Contains the Header information in an MTConnect Streams XML document
        /// </summary>
        [XmlElement("Header")]
        public Headers.MTConnectStreamsHeader Header { get; set; }

        /// <summary>
        /// Streams is a container type XML element used to group the data reported from one or more pieces of equipment into a single XML document.
        /// </summary>
        [XmlArray("Streams")]
        [XmlArrayItem("DeviceStream")]
        public List<XmlDeviceStream> Streams { get; set; }

        [XmlIgnore]
        public Version Version { get; set; }


        public XmlStreamsDocument() { }

        public XmlStreamsDocument(StreamsDocument streamsDocument)
        {
            if (streamsDocument != null)
            {
                Header = streamsDocument.Header;
                Version = streamsDocument.Version;

                var xmlStreams = new List<XmlDeviceStream>();
                if (!streamsDocument.Streams.IsNullOrEmpty())
                {
                    foreach (var stream in streamsDocument.Streams)
                    {
                        var xmlStream = new XmlDeviceStream(stream);
                        if (xmlStream != null) xmlStreams.Add(xmlStream);
                    }
                }

                Streams = xmlStreams;
            }
        }


        public StreamsDocument ToStreamsDocument()
        {
            var streamsDocument = new StreamsDocument();

            streamsDocument.Header = Header;
            streamsDocument.Version = Version;
            
            if (!Streams.IsNullOrEmpty())
            {
                var deviceStreams = new List<DeviceStream>();

                foreach (var xmlStream in Streams)
                {
                    deviceStreams.Add(xmlStream.ToDeviceStream());
                }

                streamsDocument.Streams = deviceStreams;
            }

            return streamsDocument;
        }
    }
}
