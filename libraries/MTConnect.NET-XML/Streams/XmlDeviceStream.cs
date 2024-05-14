// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Configurations;
using MTConnect.Streams.Output;
using System.Collections.Generic;
using System.Xml;

namespace MTConnect.Streams.Xml
{
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