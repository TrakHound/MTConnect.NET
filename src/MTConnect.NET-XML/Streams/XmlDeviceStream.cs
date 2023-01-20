// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Configurations;
using MTConnect.Streams.Output;
using System.Collections.Generic;
using System.Xml;

namespace MTConnect.Streams.Xml
{
    /// <summary>
    /// DeviceStream is a XML container that organizes data reported from a single piece of equipment.A DeviceStream element MUST be provided for each piece of equipment reporting data in an MTConnectStreams document.
    /// </summary>
    internal static class XmlDeviceStream
    {
        public static void WriteXml(
            XmlWriter writer, 
            ref IDeviceStreamOutput deviceStream,
            IEnumerable<NamespaceConfiguration> extendedSchemas = null,
            bool outputComments = false
            )
        {
            if (deviceStream != null && !deviceStream.ComponentStreams.IsNullOrEmpty())
            {
                writer.WriteStartElement("DeviceStream");
                writer.WriteAttributeString("name", deviceStream.Name);
                writer.WriteAttributeString("uuid", deviceStream.Uuid);

                for (var i = 0; i < deviceStream.ComponentStreams.Length; i++)
                {
                    XmlComponentStream.WriteXml(writer, ref deviceStream.ComponentStreams[i], extendedSchemas, outputComments);
                }

                writer.WriteEndElement();
            }
        }
    }
}
