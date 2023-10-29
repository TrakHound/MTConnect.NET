// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonDataItemDefinition
    {
        [JsonPropertyName("Description")]
        public JsonDescription Description { get; set; }

        [JsonPropertyName("EntryDefinitions")]
        public JsonEntryDefinitions EntryDefinitions { get; set; }

        [JsonPropertyName("CellDefinitions")]
        public JsonCellDefinitions CellDefinitions { get; set; }


        public JsonDataItemDefinition() { }

        public JsonDataItemDefinition(IDataItemDefinition dataItemDefinition)
        {
            if (dataItemDefinition != null)
            {
                if (dataItemDefinition.Description != null) Description = new JsonDescription(dataItemDefinition.Description);

                // EntryDefinitions
                if (!dataItemDefinition.EntryDefinitions.IsNullOrEmpty())
                {
                    EntryDefinitions = new JsonEntryDefinitions(dataItemDefinition.EntryDefinitions);
                }

                // CellDefinitions
                if (!dataItemDefinition.CellDefinitions.IsNullOrEmpty())
                {
                    CellDefinitions = new JsonCellDefinitions(dataItemDefinition.CellDefinitions);
                }
            }
        }



        public IDataItemDefinition ToDefinition()
        {
            var definition = new DataItemDefinition();

            if (Description != null) definition.Description = Description.ToDescription();

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