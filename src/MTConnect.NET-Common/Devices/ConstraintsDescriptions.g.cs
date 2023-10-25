// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    public static class ConstraintsDescriptions
    {
        /// <summary>
        /// Provides a means to control when an agent records updated information for a DataItem.
        /// </summary>
        public const string Filter = "Provides a means to control when an agent records updated information for a DataItem.";
        
        /// <summary>
        /// Numeric upper constraint.If the data reported for a data item is a range of numeric values, the expected value reported **MAY** be described with an upper limit defined by this constraint.
        /// </summary>
        public const string Maximum = "Numeric upper constraint.If the data reported for a data item is a range of numeric values, the expected value reported **MAY** be described with an upper limit defined by this constraint.";
        
        /// <summary>
        /// Numeric lower constraint.If the data reported for a data item is a range of numeric values, the expected value reported **MAY** be described with a lower limit defined by this constraint.
        /// </summary>
        public const string Minimum = "Numeric lower constraint.If the data reported for a data item is a range of numeric values, the expected value reported **MAY** be described with a lower limit defined by this constraint.";
        
        /// <summary>
        /// Numeric target or expected value.
        /// </summary>
        public const string Nominal = "Numeric target or expected value.";
        
        /// <summary>
        /// Single data value that is expected to be reported for a DataItem.Value **MUST NOT** be used in conjunction with any other Constraint elements.
        /// </summary>
        public const string Value = "Single data value that is expected to be reported for a DataItem.Value **MUST NOT** be used in conjunction with any other Constraint elements.";
    }
}