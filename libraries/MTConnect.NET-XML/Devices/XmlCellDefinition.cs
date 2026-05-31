// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// XML serialization surrogate for a <c>CellDefinition</c>, the schema
    /// describing one column of a table data item. Mirrors the on-the-wire
    /// element and converts to and from the strongly-typed
    /// <see cref="CellDefinition"/> model.
    /// </summary>
    public class XmlCellDefinition
    {
        /// <summary>
        /// The key that identifies the cell within a table entry.
        /// </summary>
        [XmlAttribute("key")]
        public string Key { get; set; }

        /// <summary>
        /// The semantic meaning of the cell's key.
        /// </summary>
        [XmlAttribute("keyType")]
        public string KeyType { get; set; }

        /// <summary>
        /// The engineering units of the cell's value.
        /// </summary>
        [XmlAttribute("units")]
        public string Units { get; set; }

        /// <summary>
        /// The type of the cell's value.
        /// </summary>
        [XmlAttribute("type")]
        public string Type { get; set; }

        /// <summary>
        /// The optional subtype that further qualifies <see cref="Type"/>.
        /// </summary>
        [XmlAttribute("subType")]
        public string SubType { get; set; }

        /// <summary>
        /// The free-form description of the cell.
        /// </summary>
        [XmlElement("Description")]
        public XmlDescription Description { get; set; }


        /// <summary>
        /// Converts this surrogate into the strongly-typed
        /// <see cref="CellDefinition"/>.
        /// </summary>
        public ICellDefinition ToCellDefinition()
        {
            var definition = new CellDefinition();
            definition.Key = Key;
            definition.KeyType = KeyType;
            definition.Units = Units;
            definition.Type = Type;
            definition.SubType = SubType;
            definition.Description = Description?.CDATA;
            //definition.Description = Description?.ToDescription();
            return definition;
        }

        /// <summary>
        /// Writes the given <see cref="ICellDefinition"/> to
        /// <paramref name="writer"/> as a <c>CellDefinition</c> element,
        /// omitting optional attributes that are not set.
        /// </summary>
        public static void WriteXml(XmlWriter writer, ICellDefinition cellDefinition)
        {
            if (cellDefinition != null)
            {
                writer.WriteStartElement("CellDefinition");

                if (!string.IsNullOrEmpty(cellDefinition.Key)) writer.WriteAttributeString("key", cellDefinition.Key);
                if (!string.IsNullOrEmpty(cellDefinition.KeyType)) writer.WriteAttributeString("keyType", cellDefinition.KeyType);
                if (!string.IsNullOrEmpty(cellDefinition.Units)) writer.WriteAttributeString("units", cellDefinition.Units);
                if (!string.IsNullOrEmpty(cellDefinition.Type)) writer.WriteAttributeString("type", cellDefinition.Type);
                if (!string.IsNullOrEmpty(cellDefinition.SubType)) writer.WriteAttributeString("subType", cellDefinition.SubType);

                // Write Description
                if (cellDefinition.Description != null)
                {
                    XmlDescription.WriteXml(writer, cellDefinition.Description);
                }

                writer.WriteEndElement();
            }
        }
    }
}