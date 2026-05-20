// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    /// <summary>
    /// JSON serialization surrogate for a CuttingTool
    /// <c>ProcessFeedRate</c> in the cppagent-compatible shape. Carries
    /// the feed-rate envelope (minimum, maximum, nominal) the tool
    /// should be operated within, plus an optional current value.
    /// Converts to and from the strongly-typed
    /// <see cref="ProcessFeedRate"/> model.
    /// </summary>
    public class JsonProcessFeedRate
    {
        /// <summary>
        /// Upper bound of the recommended feed-rate envelope.
        /// </summary>
        [JsonPropertyName("maximum")]
        public double? Maximum { get; set; }

        /// <summary>
        /// Lower bound of the recommended feed-rate envelope.
        /// </summary>
        [JsonPropertyName("minimum")]
        public double? Minimum { get; set; }

        /// <summary>
        /// Nominal (target) feed rate.
        /// </summary>
        [JsonPropertyName("nominal")]
        public double? Nominal { get; set; }

        /// <summary>
        /// The current operating feed rate, when reported.
        /// </summary>
        [JsonPropertyName("value")]
        public double? Value { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonProcessFeedRate() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IProcessFeedRate"/>.
        /// </summary>
        public JsonProcessFeedRate(IProcessFeedRate processFeedrate)
        {
            if (processFeedrate != null)
            {
                Maximum = processFeedrate.Maximum;
                Minimum = processFeedrate.Minimum;
                Nominal = processFeedrate.Nominal;
                Value = processFeedrate.Value;
            }
        }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="IProcessFeedRate"/>.
        /// </summary>
        public IProcessFeedRate ToProcessFeedrate()
        {
            var processFeedrate = new ProcessFeedRate();
            processFeedrate.Maximum = Maximum;
            processFeedrate.Minimum = Minimum;
            processFeedrate.Nominal = Nominal;
            processFeedrate.Value = Value;
            return processFeedrate;
        }
    }
}