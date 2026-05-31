// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    /// <summary>
    /// JSON serialization surrogate for a cutting tool <c>ItemLife</c>, the
    /// life measurement of an individual cutting item. Mirrors the on-the-wire
    /// shape so the JSON serializer can read and write it, then converts to
    /// and from the strongly-typed <see cref="ItemLife"/> model.
    /// </summary>
    public class JsonItemLife
    {
        /// <summary>
        /// The current item life value.
        /// </summary>
        [JsonPropertyName("value")]
        public double Value { get; set; }

        /// <summary>
        /// The type of life being measured (MINUTES, PART_COUNT, or WEAR).
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The direction the value counts in (UP or DOWN).
        /// </summary>
        [JsonPropertyName("countDirection")]
        public string CountDirection { get; set; }

        /// <summary>
        /// The value at which a warning is raised.
        /// </summary>
        [JsonPropertyName("warning")]
        public double? Warning { get; set; }

        /// <summary>
        /// The value at which the cutting item reaches the end of its usable
        /// life.
        /// </summary>
        [JsonPropertyName("limit")]
        public double? Limit { get; set; }

        /// <summary>
        /// The value the item life started at.
        /// </summary>
        [JsonPropertyName("initial")]
        public double? Initial { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonItemLife() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IItemLife"/>, converting enumerations to strings.
        /// </summary>
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


        /// <summary>
        /// Converts this surrogate to a strongly-typed <see cref="IItemLife"/>,
        /// parsing the type and count direction enumerations.
        /// </summary>
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