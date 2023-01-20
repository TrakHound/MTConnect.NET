// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// The current operating mode for a Rotary type axis.
    /// </summary>
    public static class RotaryModeDescriptions
    {
        /// <summary>
        /// The axis is functioning as a spindle. Generally, it is configured to rotate at a defined speed.
        /// </summary>
        public const string SPINDLE = "The axis is functioning as a spindle. Generally, it is configured to rotate at a defined speed.";

        /// <summary>
        /// The axis is configured to index to a set of fixed positions or to incrementally index by a fixed amount.
        /// </summary>
        public const string INDEX = "The axis is configured to index to a set of fixed positions or to incrementally index by a fixed amount.";

        /// <summary>
        /// The position of the axis is being interpolated.
        /// </summary>
        public const string CONTOUR = "The position of the axis is being interpolated.";


        public static string Get(RotaryMode value)
        {
            switch (value)
            {
                case RotaryMode.SPINDLE: return SPINDLE;
                case RotaryMode.INDEX: return INDEX;
                case RotaryMode.CONTOUR: return CONTOUR;
            }

            return null;
        }
    }
}
