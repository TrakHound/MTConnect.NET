// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for a typed container of
    /// <see cref="JsonCellDefinition"/> items, keyed by the singular
    /// cppagent element name <c>CellDefinition</c>.
    /// </summary>
    public class JsonCellDefinitions
    {
        /// <summary>
        /// The cell definitions in the container.
        /// </summary>
        [JsonPropertyName("CellDefinition")]
        public IEnumerable<JsonCellDefinition> CellDefinitions { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonCellDefinitions() { }

        /// <summary>
        /// Initializes the container from a cell-definition sequence.
        /// </summary>
        public JsonCellDefinitions(IEnumerable<ICellDefinition> cellDefinitions)
        {
            if (!cellDefinitions.IsNullOrEmpty())
            {
                var jsonCellDefinitions = new List<JsonCellDefinition>();

                foreach (var cellDefinition in cellDefinitions) jsonCellDefinitions.Add(new JsonCellDefinition(cellDefinition));

                CellDefinitions = jsonCellDefinitions;
            }
        }


        /// <summary>
        /// Flattens the container back into a uniform
        /// <see cref="ICellDefinition"/> sequence.
        /// </summary>
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