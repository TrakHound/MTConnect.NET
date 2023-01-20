// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Text.Json.Serialization;

namespace MTConnect.Assets.CuttingTools
{
    public class JsonLocation
    {
        [JsonPropertyName("value")]
        public int Value { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("positiveOverlap")]
        public int PositiveOverlap { get; set; }

        [JsonPropertyName("negativeOverlap")]
        public int NegativeOverlap { get; set; }


        public JsonLocation() { }

        public JsonLocation(Location location)
        {
            if (location != null)
            {
                Value = location.Value;
                Type = location.Type.ToString();
                PositiveOverlap = location.PositiveOverlap;
                NegativeOverlap = location.NegativeOverlap;
            }
        }


        public Location ToLocation()
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
