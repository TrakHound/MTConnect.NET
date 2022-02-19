// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Events
{
    public static class CompositionLateralStateDescriptions
    {
        /// <summary>
        /// The position of the Composition element is oriented to the left to the point of a positive confirmation
        /// </summary>
        public const string LEFT = "The position of the Composition element is oriented to the left to the point of a positive confirmation";

        /// <summary>
        /// The position of the Composition element is oriented to the right to the point of a positive confirmation
        /// </summary>
        public const string RIGHT = "The position of the Composition element is oriented to the right to the point of a positive confirmation";

        /// <summary>
        /// The position of the Composition element is not oriented to the right to the point of a positive confirmation and is not oriented to the left to the point of a positive confirmation.
        /// It is in an intermediate position.
        /// </summary>
        public const string TRANSITIONING = "The position of the Composition element is not oriented to the right to the point of a positive confirmation and is not oriented to the left to the point of a positive confirmation. It is in an intermediate position.";


        public static string Get(CompositionLateralState value)
        {
            switch (value)
            {
                case CompositionLateralState.LEFT: return LEFT;
                case CompositionLateralState.RIGHT: return RIGHT;
                case CompositionLateralState.TRANSITIONING: return TRANSITIONING;
            }

            return null;
        }
    }
}
