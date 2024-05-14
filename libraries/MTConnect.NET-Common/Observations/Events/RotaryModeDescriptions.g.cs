// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    public static class RotaryModeDescriptions
    {
        /// <summary>
        /// Axis is functioning as a spindle.
        /// </summary>
        public const string SPINDLE = "Axis is functioning as a spindle.";
        
        /// <summary>
        /// Axis is configured to index.
        /// </summary>
        public const string INDEX = "Axis is configured to index.";
        
        /// <summary>
        /// Position of the axis is being interpolated.
        /// </summary>
        public const string CONTOUR = "Position of the axis is being interpolated.";


        public static string Get(RotaryMode value)
        {
            switch (value)
            {
                case RotaryMode.SPINDLE: return "Axis is functioning as a spindle.";
                case RotaryMode.INDEX: return "Axis is configured to index.";
                case RotaryMode.CONTOUR: return "Position of the axis is being interpolated.";
            }

            return null;
        }
    }
}