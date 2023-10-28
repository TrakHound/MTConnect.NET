// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    public class XmlDataItemDefinition
    {
        [XmlElement("Description")]
        public XmlDescription Description { get; set; }

        [XmlArray("EntryDefinitions")]
        [XmlArrayItem("EntryDefinition")]
        public List<XmlEntryDefinition> EntryDefinitions { get; set; }

        [XmlArray("CellDefinitions")]
        [XmlArrayItem("CellDefinition")]
        public List<XmlCellDefinition> CellDefinitions { get; set; }


        public IDataItemDefinition ToDefinition()
        {
            var definition = new DataItemDefinition();
            definition.Description = Description.ToDescription();

            // Entry Definitions
            if (!EntryDefinitions.IsNullOrEmpty())
            {
                var entryDefinitions = new List<IEntryDefinition>();
                foreach (var entryDefinition in EntryDefinitions)
                {
                    entryDefinitions.Add(entryDefinition.ToEntryDefinition());
                }
                definition.EntryDefinitions = entryDefinitions;
            }

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

        public static void WriteXml(XmlWriter writer, IDataItemDefinition dataItemDefinition)
        {
            if (dataItemDefinition != null)
            {
                writer.WriteStartElement("Definition");

                //// Write Description
                //if (!string.IsNullOrEmpty(dataItemDefinition.Description))
                //{
                //    writer.WriteStartElement("Description");
                //    writer.WriteString(dataItemDefinition.Description);
                //    writer.WriteEndElement();
                //}

                // Write Entry Definitions
                if (!dataItemDefinition.EntryDefinitions.IsNullOrEmpty())
                {
                    writer.WriteStartElement("EntryDefinitions");
                    foreach (var entryDefinition in dataItemDefinition.EntryDefinitions)
                    {
                        XmlEntryDefinition.WriteXml(writer, entryDefinition);
                    }
                    writer.WriteEndElement();
                }

                // Write Cell Definitions
                if (!dataItemDefinition.CellDefinitions.IsNullOrEmpty())
                {
                    writer.WriteStartElement("CellDefinitions");
                    foreach (var cellDefinition in dataItemDefinition.CellDefinitions)
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