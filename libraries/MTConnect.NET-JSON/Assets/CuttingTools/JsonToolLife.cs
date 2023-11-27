// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    public class JsonToolLife
    {
        [JsonPropertyName("value")]
        public double Value { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("countDirection")]
        public string CountDirection { get; set; }

        [JsonPropertyName("warning")]
        public double? Warning { get; set; }

        [JsonPropertyName("limit")]
        public double? Limit { get; set; }

        [JsonPropertyName("initial")]
        public double? Initial { get; set; }


        public JsonToolLife() { }

        public JsonToolLife(IToolLife toolLife)
        {
            if (toolLife != null)
            {
                Value = toolLife.Value;
                Type = toolLife.Type.ToString();
                CountDirection = toolLife.CountDirection.ToString();
                Warning = toolLife.Warning;
                Limit = toolLife.Limit;
                Initial = toolLife.Initial;
            }
        }


        public IToolLife ToToolLife()
        {
            var toolLife = new ToolLife();
            toolLife.Value = Value;
            toolLife.Type = Type.ConvertEnum<ToolLifeType>();
            toolLife.CountDirection = CountDirection.ConvertEnum<CountDirectionType>();
            toolLife.Warning = Warning;
            toolLife.Limit = Limit;
            toolLife.Initial = Initial;
            return toolLife;
        }
    }
}