// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    public class JsonCuttingToolArchetypeReference
    {
        [JsonPropertyName("source")]
        public string Source { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }


        public JsonCuttingToolArchetypeReference() { }

        public JsonCuttingToolArchetypeReference(ICuttingToolArchetypeReference cuttingToolArchetypeReference)
        {
            if (cuttingToolArchetypeReference != null)
            {
                Source = cuttingToolArchetypeReference.Source;
                Value = cuttingToolArchetypeReference.Value;
            }
        }


        public ICuttingToolArchetypeReference ToCuttingToolArchetypeReference()
        {
            var location = new CuttingToolArchetypeReference();
            location.Source = Source;
            location.Value = Value;
            return location;
        }
    }
}