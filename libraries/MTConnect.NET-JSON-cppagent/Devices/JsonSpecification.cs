// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonSpecification : JsonAbstractSpecification
    {
        [JsonPropertyName("Maximum")]
        public double? Maximum { get; set; }

        [JsonPropertyName("UpperLimit")]
        public double? UpperLimit { get; set; }

        [JsonPropertyName("UpperWarning")]
        public double? UpperWarning { get; set; }

        [JsonPropertyName("Nominal")]
        public double? Nominal { get; set; }

        [JsonPropertyName("LowerLimit")]
        public double? LowerLimit { get; set; }

        [JsonPropertyName("LowerWarning")]
        public double? LowerWarning { get; set; }

        [JsonPropertyName("Minimum")]
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


        public override ISpecification ToSpecification()
        {
            var specification = new Specification();
            specification.Id = Id;
            specification.Name = Name;
            specification.Type = Type;
            specification.SubType = SubType;
            specification.DataItemIdRef = DataItemIdRef;
            specification.Units = Units;
            specification.CompositionIdRef = CompositionIdRef;
            specification.CoordinateSystemIdRef = CoordinateSystemIdRef;
            specification.Originator = Originator.ConvertEnum<Originator>();
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