// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    public static class ConnectionStatusDescriptions
    {
        /// <summary>
        /// No connection at all.
        /// </summary>
        public const string CLOSED = "No connection at all.";
        
        /// <summary>
        /// Agent is waiting for a connection request from an adapter.
        /// </summary>
        public const string LISTEN = "Agent is waiting for a connection request from an adapter.";
        
        /// <summary>
        /// Open connection.The normal state for the data transfer phase of the connection.
        /// </summary>
        public const string ESTABLISHED = "Open connection.The normal state for the data transfer phase of the connection.";


        public static string Get(ConnectionStatus value)
        {
            switch (value)
            {
                case ConnectionStatus.CLOSED: return "No connection at all.";
                case ConnectionStatus.LISTEN: return "Agent is waiting for a connection request from an adapter.";
                case ConnectionStatus.ESTABLISHED: return "Open connection.The normal state for the data transfer phase of the connection.";
            }

            return null;
        }
    }
}