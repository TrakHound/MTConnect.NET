// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// Description text for each <see cref="EquipmentMode"/> value as defined by the MTConnect Standard.
    /// </summary>
    public static class EquipmentModeDescriptions
    {
        /// <summary>
        /// Equipment is functioning in the mode designated by the `subType`.
        /// </summary>
        public const string ON = "Equipment is functioning in the mode designated by the `subType`.";
        
        /// <summary>
        /// Equipment is not functioning in the mode designated by the `subType`.
        /// </summary>
        public const string OFF = "Equipment is not functioning in the mode designated by the `subType`.";


        /// <summary>
        /// Returns the MTConnect Standard description text for the specified <see cref="EquipmentMode"/> value, or <c>null</c> when none is defined.
        /// </summary>
        public static string Get(EquipmentMode value)
        {
            switch (value)
            {
                case EquipmentMode.ON: return "Equipment is functioning in the mode designated by the `subType`.";
                case EquipmentMode.OFF: return "Equipment is not functioning in the mode designated by the `subType`.";
            }

            return null;
        }
    }
}