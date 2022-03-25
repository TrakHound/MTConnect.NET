// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MTConnect.Streams.Xml
{
    /// <summary>
    /// DeviceStream is a XML container that organizes data reported from a single piece of equipment.A DeviceStream element MUST be provided for each piece of equipment reporting data in an MTConnectStreams document.
    /// </summary>
    [XmlRoot("DeviceStream")]
    public class XmlDeviceStream : IXmlSerializable
    {
        /// <summary>
        /// The name of an element or a piece of equipment. The name associated with the piece of equipment reporting the data contained in this DeviceStream container.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The uuid associated with the piece of equipment reporting the data contained in this DeviceStream container.
        /// </summary>
        public string Uuid { get; set; }

        /// <summary>
        /// An XML container type element that organizes data returned from an Agent in response to a current or sample HTTP request.
        /// </summary>
        public XmlComponentStreamsContainer ComponentStreams { get; set; }


        public XmlDeviceStream() { }

        public XmlDeviceStream(IDeviceStream deviceStream)
        {
            if (deviceStream != null)
            {
                Name = deviceStream.Name;
                Uuid = deviceStream.Uuid;

                var xmlComponentStreams = new List<XmlComponentStream>();
                if (!deviceStream.ComponentStreams.IsNullOrEmpty())
                {
                    foreach (var stream in deviceStream.ComponentStreams)
                    {
                        var xmlStream = new XmlComponentStream(stream);
                        if (xmlStream != null && !xmlStream.Observations.IsNullOrEmpty()) xmlComponentStreams.Add(xmlStream);
                    }
                }

                ComponentStreams = new XmlComponentStreamsContainer { ComponentStreams = xmlComponentStreams };
            }
        }


        public IDeviceStream ToDeviceStream()
        {
            var deviceStream = new DeviceStream();

            deviceStream.Name = Name;
            deviceStream.Uuid = Uuid;

            if (ComponentStreams != null && !ComponentStreams.ComponentStreams.IsNullOrEmpty())
            {
                var componentStreams = new List<IComponentStream>();

                foreach (var xmlComponentStream in ComponentStreams.ComponentStreams)
                {
                    componentStreams.Add(xmlComponentStream.ToComponentStream());
                }

                deviceStream.ComponentStreams = componentStreams;
            }

            return deviceStream;
        }


        #region "Xml Serialization"

        public void WriteXml(XmlWriter writer)
        {
            try
            {
                if (ComponentStreams != null && !ComponentStreams.ComponentStreams.IsNullOrEmpty())
                {
                    writer.WriteStartElement("DeviceStream");
                    writer.WriteAttributeString("name", Name);
                    writer.WriteAttributeString("uuid", Uuid);

                    foreach (var componentStream in ComponentStreams.ComponentStreams)
                    {
                        componentStream.WriteXml(writer);
                    }

                    writer.WriteEndElement();
                }
            }
            catch { }
        }


        public void ReadXml(XmlReader reader)
        {
            try
            {
                // Read Attributes
                Name = reader.GetAttribute("name");
                Uuid = reader.GetAttribute("uuid");

                ComponentStreams = new XmlComponentStreamsContainer();

                // Read the Child DeviceStream Nodes
                var streamsInner = reader.ReadSubtree();

                // Read the next DeviceStream Node
                while (streamsInner.ReadToDescendant("DeviceStream"))
                {
                    // Read the DeviceStream child Nodes
                    var deviceStreamInner = streamsInner.ReadSubtree();

                    // Read the ComponentStreams Container
                    ComponentStreams = new XmlComponentStreamsContainer();
                    ComponentStreams.ReadXml(deviceStreamInner);
                }
            }
            catch { }
        }

        public XmlSchema GetSchema()
        {
            return (null);
        }

        #endregion
    }
}
