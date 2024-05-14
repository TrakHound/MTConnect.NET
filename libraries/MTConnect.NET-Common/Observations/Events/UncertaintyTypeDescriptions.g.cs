// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    public static class UncertaintyTypeDescriptions
    {
        /// <summary>
        /// Combined standard uncertainty.
        /// </summary>
        public const string COMBINED = "Combined standard uncertainty.";
        
        /// <summary>
        /// Standard uncertainty using arithmetic mean or average the observations. JCGM 100:2008 4.2
        /// </summary>
        public const string MEAN = "Standard uncertainty using arithmetic mean or average the observations. JCGM 100:2008 4.2";


        public static string Get(UncertaintyType value)
        {
            switch (value)
            {
                case UncertaintyType.COMBINED: return "Combined standard uncertainty.";
                case UncertaintyType.MEAN: return "Standard uncertainty using arithmetic mean or average the observations. JCGM 100:2008 4.2";
            }

            return null;
        }
    }
}