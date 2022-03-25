// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices
{
    /// <summary>
    /// Specification elements define information describing the design characteristics for a piece of equipment.
    /// </summary>
    public interface ISpecification : IAbstractSpecification
    {
        /// <summary>
        /// A numeric upper constraint. 
        /// </summary>
        double? Maximum { get; }

        /// <summary>
        /// The upper conformance boundary for a variable.
        /// </summary>
        double? UpperLimit { get; }

        /// <summary>
        /// The upper boundary indicating increased concern and supervision may be required.
        /// </summary>
        double? UpperWarning { get; }

        /// <summary>
        /// The ideal or desired value for a variable.
        /// </summary>
        double? Nominal { get; }

        /// <summary>
        /// The lower conformance boundary for a variable.
        /// </summary>
        double? LowerLimit { get; }

        /// <summary>
        /// The lower boundary indicating increased concern and supervision may be required.
        /// </summary>
        double? LowerWarning { get; }

        /// <summary>
        /// A numeric lower constraint. 
        /// </summary>
        double? Minimum { get; }
    }
}
