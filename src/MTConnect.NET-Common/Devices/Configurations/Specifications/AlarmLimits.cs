// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Configurations.Specifications
{
    /// <summary>
    /// A set of limits used to trigger warning or alarm indicators.
    /// </summary>
    public class AlarmLimits : IAlarmLimits
    {
        public const string DescriptionText = "A set of limits used to trigger warning or alarm indicators.";


        /// <summary>
        /// The upper conformance boundary for a variable.
        /// </summary>
        public double? UpperLimit { get; set; }

        /// <summary>
        /// The upper boundary indicating increased concern and supervision may be required.
        /// </summary>
        public double? UpperWarning { get; set; }

        /// <summary>
        /// The lower conformance boundary for a variable.
        /// </summary>
        public double? LowerLimit { get; set; }

        /// <summary>
        /// The lower boundary indicating increased concern and supervision may be required.
        /// </summary>
        public double? LowerWarning { get; set; }
    }
}
