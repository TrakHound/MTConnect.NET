// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System;
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

        [JsonPropertyName("Description")]
        public string Description { get; set; }

        [JsonPropertyName("CellDefinitions")]
        public JsonCellDefinitions CellDefinitions { get; set; }


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