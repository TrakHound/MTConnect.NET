// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// XML serialization surrogate for a component's <c>Description</c>.
    /// Mirrors the on-the-wire element, where the free text is carried as the
    /// element body and the identification fields as attributes, and converts
    /// to and from the strongly-typed <see cref="Description"/> model.
    /// </summary>
    public class XmlDescription
    {
        /// <summary>
        /// The manufacturer of the component being described.
        /// </summary>
        [XmlAttribute("manufacturer")]
        public string Manufacturer { get; set; }

        /// <summary>
        /// The model of the component being described.
        /// </summary>
        [XmlAttribute("model")]
        public string Model { get; set; }

        /// <summary>
        /// The serial number of the component being described.
        /// </summary>
        [XmlAttribute("serialNumber")]
        public string SerialNumber { get; set; }

        /// <summary>
        /// The station the component being described belongs to.
        /// </summary>
        [XmlAttribute("station")]
        public string Station { get; set; }

        /// <summary>
        /// The free-text description, carried as the element body.
        /// </summary>
        [XmlText]
        public string CDATA { get; set; }


        /// <summary>
        /// Converts this surrogate into the strongly-typed
        /// <see cref="Description"/>, trimming the free-text body.
        /// </summary>
        public IDescription ToDescription()
        {
            var description = new Description();
            description.Manufacturer = Manufacturer;
            description.Model = Model;
            description.SerialNumber = SerialNumber;
            description.Station = Station;
            description.Value = CDATA != null ? CDATA.Trim() : null;
            return description;
        }

        /// <summary>
        /// Writes the given <see cref="IDescription"/> to
        /// <paramref name="writer"/> as a <c>Description</c> element, skipping
        /// the element entirely when every field is empty.
        /// </summary>
        public static void WriteXml(XmlWriter writer, IDescription description)
        {
            if (description != null &&
                (!string.IsNullOrEmpty(description.Manufacturer) ||
                !string.IsNullOrEmpty(description.Model) ||
                !string.IsNullOrEmpty(description.SerialNumber) ||
                !string.IsNullOrEmpty(description.Value)
                ))
            {
                writer.WriteStartElement("Description");

                // Write Description Properties
                if (!string.IsNullOrEmpty(description.Manufacturer)) writer.WriteAttributeString("manufacturer", description.Manufacturer);
                if (!string.IsNullOrEmpty(description.Model)) writer.WriteAttributeString("model", description.Model);
                if (!string.IsNullOrEmpty(description.SerialNumber)) writer.WriteAttributeString("serialNumber", description.SerialNumber);
                if (!string.IsNullOrEmpty(description.Value)) writer.WriteString(description.Value);

                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Writes a plain string as a <c>Description</c> element, skipping the
        /// element when the string is empty.
        /// </summary>
        public static void WriteXml(XmlWriter writer, string description)
        {
            if (!string.IsNullOrEmpty(description))
            {
                writer.WriteStartElement("Description");
                writer.WriteString(description);
                writer.WriteEndElement();
            }
        }
    }
}