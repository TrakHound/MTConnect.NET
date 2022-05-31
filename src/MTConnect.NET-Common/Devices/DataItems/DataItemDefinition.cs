// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Collections.Generic;
using System.Xml.Serialization;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// The Definition provides additional descriptive information for any DataItem representations.
    /// When the representation is either DATA_SET or TABLE, it gives the specific meaning of a key and MAY provide a Description, type, and units for semantic interpretation of data.
    /// </summary>
    public class DataItemDefinition : IDataItemDefinition
    {
        /// <summary>
        /// The Description of the Definition.
        /// </summary>
        [XmlElement("Description")]
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// The EntryDefinitions aggregates EntryDefinition.
        /// </summary>
        [XmlArray("EntryDefinitions")]
        [XmlArrayItem("EntryDefinition")]
        [JsonPropertyName("entryDefinitions")]
        public List<EntryDefinition> EntryDefinitions { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public bool EntryDefinitionsSpecified => !EntryDefinitions.IsNullOrEmpty();

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
