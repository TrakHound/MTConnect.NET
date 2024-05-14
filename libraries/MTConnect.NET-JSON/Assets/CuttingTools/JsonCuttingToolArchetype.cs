// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    public class JsonCuttingToolArchetype
    {
        [JsonPropertyName("cuttingToolDefinition")]
        public JsonCuttingToolDefinition CuttingToolDefinition { get; set; }


        public JsonCuttingToolArchetype() { }

        public JsonCuttingToolArchetype(ICuttingToolArchetypeAsset cuttingToolArchetype)
        {
            if (cuttingToolArchetype != null)
            {
                CuttingToolDefinition = new JsonCuttingToolDefinition(cuttingToolArchetype.CuttingToolDefinition);
            }
        }


        public ICuttingToolArchetypeAsset ToCuttingToolArchetype()
        {
            var cuttingToolArchetype = new CuttingToolArchetypeAsset();
            if (CuttingToolDefinition != null) cuttingToolArchetype.CuttingToolDefinition = CuttingToolDefinition.ToCuttingToolDefinition();
            return cuttingToolArchetype;
        }
    }
}