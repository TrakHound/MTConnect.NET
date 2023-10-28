// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = EAID_E6F955EB_46CA_4c72_B547_8F4465A9620A

namespace MTConnect.Devices
{
    /// <summary>
    /// Organize a set of expected values that can be reported for a DataItem.
    /// </summary>
    public class Constraints : IConstraints
    {
        public const string DescriptionText = "Organize a set of expected values that can be reported for a DataItem.";


        /// <summary>
        /// Provides a means to control when an agent records updated information for a DataItem.
        /// </summary>
        public MTConnect.Devices.IFilter Filter { get; set; }
        
        /// <summary>
        /// Numeric upper constraint.If the data reported for a data item is a range of numeric values, the expected value reported **MAY** be described with an upper limit defined by this constraint.
        /// </summary>
        public double? Maximum { get; set; }
        
        /// <summary>
        /// Numeric lower constraint.If the data reported for a data item is a range of numeric values, the expected value reported **MAY** be described with a lower limit defined by this constraint.
        /// </summary>
        public double? Minimum { get; set; }
        
        /// <summary>
        /// Numeric target or expected value.
        /// </summary>
        public double? Nominal { get; set; }
        
        /// <summary>
        /// Single data value that is expected to be reported for a DataItem.Value **MUST NOT** be used in conjunction with any other Constraint elements.
        /// </summary>
        public System.Collections.Generic.IEnumerable<string> Values { get; set; }
    }
}