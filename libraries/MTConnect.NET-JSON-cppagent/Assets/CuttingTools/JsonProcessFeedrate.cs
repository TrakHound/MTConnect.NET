// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    public class JsonProcessFeedRate
    {
        [JsonPropertyName("maximum")]
        public double? Maximum { get; set; }

        [JsonPropertyName("minimum")]
        public double? Minimum { get; set; }

        [JsonPropertyName("nominal")]
        public double? Nominal { get; set; }

        [JsonPropertyName("value")]
        public double? Value { get; set; }


        public JsonProcessFeedRate() { }

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