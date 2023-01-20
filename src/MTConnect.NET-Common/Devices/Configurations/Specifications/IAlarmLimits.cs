// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations.Specifications
{
    /// <summary>
    /// A set of limits used to trigger warning or alarm indicators.
    /// </summary>
    public interface IAlarmLimits
    {
        /// <summary>
        /// The upper conformance boundary for a variable.
        /// </summary>
        double? UpperLimit { get; }

        /// <summary>
        /// The upper boundary indicating increased concern and supervision may be required.
        /// </summary>
        double? UpperWarning { get; }

        /// <summary>
        /// The lower conformance boundary for a variable.
        /// </summary>
        double? LowerLimit { get; }

        /// <summary>
        /// The lower boundary indicating increased concern and supervision may be required.
        /// </summary>
        double? LowerWarning { get; }
    }
}