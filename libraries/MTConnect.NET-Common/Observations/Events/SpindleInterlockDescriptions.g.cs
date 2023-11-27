// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    public static class SpindleInterlockDescriptions
    {
        /// <summary>
        /// Power has been removed and the spindle cannot be operated.
        /// </summary>
        public const string ACTIVE = "Power has been removed and the spindle cannot be operated.";
        
        /// <summary>
        /// Spindle has not been deactivated.
        /// </summary>
        public const string INACTIVE = "Spindle has not been deactivated.";


        public static string Get(SpindleInterlock value)
        {
            switch (value)
            {
                case SpindleInterlock.ACTIVE: return "Power has been removed and the spindle cannot be operated.";
                case SpindleInterlock.INACTIVE: return "Spindle has not been deactivated.";
            }

            return null;
        }
    }
}