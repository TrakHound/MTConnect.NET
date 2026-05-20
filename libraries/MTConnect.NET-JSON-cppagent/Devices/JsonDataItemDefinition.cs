// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for a <c>DataItemDefinition</c> in
    /// the cppagent-compatible shape. Carries the per-entry and
    /// per-cell schema for DATA_SET and TABLE data items, plus the
    /// free-form description text. Converts to and from the
    /// strongly-typed <see cref="DataItemDefinition"/> model.
    /// </summary>
    public class JsonDataItemDefinition
    {
        /// <summary>
        /// Free-form description of the data item.
        /// </summary>
        [JsonPropertyName("Description")]
        public string Description { get; set; }

        /// <summary>
        /// Per-entry schema for a DATA_SET or TABLE data item.
        /// </summary>
        [JsonPropertyName("EntryDefinitions")]
        public JsonEntryDefinitions EntryDefinitions { get; set; }

        /// <summary>
        /// Per-cell schema for a TABLE data item.
        /// </summary>
        [JsonPropertyName("CellDefinitions")]
        public JsonCellDefinitions CellDefinitions { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonDataItemDefinition() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IDataItemDefinition"/>, suppressing the entry and
        /// cell containers when their source collections are empty.
        /// </summary>
        public JsonDataItemDefinition(IDataItemDefinition definition)
        {
            if (definition != null)
            {
                if (definition.Description != null)
                {
                    Description = definition.Description;
                    //Description = definition.Description.Value;
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



        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="IDataItemDefinition"/>.
        /// </summary>
        public IDataItemDefinition ToDefinition()
        {
            var definition = new DataItemDefinition();

            if (Description != null)
            {
                //var description = new Description();
                //description.Value = Description;
                //definition.Description = description;
                definition.Description = Description;
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