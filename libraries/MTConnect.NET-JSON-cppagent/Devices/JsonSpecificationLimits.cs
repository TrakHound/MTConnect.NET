// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for the <c>SpecificationLimits</c>
    /// sub-element of a <c>ProcessSpecification</c> in the
    /// cppagent-compatible shape. Carries the absolute conformance
    /// bounds plus the nominal target value. Converts to and from the
    /// strongly-typed <see cref="SpecificationLimits"/> model.
    /// </summary>
    public class JsonSpecificationLimits
    {
        /// <summary>
        /// The upper specification limit; values above this are
        /// non-conforming.
        /// </summary>
        [JsonPropertyName("UpperLimit")]
        public double? UpperLimit { get; set; }

        /// <summary>
        /// The nominal (target) value of the specification.
        /// </summary>
        [JsonPropertyName("Nominal")]
        public double? Nominal { get; set; }

        /// <summary>
        /// The lower specification limit; values below this are
        /// non-conforming.
        /// </summary>
        [JsonPropertyName("LowerLimit")]
        public double? LowerLimit { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonSpecificationLimits() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="ISpecificationLimits"/>.
        /// </summary>
        public JsonSpecificationLimits(ISpecificationLimits specificationLimits)
        {
            if (specificationLimits != null)
            {
                UpperLimit = specificationLimits.UpperLimit;
                Nominal = specificationLimits.Nominal;
                LowerLimit = specificationLimits.LowerLimit;
            }
        }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="ISpecificationLimits"/>.
        /// </summary>
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