// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    public enum PowerSourceType
    {
        /// <summary>
        /// Main or principle.
        /// </summary>
        PRIMARY,
        
        /// <summary>
        /// Alternate or not primary.
        /// </summary>
        SECONDARY,
        
        /// <summary>
        /// Held near at hand and ready for use and is uninterruptible.
        /// </summary>
        STANDBY
    }
}