// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for a Configuration <c>Specification</c>,
    /// extending <see cref="JsonAbstractSpecification"/> with the scalar limit
    /// values. Converts to and from the strongly-typed
    /// <see cref="Specification"/> model.
    /// </summary>
    public class JsonSpecification : JsonAbstractSpecification
    {
        /// <summary>
        /// The absolute maximum permitted value.
        /// </summary>
        [JsonPropertyName("maximum")]
        public double? Maximum { get; set; }

        /// <summary>
        /// The upper limit at which the value is out of specification.
        /// </summary>
        [JsonPropertyName("upperLimit")]
        public double? UpperLimit { get; set; }

        /// <summary>
        /// The upper value at which a warning is raised.
        /// </summary>
        [JsonPropertyName("upperWarning")]
        public double? UpperWarning { get; set; }

        /// <summary>
        /// The nominal (target) value.
        /// </summary>
        [JsonPropertyName("nominal")]
        public double? Nominal { get; set; }

        /// <summary>
        /// The lower limit at which the value is out of specification.
        /// </summary>
        [JsonPropertyName("lowerLimit")]
        public double? LowerLimit { get; set; }

        /// <summary>
        /// The lower value at which a warning is raised.
        /// </summary>
        [JsonPropertyName("lowerWarning")]
        public double? LowerWarning { get; set; }

        /// <summary>
        /// The absolute minimum permitted value.
        /// </summary>
        [JsonPropertyName("minimum")]
        public double? Minimum { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonSpecification() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="ISpecification"/>, copying the common fields via the base
        /// constructor and the scalar limits here.
        /// </summary>
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


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="ISpecification"/>, parsing the originator enumeration and
        /// applying the scalar limit values.
        /// </summary>
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