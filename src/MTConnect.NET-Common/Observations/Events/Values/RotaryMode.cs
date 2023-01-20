// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// The current operating mode for a Rotary type axis.
    /// </summary>
    public enum RotaryMode
    {
        NOT_SPECIFIED,

        /// <summary>
        /// The axis is functioning as a spindle. Generally, it is configured to rotate at a defined speed.
        /// </summary>
        SPINDLE,

        /// <summary>
        /// The axis is configured to index to a set of fixed positions or to incrementally index by a fixed amount
        /// </summary>
        INDEX,

        /// <summary>
        /// The position of the axis is being interpolated
        /// </summary>
        CONTOUR
    }
}