// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// The state of a valve that is one of open, closed, or transitioning between the states.
    /// </summary>
    public enum ValveState
    {
        /// <summary>
        /// Where flow is not possible, the aperture is static and the valve is completely shut.
        /// </summary>
        CLOSED,

        /// <summary>
        /// The VALVE is transitioning from an OPEN state to a CLOSED state.
        /// </summary>
        CLOSING,

        /// <summary>
        /// The VALVE is transitioning from a CLOSED state to an OPEN state.
        /// </summary>
        OPENING,

        /// <summary>
        /// where flow is allowed and the aperture is static
        /// </summary>
        OPEN
    }
}