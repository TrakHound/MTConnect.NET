// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// XML serialization surrogate for an <c>EntryDefinition</c>, the schema
    /// describing one keyed entry of a data-set or table data item. Mirrors the
    /// on-the-wire element and converts to and from the strongly-typed
    /// <see cref="EntryDefinition"/> model.
    /// </summary>
    public class XmlEntryDefinition
    {
        /// <summary>
        /// The key that identifies the entry.
        /// </summary>
        [XmlAttribute("key")]
        public string Key { get; set; }

        /// <summary>
        /// The semantic meaning of the entry's key.
        /// </summary>
        [XmlAttribute("keyType")]
        public string KeyType { get; set; }

        /// <summary>
        /// The engineering units of the entry's value.
        /// </summary>
        [XmlAttribute("units")]
        public string Units { get; set; }

        /// <summary>
        /// The type of the entry's value.
        /// </summary>
        [XmlAttribute("type")]
        public string Type { get; set; }

        /// <summary>
        /// The optional subtype that further qualifies <see cref="Type"/>.
        /// </summary>
        [XmlAttribute("subType")]
        public string SubType { get; set; }

        /// <summary>
        /// The free-form description of the entry.
        /// </summary>
        [XmlElement("Description")]
        public XmlDescription Description { get; set; }

        /// <summary>
        /// The definitions of the cells of the entry when it is part of a table
        /// data item.
        /// </summary>
        [XmlArray("CellDefinitions")]
        [XmlArrayItem("CellDefinition")]
        public List<XmlCellDefinition> CellDefinitions { get; set; }


        /// <summary>
        /// Converts this surrogate into the strongly-typed
        /// <see cref="EntryDefinition"/>, projecting the nested cell
        /// definitions into their model representation.
        /// </summary>
        public IEntryDefinition ToEntryDefinition()
        {
            var definition = new EntryDefinition();
            definition.Key = Key;
            definition.KeyType = KeyType;
            definition.Units = Units;
            definition.Type = Type;
            definition.SubType = SubType; ;
            definition.Description = Description?.CDATA;
            //definition.Description = Description?.ToDescription();

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
        /// Writes the given <see cref="IEntryDefinition"/> to
        /// <paramref name="writer"/> as an <c>EntryDefinition</c> element,
        /// omitting optional attributes and the cell definitions when not set.
        /// </summary>
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
                if (entryDefinition.Description != null)
                {
                    XmlDescription.WriteXml(writer, entryDefinition.Description);
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