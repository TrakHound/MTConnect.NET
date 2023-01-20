// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// The status of the connection between an Adapter and an Agent.
    /// </summary>
    public enum ConnectionStatus
    {
        /// <summary>
        /// represents no connection at all.
        /// </summary>
        CLOSED,

        /// <summary>
        /// represents the Agent waiting for a connection request from an Adapter.
        /// </summary>
        LISTEN,

        /// <summary>
        /// represents an open connection. The normal state for the data transfer phase of the connection.
        /// </summary>
        ESTABLISHED
    }
}
