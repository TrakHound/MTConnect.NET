// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// Indication of whether the end of a piece of bar stock being feed by a bar feeder has been reached.
    /// </summary>
    public enum EndOfBar
    {
        /// <summary>
        /// EndOfBar has been reached.
        /// </summary>
        YES,
        
        /// <summary>
        /// EndOfBar has not been reached.
        /// </summary>
        NO
    }
}