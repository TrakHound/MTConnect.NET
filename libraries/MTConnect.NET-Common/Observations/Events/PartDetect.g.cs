// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// Indication designating whether a part or work piece has been detected or is present.
    /// </summary>
    public enum PartDetect
    {
        /// <summary>
        /// Part or work piece is detected or is present.
        /// </summary>
        PRESENT,
        
        /// <summary>
        /// Part or work piece is not detected or is not present.
        /// </summary>
        NOT_PRESENT
    }
}