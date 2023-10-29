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

        public JsonCuttingToolArchetype(CuttingToolArchetype cuttingToolArchetype)
        {
            if (cuttingToolArchetype != null)
            {
                CuttingToolDefinition = new JsonCuttingToolDefinition(cuttingToolArchetype.CuttingToolDefinition);
            }
        }


        public CuttingToolArchetype ToCuttingToolArchetype()
        {
            var cuttingToolArchetype = new CuttingToolArchetype();
            if (CuttingToolDefinition != null) cuttingToolArchetype.CuttingToolDefinition = CuttingToolDefinition.ToCuttingToolDefinition();
            return cuttingToolArchetype;
        }
    }
}