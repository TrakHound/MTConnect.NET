// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    /// <summary>
    /// JSON serialization surrogate for a cutting tool <c>ToolLife</c>, the
    /// life measurement of the tool as a whole. Mirrors the on-the-wire shape
    /// so the JSON serializer can read and write it, then converts to and from
    /// the strongly-typed <see cref="ToolLife"/> model.
    /// </summary>
    public class JsonToolLife
    {
        /// <summary>
        /// The current tool life value.
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
        /// The value at which the tool reaches the end of its usable life.
        /// </summary>
        [JsonPropertyName("limit")]
        public double? Limit { get; set; }

        /// <summary>
        /// The value the tool life started at.
        /// </summary>
        [JsonPropertyName("initial")]
        public double? Initial { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonToolLife() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IToolLife"/>, converting enumerations to strings.
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
        /// Converts this surrogate to a strongly-typed <see cref="IToolLife"/>,
        /// parsing the type and count direction enumerations.
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