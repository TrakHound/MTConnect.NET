// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// Describes the way the axes will be associated to each other.   This is used in conjunction with `COUPLED_AXES` to indicate the way they are interacting.
    /// </summary>
    public enum AxisCoupling
    {
        /// <summary>
        /// Axes are physically connected to each other and operate as a single unit.
        /// </summary>
        TANDEM,
        
        /// <summary>
        /// Axes are not physically connected to each other but are operating together in lockstep.
        /// </summary>
        SYNCHRONOUS,
        
        /// <summary>
        /// Axis is the master of the CoupledAxes.
        /// </summary>
        MASTER,
        
        /// <summary>
        /// Axis is a slave to the CoupledAxes.
        /// </summary>
        SLAVE
    }
}