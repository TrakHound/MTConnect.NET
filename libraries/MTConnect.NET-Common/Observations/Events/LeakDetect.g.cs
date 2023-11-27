// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// Indication designating whether a leak has been detected.
    /// </summary>
    public enum LeakDetect
    {
        /// <summary>
        /// Leak is currently being detected.
        /// </summary>
        DETECTED,
        
        /// <summary>
        /// Leak is currently not being detected.
        /// </summary>
        NOT_DETECTED
    }
}