// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonDataItemDefinition
    {
        [JsonPropertyName("description")]
        public JsonDescription Description { get; set; }

        [JsonPropertyName("entryDefinitions")]
        public IEnumerable<JsonEntryDefinition> EntryDefinitions { get; set; }

        [JsonPropertyName("cellDefinitions")]
        public IEnumerable<JsonCellDefinition> CellDefinitions { get; set; }


        public JsonDataItemDefinition() { }

        public JsonDataItemDefinition(IDataItemDefinition dataItemDefinition)
        {
            if (dataItemDefinition != null)
            {
                if (dataItemDefinition.Description != null) Description = new JsonDescription(dataItemDefinition.Description);

                // EntryDefinitions
                if (!dataItemDefinition.EntryDefinitions.IsNullOrEmpty())
                {
                    var entryDefinitions = new List<JsonEntryDefinition>();
                    foreach (var entryDefinition in dataItemDefinition.EntryDefinitions)
                    {
                        entryDefinitions.Add(new JsonEntryDefinition(entryDefinition));
                    }
                    EntryDefinitions = entryDefinitions;
                }

                // CellDefinitions
                if (!dataItemDefinition.CellDefinitions.IsNullOrEmpty())
                {
                    var cellDefinitions = new List<JsonCellDefinition>();
                    foreach (var cellDefinition in dataItemDefinition.CellDefinitions)
                    {
                        cellDefinitions.Add(new JsonCellDefinition(cellDefinition));
                    }
                    CellDefinitions = cellDefinitions;
                }
            }
        }



        public IDataItemDefinition ToDefinition()
        {
            var definition = new DataItemDefinition();

            if (Description != null) definition.Description = Description.ToDescription();

            // Entry Definitions
            if (!EntryDefinitions.IsNullOrEmpty())
            {
                var entryDefinitions = new List<IEntryDefinition>();
                foreach (var entryDefinition in EntryDefinitions)
                {
                    entryDefinitions.Add(entryDefinition.ToEntryDefinition());
                }
                definition.EntryDefinitions = entryDefinitions;
            }

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