// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.Files;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml.Files
{
    /// <summary>
    /// XML serialization surrogate for a File asset <c>Destination</c>,
    /// identifying a device the file is intended to be deployed to.
    /// </summary>
    public class XmlDestination
    {
        /// <summary>
        /// The <c>uuid</c> of the target device, carried by the
        /// <c>deviceUuid</c> attribute.
        /// </summary>
        [XmlAttribute("deviceUuid")]
        public string DeviceUuid { get; set; }


        /// <summary>
        /// Converts this surrogate to a strongly-typed <see cref="Destination"/>.
        /// </summary>
        public IDestination ToDestination()
        {
            var destination = new Destination();
            destination.DeviceUuid = DeviceUuid;
            return destination;
        }

        /// <summary>
        /// Writes the <c>Destinations</c> container with one <c>Destination</c>
        /// element per device; nothing is written when the collection is empty.
        /// </summary>
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