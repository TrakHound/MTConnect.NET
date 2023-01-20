// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// The operational state of a DOOR type component or composition element.
    /// </summary>
    public static class DoorStateDescriptions
    {
        /// <summary>
        /// The DOOR is open to the point of a positive confirmation
        /// </summary>
        public const string OPEN = "The DOOR is open to the point of a positive confirmation";

        /// <summary>
        /// The DOOR is not closed to the point of a positive confirmation and is not open to the point of a positive confirmation.
        /// It is in an intermediate position.
        /// </summary>
        public const string UNLATCHED = "The DOOR is not closed to the point of a positive confirmation and is not open to the point of a positive confirmation. It is in an intermediate position.";

        /// <summary>
        /// The DOOR is closed to the point of a positive confirmation
        /// </summary>
        public const string CLOSED = "The DOOR is closed to the point of a positive confirmation";


        public static string Get(DoorState value)
        {
            switch (value)
            {
                case DoorState.OPEN: return OPEN;
                case DoorState.UNLATCHED: return UNLATCHED;
                case DoorState.CLOSED: return CLOSED;
            }

            return null;
        }
    }
}