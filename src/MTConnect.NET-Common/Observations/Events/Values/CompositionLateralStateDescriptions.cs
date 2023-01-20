// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events.Values
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