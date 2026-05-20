// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for the <c>SpecificationLimits</c> of a
    /// process specification, namely the tolerance band around the nominal
    /// value. Converts to and from the strongly-typed
    /// <see cref="SpecificationLimits"/> model.
    /// </summary>
    public class JsonSpecificationLimits
    {
        /// <summary>
        /// The upper specification limit (USL).
        /// </summary>
        [JsonPropertyName("upperLimit")]
        public double? UpperLimit { get; set; }

        /// <summary>
        /// The nominal (target) value.
        /// </summary>
        [JsonPropertyName("nominal")]
        public double? Nominal { get; set; }

        /// <summary>
        /// The lower specification limit (LSL).
        /// </summary>
        [JsonPropertyName("lowerLimit")]
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