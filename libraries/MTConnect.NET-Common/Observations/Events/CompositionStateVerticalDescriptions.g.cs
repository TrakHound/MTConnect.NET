// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// Description text for each <see cref="CompositionStateVertical"/> value as defined by the MTConnect Standard.
    /// </summary>
    public static class CompositionStateVerticalDescriptions
    {
        /// <summary>
        /// Position of the Composition element is oriented in a downward direction to the point of a positive confirmation.
        /// </summary>
        public const string DOWN = "Position of the Composition element is oriented in a downward direction to the point of a positive confirmation.";
        
        /// <summary>
        /// Position of the Composition element is not oriented in an upward direction to the point of a positive confirmation and is not oriented in a downward direction to the point of a positive confirmation. It is in an intermediate position.
        /// </summary>
        public const string TRANSITIONING = "Position of the Composition element is not oriented in an upward direction to the point of a positive confirmation and is not oriented in a downward direction to the point of a positive confirmation. It is in an intermediate position.";
        
        /// <summary>
        /// Position of the Composition element is oriented in an upward direction to the point of a positive confirmation.
        /// </summary>
        public const string UP = "Position of the Composition element is oriented in an upward direction to the point of a positive confirmation.";


        /// <summary>
        /// Returns the MTConnect Standard description text for the specified <see cref="CompositionStateVertical"/> value, or <c>null</c> when none is defined.
        /// </summary>
        public static string Get(CompositionStateVertical value)
        {
            switch (value)
            {
                case CompositionStateVertical.DOWN: return "Position of the Composition element is oriented in a downward direction to the point of a positive confirmation.";
                case CompositionStateVertical.TRANSITIONING: return "Position of the Composition element is not oriented in an upward direction to the point of a positive confirmation and is not oriented in a downward direction to the point of a positive confirmation. It is in an intermediate position.";
                case CompositionStateVertical.UP: return "Position of the Composition element is oriented in an upward direction to the point of a positive confirmation.";
            }

            return null;
        }
    }
}