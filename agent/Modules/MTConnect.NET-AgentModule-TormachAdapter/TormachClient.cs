// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace MTConnect.Tormach
{
    /// <summary>
    /// TCP client for the PathPilot status server. Messages are buffered by the socket
    /// and read without blocking by <see cref="ReadMessages"/>.
    /// </summary>
    public class TormachClient : IDisposable
    {
        private readonly StringBuilder _buffer = new StringBuilder();
        private readonly byte[] _readBuffer = new byte[8192];
        private readonly char[] _charBuffer = new char[8192];
        private Decoder _decoder = Encoding.UTF8.GetDecoder();
        private readonly Stopwatch _lastData = new Stopwatch();
        private TcpClient _client;
        private NetworkStream _stream;


        /// <summary>
        /// Gets whether the client is connected.
        /// </summary>
        public bool IsConnected => _client != null && _client.Connected;

        /// <summary>
        /// Gets the time (in milliseconds) since data was last received.
        /// </summary>
        public long MillisecondsSinceData => _lastData.ElapsedMilliseconds;


        /// <summary>
        /// Connects to the status server.
        /// </summary>
        /// <param name="server">The hostname or IP address of the PathPilot controller.</param>
        /// <param name="port">The status server port.</param>
        /// <param name="timeout">Maximum time (in milliseconds) to wait for the connection to open.</param>
        public void Connect(string server, int port, int timeout)
        {
            Close();

            var client = new TcpClient();
            try
            {
                if (!client.ConnectAsync(server, port).Wait(timeout))
                {
                    throw new TimeoutException($"Timed out connecting to {server}:{port}");
                }
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

            _client = client;
            _stream = client.GetStream();
            _buffer.Clear();
            _decoder = Encoding.UTF8.GetDecoder();
            _lastData.Restart();
        }

        /// <summary>
        /// Reads every complete message received since the last call, without blocking.
        /// </summary>
        /// <returns>The parsed messages (<see cref="TormachInfo"/> or <see cref="TormachStatus"/>), in the order received.</returns>
        /// <exception cref="System.IO.IOException">The connection was closed.</exception>
        public List<object> ReadMessages()
        {
            var messages = new List<object>();
            if (_stream == null) return messages;

            while (_stream.DataAvailable)
            {
                var count = _stream.Read(_readBuffer, 0, _readBuffer.Length);
                if (count <= 0) break;

                // The decoder keeps partial multi-byte characters between reads
                var chars = _decoder.GetChars(_readBuffer, 0, count, _charBuffer, 0);
                _buffer.Append(_charBuffer, 0, chars);
                _lastData.Restart();
            }

            // A closed connection is readable with no data
            if (_client.Client.Poll(0, SelectMode.SelectRead) && _client.Client.Available == 0)
            {
                throw new System.IO.IOException("Connection closed by the status server");
            }

            var text = _buffer.ToString();
            var end = text.LastIndexOf('\n');
            if (end >= 0)
            {
                foreach (var line in text.Substring(0, end).Split('\n'))
                {
                    var message = TormachMessageParser.Parse(line.Trim());
                    if (message != null) messages.Add(message);
                }

                _buffer.Remove(0, end + 1);
            }

            return messages;
        }

        /// <summary>
        /// Closes the connection.
        /// </summary>
        public void Close()
        {
            try
            {
                if (_stream != null) _stream.Dispose();
                if (_client != null) _client.Dispose();
            }
            catch { }

            _stream = null;
            _client = null;
        }

        /// <summary>
        /// Closes the connection.
        /// </summary>
        public void Dispose() => Close();
    }
}
