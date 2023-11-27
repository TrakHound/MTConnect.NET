// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonCellDefinitions
    {
        [JsonPropertyName("CellDefinition")]
        public IEnumerable<JsonCellDefinition> CellDefinitions { get; set; }


        public JsonCellDefinitions() { }

        public JsonCellDefinitions(IEnumerable<ICellDefinition> cellDefinitions)
        {
            if (!cellDefinitions.IsNullOrEmpty())
            {
                var jsonCellDefinitions = new List<JsonCellDefinition>();

                foreach (var cellDefinition in cellDefinitions) jsonCellDefinitions.Add(new JsonCellDefinition(cellDefinition));

                CellDefinitions = jsonCellDefinitions;
            }
        }


        public IEnumerable<ICellDefinition> ToCellDefinitions()
        {
            var cellDefinitions = new List<ICellDefinition>();

            if (!CellDefinitions.IsNullOrEmpty())
            {
                foreach (var cellDefinition in CellDefinitions) cellDefinitions.Add(cellDefinition.ToCellDefinition());
            }

            return cellDefinitions;
        }
    }
}