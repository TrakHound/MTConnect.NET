// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// Status of the connection between an adapter and an agent.
    /// </summary>
    public enum ConnectionStatus
    {
        /// <summary>
        /// No connection at all.
        /// </summary>
        CLOSED,
        
        /// <summary>
        /// Agent is waiting for a connection request from an adapter.
        /// </summary>
        LISTEN,
        
        /// <summary>
        /// Open connection.The normal state for the data transfer phase of the connection.
        /// </summary>
        ESTABLISHED
    }
}