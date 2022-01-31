// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Events
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
