// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_68e0225_1605643883082_895051_1004

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Set of limits that define a range of values designating acceptable performance for a variable.
    /// </summary>
    public class SpecificationLimits : ISpecificationLimits
    {
        public const string DescriptionText = "Set of limits that define a range of values designating acceptable performance for a variable.";


        /// <summary>
        /// Lower conformance boundary for a variable.> Note: immediate concern or action may be required.
        /// </summary>
        public double? LowerLimit { get; set; }
        
        /// <summary>
        /// Numeric target or expected value.
        /// </summary>
        public double? Nominal { get; set; }
        
        /// <summary>
        /// Upper conformance boundary for a variable.> Note: immediate concern or action may be required.
        /// </summary>
        public double? UpperLimit { get; set; }
    }
}