// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations.Specifications
{
    /// <summary>
    /// Specification elements define information describing the design characteristics for a piece of equipment.
    /// </summary>
    public class Specification : AbstractSpecification, ISpecification
    {
        /// <summary>
        /// A numeric upper constraint. 
        /// </summary>
        public double? Maximum { get; set; }

        /// <summary>
        /// The upper conformance boundary for a variable.
        /// </summary>
        public double? UpperLimit { get; set; }

        /// <summary>
        /// The upper boundary indicating increased concern and supervision may be required.
        /// </summary>
        public double? UpperWarning { get; set; }

        /// <summary>
        /// The ideal or desired value for a variable.
        /// </summary>
        public double? Nominal { get; set; }

        /// <summary>
        /// The lower conformance boundary for a variable.
        /// </summary>
        public double? LowerLimit { get; set; }

        /// <summary>
        /// The lower boundary indicating increased concern and supervision may be required.
        /// </summary>
        public double? LowerWarning { get; set; }

        /// <summary>
        /// A numeric lower constraint. 
        /// </summary>
        public double? Minimum { get; set; }
    }
}