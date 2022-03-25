// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Observations.Events.Values
{
    public static class CompositionMotionStateDescriptions
    {
        /// <summary>
        /// The position of the Composition element is open to the point of a positive confirmation
        /// </summary>
        public const string OPEN = "The position of the Composition element is open to the point of a positive confirmation";

        /// <summary>
        /// The position of the Composition element is not open to the point of a positive confirmation and is not closed to the point of a positive confirmation. 
        /// It is in an intermediate position.
        /// </summary>
        public const string UNLATCHED = "The position of the Composition element is not open to the point of a positive confirmation and is not closed to the point of a positive confirmation. It is in an intermediate position.";

        /// <summary>
        /// The position of the Composition element is closed to the point of a positive confirmation
        /// </summary>
        public const string CLOSED = "The position of the Composition element is closed to the point of a positive confirmation";


        public static string Get(CompositionMotionState value)
        {
            switch (value)
            {
                case CompositionMotionState.OPEN: return OPEN;
                case CompositionMotionState.UNLATCHED: return UNLATCHED;
                case CompositionMotionState.CLOSED: return CLOSED;
            }

            return null;
        }
    }
}
