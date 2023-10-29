// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    public class JsonCuttingToolDefinition
    {
        [JsonPropertyName("format")]
        public string Format { get; set; }


        public JsonCuttingToolDefinition() { }

        public JsonCuttingToolDefinition(ICuttingToolDefinition cuttingToolDefinition)
        {
            if (cuttingToolDefinition != null)
            {
                Format = cuttingToolDefinition.Format.ToString();
            }
        }


        public CuttingToolDefinition ToCuttingToolDefinition()
        {
            var cuttingToolDefinition = new CuttingToolDefinition();
            cuttingToolDefinition.Format = Format.ConvertEnum<FormatType>();
            return cuttingToolDefinition;
        }
    }
}