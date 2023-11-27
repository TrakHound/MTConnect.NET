// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    public static class NetworkWirelessDescriptions
    {
        /// <summary>
        /// 
        /// </summary>
        public const string YES = "";
        
        /// <summary>
        /// 
        /// </summary>
        public const string NO = "";


        public static string Get(NetworkWireless value)
        {
            switch (value)
            {
                case NetworkWireless.YES: return "";
                case NetworkWireless.NO: return "";
            }

            return null;
        }
    }
}