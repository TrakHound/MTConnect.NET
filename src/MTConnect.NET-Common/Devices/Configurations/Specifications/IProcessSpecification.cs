// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Configurations.Specifications
{
    /// <summary>
    /// ProcessSpecification provides information used to assess the conformance of a variable to process requirements.
    /// </summary>
    public interface IProcessSpecification : IAbstractSpecification
    {
        /// <summary>
        /// A set of limits used to indicate whether a process variable is stable and in control.
        /// </summary>
        IControlLimits ControlLimits { get; }

        /// <summary>
        /// A set of limits defining a range of values designating acceptable performance for a variable.
        /// </summary>
        ISpecificationLimits SpecificationLimits { get; }

        /// <summary>
        /// A set of limits used to trigger warning or alarm indicators.
        /// </summary>
        IAlarmLimits AlarmLimits { get; }
    }
}
