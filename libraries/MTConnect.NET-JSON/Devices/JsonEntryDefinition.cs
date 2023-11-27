// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonEntryDefinition
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("keyType")]
        public string KeyType { get; set; }

        [JsonPropertyName("units")]
        public string Units { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("subType")]
        public string SubType { get; set; }

        [JsonPropertyName("description")]
        public JsonDescription Description { get; set; }

        [JsonPropertyName("cellDefinitions")]
        public IEnumerable<JsonCellDefinition> CellDefinitions { get; set; }


        public JsonEntryDefinition() { }

        public JsonEntryDefinition(IEntryDefinition definition)
        {
            if (definition != null)
            {
                Key = definition.Key;
                KeyType = definition.KeyType;
                Units = definition.Units;
                Type = definition.Type;
                SubType = definition.SubType;

                if (definition.Description != null) Description = new JsonDescription(definition.Description);

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


        public IEntryDefinition ToEntryDefinition()
        {
            var definition = new EntryDefinition();
            definition.Key = Key;
            definition.KeyType = KeyType;
            definition.Units = Units;
            definition.Type = Type;
            definition.SubType = SubType;

            if (Description != null) definition.Description = Description.ToDescription();

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