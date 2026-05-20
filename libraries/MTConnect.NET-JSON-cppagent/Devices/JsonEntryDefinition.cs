// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for an <c>EntryDefinition</c> in the
    /// cppagent-compatible shape. Describes a single named entry within a
    /// DATA_SET or TABLE data-item definition, including its expected
    /// units, type/sub-type, and (for tables) the per-cell schema.
    /// Converts to and from the strongly-typed
    /// <see cref="EntryDefinition"/> model.
    /// </summary>
    public class JsonEntryDefinition
    {
        /// <summary>
        /// The dictionary key the entry is keyed on at runtime.
        /// </summary>
        [JsonPropertyName("key")]
        public string Key { get; set; }

        /// <summary>
        /// The semantic type of the key (for example a controlled
        /// vocabulary identifier).
        /// </summary>
        [JsonPropertyName("keyType")]
        public string KeyType { get; set; }

        /// <summary>
        /// Engineering units of the entry's value.
        /// </summary>
        [JsonPropertyName("units")]
        public string Units { get; set; }

        /// <summary>
        /// The MTConnect type of the entry's value.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// Optional sub-type qualifier of the entry's value.
        /// </summary>
        [JsonPropertyName("subType")]
        public string SubType { get; set; }

        /// <summary>
        /// Free-form description of the entry.
        /// </summary>
        [JsonPropertyName("Description")]
        public string Description { get; set; }

        /// <summary>
        /// Per-cell schema, populated only when the parent definition
        /// describes a TABLE data item.
        /// </summary>
        [JsonPropertyName("CellDefinitions")]
        public JsonCellDefinitions CellDefinitions { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonEntryDefinition() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IEntryDefinition"/>, suppressing the cell-definition
        /// container when the source has none.
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

                if (definition.Description != null)
                {
                    Description = definition.Description;
                    //Description = definition.Description.Value;
                }

                // CellDefinitions
                if (!definition.CellDefinitions.IsNullOrEmpty())
                {
                    CellDefinitions = new JsonCellDefinitions(definition.CellDefinitions);
                }
            }
        }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="IEntryDefinition"/>.
        /// </summary>
        public IEntryDefinition ToEntryDefinition()
        {
            var definition = new EntryDefinition();
            definition.Key = Key;
            definition.KeyType = KeyType;
            definition.Units = Units;
            definition.Type = Type;
            definition.SubType = SubType;

            if (Description != null)
            {
                //var description = new Description();
                //description.Value = Description;
                //definition.Description = description;
                definition.Description = Description;
            }

            // Cell Definitions
            if (CellDefinitions != null)
            {
                definition.CellDefinitions = CellDefinitions.ToCellDefinitions();
            }

            return definition;
        }
    }
}