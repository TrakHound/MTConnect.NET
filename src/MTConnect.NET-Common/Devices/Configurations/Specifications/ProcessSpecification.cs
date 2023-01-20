// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace MTConnect.Devices.Configurations.Specifications
{
    /// <summary>
    /// ProcessSpecification provides information used to assess the conformance of a variable to process requirements.
    /// </summary>
    public class ProcessSpecification : AbstractSpecification, IProcessSpecification
    {
        /// <summary>
        /// A set of limits used to indicate whether a process variable is stable and in control.
        /// </summary>
        public IControlLimits ControlLimits { get; set; }

        /// <summary>
        /// A set of limits defining a range of values designating acceptable performance for a variable.
        /// </summary>
        public ISpecificationLimits SpecificationLimits { get; set; }

        /// <summary>
        /// A set of limits used to trigger warning or alarm indicators.
        /// </summary>
        public IAlarmLimits AlarmLimits { get; set; }
    }
}