// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// State or operating mode of a Lock.
    /// </summary>
    public enum LockState
    {
        /// <summary>
        /// Mechanism is engaged and preventing the associated Component from being opened or operated.
        /// </summary>
        LOCKED,
        
        /// <summary>
        /// Mechanism is disengaged and the associated Component is able to be opened or operated.
        /// </summary>
        UNLOCKED
    }
}