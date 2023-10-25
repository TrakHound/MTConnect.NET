// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Set of limits that define a range of values designating acceptable performance for a variable.
    /// </summary>
    public interface ISpecificationLimits
    {
        /// <summary>
        /// Lower conformance boundary for a variable.> Note: immediate concern or action may be required.
        /// </summary>
        double LowerLimit { get; }
        
        /// <summary>
        /// Numeric target or expected value.
        /// </summary>
        double Nominal { get; }
        
        /// <summary>
        /// Upper conformance boundary for a variable.> Note: immediate concern or action may be required.
        /// </summary>
        double UpperLimit { get; }
    }
}