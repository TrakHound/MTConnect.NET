// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace MTConnect.Assets.CuttingTools
{
    public class JsonCuttingToolDefinition
    {
        [JsonPropertyName("format")]
        public string Format { get; set; }


        public JsonCuttingToolDefinition() { }

        public JsonCuttingToolDefinition(CuttingToolDefinition cuttingToolDefinition)
        {
            if (cuttingToolDefinition != null)
            {
                Format = cuttingToolDefinition.Format.ToString();
            }
        }


        public CuttingToolDefinition ToCuttingToolDefinition()
        {
            var cuttingToolDefinition = new CuttingToolDefinition();
            cuttingToolDefinition.Format = Format.ConvertEnum<CuttingToolDefinitionFormat>();
            return cuttingToolDefinition;
        }
    }
}