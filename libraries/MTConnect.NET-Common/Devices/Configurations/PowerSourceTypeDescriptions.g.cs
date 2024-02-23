// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    public static class PowerSourceTypeDescriptions
    {
        /// <summary>
        /// Main or principle.
        /// </summary>
        public const string PRIMARY = "Main or principle.";
        
        /// <summary>
        /// Alternate or not primary.
        /// </summary>
        public const string SECONDARY = "Alternate or not primary.";
        
        /// <summary>
        /// Held near at hand and ready for use and is uninterruptible.
        /// </summary>
        public const string STANDBY = "Held near at hand and ready for use and is uninterruptible.";


        public static string Get(PowerSourceType value)
        {
            switch (value)
            {
                case PowerSourceType.PRIMARY: return "Main or principle.";
                case PowerSourceType.SECONDARY: return "Alternate or not primary.";
                case PowerSourceType.STANDBY: return "Held near at hand and ready for use and is uninterruptible.";
            }

            return null;
        }
    }
}