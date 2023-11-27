// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    public static class CompositionStateMotionDescriptions
    {
        /// <summary>
        /// Position of the Composition is open to the point of a positive confirmation.
        /// </summary>
        public const string OPEN = "Position of the Composition is open to the point of a positive confirmation.";
        
        /// <summary>
        /// Position of the Composition is not open to thepoint of a positive confirmation and is not closed to the point of a positive confirmation. It is in an intermediate position.
        /// </summary>
        public const string UNLATCHED = "Position of the Composition is not open to thepoint of a positive confirmation and is not closed to the point of a positive confirmation. It is in an intermediate position.";
        
        /// <summary>
        /// Position of the Composition is closed to the point of a positive confirmation.
        /// </summary>
        public const string CLOSED = "Position of the Composition is closed to the point of a positive confirmation.";


        public static string Get(CompositionStateMotion value)
        {
            switch (value)
            {
                case CompositionStateMotion.OPEN: return "Position of the Composition is open to the point of a positive confirmation.";
                case CompositionStateMotion.UNLATCHED: return "Position of the Composition is not open to thepoint of a positive confirmation and is not closed to the point of a positive confirmation. It is in an intermediate position.";
                case CompositionStateMotion.CLOSED: return "Position of the Composition is closed to the point of a positive confirmation.";
            }

            return null;
        }
    }
}