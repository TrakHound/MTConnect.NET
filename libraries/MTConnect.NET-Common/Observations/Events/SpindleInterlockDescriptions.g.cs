// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// Description text for each <see cref="SpindleInterlock"/> value as defined by the MTConnect Standard.
    /// </summary>
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


        /// <summary>
        /// Returns the MTConnect Standard description text for the specified <see cref="SpindleInterlock"/> value, or <c>null</c> when none is defined.
        /// </summary>
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