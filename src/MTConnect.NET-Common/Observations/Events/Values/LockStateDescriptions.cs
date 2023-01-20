// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// The state or operating mode of a Lock.
    /// </summary>
    public static class LockStateDescriptions
    {
        /// <summary>
        /// The mechanism is disengaged and the associated component is able to be opened or operated.
        /// </summary>
        public const string UNLOCKED = "The mechanism is disengaged and the associated component is able to be opened or operated.";

        /// <summary>
        /// The mechanism is engaged and preventing the associated component from being opened or operated.
        /// </summary>
        public const string LOCKED = "The mechanism is engaged and preventing the associated component from being opened or operated.";


        public static string Get(LockState value)
        {
            switch (value)
            {
                case LockState.UNLOCKED: return UNLOCKED;
                case LockState.LOCKED: return LOCKED;
            }

            return null;
        }
    }
}