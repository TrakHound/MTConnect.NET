// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools
{
    public static class CountDirectionTypeDescriptions
    {
        /// <summary>
        /// Tool life counts down from the maximum to zero.
        /// </summary>
        public const string DOWN = "Tool life counts down from the maximum to zero.";
        
        /// <summary>
        /// Tool life counts up from zero to the maximum.
        /// </summary>
        public const string UP = "Tool life counts up from zero to the maximum.";


        public static string Get(CountDirectionType value)
        {
            switch (value)
            {
                case CountDirectionType.DOWN: return "Tool life counts down from the maximum to zero.";
                case CountDirectionType.UP: return "Tool life counts up from zero to the maximum.";
            }

            return null;
        }
    }
}