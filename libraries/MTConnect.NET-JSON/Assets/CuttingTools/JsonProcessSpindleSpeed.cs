// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    public class JsonProcessSpindleSpeed
    {
        [JsonPropertyName("maximum")]
        public double? Maximum { get; set; }

        [JsonPropertyName("minimum")]
        public double? Minimum { get; set; }

        [JsonPropertyName("nominal")]
        public double? Nominal { get; set; }

        [JsonPropertyName("value")]
        public double? Value { get; set; }


        public JsonProcessSpindleSpeed() { }

        public JsonProcessSpindleSpeed(IProcessSpindleSpeed processSpindleSpeed)
        {
            if (processSpindleSpeed != null)
            {
                Maximum = processSpindleSpeed.Maximum;
                Minimum = processSpindleSpeed.Minimum;
                Nominal = processSpindleSpeed.Nominal;
                Value = processSpindleSpeed.Value;
            }
        }


        public IProcessSpindleSpeed ToProcessSpindleSpeed()
        {
            var processSpindleSpeed = new ProcessSpindleSpeed();
            processSpindleSpeed.Maximum = Maximum;
            processSpindleSpeed.Minimum = Minimum;
            processSpindleSpeed.Nominal = Nominal;
            processSpindleSpeed.Value = Value;
            return processSpindleSpeed;
        }
    }
}