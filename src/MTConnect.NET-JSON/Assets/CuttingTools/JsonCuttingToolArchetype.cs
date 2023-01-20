// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace MTConnect.Assets.CuttingTools
{
    public class JsonCuttingToolArchetype
    {
        [JsonPropertyName("cuttingToolDefinition")]
        public CuttingToolDefinition CuttingToolDefinition { get; set; }


        public JsonCuttingToolArchetype() { }

        public JsonCuttingToolArchetype(CuttingToolArchetype cuttingToolArchetype)
        {
            if (cuttingToolArchetype != null)
            {
                CuttingToolDefinition = cuttingToolArchetype.CuttingToolDefinition;
            }
        }


        public CuttingToolArchetype ToCuttingToolArchetype()
        {
            var cuttingToolArchetype = new CuttingToolArchetype();
            cuttingToolArchetype.CuttingToolDefinition = CuttingToolDefinition;
            return cuttingToolArchetype;
        }
    }
}