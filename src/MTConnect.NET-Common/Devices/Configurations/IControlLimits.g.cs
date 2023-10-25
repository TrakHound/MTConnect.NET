// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Set of limits that is used to indicate whether a process variable is stable and in control.
    /// </summary>
    public interface IControlLimits
    {
        /// <summary>
        /// Lower conformance boundary for a variable.> Note: immediate concern or action may be required.
        /// </summary>
        double LowerLimit { get; }
        
        /// <summary>
        /// Lower boundary indicating increased concern and supervision may be required.
        /// </summary>
        double LowerWarning { get; }
        
        /// <summary>
        /// Numeric target or expected value.
        /// </summary>
        double Nominal { get; }
        
        /// <summary>
        /// Upper conformance boundary for a variable.> Note: immediate concern or action may be required.
        /// </summary>
        double UpperLimit { get; }
        
        /// <summary>
        /// Upper boundary indicating increased concern and supervision may be required.
        /// </summary>
        double UpperWarning { get; }
    }
}