// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// Description text for each <see cref="EndOfBar"/> value as defined by the MTConnect Standard.
    /// </summary>
    public static class EndOfBarDescriptions
    {
        /// <summary>
        /// EndOfBar has been reached.
        /// </summary>
        public const string YES = "EndOfBar has been reached.";
        
        /// <summary>
        /// EndOfBar has not been reached.
        /// </summary>
        public const string NO = "EndOfBar has not been reached.";


        /// <summary>
        /// Returns the MTConnect Standard description text for the specified <see cref="EndOfBar"/> value, or <c>null</c> when none is defined.
        /// </summary>
        public static string Get(EndOfBar value)
        {
            switch (value)
            {
                case EndOfBar.YES: return "EndOfBar has been reached.";
                case EndOfBar.NO: return "EndOfBar has not been reached.";
            }

            return null;
        }
    }
}