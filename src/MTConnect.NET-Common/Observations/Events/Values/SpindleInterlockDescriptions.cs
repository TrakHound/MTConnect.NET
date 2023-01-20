// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// An indication of the status of the spindle for a piece of equipment when power has been removed and it is free to rotate.
    /// </summary>
    public static class SpindleInterlockDescriptions
    {
        /// <summary>
        /// Spindle has not been deactivated.
        /// </summary>
        public const string INACTIVE = "Spindle has not been deactivated.";

        /// <summary>
        /// Power has been removed and the spindle cannot be operated.
        /// </summary>
        public const string ACTIVE = "Power has been removed and the spindle cannot be operated.";


        public static string Get(SpindleInterlock value)
        {
            switch (value)
            {
                case SpindleInterlock.INACTIVE: return INACTIVE;
                case SpindleInterlock.ACTIVE: return ACTIVE;
            }

            return null;
        }
    }
}