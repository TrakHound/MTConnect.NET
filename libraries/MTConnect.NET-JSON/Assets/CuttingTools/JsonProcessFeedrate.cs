// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    /// <summary>
    /// JSON serialization surrogate for a cutting tool <c>ProcessFeedRate</c>,
    /// the feed rate limits the tool is intended to operate within. Converts to
    /// and from the strongly-typed <see cref="ProcessFeedRate"/> model.
    /// </summary>
    public class JsonProcessFeedrate
    {
        /// <summary>
        /// The maximum feed rate the tool may be operated at.
        /// </summary>
        [JsonPropertyName("maximum")]
        public double? Maximum { get; set; }

        /// <summary>
        /// The minimum feed rate the tool may be operated at.
        /// </summary>
        [JsonPropertyName("minimum")]
        public double? Minimum { get; set; }

        /// <summary>
        /// The nominal (recommended) feed rate for the tool.
        /// </summary>
        [JsonPropertyName("nominal")]
        public double? Nominal { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonProcessFeedrate() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IProcessFeedRate"/>.
        /// </summary>
        public JsonProcessFeedrate(IProcessFeedRate processFeedrate)
        {
            if (processFeedrate != null)
            {
                Maximum = processFeedrate.Maximum;
                Minimum = processFeedrate.Minimum;
                Nominal = processFeedrate.Nominal;
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
            return processFeedrate;
        }
    }
}