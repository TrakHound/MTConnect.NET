// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations.Specifications
{
    public static class AlarmLimitsAttributeDescriptions
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
        /// The lower conformance boundary for a variable.
        /// </summary>
        public const string LowerLimit = "The lower conformance boundary for a variable.";

        /// <summary>
        /// The lower boundary indicating increased concern and supervision may be required.
        /// </summary>
        public const string LowerWarning = "The lower boundary indicating increased concern and supervision may be required.";
    }
}