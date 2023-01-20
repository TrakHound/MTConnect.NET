// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// The operational state of a DOOR type component or composition element.
    /// </summary>
    public enum DoorState
    {
        /// <summary>
        /// The DOOR is open to the point of a positive confirmation
        /// </summary>
        OPEN,

        /// <summary>
        /// The DOOR is not closed to the point of a positive confirmation and is not open to the point of a positive confirmation.
        /// It is in an intermediate position.
        /// </summary>
        UNLATCHED,

        /// <summary>
        /// The DOOR is closed to the point of a positive confirmation
        /// </summary>
        CLOSED
    }
}