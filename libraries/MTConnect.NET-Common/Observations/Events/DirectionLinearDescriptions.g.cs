// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// Description text for each <see cref="DirectionLinear"/> value as defined by the MTConnect Standard.
    /// </summary>
    public static class DirectionLinearDescriptions
    {
        /// <summary>
        /// Linear position is decreasing.
        /// </summary>
        public const string NEGATIVE = "Linear position is decreasing.";
        
        /// <summary>
        /// No direction.
        /// </summary>
        public const string NONE = "No direction.";
        
        /// <summary>
        /// Linear position is increasing.
        /// </summary>
        public const string POSITIVE = "Linear position is increasing.";


        /// <summary>
        /// Returns the MTConnect Standard description text for the specified <see cref="DirectionLinear"/> value, or <c>null</c> when none is defined.
        /// </summary>
        public static string Get(DirectionLinear value)
        {
            switch (value)
            {
                case DirectionLinear.NEGATIVE: return "Linear position is decreasing.";
                case DirectionLinear.NONE: return "No direction.";
                case DirectionLinear.POSITIVE: return "Linear position is increasing.";
            }

            return null;
        }
    }
}