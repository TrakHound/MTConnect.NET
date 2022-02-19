// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Events
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
