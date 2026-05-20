// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// XML serialization surrogate for a DataItem <c>Definition</c>, describing
    /// the structure of complex DATA_SET/TABLE observations through entry and
    /// cell definitions.
    /// </summary>
    public class XmlDataItemDefinition
    {
        /// <summary>
        /// The optional free-text description of the definition, serialized as
        /// a <c>Description</c> element.
        /// </summary>
        [XmlElement("Description")]
        public XmlDescription Description { get; set; }

        /// <summary>
        /// The definitions of the keyed entries the data item may report,
        /// serialized as <c>EntryDefinition</c> elements within
        /// <c>EntryDefinitions</c>.
        /// </summary>
        [XmlArray("EntryDefinitions")]
        [XmlArrayItem("EntryDefinition")]
        public List<XmlEntryDefinition> EntryDefinitions { get; set; }

        /// <summary>
        /// The definitions of the cells within each table entry, serialized as
        /// <c>CellDefinition</c> elements within <c>CellDefinitions</c>.
        /// </summary>
        [XmlArray("CellDefinitions")]
        [XmlArrayItem("CellDefinition")]
        public List<XmlCellDefinition> CellDefinitions { get; set; }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="DataItemDefinition"/>, copying the description and
        /// converting each entry and cell definition.
        /// </summary>
        public IDataItemDefinition ToDefinition()
        {
            var definition = new DataItemDefinition();

            if (Description != null) definition.Description = Description?.CDATA;
            //if (Description != null) definition.Description = Description.ToDescription();

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

        /// <summary>
        /// Writes the <c>Definition</c> element, emitting the
        /// <c>EntryDefinitions</c> and <c>CellDefinitions</c> containers only
        /// when populated.
        /// </summary>
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