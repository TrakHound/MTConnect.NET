// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for an <c>EntryDefinition</c>, which
    /// describes the structure of an entry of a DATA_SET or TABLE data item.
    /// Mirrors the on-the-wire shape so the JSON serializer can read and write
    /// it, then converts to and from the strongly-typed
    /// <see cref="EntryDefinition"/> model.
    /// </summary>
    public class JsonEntryDefinition
    {
        /// <summary>
        /// The key identifying the entry within the data set or table.
        /// </summary>
        [JsonPropertyName("key")]
        public string Key { get; set; }

        /// <summary>
        /// The kind of value <see cref="Key"/> is interpreted as.
        /// </summary>
        [JsonPropertyName("keyType")]
        public string KeyType { get; set; }

        /// <summary>
        /// The engineering units the entry values are expressed in.
        /// </summary>
        [JsonPropertyName("units")]
        public string Units { get; set; }

        /// <summary>
        /// The MTConnect type the entry values report.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The subtype further qualifying <see cref="Type"/>.
        /// </summary>
        [JsonPropertyName("subType")]
        public string SubType { get; set; }

        /// <summary>
        /// The free-form description of the entry.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// The cell definitions describing the columns of a TABLE entry.
        /// </summary>
        [JsonPropertyName("cellDefinitions")]
        public IEnumerable<JsonCellDefinition> CellDefinitions { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonEntryDefinition() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IEntryDefinition"/>, converting each cell definition.
        /// </summary>
        public JsonEntryDefinition(IEntryDefinition definition)
        {
            if (definition != null)
            {
                Key = definition.Key;
                KeyType = definition.KeyType;
                Units = definition.Units;
                Type = definition.Type;
                SubType = definition.SubType;

                if (definition.Description != null) Description = definition.Description; // v2.5
                //if (definition.Description != null) Description = new JsonDescription(definition.Description);

                // CellDefinitions
                if (!definition.CellDefinitions.IsNullOrEmpty())
                {
                    var cellDefinitions = new List<JsonCellDefinition>();
                    foreach (var cellDefinition in definition.CellDefinitions)
                    {
                        cellDefinitions.Add(new JsonCellDefinition(cellDefinition));
                    }
                    CellDefinitions = cellDefinitions;
                }
            }
        }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="IEntryDefinition"/>, converting each cell definition.
        /// </summary>
        public IEntryDefinition ToEntryDefinition()
        {
            var definition = new EntryDefinition();
            definition.Key = Key;
            definition.KeyType = KeyType;
            definition.Units = Units;
            definition.Type = Type;
            definition.SubType = SubType;

            if (Description != null) definition.Description = Description; // v2.5
            //if (Description != null) definition.Description = Description.ToDescription();

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
    }
}