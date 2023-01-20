// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    public class XmlCellDefinition
    {
        [XmlAttribute("key")]
        public string Key { get; set; }

        [XmlAttribute("keyType")]
        public string KeyType { get; set; }

        [XmlAttribute("units")]
        public string Units { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("subType")]
        public string SubType { get; set; }

        [XmlElement("Description")]
        public string Description { get; set; }


        public ICellDefinition ToCellDefinition()
        {
            var definition = new CellDefinition();
            definition.Key = Key;
            definition.KeyType = KeyType;
            definition.Units = Units;
            definition.Type = Type;
            definition.SubType = SubType;
            definition.Description = Description;
            return definition;
        }

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
                if (!string.IsNullOrEmpty(cellDefinition.Description))
                {
                    writer.WriteStartElement("Description");
                    writer.WriteString(cellDefinition.Description);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }
    }
}