// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace MTConnect.Assets.CuttingTools
{
    public class JsonProcessFeedrate
    {
        [JsonPropertyName("maximum")]
        public int Maximum { get; set; }

        [JsonPropertyName("minimum")]
        public int Minimum { get; set; }

        [JsonPropertyName("nominal")]
        public int Nominal { get; set; }


        public JsonProcessFeedrate() { }

        public JsonProcessFeedrate(ProcessFeedrate processFeedrate)
        {
            if (processFeedrate != null)
            {
                Maximum = processFeedrate.Maximum;
                Minimum = processFeedrate.Minimum;
                Nominal = processFeedrate.Nominal;
            }
        }


        public ProcessFeedrate ToProcessFeedrate()
        {
            var processFeedrate = new ProcessFeedrate();
            processFeedrate.Maximum = Maximum;
            processFeedrate.Minimum = Minimum;
            processFeedrate.Nominal = Nominal;
            return processFeedrate;
        }
    }
}