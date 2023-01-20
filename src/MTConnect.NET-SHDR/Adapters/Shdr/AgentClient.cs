// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Net.Sockets;

namespace MTConnect.Adapters.Shdr
{
    /// <summary>
    /// An Agent connection that consumes the SHDR Adapter output
    /// </summary>
    class AgentClient
    {
        /// <summary>
        /// A Unique ID for the instance of this Connection
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The Underlying TCP Client Connection to the Agent
        /// </summary>
        public TcpClient TcpClient { get; set; }


        public AgentClient(string id, TcpClient tcpClient)
        {
            Id = id;
            TcpClient = tcpClient;
        }
    }
}