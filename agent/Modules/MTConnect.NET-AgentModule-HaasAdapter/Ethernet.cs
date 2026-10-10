// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Net.Sockets;

namespace MTConnect.Haas
{
    /// <summary>
    /// TCP connection to the Q-command port of a Haas control.
    /// </summary>
    public class Ethernet : HaasConnection
    {
        private TcpClient _client;


        /// <summary>
        /// The hostname or IP address of the Haas control.
        /// </summary>
        public string Server { get; }

        /// <summary>
        /// The TCP port of the Haas control (Setting 143, typically 5051).
        /// </summary>
        public int Port { get; }

        /// <summary>
        /// Maximum time (in milliseconds) to wait for the TCP connection to open.
        /// </summary>
        public int ConnectionTimeout { get; set; } = 5000;

        /// <inheritdoc/>
        public override bool IsConnected => _client != null && _client.Connected;


        /// <summary>
        /// Initializes a new connection to the specified Haas control.
        /// </summary>
        /// <param name="server">The hostname or IP address of the Haas control.</param>
        /// <param name="port">The TCP port of the Haas control.</param>
        public Ethernet(string server, int port)
        {
            Server = server;
            Port = port;
        }


        /// <inheritdoc/>
        protected override Stream OnOpen()
        {
            var client = new TcpClient();
            try
            {
                if (!client.ConnectAsync(Server, Port).Wait(ConnectionTimeout))
                {
                    throw new TimeoutException($"Timed out connecting to {Server}:{Port}");
                }

                client.NoDelay = true;
                _client = client;
                return client.GetStream();
            }
            catch (AggregateException ex) when (ex.InnerException != null)
            {
                client.Dispose();
                throw ex.InnerException;
            }
            catch
            {
                client.Dispose();
                throw;
            }
        }

        /// <inheritdoc/>
        protected override void OnClose()
        {
            if (_client != null) _client.Dispose();
            _client = null;
        }

        /// <inheritdoc/>
        protected override void DiscardInput(Stream stream)
        {
            if (stream is NetworkStream networkStream)
            {
                var buffer = new byte[256];
                while (networkStream.DataAvailable)
                {
                    if (networkStream.Read(buffer, 0, buffer.Length) <= 0) break;
                }
            }
        }
    }
}
