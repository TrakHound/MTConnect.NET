// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// Current operating mode for a Rotary type axis.
    /// </summary>
    public enum RotaryMode
    {
        /// <summary>
        /// Axis is functioning as a spindle.
        /// </summary>
        SPINDLE,
        
        /// <summary>
        /// Axis is configured to index.
        /// </summary>
        INDEX,
        
        /// <summary>
        /// Position of the axis is being interpolated.
        /// </summary>
        CONTOUR
    }
}