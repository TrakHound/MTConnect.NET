// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
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
        public double? Warning { get; set; }

        [JsonPropertyName("limit")]
        public double? Limit { get; set; }

        [JsonPropertyName("initial")]
        public double? Initial { get; set; }


        public JsonItemLife() { }

        public JsonItemLife(IItemLife itemLife)
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


        public IItemLife ToItemLife()
        {
            var itemLife = new ItemLife();
            itemLife.Value = Value;
            itemLife.Type = Type.ConvertEnum<ToolLifeType>();
            itemLife.CountDirection = CountDirection.ConvertEnum<CountDirectionType>();
            itemLife.Warning = Warning;
            itemLife.Limit = Limit;
            itemLife.Initial = Initial;
            return itemLife;
        }
    }
}