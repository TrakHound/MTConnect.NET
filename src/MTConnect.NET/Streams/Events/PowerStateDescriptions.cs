// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Events
{
    /// <summary>
    /// The indication of the status of the source of energy for a Structural Element to allow it to perform
    /// its intended function or the state of an enabling signal providing permission for the Structural Element to perform its functions.
    /// </summary>
    public static class PowerStateDescriptions
    {
        /// <summary>
        /// The source of energy for a Structural Element or the enabling signal providing permission for the Structural Element to perform its function(s) is not present or is disconnected.
        /// </summary>
        public const string OFF = "The source of energy for a Structural Element or the enabling signal providing permission for the Structural Element to perform its function(s) is not present or is disconnected.";

        /// <summary>
        /// The source of energy for a Structural Element or the enabling signal providing permission for the Structural Element to perform its function(s) is present and active.
        /// </summary>
        public const string ON = "The source of energy for a Structural Element or the enabling signal providing permission for the Structural Element to perform its function(s) is present and active.";


        public static string Get(PowerState value)
        {
            switch (value)
            {
                case PowerState.OFF: return OFF;
                case PowerState.ON: return ON;
            }

            return null;
        }
    }
}
