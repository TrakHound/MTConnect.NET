// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_68e0225_1605643801279_773776_859

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Specification that provides information used to assess the conformance of a variable to process requirements.
    /// </summary>
    public class ProcessSpecification : Specification, IProcessSpecification
    {
        public new const string DescriptionText = "Specification that provides information used to assess the conformance of a variable to process requirements.";


        /// <summary>
        /// Set of limits that is used to trigger warning or alarm indicators.
        /// </summary>
        public MTConnect.Devices.Configurations.IAlarmLimits AlarmLimits { get; set; }
        
        /// <summary>
        /// Set of limits that is used to indicate whether a process variable is stable and in control.
        /// </summary>
        public MTConnect.Devices.Configurations.IControlLimits ControlLimits { get; set; }
        
        /// <summary>
        /// Set of limits that define a range of values designating acceptable performance for a variable.
        /// </summary>
        public MTConnect.Devices.Configurations.ISpecificationLimits SpecificationLimits { get; set; }
    }
}