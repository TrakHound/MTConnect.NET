// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    public static class CompositionStateLateralDescriptions
    {
        /// <summary>
        /// Position of the Composition is oriented to the right to the point of a positive confirmation.
        /// </summary>
        public const string RIGHT = "Position of the Composition is oriented to the right to the point of a positive confirmation.";
        
        /// <summary>
        /// Position of the Composition is oriented to the left to the point of a positive confirmation.
        /// </summary>
        public const string LEFT = "Position of the Composition is oriented to the left to the point of a positive confirmation.";
        
        /// <summary>
        /// Position of the Composition is not oriented to the right to the point of a positive confirmation and is not oriented to the left to the point of a positive confirmation. It is in an intermediate position.
        /// </summary>
        public const string TRANSITIONING = "Position of the Composition is not oriented to the right to the point of a positive confirmation and is not oriented to the left to the point of a positive confirmation. It is in an intermediate position.";


        public static string Get(CompositionStateLateral value)
        {
            switch (value)
            {
                case CompositionStateLateral.RIGHT: return "Position of the Composition is oriented to the right to the point of a positive confirmation.";
                case CompositionStateLateral.LEFT: return "Position of the Composition is oriented to the left to the point of a positive confirmation.";
                case CompositionStateLateral.TRANSITIONING: return "Position of the Composition is not oriented to the right to the point of a positive confirmation and is not oriented to the left to the point of a positive confirmation. It is in an intermediate position.";
            }

            return null;
        }
    }
}