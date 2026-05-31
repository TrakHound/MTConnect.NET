// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// Description text for each <see cref="ChuckInterlock"/> value as defined by the MTConnect Standard.
    /// </summary>
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


        /// <summary>
        /// Returns the MTConnect Standard description text for the specified <see cref="ChuckInterlock"/> value, or <c>null</c> when none is defined.
        /// </summary>
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