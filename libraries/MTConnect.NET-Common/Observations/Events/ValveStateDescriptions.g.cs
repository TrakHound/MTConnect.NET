// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    public static class ValveStateDescriptions
    {
        /// <summary>
        /// ValveState where flow is allowed and the aperture is static.> Note: For a binary value, `OPEN` indicates the valve has the maximum possible aperture.
        /// </summary>
        public const string OPEN = "ValveState where flow is allowed and the aperture is static.> Note: For a binary value, `OPEN` indicates the valve has the maximum possible aperture.";
        
        /// <summary>
        /// Valve is transitioning from a `CLOSED` state to an `OPEN` state.
        /// </summary>
        public const string OPENING = "Valve is transitioning from a `CLOSED` state to an `OPEN` state.";
        
        /// <summary>
        /// ValveState where flow is not possible, the aperture is static, and the valve is completely shut.
        /// </summary>
        public const string CLOSED = "ValveState where flow is not possible, the aperture is static, and the valve is completely shut.";
        
        /// <summary>
        /// Valve is transitioning from an `OPEN` state to a `CLOSED` state.
        /// </summary>
        public const string CLOSING = "Valve is transitioning from an `OPEN` state to a `CLOSED` state.";


        public static string Get(ValveState value)
        {
            switch (value)
            {
                case ValveState.OPEN: return "ValveState where flow is allowed and the aperture is static.> Note: For a binary value, `OPEN` indicates the valve has the maximum possible aperture.";
                case ValveState.OPENING: return "Valve is transitioning from a `CLOSED` state to an `OPEN` state.";
                case ValveState.CLOSED: return "ValveState where flow is not possible, the aperture is static, and the valve is completely shut.";
                case ValveState.CLOSING: return "Valve is transitioning from an `OPEN` state to a `CLOSED` state.";
            }

            return null;
        }
    }
}