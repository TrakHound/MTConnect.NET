// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for a <c>CellDefinition</c>, which
    /// describes a column of a TABLE entry. Mirrors the on-the-wire shape so
    /// the JSON serializer can read and write it, then converts to and from
    /// the strongly-typed <see cref="CellDefinition"/> model.
    /// </summary>
    public class JsonCellDefinition
    {
        /// <summary>
        /// The key identifying the cell within the table row.
        /// </summary>
        [JsonPropertyName("key")]
        public string Key { get; set; }

        /// <summary>
        /// The kind of value <see cref="Key"/> is interpreted as.
        /// </summary>
        [JsonPropertyName("keyType")]
        public string KeyType { get; set; }

        /// <summary>
        /// The engineering units the cell values are expressed in.
        /// </summary>
        [JsonPropertyName("units")]
        public string Units { get; set; }

        /// <summary>
        /// The MTConnect type the cell values report.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The subtype further qualifying <see cref="Type"/>.
        /// </summary>
        [JsonPropertyName("subType")]
        public string SubType { get; set; }

        /// <summary>
        /// The free-form description of the cell.
        /// </summary>
        [JsonPropertyName("description")]
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
                if (definition.Description != null) Description = definition.Description; // v2.5
                //if (definition.Description != null) Description = new JsonDescription(definition.Description);
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
            if (Description != null) definition.Description = Description; // v2.5
            //if (Description != null) definition.Description = Description.ToDescription();
            return definition;
        }
    }
}