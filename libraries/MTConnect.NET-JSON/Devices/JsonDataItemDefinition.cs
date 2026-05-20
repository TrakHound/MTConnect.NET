// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for a <c>Definition</c> describing the
    /// shape of a DATA_SET or TABLE data item's entries. Mirrors the
    /// on-the-wire shape so the JSON serializer can read and write it, then
    /// converts to and from the strongly-typed <see cref="DataItemDefinition"/>
    /// model.
    /// </summary>
    public class JsonDataItemDefinition
    {
        /// <summary>
        /// The free-form description of the data item definition.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// The entry definitions describing the keys of a DATA_SET or TABLE.
        /// </summary>
        [JsonPropertyName("entryDefinitions")]
        public IEnumerable<JsonEntryDefinition> EntryDefinitions { get; set; }

        /// <summary>
        /// The cell definitions describing the columns shared by every TABLE
        /// entry.
        /// </summary>
        [JsonPropertyName("cellDefinitions")]
        public IEnumerable<JsonCellDefinition> CellDefinitions { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonDataItemDefinition() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IDataItemDefinition"/>, converting each entry and cell
        /// definition.
        /// </summary>
        public JsonDataItemDefinition(IDataItemDefinition dataItemDefinition)
        {
            if (dataItemDefinition != null)
            {
                if (dataItemDefinition.Description != null) Description = dataItemDefinition.Description; // v2.5
                //if (dataItemDefinition.Description != null) Description = new JsonDescription(dataItemDefinition.Description);

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



        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="IDataItemDefinition"/>, converting each entry and cell
        /// definition.
        /// </summary>
        public IDataItemDefinition ToDefinition()
        {
            var definition = new DataItemDefinition();

            if (Description != null) definition.Description = Description; // v2.5
            //if (Description != null) definition.Description = Description.ToDescription();

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