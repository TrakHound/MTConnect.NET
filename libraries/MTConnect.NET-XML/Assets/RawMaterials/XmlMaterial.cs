// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.RawMaterials;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml.RawMaterials
{
    /// <summary>
    /// XML serialization surrogate for the <c>Material</c> a raw-material asset
    /// is made of. Mirrors the on-the-wire element and converts to and from the
    /// strongly-typed <see cref="Material"/> model.
    /// </summary>
    public class XmlMaterial
    {
        /// <summary>
        /// The identifier of the material.
        /// </summary>
        [XmlAttribute("id")]
        public string Id { get; set; }

        /// <summary>
        /// The name of the material.
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// The type of the material.
        /// </summary>
        [XmlAttribute("type")]
        public string Type { get; set; }

        /// <summary>
        /// The lot the material belongs to.
        /// </summary>
        [XmlElement("Lot")]
        public string Lot { get; set; }

        /// <summary>
        /// The manufacturer of the material.
        /// </summary>
        [XmlElement("Manufacturer")]
        public string Manufacturer { get; set; }

        /// <summary>
        /// The date the material was manufactured, as the raw element text.
        /// </summary>
        [XmlElement("ManufacturingDate")]
        public string ManufacturingDate { get; set; }

        /// <summary>
        /// The manufacturer's code for the material.
        /// </summary>
        [XmlElement("ManufacturingCode")]
        public string ManufacturingCode { get; set; }

        /// <summary>
        /// The standardized material code.
        /// </summary>
        [XmlElement("MaterialCode")]
        public string MaterialCode { get; set; }


        /// <summary>
        /// Converts this surrogate into the strongly-typed
        /// <see cref="Material"/>, parsing the manufacturing date.
        /// </summary>
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

        /// <summary>
        /// Writes the given <see cref="IMaterial"/> to <paramref name="writer"/>
        /// as a <c>Material</c> element, omitting optional child elements that
        /// are not set.
        /// </summary>
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