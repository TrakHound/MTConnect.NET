// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    public static class BatteryStateDescriptions
    {
        /// <summary>
        /// Component is at it's maximum rated charge level.
        /// </summary>
        public const string CHARGED = "Component is at it's maximum rated charge level.";
        
        /// <summary>
        /// Component's charge is increasing.
        /// </summary>
        public const string CHARGING = "Component's charge is increasing.";
        
        /// <summary>
        /// Component's charge is decreasing.
        /// </summary>
        public const string DISCHARGING = "Component's charge is decreasing.";
        
        /// <summary>
        /// Component is at it's minimum charge level.
        /// </summary>
        public const string DISCHARGED = "Component is at it's minimum charge level.";


        public static string Get(BatteryState value)
        {
            switch (value)
            {
                case BatteryState.CHARGED: return "Component is at it's maximum rated charge level.";
                case BatteryState.CHARGING: return "Component's charge is increasing.";
                case BatteryState.DISCHARGING: return "Component's charge is decreasing.";
                case BatteryState.DISCHARGED: return "Component is at it's minimum charge level.";
            }

            return null;
        }
    }
}