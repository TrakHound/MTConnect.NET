// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1605643893577_939623_1049

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Set of limits that is used to trigger warning or alarm indicators.
    /// </summary>
    public class AlarmLimits : IAlarmLimits
    {
        public const string DescriptionText = "Set of limits that is used to trigger warning or alarm indicators.";


        /// <summary>
        /// Lower conformance boundary for a variable.> Note: immediate concern or action may be required.
        /// </summary>
        public double? LowerLimit { get; set; }
        
        /// <summary>
        /// Lower boundary indicating increased concern and supervision may be required.
        /// </summary>
        public double? LowerWarning { get; set; }
        
        /// <summary>
        /// Upper conformance boundary for a variable.> Note: immediate concern or action may be required.
        /// </summary>
        public double? UpperLimit { get; set; }
        
        /// <summary>
        /// Upper boundary indicating increased concern and supervision may be required.
        /// </summary>
        public double? UpperWarning { get; set; }
    }
}