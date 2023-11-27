// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    public static class AxisStateDescriptions
    {
        /// <summary>
        /// Axis is in its home position.
        /// </summary>
        public const string HOME = "Axis is in its home position.";
        
        /// <summary>
        /// Axis is in motion.
        /// </summary>
        public const string TRAVEL = "Axis is in motion.";
        
        /// <summary>
        /// Axis has been moved to a fixed position and is being maintained in that position either electrically or mechanically. Action is required to release the axis from this position.
        /// </summary>
        public const string PARKED = "Axis has been moved to a fixed position and is being maintained in that position either electrically or mechanically. Action is required to release the axis from this position.";
        
        /// <summary>
        /// Axis is stopped.
        /// </summary>
        public const string STOPPED = "Axis is stopped.";


        public static string Get(AxisState value)
        {
            switch (value)
            {
                case AxisState.HOME: return "Axis is in its home position.";
                case AxisState.TRAVEL: return "Axis is in motion.";
                case AxisState.PARKED: return "Axis has been moved to a fixed position and is being maintained in that position either electrically or mechanically. Action is required to release the axis from this position.";
                case AxisState.STOPPED: return "Axis is stopped.";
            }

            return null;
        }
    }
}