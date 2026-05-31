// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    /// <summary>
    /// JSON serialization surrogate for a CuttingTool <c>Location</c> in
    /// the cppagent-compatible shape. Identifies the storage position of a
    /// cutting tool inside a tool changer, magazine, turret, or rack, with
    /// optional overlap counts describing how many adjacent pockets the
    /// tool occupies. Converts to and from the strongly-typed
    /// <see cref="Location"/> model.
    /// </summary>
    public class JsonLocation
    {
        /// <summary>
        /// The kind of storage location (for example POT, STATION, CRIB).
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// Number of additional pockets the tool occupies after its base
        /// position.
        /// </summary>
        [JsonPropertyName("positiveOverlap")]
        public int? PositiveOverlap { get; set; }

        /// <summary>
        /// Number of additional pockets the tool occupies before its base
        /// position.
        /// </summary>
        [JsonPropertyName("negativeOverlap")]
        public int? NegativeOverlap { get; set; }

        /// <summary>
        /// Identifier of the turret holding the tool.
        /// </summary>
        [JsonPropertyName("turret")]
        public string Turret { get; set; }

        /// <summary>
        /// Identifier of the tool magazine holding the tool.
        /// </summary>
        [JsonPropertyName("toolMagazine")]
        public string ToolMagazine { get; set; }

        /// <summary>
        /// Identifier of the tool bar holding the tool.
        /// </summary>
        [JsonPropertyName("toolBar")]
        public string ToolBar { get; set; }

        /// <summary>
        /// Identifier of the tool rack holding the tool.
        /// </summary>
        [JsonPropertyName("toolRack")]
        public string ToolRack { get; set; }

        /// <summary>
        /// Identifier of the automatic tool changer responsible for the
        /// tool.
        /// </summary>
        [JsonPropertyName("automaticToolChanger")]
        public string AutomaticToolChanger { get; set; }

        /// <summary>
        /// The pocket or station value of the location within its
        /// container.
        /// </summary>
        [JsonPropertyName("value")]
        public string Value { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonLocation() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="ILocation"/>, serializing the location type to its
        /// enumeration name.
        /// </summary>
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


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="ILocation"/>, parsing the location-type enumeration
        /// from its serialized form.
        /// </summary>
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