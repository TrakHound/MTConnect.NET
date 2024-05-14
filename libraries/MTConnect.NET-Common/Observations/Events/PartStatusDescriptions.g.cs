// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    public static class PartStatusDescriptions
    {
        /// <summary>
        /// Part conforms to given requirements.
        /// </summary>
        public const string PASS = "Part conforms to given requirements.";
        
        /// <summary>
        /// Part does not conform to some given requirements.
        /// </summary>
        public const string FAIL = "Part does not conform to some given requirements.";


        public static string Get(PartStatus value)
        {
            switch (value)
            {
                case PartStatus.PASS: return "Part conforms to given requirements.";
                case PartStatus.FAIL: return "Part does not conform to some given requirements.";
            }

            return null;
        }
    }
}