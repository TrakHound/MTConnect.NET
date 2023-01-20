// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// The status of the connection between an Adapter and an Agent.
    /// </summary>
    public static class ConnectionStatusDescriptions
    {
        /// <summary>
        /// represents no connection at all.
        /// </summary>
        public const string CLOSED = "represents no connection at all.";

        /// <summary>
        /// represents the Agent waiting for a connection request from an Adapter.
        /// </summary>
        public const string LISTEN = "represents the Agent waiting for a connection request from an Adapter.";

        /// <summary>
        /// represents an open connection. The normal state for the data transfer phase of the connection.
        /// </summary>
        public const string ESTABLISHED = "represents an open connection. The normal state for the data transfer phase of the connection.";


        public static string Get(ConnectionStatus value)
        {
            switch (value)
            {
                case ConnectionStatus.CLOSED: return CLOSED;
                case ConnectionStatus.LISTEN: return LISTEN;
                case ConnectionStatus.ESTABLISHED: return ESTABLISHED;
            }

            return null;
        }
    }
}
