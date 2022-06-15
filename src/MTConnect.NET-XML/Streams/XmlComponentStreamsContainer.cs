// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MTConnect.Streams
{
    public class XmlComponentStreamsContainer : IXmlSerializable
    {
        /// <summary>
        /// An XML container type element that organizes the data reported in the MTConnectStreams document for DataItem elements defined in the MTConnectDevices document with a category attribute of EVENT.
        /// </summary>
        [XmlIgnore]
        public List<XmlComponentStream> ComponentStreams { get; set; }


        public XmlComponentStreamsContainer()
        {
            ComponentStreams = new List<XmlComponentStream>();
        }


        #region "Xml Serialization"

        public void WriteXml(XmlWriter writer)
        {
            if (!ComponentStreams.IsNullOrEmpty())
            {
                foreach (var componentStream in ComponentStreams)
                {
                    componentStream.WriteXml(writer);
                }
            }
        }

        public void ReadXml(XmlReader reader)
        {
            try
            {
                // Read to the next ComponentStream Node
                while (reader.ReadToFollowing("ComponentStream"))
                {
                    // Read ComponentStream XML
                    var componentStream = new XmlComponentStream();
                    componentStream.ReadXml(reader);
                    ComponentStreams.Add(componentStream);
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
