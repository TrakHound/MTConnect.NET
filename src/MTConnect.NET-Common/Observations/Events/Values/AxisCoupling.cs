// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// Describes the way the axes will be associated to each other.
    /// </summary>
    public enum AxisCoupling
    {
        /// <summary>
        /// The axes are physically connected to each other and operate as a single unit.
        /// </summary>
        TANDEM,

        /// <summary>
        /// The axes are not physically connected to each other but are operating together in lockstep.
        /// </summary>
        SYNCHRONOUS,

        /// <summary>
        /// The axis is the master of the CoupledAxes
        /// </summary>
        MASTER,

        /// <summary>
        /// The axis is a slave to the CoupledAxes
        /// </summary>
        SLAVE
    }
}