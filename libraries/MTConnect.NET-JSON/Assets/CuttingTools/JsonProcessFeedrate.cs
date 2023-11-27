// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    public class JsonProcessFeedrate
    {
        [JsonPropertyName("maximum")]
        public double? Maximum { get; set; }

        [JsonPropertyName("minimum")]
        public double? Minimum { get; set; }

        [JsonPropertyName("nominal")]
        public double? Nominal { get; set; }


        public JsonProcessFeedrate() { }

        public JsonProcessFeedrate(IProcessFeedRate processFeedrate)
        {
            if (processFeedrate != null)
            {
                Maximum = processFeedrate.Maximum;
                Minimum = processFeedrate.Minimum;
                Nominal = processFeedrate.Nominal;
            }
        }


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