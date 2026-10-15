// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace MTConnect.Adapters
{
    /// <summary>
    /// An Agent connection that consumes the SHDR Adapter output
    /// </summary>
    public class AgentClient
    {
        /// <summary>
        /// A Unique ID for the instance of this Connection
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The Underlying TCP Client Connection to the Agent
        /// </summary>
        public TcpClient TcpClient { get; set; }


        /// <summary>Constructs the connection wrapper with a unique <paramref name="id"/> and the already-open <paramref name="tcpClient"/> bound to the agent.</summary>
        public AgentClient(string id, TcpClient tcpClient)
        {
            Id = id;
            TcpClient = tcpClient;
        }


        private static readonly ConditionalWeakTable<TcpClient, object> _writeLocks = new ConditionalWeakTable<TcpClient, object>();

        /// <summary>
        /// The lock every writer to <paramref name="tcpClient"/> holds while writing (data lines and heartbeat PONGs),
        /// so writes never interleave, e.g. inside a multiline asset.
        /// </summary>
        internal static object GetWriteLock(TcpClient tcpClient)
        {
            return _writeLocks.GetValue(tcpClient, _ => new object());
        }
    }
}