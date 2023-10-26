// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// Operational state of a Door component or composition element.
    /// </summary>
    public enum DoorState
    {
        /// <summary>
        /// Door is open to the point of a positive confirmation.
        /// </summary>
        OPEN,
        
        /// <summary>
        /// Door is closed to the point of a positive confirmation.
        /// </summary>
        CLOSED,
        
        /// <summary>
        /// Door is not closed to the point of a positive confirmation and not open to the point of a positive confirmation. It is in an intermediate position.
        /// </summary>
        UNLATCHED
    }
}