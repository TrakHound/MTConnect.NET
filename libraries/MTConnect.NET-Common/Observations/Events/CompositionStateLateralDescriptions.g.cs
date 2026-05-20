// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// Description text for each <see cref="CompositionStateLateral"/> value as defined by the MTConnect Standard.
    /// </summary>
    public static class CompositionStateLateralDescriptions
    {
        /// <summary>
        /// Position of the Composition is oriented to the left to the point of a positive confirmation.
        /// </summary>
        public const string LEFT = "Position of the Composition is oriented to the left to the point of a positive confirmation.";
        
        /// <summary>
        /// Position of the Composition is oriented to the right to the point of a positive confirmation.
        /// </summary>
        public const string RIGHT = "Position of the Composition is oriented to the right to the point of a positive confirmation.";
        
        /// <summary>
        /// Position of the Composition is not oriented to the right to the point of a positive confirmation and is not oriented to the left to the point of a positive confirmation. It is in an intermediate position.
        /// </summary>
        public const string TRANSITIONING = "Position of the Composition is not oriented to the right to the point of a positive confirmation and is not oriented to the left to the point of a positive confirmation. It is in an intermediate position.";


        /// <summary>
        /// Returns the MTConnect Standard description text for the specified <see cref="CompositionStateLateral"/> value, or <c>null</c> when none is defined.
        /// </summary>
        public static string Get(CompositionStateLateral value)
        {
            switch (value)
            {
                case CompositionStateLateral.LEFT: return "Position of the Composition is oriented to the left to the point of a positive confirmation.";
                case CompositionStateLateral.RIGHT: return "Position of the Composition is oriented to the right to the point of a positive confirmation.";
                case CompositionStateLateral.TRANSITIONING: return "Position of the Composition is not oriented to the right to the point of a positive confirmation and is not oriented to the left to the point of a positive confirmation. It is in an intermediate position.";
            }

            return null;
        }
    }
}