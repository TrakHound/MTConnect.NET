// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// Description text for each <see cref="PowerStatus"/> value as defined by the MTConnect Standard.
    /// </summary>
    public static class PowerStatusDescriptions
    {
        /// <summary>
        /// 
        /// </summary>
        public const string ON = "";
        
        /// <summary>
        /// 
        /// </summary>
        public const string OFF = "";


        /// <summary>
        /// Returns the MTConnect Standard description text for the specified <see cref="PowerStatus"/> value, or <c>null</c> when none is defined.
        /// </summary>
        public static string Get(PowerStatus value)
        {
            switch (value)
            {
                case PowerStatus.ON: return "";
                case PowerStatus.OFF: return "";
            }

            return null;
        }
    }
}