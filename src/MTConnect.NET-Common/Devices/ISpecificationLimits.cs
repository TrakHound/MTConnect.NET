// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices
{
    /// <summary>
    /// A set of limits defining a range of values designating acceptable performance for a variable.
    /// </summary>
    public interface ISpecificationLimits
    {
        /// <summary>
        /// The upper conformance boundary for a variable.
        /// </summary>
        double? UpperLimit { get; }

        /// <summary>
        /// The ideal or desired value for a variable.
        /// </summary>
        double? Nominal { get; }

        /// <summary>
        /// The lower conformance boundary for a variable.
        /// </summary>
        double? LowerLimit { get; }
    }
}
