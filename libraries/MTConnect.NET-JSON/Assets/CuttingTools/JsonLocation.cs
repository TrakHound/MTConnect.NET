// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    /// <summary>
    /// JSON serialization surrogate for a cutting tool <c>Location</c>, the
    /// position of the tool in the machine. Mirrors the on-the-wire shape so
    /// the JSON serializer can read and write it, then converts to and from
    /// the strongly-typed <see cref="Location"/> model.
    /// </summary>
    public class JsonLocation
    {
        /// <summary>
        /// The location identifier (for example a pot or station number).
        /// </summary>
        [JsonPropertyName("value")]
        public string Value { get; set; }

        /// <summary>
        /// The kind of location (for example POT, STATION, or SPINDLE).
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The number of adjacent locations occupied in the positive
        /// direction by an oversized tool.
        /// </summary>
        [JsonPropertyName("positiveOverlap")]
        public int? PositiveOverlap { get; set; }

        /// <summary>
        /// The number of adjacent locations occupied in the negative
        /// direction by an oversized tool.
        /// </summary>
        [JsonPropertyName("negativeOverlap")]
        public int? NegativeOverlap { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonLocation() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="ILocation"/>, converting the type enumeration to a
        /// string.
        /// </summary>
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


        /// <summary>
        /// Converts this surrogate to a strongly-typed <see cref="ILocation"/>,
        /// parsing the type enumeration.
        /// </summary>
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