// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for a <c>CellDefinition</c> in the
    /// cppagent-compatible shape. Describes a single cell schema within
    /// a TABLE entry definition. Converts to and from the strongly-typed
    /// <see cref="CellDefinition"/> model.
    /// </summary>
    public class JsonCellDefinition
    {
        /// <summary>
        /// The dictionary key the cell is keyed on at runtime.
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
        /// Engineering units of the cell's value.
        /// </summary>
        [JsonPropertyName("units")]
        public string Units { get; set; }

        /// <summary>
        /// The MTConnect type of the cell's value.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// Optional sub-type qualifier of the cell's value.
        /// </summary>
        [JsonPropertyName("subType")]
        public string SubType { get; set; }

        /// <summary>
        /// Free-form description of the cell.
        /// </summary>
        [JsonPropertyName("Description")]
        public string Description { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonCellDefinition() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="ICellDefinition"/>.
        /// </summary>
        public JsonCellDefinition(ICellDefinition definition)
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
            }
        }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="ICellDefinition"/>.
        /// </summary>
        public ICellDefinition ToCellDefinition()
        {
            var definition = new CellDefinition();
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

            return definition;
        }
    }
}