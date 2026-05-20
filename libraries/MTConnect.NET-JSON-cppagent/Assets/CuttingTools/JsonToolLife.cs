// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    /// <summary>
    /// JSON serialization surrogate for a CuttingTool <c>ToolLife</c> in
    /// the cppagent-compatible shape. Tracks the consumed life of an
    /// entire tool against a typed counter (parts, time, or wear) with a
    /// configurable count direction and optional warning/limit/initial
    /// thresholds. Converts to and from the strongly-typed
    /// <see cref="ToolLife"/> model.
    /// </summary>
    public class JsonToolLife
    {
        /// <summary>
        /// The current value of the life counter.
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
        /// The hard limit at which the tool is considered fully
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
        public JsonToolLife() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IToolLife"/>, serializing the type and count
        /// direction to their enumeration names.
        /// </summary>
        public JsonToolLife(IToolLife toolLife)
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


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="IToolLife"/>, parsing the type and count direction
        /// enumerations from their serialized forms.
        /// </summary>
        public IToolLife ToToolLife()
        {
            var toolLife = new ToolLife();
            toolLife.Value = Value;
            toolLife.Type = Type.ConvertEnum<ToolLifeType>();
            toolLife.CountDirection = CountDirection.ConvertEnum<CountDirectionType>();
            toolLife.Warning = Warning;
            toolLife.Limit = Limit;
            toolLife.Initial = Initial;
            return toolLife;
        }
    }
}