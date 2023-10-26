// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    public static class LockStateDescriptions
    {
        /// <summary>
        /// Mechanism is engaged and preventing the associated Component from being opened or operated.
        /// </summary>
        public const string LOCKED = "Mechanism is engaged and preventing the associated Component from being opened or operated.";
        
        /// <summary>
        /// Mechanism is disengaged and the associated Component is able to be opened or operated.
        /// </summary>
        public const string UNLOCKED = "Mechanism is disengaged and the associated Component is able to be opened or operated.";


        public static string Get(LockState value)
        {
            switch (value)
            {
                case LockState.LOCKED: return "Mechanism is engaged and preventing the associated Component from being opened or operated.";
                case LockState.UNLOCKED: return "Mechanism is disengaged and the associated Component is able to be opened or operated.";
            }

            return null;
        }
    }
}