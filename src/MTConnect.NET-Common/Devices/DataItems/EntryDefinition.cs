// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Collections.Generic;
using System.Xml.Serialization;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.DataItems
{
    public class EntryDefinition : IEntryDefinition
    {
        /// <summary>
        /// The unique identification of the Entry in the Definition. The description applies to all Entry observations having this key.
        /// </summary>
        [XmlAttribute("key")]
        [JsonPropertyName("key")]
        public string Key { get; set; }

        /// <summary>
        /// The DataItem type that defines the meaning of the key.
        /// </summary>
        [XmlAttribute("keyType")]
        [JsonPropertyName("keyType")]
        public string KeyType { get; set; }

        /// <summary>
        /// Units MUST be present for all DataItem elements in the SAMPLE category.
        /// If the data represented by a DataItem is a numeric value, except for line number and count, the units MUST be specified.
        /// </summary>
        [XmlAttribute("units")]
        [JsonPropertyName("units")]
        public string Units { get; set; }

        /// <summary>
        /// The type of data being measured.
        /// Examples of types are POSITION, VELOCITY, ANGLE, BLOCK, ROTARY_VELOCITY, etc.
        /// </summary>
        [XmlAttribute("type")]
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// A sub-categorization of the data item type.
        /// For example, the Sub-types of POSITION can be ACTUAL or COMMANDED.
        /// Not all types have subTypes and they can be optional.
        /// </summary>
        [XmlAttribute("subType")]
        [JsonPropertyName("subType")]
        public string SubType { get; set; }

        /// <summary>
        /// The Description of the EntryDefinition.
        /// </summary>
        [XmlElement("Description")]
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// The CellDefinitions aggregates CellDefinition.
        /// </summary>
        [XmlArray("CellDefinitions")]
        [XmlArrayItem("CellDefinition")]
        [JsonPropertyName("cellDefinitions")]
        public List<CellDefinition> CellDefinitions { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public bool CellDefinitionsSpecified => !CellDefinitions.IsNullOrEmpty();
    }
}
