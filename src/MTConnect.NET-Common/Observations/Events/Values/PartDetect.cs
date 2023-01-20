// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// An indication designating whether a part or work piece has been detected or is present.
    /// </summary>
    public enum PartDetect
    {
        /// <summary>
        /// If a part or work piece is not detected or is not present
        /// </summary>
        NOT_PRESENT,

        /// <summary>
        /// If a part or work piece has been detected or is present.
        /// </summary>
        PRESENT
    }
}