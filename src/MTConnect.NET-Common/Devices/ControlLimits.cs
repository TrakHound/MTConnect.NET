// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Text.Json.Serialization;

namespace MTConnect.Devices
{
    /// <summary>
    /// A set of limits used to indicate whether a process variable is stable and in control.
    /// </summary>
    public class ControlLimits : IControlLimits
    {
        public const string DescriptionText = "A set of limits used to indicate whether a process variable is stable and in control.";


        /// <summary>
        /// The upper conformance boundary for a variable.
        /// </summary>
        [JsonPropertyName("upperLimit")]
        public double? UpperLimit { get; set; }

        /// <summary>
        /// The upper boundary indicating increased concern and supervision may be required.
        /// </summary>
        [JsonPropertyName("upperWarning")]
        public double? UpperWarning { get; set; }

        /// <summary>
        /// The ideal or desired value for a variable.
        /// </summary>
        [JsonPropertyName("nominal")]
        public double? Nominal { get; set; }

        /// <summary>
        /// The lower conformance boundary for a variable.
        /// </summary>
        [JsonPropertyName("lowerLimit")]
        public double? LowerLimit { get; set; }

        /// <summary>
        /// The lower boundary indicating increased concern and supervision may be required.
        /// </summary>
        [JsonPropertyName("lowerWarning")]
        public double? LowerWarning { get; set; }
    }
}
