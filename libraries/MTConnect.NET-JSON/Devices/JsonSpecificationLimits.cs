// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonSpecificationLimits
    {
        [JsonPropertyName("upperLimit")]
        public double? UpperLimit { get; set; }

        [JsonPropertyName("nominal")]
        public double? Nominal { get; set; }

        [JsonPropertyName("lowerLimit")]
        public double? LowerLimit { get; set; }


        public JsonSpecificationLimits() { }

        public JsonSpecificationLimits(ISpecificationLimits specificationLimits)
        {
            if (specificationLimits != null)
            {
                UpperLimit = specificationLimits.UpperLimit;
                Nominal = specificationLimits.Nominal;
                LowerLimit = specificationLimits.LowerLimit;
            }
        }


        public ISpecificationLimits ToSpecificationLimits()
        {
            var specificationLimits = new SpecificationLimits();
            specificationLimits.UpperLimit = UpperLimit;
            specificationLimits.Nominal = Nominal;
            specificationLimits.LowerLimit = LowerLimit;
            return specificationLimits;
        }
    }
}