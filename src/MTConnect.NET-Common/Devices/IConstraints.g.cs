// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    /// <summary>
    /// Organize a set of expected values that can be reported for a DataItem.
    /// </summary>
    public interface IConstraints
    {
        /// <summary>
        /// Provides a means to control when an agent records updated information for a DataItem.
        /// </summary>
        MTConnect.Devices.IFilter Filter { get; }
        
        /// <summary>
        /// Numeric upper constraint.If the data reported for a data item is a range of numeric values, the expected value reported **MAY** be described with an upper limit defined by this constraint.
        /// </summary>
        double? Maximum { get; }
        
        /// <summary>
        /// Numeric lower constraint.If the data reported for a data item is a range of numeric values, the expected value reported **MAY** be described with a lower limit defined by this constraint.
        /// </summary>
        double? Minimum { get; }
        
        /// <summary>
        /// Numeric target or expected value.
        /// </summary>
        double? Nominal { get; }
        
        /// <summary>
        /// Single data value that is expected to be reported for a DataItem.Value **MUST NOT** be used in conjunction with any other Constraint elements.
        /// </summary>
        System.Collections.Generic.IEnumerable<string> Values { get; }
    }
}