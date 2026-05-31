// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for the <c>ControlLimits</c>
    /// sub-element of a <c>ProcessSpecification</c> in the
    /// cppagent-compatible shape. Carries the engineering limits the
    /// process should be held within and the early-warning thresholds
    /// before each limit. Converts to and from the strongly-typed
    /// <see cref="ControlLimits"/> model.
    /// </summary>
    public class JsonControlLimits
    {
        /// <summary>
        /// The upper engineering limit of the controlled process.
        /// </summary>
        [JsonPropertyName("UpperLimit")]
        public double? UpperLimit { get; set; }

        /// <summary>
        /// The upper warning threshold before the upper limit.
        /// </summary>
        [JsonPropertyName("UpperWarning")]
        public double? UpperWarning { get; set; }

        /// <summary>
        /// The nominal (target) value of the controlled process.
        /// </summary>
        [JsonPropertyName("Nominal")]
        public double? Nominal { get; set; }

        /// <summary>
        /// The lower engineering limit of the controlled process.
        /// </summary>
        [JsonPropertyName("LowerLimit")]
        public double? LowerLimit { get; set; }

        /// <summary>
        /// The lower warning threshold before the lower limit.
        /// </summary>
        [JsonPropertyName("LowerWarning")]
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
        /// Converts this surrogate to a strongly-typed
        /// <see cref="IControlLimits"/>.
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