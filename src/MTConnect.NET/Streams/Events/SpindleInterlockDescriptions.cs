// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Events
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
