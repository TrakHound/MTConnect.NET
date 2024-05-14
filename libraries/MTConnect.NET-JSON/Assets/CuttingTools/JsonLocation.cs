// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    public class JsonLocation
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("positiveOverlap")]
        public int? PositiveOverlap { get; set; }

        [JsonPropertyName("negativeOverlap")]
        public int? NegativeOverlap { get; set; }


        public JsonLocation() { }

        public JsonLocation(ILocation location)
        {
            if (location != null)
            {
                Value = location.Value;
                Type = location.Type.ToString();
                PositiveOverlap = location.PositiveOverlap;
                NegativeOverlap = location.NegativeOverlap;
            }
        }


        public ILocation ToLocation()
        {
            var location = new Location();
            location.Value = Value;
            location.Type = Type.ConvertEnum<LocationType>();
            location.PositiveOverlap = PositiveOverlap;
            location.NegativeOverlap = NegativeOverlap;
            return location;
        }
    }
}