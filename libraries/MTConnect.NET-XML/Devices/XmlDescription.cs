// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    public class XmlDescription
    {
        [XmlAttribute("manufacturer")]
        public string Manufacturer { get; set; }

        [XmlAttribute("model")]
        public string Model { get; set; }

        [XmlAttribute("serialNumber")]
        public string SerialNumber { get; set; }

        [XmlAttribute("station")]
        public string Station { get; set; }

        [XmlText]
        public string CDATA { get; set; }


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
    }
}