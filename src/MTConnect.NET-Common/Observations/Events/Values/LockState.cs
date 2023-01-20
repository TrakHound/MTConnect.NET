// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// The state or operating mode of a Lock.
    /// </summary>
    public enum LockState
    {
        /// <summary>
        /// The mechanism is disengaged and the associated component is able to be opened or operated.
        /// </summary>
        UNLOCKED,

        /// <summary>
        /// The mechanism is engaged and preventing the associated component from being opened or operated.
        /// </summary>
        LOCKED
    }
}
