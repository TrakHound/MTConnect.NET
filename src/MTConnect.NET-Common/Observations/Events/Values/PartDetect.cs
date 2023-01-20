// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
