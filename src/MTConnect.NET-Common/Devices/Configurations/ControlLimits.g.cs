// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_68e0225_1605643876416_54094_959

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Set of limits that is used to indicate whether a process variable is stable and in control.
    /// </summary>
    public class ControlLimits : IControlLimits
    {
        public const string DescriptionText = "Set of limits that is used to indicate whether a process variable is stable and in control.";


        /// <summary>
        /// Lower conformance boundary for a variable.> Note: immediate concern or action may be required.
        /// </summary>
        public double LowerLimit { get; set; }
        
        /// <summary>
        /// Lower boundary indicating increased concern and supervision may be required.
        /// </summary>
        public double LowerWarning { get; set; }
        
        /// <summary>
        /// Numeric target or expected value.
        /// </summary>
        public double Nominal { get; set; }
        
        /// <summary>
        /// Upper conformance boundary for a variable.> Note: immediate concern or action may be required.
        /// </summary>
        public double UpperLimit { get; set; }
        
        /// <summary>
        /// Upper boundary indicating increased concern and supervision may be required.
        /// </summary>
        public double UpperWarning { get; set; }
    }
}