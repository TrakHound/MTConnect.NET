// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// An indication of the operating state of a mechanism that holds a part or stock material during a manufacturing process. 
    /// It may also represent a mechanism that holds any other mechanism in place within a piece of equipment.
    /// </summary>
    public static class ChuckStateDescriptions
    {
        /// <summary>
        /// The CHUCK component or composition element is open to the point of a positive confirmation
        /// </summary>
        public const string OPEN = "The CHUCK component or composition element is open to the point of a positive confirmation";

        /// <summary>
        /// The CHUCK component or composition element is not closed to the point of a positive confirmation and not open to the point of a positive confirmation.
        /// It is in an intermediate position.
        /// </summary>
        public const string UNLATCHED = "The CHUCK component or composition element is not closed to the point of a positive confirmation and not open to the point of a positive confirmation. It is in an intermediate position.";

        /// <summary>
        /// The CHUCK component or composition element is closed to the point of a positive confirmation
        /// </summary>
        public const string CLOSED = "The CHUCK component or composition element is closed to the point of a positive confirmation";


        public static string Get(ChuckState value)
        {
            switch (value)
            {
                case ChuckState.OPEN: return OPEN;
                case ChuckState.UNLATCHED: return UNLATCHED;
                case ChuckState.CLOSED: return CLOSED;
            }

            return null;
        }
    }
}
