// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Configurations.Specifications
{
    public static class ControlLimitsAttributeDescriptions
    {
        /// <summary>
        /// The upper conformance boundary for a variable.
        /// </summary>
        public const string UpperLimit = "The upper conformance boundary for a variable.";

        /// <summary>
        /// The upper boundary indicating increased concern and supervision may be required.
        /// </summary>
        public const string UpperWarning = "The upper boundary indicating increased concern and supervision may be required.";

        /// <summary>
        /// The ideal or desired value for a variable.
        /// </summary>
        public const string Nominal = "The ideal or desired value for a variable.";

        /// <summary>
        /// The lower conformance boundary for a variable.
        /// </summary>
        public const string LowerLimit = "The lower conformance boundary for a variable.";

        /// <summary>
        /// The lower boundary indicating increased concern and supervision may be required.
        /// </summary>
        public const string LowerWarning = "The lower boundary indicating increased concern and supervision may be required.";
    }
}
