// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonEntryDefinitions
    {
        [JsonPropertyName("EntryDefinition")]
        public IEnumerable<JsonEntryDefinition> EntryDefinitions { get; set; }


        public JsonEntryDefinitions() { }

        public JsonEntryDefinitions(IEnumerable<IEntryDefinition> entryDefinitions)
        {
            if (!entryDefinitions.IsNullOrEmpty())
            {
                var jsonEntryDefinitions = new List<JsonEntryDefinition>();

                foreach (var entryDefinition in entryDefinitions) jsonEntryDefinitions.Add(new JsonEntryDefinition(entryDefinition));

                EntryDefinitions = jsonEntryDefinitions;
            }
        }


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