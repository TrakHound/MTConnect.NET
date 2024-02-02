// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    public static class ChuckInterlockDescriptions
    {
        /// <summary>
        /// Chuck cannot be unclamped.
        /// </summary>
        public const string ACTIVE = "Chuck cannot be unclamped.";
        
        /// <summary>
        /// Chuck can be unclamped.
        /// </summary>
        public const string INACTIVE = "Chuck can be unclamped.";


        public static string Get(ChuckInterlock value)
        {
            switch (value)
            {
                case ChuckInterlock.ACTIVE: return "Chuck cannot be unclamped.";
                case ChuckInterlock.INACTIVE: return "Chuck can be unclamped.";
            }

            return null;
        }
    }
}