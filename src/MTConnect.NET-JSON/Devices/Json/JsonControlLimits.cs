// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations.Specifications;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonControlLimits
    {
        [JsonPropertyName("upperLimit")]
        public double? UpperLimit { get; set; }

        [JsonPropertyName("upperWarning")]
        public double? UpperWarning { get; set; }

        [JsonPropertyName("nominal")]
        public double? Nominal { get; set; }

        [JsonPropertyName("lowerLimit")]
        public double? LowerLimit { get; set; }

        [JsonPropertyName("lowerWarning")]
        public double? LowerWarning { get; set; }


        public JsonControlLimits() { }

        public JsonControlLimits(IControlLimits controlLimits)
        {
            if (controlLimits != null)
            {
                UpperLimit = controlLimits.UpperLimit;
                UpperWarning = controlLimits.UpperWarning;
                Nominal = controlLimits.Nominal;
                LowerLimit = controlLimits.LowerLimit;
                LowerWarning = controlLimits.LowerWarning;
            }
        }


        public IControlLimits ToControlLimits()
        {
            var controlLimits = new ControlLimits();
            controlLimits.UpperLimit = UpperLimit;
            controlLimits.UpperWarning = UpperWarning;
            controlLimits.Nominal = Nominal;
            controlLimits.LowerLimit = LowerLimit;
            controlLimits.LowerWarning = LowerWarning;
            return controlLimits;
        }
    }
}