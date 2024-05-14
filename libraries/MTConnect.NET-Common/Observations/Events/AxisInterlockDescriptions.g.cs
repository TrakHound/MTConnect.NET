// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    public static class AxisInterlockDescriptions
    {
        /// <summary>
        /// Axis lockout function is activated, power has been removed from the axis, and the axis is allowed to move freely.
        /// </summary>
        public const string ACTIVE = "Axis lockout function is activated, power has been removed from the axis, and the axis is allowed to move freely.";
        
        /// <summary>
        /// Axis lockout function has not been activated, the axis may be powered, and the axis is capable of being controlled by another component.
        /// </summary>
        public const string INACTIVE = "Axis lockout function has not been activated, the axis may be powered, and the axis is capable of being controlled by another component.";


        public static string Get(AxisInterlock value)
        {
            switch (value)
            {
                case AxisInterlock.ACTIVE: return "Axis lockout function is activated, power has been removed from the axis, and the axis is allowed to move freely.";
                case AxisInterlock.INACTIVE: return "Axis lockout function has not been activated, the axis may be powered, and the axis is capable of being controlled by another component.";
            }

            return null;
        }
    }
}