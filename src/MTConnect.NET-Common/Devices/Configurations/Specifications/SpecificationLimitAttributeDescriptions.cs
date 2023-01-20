// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations.Specifications
{
    /// <summary>
    /// A set of limits defining a range of values designating acceptable performance for a variable.
    /// </summary>
    public static class SpecificationLimitAttributeDescriptions
    {
        /// <summary>
        /// The upper conformance boundary for a variable.
        /// </summary>
        public const string UpperLimit = "The upper conformance boundary for a variable.";

        /// <summary>
        /// The ideal or desired value for a variable.
        /// </summary>
        public const string Nominal = "The ideal or desired value for a variable.";

        /// <summary>
        /// The lower conformance boundary for a variable.
        /// </summary>
        public const string LowerLimit = "The lower conformance boundary for a variable.";
    }
}