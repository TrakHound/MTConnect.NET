// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// Description text for each <see cref="DoorState"/> value as defined by the MTConnect Standard.
    /// </summary>
    public static class DoorStateDescriptions
    {
        /// <summary>
        /// Door is open to the point of a positive confirmation.
        /// </summary>
        public const string OPEN = "Door is open to the point of a positive confirmation.";
        
        /// <summary>
        /// Door is closed to the point of a positive confirmation.
        /// </summary>
        public const string CLOSED = "Door is closed to the point of a positive confirmation.";
        
        /// <summary>
        /// Door is not closed to the point of a positive confirmation and not open to the point of a positive confirmation. It is in an intermediate position.
        /// </summary>
        public const string UNLATCHED = "Door is not closed to the point of a positive confirmation and not open to the point of a positive confirmation. It is in an intermediate position.";


        /// <summary>
        /// Returns the MTConnect Standard description text for the specified <see cref="DoorState"/> value, or <c>null</c> when none is defined.
        /// </summary>
        public static string Get(DoorState value)
        {
            switch (value)
            {
                case DoorState.OPEN: return "Door is open to the point of a positive confirmation.";
                case DoorState.CLOSED: return "Door is closed to the point of a positive confirmation.";
                case DoorState.UNLATCHED: return "Door is not closed to the point of a positive confirmation and not open to the point of a positive confirmation. It is in an intermediate position.";
            }

            return null;
        }
    }
}