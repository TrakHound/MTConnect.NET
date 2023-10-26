// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
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