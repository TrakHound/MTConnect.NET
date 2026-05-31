// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for a typed container of
    /// <see cref="JsonEntryDefinition"/> items, keyed by the singular
    /// cppagent element name <c>EntryDefinition</c>.
    /// </summary>
    public class JsonEntryDefinitions
    {
        /// <summary>
        /// The entry definitions in the container.
        /// </summary>
        [JsonPropertyName("EntryDefinition")]
        public IEnumerable<JsonEntryDefinition> EntryDefinitions { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonEntryDefinitions() { }

        /// <summary>
        /// Initializes the container from an entry-definition
        /// sequence.
        /// </summary>
        public JsonEntryDefinitions(IEnumerable<IEntryDefinition> entryDefinitions)
        {
            if (!entryDefinitions.IsNullOrEmpty())
            {
                var jsonEntryDefinitions = new List<JsonEntryDefinition>();

                foreach (var entryDefinition in entryDefinitions) jsonEntryDefinitions.Add(new JsonEntryDefinition(entryDefinition));

                EntryDefinitions = jsonEntryDefinitions;
            }
        }


        /// <summary>
        /// Flattens the container back into a uniform
        /// <see cref="IEntryDefinition"/> sequence.
        /// </summary>
        public IEnumerable<IEntryDefinition> ToEntryDefinitions()
        {
            var entryDefinitions = new List<IEntryDefinition>();

            if (!EntryDefinitions.IsNullOrEmpty())
            {
                foreach (var entryDefinition in EntryDefinitions) entryDefinitions.Add(entryDefinition.ToEntryDefinition());
            }

            return entryDefinitions;
        }
    }
}