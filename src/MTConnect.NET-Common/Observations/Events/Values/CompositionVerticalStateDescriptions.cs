// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events.Values
{
    public static class CompositionVerticalStateDescriptions
    {
        /// <summary>
        /// The position of the Composition element is oriented in a downward direction to the point of a positive confirmation
        /// </summary>v
        public const string DOWN = "The position of the Composition element is oriented in a downward direction to the point of a positive confirmation";

        /// <summary>
        /// The position of the Composition element is oriented in an upward direction to the point of a positive confirmation
        /// </summary>
        public const string UP = "The position of the Composition element is oriented in an upward direction to the point of a positive confirmation";

        /// <summary>
        /// The position of the Composition element is not oriented in an upward direction to the point of a positive confirmation and is not oriented in a downward direction to the point of a positive confirmation. 
        /// It is in an intermediate position.
        /// </summary>
        public const string TRANSITIONING = "The position of the Composition element is not oriented in an upward direction to the point of a positive confirmation and is not oriented in a downward direction to the point of a positive confirmation. It is in an intermediate position.";


        public static string Get(CompositionVerticalState value)
        {
            switch (value)
            {
                case CompositionVerticalState.DOWN: return DOWN;
                case CompositionVerticalState.UP: return UP;
                case CompositionVerticalState.TRANSITIONING: return TRANSITIONING;
            }

            return null;
        }
    }
}