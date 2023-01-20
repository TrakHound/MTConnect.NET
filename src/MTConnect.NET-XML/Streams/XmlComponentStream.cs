// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Configurations;
using MTConnect.Devices.DataItems;
using MTConnect.Streams.Output;
using System.Collections.Generic;
using System.Xml;

namespace MTConnect.Streams.Xml
{
    /// <summary>
    /// ComponentStream is a XML container that organizes the data associated with each Structural Element defined for that piece of equipment in the associated MTConnectDevices XML document
    /// </summary>
    internal static class XmlComponentStream
    {
        public static void WriteXml(
            XmlWriter writer,
            ref IComponentStreamOutput componentStream,
            IEnumerable<NamespaceConfiguration> extendedSchemas = null,
            bool outputComments = false
            )
        {
            if (componentStream != null && componentStream.Observations != null && componentStream.Observations.Length > 0)
            {
                if (outputComments && componentStream.Component != null)
                {
                    writer.WriteComment(componentStream.Component.TypeDescription);
                }

                writer.WriteStartElement("ComponentStream");
                writer.WriteAttributeString("component", componentStream.ComponentType);
                writer.WriteAttributeString("componentId", componentStream.ComponentId);
                if (!string.IsNullOrEmpty(componentStream.Name)) writer.WriteAttributeString("name", componentStream.Name);
                if (!string.IsNullOrEmpty(componentStream.NativeName)) writer.WriteAttributeString("nativeName", componentStream.NativeName);
                if (!string.IsNullOrEmpty(componentStream.Uuid)) writer.WriteAttributeString("uuid", componentStream.Uuid);


                // Write Observations
                var observations = componentStream.Observations;
                XmlObservationContainer.WriteXml(writer, ref observations, DataItemCategory.SAMPLE, extendedSchemas, outputComments);
                XmlObservationContainer.WriteXml(writer, ref observations, DataItemCategory.EVENT, extendedSchemas, outputComments);
                XmlObservationContainer.WriteXml(writer, ref observations, DataItemCategory.CONDITION, extendedSchemas, outputComments);

                writer.WriteEndElement();
            }
        }
    }
}