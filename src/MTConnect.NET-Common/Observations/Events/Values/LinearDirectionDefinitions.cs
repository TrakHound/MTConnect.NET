// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// The direction of motion.
    /// </summary>
    public static class LinearDirectionDescriptions
    {
        /// <summary>
        /// No direction
        /// </summary>
        public const string NONE = "No direction";

        /// <summary>
        /// Linear position is decreasing.
        /// </summary>
        public const string NEGATIVE = "Linear position is decreasing.";

        /// <summary>
        /// Linear position is increasing.
        /// </summary>
        public const string POSITIVE = "Linear position is increasing.";


        public static string Get(LinearDirection value)
        {
            switch (value)
            {
                case LinearDirection.NONE: return NONE;
                case LinearDirection.NEGATIVE: return NEGATIVE;
                case LinearDirection.POSITIVE: return POSITIVE;
            }

            return null;
        }
    }
}