// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.RawMaterials;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml.RawMaterials
{
    public class XmlMaterial
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlElement("Lot")]
        public string Lot { get; set; }

        [XmlElement("Manufacturer")]
        public string Manufacturer { get; set; }

        [XmlElement("ManufacturingDate")]
        public string ManufacturingDate { get; set; }

        [XmlElement("ManufacturingCode")]
        public string ManufacturingCode { get; set; }

        [XmlElement("MaterialCode")]
        public string MaterialCode { get; set; }


        public IMaterial ToMaterial()
        {
            var material = new Material();
            material.Id = Id;
            material.Name = Name;
            material.Type = Type;
            material.Lot = Lot;
            material.Manufacturer = Manufacturer;
            if (ManufacturingDate != null) material.ManufacturingDate = ManufacturingDate.ToDateTime();
            material.ManufacturingCode = ManufacturingCode;
            material.MaterialCode = MaterialCode;
            return material;
        }

        public static void WriteXml(XmlWriter writer, IMaterial material)
        {
            if (material != null)
            {
                writer.WriteStartElement("Material");

                writer.WriteAttributeString("id", material.Id);
                writer.WriteAttributeString("name", material.Name);
                writer.WriteAttributeString("type", material.Type);

                // Lot
                if (!string.IsNullOrEmpty(material.Lot))
                {
                    writer.WriteStartElement("Lot");
                    writer.WriteString(material.Lot);
                    writer.WriteEndElement();
                }

                // Manufacturer
                if (!string.IsNullOrEmpty(material.Manufacturer))
                {
                    writer.WriteStartElement("Manufacturer");
                    writer.WriteString(material.Manufacturer);
                    writer.WriteEndElement();
                }

                // ManufacturingDate
                if (material.ManufacturingDate != null)
                {
                    writer.WriteStartElement("ManufacturingDate");
                    writer.WriteString(material.ManufacturingDate?.ToString());
                    writer.WriteEndElement();
                }

                // ManufacturingCode
                if (!string.IsNullOrEmpty(material.ManufacturingCode))
                {
                    writer.WriteStartElement("ManufacturingCode");
                    writer.WriteString(material.ManufacturingCode);
                    writer.WriteEndElement();
                }

                // MaterialCode
                if (!string.IsNullOrEmpty(material.MaterialCode))
                {
                    writer.WriteStartElement("MaterialCode");
                    writer.WriteString(material.MaterialCode);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }
    }
}