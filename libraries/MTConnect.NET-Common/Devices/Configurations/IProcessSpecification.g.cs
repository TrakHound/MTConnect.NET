// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Specification that provides information used to assess the conformance of a variable to process requirements.
    /// </summary>
    public interface IProcessSpecification : ISpecification
    {
        /// <summary>
        /// Set of limits that is used to trigger warning or alarm indicators.
        /// </summary>
        MTConnect.Devices.Configurations.IAlarmLimits AlarmLimits { get; }
        
        /// <summary>
        /// Set of limits that is used to indicate whether a process variable is stable and in control.
        /// </summary>
        MTConnect.Devices.Configurations.IControlLimits ControlLimits { get; }
        
        /// <summary>
        /// Set of limits that define a range of values designating acceptable performance for a variable.
        /// </summary>
        MTConnect.Devices.Configurations.ISpecificationLimits SpecificationLimits { get; }
    }
}