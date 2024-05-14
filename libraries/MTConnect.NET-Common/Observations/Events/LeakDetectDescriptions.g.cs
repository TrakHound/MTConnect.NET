// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    public static class LeakDetectDescriptions
    {
        /// <summary>
        /// Leak is currently being detected.
        /// </summary>
        public const string DETECTED = "Leak is currently being detected.";
        
        /// <summary>
        /// Leak is currently not being detected.
        /// </summary>
        public const string NOT_DETECTED = "Leak is currently not being detected.";


        public static string Get(LeakDetect value)
        {
            switch (value)
            {
                case LeakDetect.DETECTED: return "Leak is currently being detected.";
                case LeakDetect.NOT_DETECTED: return "Leak is currently not being detected.";
            }

            return null;
        }
    }
}