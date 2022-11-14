// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Text.Json.Serialization;

namespace MTConnect.Assets.CuttingTools
{
    public class JsonToolLife
    {
        [JsonPropertyName("value")]
        public long Value { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("countDirection")]
        public string CountDirection { get; set; }

        [JsonPropertyName("warning")]
        public long Warning { get; set; }

        [JsonPropertyName("limit")]
        public long Limit { get; set; }

        [JsonPropertyName("initial")]
        public long Initial { get; set; }


        public JsonToolLife() { }

        public JsonToolLife(ToolLife toolLife)
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


        public ToolLife ToToolLife()
        {
            var toolLife = new ToolLife();
            toolLife.Value = Value;
            toolLife.Type = Type.ConvertEnum<ToolLifeType>();
            toolLife.CountDirection = CountDirection.ConvertEnum<ToolLifeCountDirection>();
            toolLife.Warning = Warning;
            toolLife.Limit = Limit;
            toolLife.Initial = Initial;
            return toolLife;
        }
    }
}
