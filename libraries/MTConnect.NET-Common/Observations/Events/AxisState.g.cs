// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// State of a Linear or Rotary component representing an axis.
    /// </summary>
    public enum AxisState
    {
        /// <summary>
        /// Axis is in its home position.
        /// </summary>
        HOME,
        
        /// <summary>
        /// Axis is in motion.
        /// </summary>
        TRAVEL,
        
        /// <summary>
        /// Axis has been moved to a fixed position and is being maintained in that position either electrically or mechanically. Action is required to release the axis from this position.
        /// </summary>
        PARKED,
        
        /// <summary>
        /// Axis is stopped.
        /// </summary>
        STOPPED
    }
}