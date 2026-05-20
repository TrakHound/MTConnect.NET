// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    /// <summary>
    /// JSON serialization surrogate for a CuttingItem <c>ItemLife</c> in
    /// the cppagent-compatible shape. Tracks the consumed life of a
    /// single cutting item (insert or edge) using the same counter
    /// vocabulary as <see cref="JsonToolLife"/>. Converts to and from
    /// the strongly-typed <see cref="ItemLife"/> model.
    /// </summary>
    public class JsonItemLife
    {
        /// <summary>
        /// The current value of the item life counter.
        /// </summary>
        [JsonPropertyName("value")]
        public double Value { get; set; }

        /// <summary>
        /// The kind of life being counted (for example MINUTES, PARTS,
        /// WEAR).
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// Whether the counter counts UP or DOWN.
        /// </summary>
        [JsonPropertyName("countDirection")]
        public string CountDirection { get; set; }

        /// <summary>
        /// Threshold at which a warning should be raised before the
        /// limit is reached.
        /// </summary>
        [JsonPropertyName("warning")]
        public double? Warning { get; set; }

        /// <summary>
        /// The hard limit at which the cutting item is considered fully
        /// consumed.
        /// </summary>
        [JsonPropertyName("limit")]
        public double? Limit { get; set; }

        /// <summary>
        /// The initial value the counter starts at after a reset.
        /// </summary>
        [JsonPropertyName("initial")]
        public double? Initial { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonItemLife() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IItemLife"/>, serializing the type and count
        /// direction to their enumeration names.
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
        /// Converts this surrogate to a strongly-typed
        /// <see cref="IItemLife"/>, parsing the type and count direction
        /// enumerations from their serialized forms.
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