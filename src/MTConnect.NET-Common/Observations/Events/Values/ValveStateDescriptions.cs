// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// The state of a valve that is one of open, closed, or transitioning between the states.
    /// </summary>
    public static class ValveStateDescriptions
    {
        /// <summary>
        /// Where flow is not possible, the aperture is static and the valve is completely shut.
        /// </summary>
        public const string CLOSED = "Where flow is not possible, the aperture is static and the valve is completely shut.";

        /// <summary>
        /// The VALVE is transitioning from an OPEN state to a CLOSED state.
        /// </summary>
        public const string CLOSING = "The VALVE is transitioning from an OPEN state to a CLOSED state.";

        /// <summary>
        /// The VALVE is transitioning from a CLOSED state to an OPEN state.
        /// </summary>
        public const string OPENING = "The VALVE is transitioning from a CLOSED state to an OPEN state.";

        /// <summary>
        /// Where flow is allowed and the aperture is static.
        /// </summary>
        public const string OPEN = "Where flow is allowed and the aperture is static.";


        public static string Get(ValveState value)
        {
            switch (value)
            {
                case ValveState.CLOSED: return CLOSED;
                case ValveState.CLOSING: return CLOSING;
                case ValveState.OPENING: return OPENING;
                case ValveState.OPEN: return OPEN;
            }

            return null;
        }
    }
}
