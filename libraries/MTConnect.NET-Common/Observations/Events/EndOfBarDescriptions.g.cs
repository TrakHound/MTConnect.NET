// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
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