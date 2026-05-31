// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for the <c>ControlLimits</c> of a
    /// Specification, carrying the upper/lower control and warning bounds and
    /// the nominal target.
    /// </summary>
    public class JsonControlLimits
    {
        /// <summary>
        /// The upper control bound.
        /// </summary>
        [JsonPropertyName("upperLimit")]
        public double? UpperLimit { get; set; }

        /// <summary>
        /// The upper warning bound.
        /// </summary>
        [JsonPropertyName("upperWarning")]
        public double? UpperWarning { get; set; }

        /// <summary>
        /// The nominal (target) value.
        /// </summary>
        [JsonPropertyName("nominal")]
        public double? Nominal { get; set; }

        /// <summary>
        /// The lower control bound.
        /// </summary>
        [JsonPropertyName("lowerLimit")]
        public double? LowerLimit { get; set; }

        /// <summary>
        /// The lower warning bound.
        /// </summary>
        [JsonPropertyName("lowerWarning")]
        public double? LowerWarning { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonControlLimits() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IControlLimits"/>.
        /// </summary>
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


        /// <summary>
        /// Converts this surrogate to a strongly-typed <see cref="ControlLimits"/>.
        /// </summary>
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