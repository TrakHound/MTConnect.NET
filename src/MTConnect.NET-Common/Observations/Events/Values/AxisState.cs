// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// An indicator of the controlled state of a Linear or Rotary component representing an axis.
    /// </summary>
    public enum AxisState
    {
        /// <summary>
        /// The axis is stopped
        /// </summary>
        STOPPED,

        /// <summary>
        /// The axis has been moved to a fixed position and is being maintained in that position either electrically or mechanically. Action is required to release the axis from this position.
        /// </summary>
        PARKED,

        /// <summary>
        /// The axis is in its home position
        /// </summary>
        HOME,

        /// <summary>
        /// The Axis is in motion
        /// </summary>
        TRAVEL
    }
}