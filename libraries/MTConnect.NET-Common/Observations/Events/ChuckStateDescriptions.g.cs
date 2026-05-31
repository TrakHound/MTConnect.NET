// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// Description text for each <see cref="ChuckState"/> value as defined by the MTConnect Standard.
    /// </summary>
    public static class ChuckStateDescriptions
    {
        /// <summary>
        /// Chuck is open to the point of a positive confirmation.
        /// </summary>
        public const string OPEN = "Chuck is open to the point of a positive confirmation.";
        
        /// <summary>
        /// Chuck is closed to the point of a positive confirmation.
        /// </summary>
        public const string CLOSED = "Chuck is closed to the point of a positive confirmation.";
        
        /// <summary>
        /// Chuck is not closed to the point of a positive confirmation and not open to the point of a positive confirmation. It is in an intermediate position.
        /// </summary>
        public const string UNLATCHED = "Chuck is not closed to the point of a positive confirmation and not open to the point of a positive confirmation. It is in an intermediate position.";


        /// <summary>
        /// Returns the MTConnect Standard description text for the specified <see cref="ChuckState"/> value, or <c>null</c> when none is defined.
        /// </summary>
        public static string Get(ChuckState value)
        {
            switch (value)
            {
                case ChuckState.OPEN: return "Chuck is open to the point of a positive confirmation.";
                case ChuckState.CLOSED: return "Chuck is closed to the point of a positive confirmation.";
                case ChuckState.UNLATCHED: return "Chuck is not closed to the point of a positive confirmation and not open to the point of a positive confirmation. It is in an intermediate position.";
            }

            return null;
        }
    }
}