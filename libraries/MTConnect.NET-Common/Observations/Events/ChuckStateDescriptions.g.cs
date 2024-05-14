// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
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