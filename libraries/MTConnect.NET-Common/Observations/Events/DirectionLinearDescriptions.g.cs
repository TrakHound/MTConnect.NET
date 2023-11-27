// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    public static class DirectionLinearDescriptions
    {
        /// <summary>
        /// Linear position is increasing.
        /// </summary>
        public const string POSITIVE = "Linear position is increasing.";
        
        /// <summary>
        /// Linear position is decreasing.
        /// </summary>
        public const string NEGATIVE = "Linear position is decreasing.";
        
        /// <summary>
        /// No direction.
        /// </summary>
        public const string NONE = "No direction.";


        public static string Get(DirectionLinear value)
        {
            switch (value)
            {
                case DirectionLinear.POSITIVE: return "Linear position is increasing.";
                case DirectionLinear.NEGATIVE: return "Linear position is decreasing.";
                case DirectionLinear.NONE: return "No direction.";
            }

            return null;
        }
    }
}