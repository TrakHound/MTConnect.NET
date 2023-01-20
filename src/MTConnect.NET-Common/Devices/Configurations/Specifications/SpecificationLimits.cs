// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Configurations.Specifications
{
    /// <summary>
    /// A set of limits defining a range of values designating acceptable performance for a variable.
    /// </summary>
    public class SpecificationLimits : ISpecificationLimits
    {
        public const string DescriptionText = "A set of limits defining a range of values designating acceptable performance for a variable.";


        /// <summary>
        /// The upper conformance boundary for a variable.
        /// </summary>
        public double? UpperLimit { get; set; }

        /// <summary>
        /// The ideal or desired value for a variable.
        /// </summary>
        public double? Nominal { get; set; }

        /// <summary>
        /// The lower conformance boundary for a variable.
        /// </summary>
        public double? LowerLimit { get; set; }
    }
}
