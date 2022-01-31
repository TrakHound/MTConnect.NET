// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
