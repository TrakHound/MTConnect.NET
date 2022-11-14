// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Text.Json.Serialization;

namespace MTConnect.Assets.CuttingTools
{
    public class JsonItemLife
    {
        [JsonPropertyName("value")]
        public double Value { get; set; }

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


        public JsonItemLife() { }

        public JsonItemLife(ItemLife itemLife)
        {
            if (itemLife != null)
            {
                Value = itemLife.Value;
                Type = itemLife.Type.ToString();
                CountDirection = itemLife.CountDirection.ToString();
                Warning = itemLife.Warning;
                Limit = itemLife.Limit;
                Initial = itemLife.Initial;
            }
        }


        public ItemLife ToItemLife()
        {
            var itemLife = new ItemLife();
            itemLife.Value = Value;
            itemLife.Type = Type.ConvertEnum<ToolLifeType>();
            itemLife.CountDirection = CountDirection.ConvertEnum<ToolLifeCountDirection>();
            itemLife.Warning = Warning;
            itemLife.Limit = Limit;
            itemLife.Initial = Initial;
            return itemLife;
        }
    }
}
