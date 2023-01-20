// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices.DataItems;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonDataItemDefinition
    {
        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("entryDefinitions")]
        public IEnumerable<JsonEntryDefinition> EntryDefinitions { get; set; }

        [JsonPropertyName("cellDefinitions")]
        public IEnumerable<JsonCellDefinition> CellDefinitions { get; set; }


        public JsonDataItemDefinition() { }

        public JsonDataItemDefinition(IDataItemDefinition dataItemDefinition)
        {
            if (dataItemDefinition != null)
            {
                Description = dataItemDefinition.Description;

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
            definition.Description = Description;

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
