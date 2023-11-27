// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonDataItemDefinition
    {
        [JsonPropertyName("Description")]
        public string Description { get; set; }

        [JsonPropertyName("EntryDefinitions")]
        public JsonEntryDefinitions EntryDefinitions { get; set; }

        [JsonPropertyName("CellDefinitions")]
        public JsonCellDefinitions CellDefinitions { get; set; }


        public JsonDataItemDefinition() { }

        public JsonDataItemDefinition(IDataItemDefinition definition)
        {
            if (definition != null)
            {
                if (definition.Description != null)
                {
                    Description = definition.Description.Value;
                }

                // EntryDefinitions
                if (!definition.EntryDefinitions.IsNullOrEmpty())
                {
                    EntryDefinitions = new JsonEntryDefinitions(definition.EntryDefinitions);
                }

                // CellDefinitions
                if (!definition.CellDefinitions.IsNullOrEmpty())
                {
                    CellDefinitions = new JsonCellDefinitions(definition.CellDefinitions);
                }
            }
        }



        public IDataItemDefinition ToDefinition()
        {
            var definition = new DataItemDefinition();

            if (Description != null)
            {
                var description = new Description();
                description.Value = Description;
                definition.Description = description;
            }

            // Entry Definitions
            if (EntryDefinitions != null)
            {
                definition.EntryDefinitions = EntryDefinitions.ToEntryDefinitions();
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