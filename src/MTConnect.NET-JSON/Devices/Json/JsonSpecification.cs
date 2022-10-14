// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices.Configurations.Specifications;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonSpecification : JsonAbstractSpecification
    {
        [JsonPropertyName("maximum")]
        public double? Maximum { get; set; }

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

        [JsonPropertyName("minimum")]
        public double? Minimum { get; set; }


        public JsonSpecification() { }

        public JsonSpecification(ISpecification specification) : base(specification)
        {
            if (specification != null)
            {
                Maximum = specification.Maximum;
                UpperLimit = specification.UpperLimit;
                UpperWarning = specification.UpperWarning;
                Nominal = specification.Nominal;
                LowerLimit = specification.LowerLimit;
                LowerWarning = specification.LowerWarning;
                Minimum = specification.Minimum;
            }
        }


        public override IAbstractSpecification ToSpecification()
        {
            var specification = new Specification();
            specification.Id = Id;
            specification.Name = Name;
            specification.Type = Type;
            specification.SubType = SubType;
            specification.DataItemIdRef = DataItemIdRef;
            specification.Units = Units;
            specification.CompositionIdRef = CompositionIdRef;
            specification.CoordinateIdRef = CoordinateIdRef;
            specification.Originator = Originator;
            specification.Maximum = Maximum;
            specification.UpperLimit = UpperLimit;
            specification.UpperWarning = UpperWarning;
            specification.Nominal = Nominal;
            specification.LowerLimit = LowerLimit;
            specification.LowerWarning = LowerWarning;
            specification.Minimum = Minimum;
            return specification;
        }
    }
}
