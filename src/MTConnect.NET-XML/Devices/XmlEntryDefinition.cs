// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    public class XmlEntryDefinition
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

        [XmlArray("CellDefinitions")]
        [XmlArrayItem("CellDefinition")]
        public List<XmlCellDefinition> CellDefinitions { get; set; }


        public IEntryDefinition ToEntryDefinition()
        {
            var definition = new EntryDefinition();
            definition.Key = Key;
            definition.KeyType = KeyType;
            definition.Units = Units;
            definition.Type = Type;
            definition.SubType = SubType;
            definition.Description = Description;

            // Cell Definitions
            if (!CellDefinitions.IsNullOrEmpty())
            {
                var cellDefinitions = new List<ICellDefinition>();
                foreach (var cellDefinition in CellDefinitions)
                {
                    cellDefinitions.Add(cellDefinition.ToCellDefinition());
                }
                definition.CellDefinitions = cellDefinitions;
            }

            return definition;
        }

        public static void WriteXml(XmlWriter writer, IEntryDefinition entryDefinition)
        {
            if (entryDefinition != null)
            {
                writer.WriteStartElement("EntryDefinition");

                if (!string.IsNullOrEmpty(entryDefinition.Key)) writer.WriteAttributeString("key", entryDefinition.Key);
                if (!string.IsNullOrEmpty(entryDefinition.KeyType)) writer.WriteAttributeString("keyType", entryDefinition.KeyType);
                if (!string.IsNullOrEmpty(entryDefinition.Units)) writer.WriteAttributeString("units", entryDefinition.Units);
                if (!string.IsNullOrEmpty(entryDefinition.Type)) writer.WriteAttributeString("type", entryDefinition.Type);
                if (!string.IsNullOrEmpty(entryDefinition.SubType)) writer.WriteAttributeString("subType", entryDefinition.SubType);

                // Write Description
                if (!string.IsNullOrEmpty(entryDefinition.Description))
                {
                    writer.WriteStartElement("Description");
                    writer.WriteString(entryDefinition.Description);
                    writer.WriteEndElement();
                }

                // Write Cell Definitions
                if (!entryDefinition.CellDefinitions.IsNullOrEmpty())
                {
                    writer.WriteStartElement("CellDefinitions");
                    foreach (var cellDefinition in entryDefinition.CellDefinitions)
                    {
                        XmlCellDefinition.WriteXml(writer, cellDefinition);
                    }
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }
    }
}