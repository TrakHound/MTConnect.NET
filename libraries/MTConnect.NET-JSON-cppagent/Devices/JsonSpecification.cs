// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for a scalar
    /// <c>Specification</c> in the cppagent-compatible shape, adding the
    /// numeric limit bands (absolute and warning tiers, nominal value) on
    /// top of the shared <see cref="JsonAbstractSpecification"/>
    /// attributes. Converts to and from the strongly-typed
    /// <see cref="Specification"/> model.
    /// </summary>
    public class JsonSpecification : JsonAbstractSpecification
    {
        /// <summary>
        /// The absolute upper bound; values above this are out of
        /// specification.
        /// </summary>
        [JsonPropertyName("Maximum")]
        public double? Maximum { get; set; }

        /// <summary>
        /// The upper engineering limit; the largest value still
        /// considered conforming.
        /// </summary>
        [JsonPropertyName("UpperLimit")]
        public double? UpperLimit { get; set; }

        /// <summary>
        /// The upper warning threshold; values above this should raise
        /// an early-warning signal.
        /// </summary>
        [JsonPropertyName("UpperWarning")]
        public double? UpperWarning { get; set; }

        /// <summary>
        /// The nominal (target) value.
        /// </summary>
        [JsonPropertyName("Nominal")]
        public double? Nominal { get; set; }

        /// <summary>
        /// The lower engineering limit; the smallest value still
        /// considered conforming.
        /// </summary>
        [JsonPropertyName("LowerLimit")]
        public double? LowerLimit { get; set; }

        /// <summary>
        /// The lower warning threshold; values below this should raise
        /// an early-warning signal.
        /// </summary>
        [JsonPropertyName("LowerWarning")]
        public double? LowerWarning { get; set; }

        /// <summary>
        /// The absolute lower bound; values below this are out of
        /// specification.
        /// </summary>
        [JsonPropertyName("Minimum")]
        public double? Minimum { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonSpecification() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="ISpecification"/>, delegating the shared attributes
        /// to <see cref="JsonAbstractSpecification"/>.
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
        /// <see cref="ISpecification"/>, applying the scalar limit bands
        /// in addition to the shared attributes parsed from the base.
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