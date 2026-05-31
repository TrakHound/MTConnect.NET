// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// Description text for each <see cref="LeakDetect"/> value as defined by the MTConnect Standard.
    /// </summary>
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


        /// <summary>
        /// Returns the MTConnect Standard description text for the specified <see cref="LeakDetect"/> value, or <c>null</c> when none is defined.
        /// </summary>
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