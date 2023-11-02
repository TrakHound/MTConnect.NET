// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    public class JsonLocation
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("positiveOverlap")]
        public int? PositiveOverlap { get; set; }

        [JsonPropertyName("negativeOverlap")]
        public int? NegativeOverlap { get; set; }

        [JsonPropertyName("turret")]
        public string Turret { get; set; }

        [JsonPropertyName("toolMagazine")]
        public string ToolMagazine { get; set; }

        [JsonPropertyName("toolBar")]
        public string ToolBar { get; set; }

        [JsonPropertyName("toolRack")]
        public string ToolRack { get; set; }

        [JsonPropertyName("automaticToolChanger")]
        public string AutomaticToolChanger { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }


        public JsonLocation() { }

        public JsonLocation(ILocation location)
        {
            if (location != null)
            {
                Type = location.Type.ToString();
                PositiveOverlap = location.PositiveOverlap;
                NegativeOverlap = location.NegativeOverlap;
                Turret = location.Turret;
                ToolMagazine = location.ToolMagazine;
                ToolBar = location.ToolBar;
                ToolRack = location.ToolRack;
                AutomaticToolChanger = location.AutomaticToolChanger;
                Value = location.Value;
            }
        }


        public ILocation ToLocation()
        {
            var location = new Location();
            location.Type = Type.ConvertEnum<LocationType>();
            location.PositiveOverlap = PositiveOverlap;
            location.NegativeOverlap = NegativeOverlap;
            location.Turret = Turret;
            location.ToolMagazine = ToolMagazine;
            location.ToolBar = ToolBar;
            location.ToolRack = ToolRack;
            location.AutomaticToolChanger = AutomaticToolChanger;
            location.Value = Value;
            return location;
        }
    }
}