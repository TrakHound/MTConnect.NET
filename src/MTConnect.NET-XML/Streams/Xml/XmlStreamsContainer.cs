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
    /// Conditions is a XML container type element. 
    /// Conditions organizes the Data Entities returned in the MTConnectStreams XML document for those DataItem elements defined with a category attribute of EVENT in the MTConnectDevices document.
    /// </summary>
    public class XmlStreamsContainer : IXmlSerializable
    {
        /// <summary>
        /// An XML container type element that organizes the data reported in the MTConnectStreams document for DataItem elements defined in the MTConnectDevices document with a category attribute of EVENT.
        /// </summary>
        [XmlIgnore]
        public List<XmlDeviceStream> DeviceStreams { get; set; }


        public XmlStreamsContainer()
        {
            // Initialize the Conditions List
            DeviceStreams = new List<XmlDeviceStream>();
        }


        #region "Xml Serialization"

        public void WriteXml(XmlWriter writer)
        {
            if (!DeviceStreams.IsNullOrEmpty())
            {
                foreach (var deviceStream in DeviceStreams)
                {
                    deviceStream.WriteXml(writer);
                }
            }
        }

        public void ReadXml(XmlReader reader)
        {
            try
            {
                // Read to first child DeviceStream node
                reader.ReadToDescendant("DeviceStream");

                do
                {
                    // Read the XML DeviceStream
                    var deviceStream = new XmlDeviceStream();
                    deviceStream.ReadXml(reader);

                    if (!deviceStream.ComponentStreams.ComponentStreams.IsNullOrEmpty())
                    {
                        DeviceStreams.Add(deviceStream);
                    }
                }
                while (reader.ReadToNextSibling("DeviceStream"));
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
