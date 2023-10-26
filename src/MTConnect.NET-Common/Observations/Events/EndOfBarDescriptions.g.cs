// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    public static class EndOfBarDescriptions
    {
        /// <summary>
        /// Endofbar has been reached.
        /// </summary>
        public const string YES = "Endofbar has been reached.";
        
        /// <summary>
        /// Endofbar has not been reached.
        /// </summary>
        public const string NO = "Endofbar has not been reached.";


        public static string Get(EndOfBar value)
        {
            switch (value)
            {
                case EndOfBar.YES: return "Endofbar has been reached.";
                case EndOfBar.NO: return "Endofbar has not been reached.";
            }

            return null;
        }
    }
}