// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// State of a valve is one of open, closed, or transitioning between the states.
    /// </summary>
    public enum ValveState
    {
        /// <summary>
        /// Valvestate where flow is allowed and the aperture is static.> Note: For a binary value, `OPEN` indicates the valve has the maximum possible aperture.
        /// </summary>
        OPEN,
        
        /// <summary>
        /// Valve is transitioning from a `CLOSED` state to an `OPEN` state.
        /// </summary>
        OPENING,
        
        /// <summary>
        /// Valvestate where flow is not possible, the aperture is static, and the valve is completely shut.
        /// </summary>
        CLOSED,
        
        /// <summary>
        /// Valve is transitioning from an `OPEN` state to a `CLOSED` state.
        /// </summary>
        CLOSING
    }
}