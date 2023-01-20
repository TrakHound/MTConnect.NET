// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// An indicator of the controlled state of a Linear or Rotary component representing an axis.
    /// </summary>
    public static class AxisStateDescriptions
    {
        /// <summary>
        /// The axis is stopped
        /// </summary>
        public const string STOPPED = "The axis is stopped";

        /// <summary>
        /// The axis has been moved to a fixed position and is being maintained in that position either electrically or mechanically. Action is required to release the axis from this position.
        /// </summary>
        public const string PARKED = "The axis has been moved to a fixed position and is being maintained in that position either electrically or mechanically. Action is required to release the axis from this position.";

        /// <summary>
        /// The axis is in its home position
        /// </summary>
        public const string HOME = "The axis is in its home position";

        /// <summary>
        /// The Axis is in motion
        /// </summary>
        public const string TRAVEL = "The Axis is in motion";


        public static string Get(AxisState value)
        {
            switch (value)
            {
                case AxisState.STOPPED: return STOPPED;
                case AxisState.PARKED: return PARKED;
                case AxisState.HOME: return HOME;
                case AxisState.TRAVEL: return TRAVEL;
            }

            return null;
        }
    }
}