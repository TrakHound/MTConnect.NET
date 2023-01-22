// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// Present status of the battery.
    /// </summary>
    public static class BatteryStateDescriptions
    {
        /// <summary>
        /// Component is at it’s maximum rated charge level.
        /// </summary>
        public const string CHARGED = "Component is at it’s maximum rated charge level.";

        /// <summary>
        /// Component's charge is increasing.
        /// </summary>
        public const string CHARGING = "Component's charge is increasing.";

        /// <summary>
        /// Component is at it’s minimum charge level.
        /// </summary>
        public const string DISCHARGED = "Component is at it’s minimum charge level.";

        /// <summary>
        /// Component's charge is decreasing.
        /// </summary>
        public const string DISCHARGING = "Component's charge is decreasing.";


        public static string Get(BatteryState value)
        {
            switch (value)
            {
                case BatteryState.CHARGED: return CHARGED;
                case BatteryState.CHARGING: return CHARGING;
                case BatteryState.DISCHARGED: return DISCHARGED;
                case BatteryState.DISCHARGING: return DISCHARGING;
            }

            return null;
        }
    }
}