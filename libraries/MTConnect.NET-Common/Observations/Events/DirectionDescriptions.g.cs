// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    public static class DirectionDescriptions
    {
        /// <summary>
        /// Clockwise rotation using the right-hand rule.
        /// </summary>
        public const string CLOCKWISE = "Clockwise rotation using the right-hand rule.";
        
        /// <summary>
        /// Counter-clockwise rotation using the right-hand rule.
        /// </summary>
        public const string COUNTER_CLOCKWISE = "Counter-clockwise rotation using the right-hand rule.";
        
        /// <summary>
        /// 
        /// </summary>
        public const string POSITIVE = "";
        
        /// <summary>
        /// 
        /// </summary>
        public const string NEGATIVE = "";


        public static string Get(Direction value)
        {
            switch (value)
            {
                case Direction.CLOCKWISE: return "Clockwise rotation using the right-hand rule.";
                case Direction.COUNTER_CLOCKWISE: return "Counter-clockwise rotation using the right-hand rule.";
                case Direction.POSITIVE: return "";
                case Direction.NEGATIVE: return "";
            }

            return null;
        }
    }
}