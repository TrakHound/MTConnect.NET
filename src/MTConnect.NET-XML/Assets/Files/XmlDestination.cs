// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.Files;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml.Files
{
    public class XmlDestination
    {
        [XmlAttribute("deviceUuid")]
        public string DeviceUuid { get; set; }


        public IDestination ToDestination()
        {
            var destination = new Destination();
            destination.DeviceUuid = DeviceUuid;
            return destination;
        }

        public static void WriteXml(XmlWriter writer, IEnumerable<IDestination> destinations)
        {
            if (!destinations.IsNullOrEmpty())
            {
                writer.WriteStartElement("Destinations");

                foreach (var destination in destinations)
                {
                    writer.WriteStartElement("Destination");
                    writer.WriteAttributeString("deviceUuid", destination.DeviceUuid);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }
    }
}