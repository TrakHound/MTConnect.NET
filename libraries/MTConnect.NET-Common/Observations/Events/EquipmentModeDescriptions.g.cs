// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
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