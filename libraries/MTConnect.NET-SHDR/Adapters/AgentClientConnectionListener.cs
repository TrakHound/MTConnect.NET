// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Adapters
{
    /// <summary>Delegate raised when an MTConnect agent opens a TCP connection to the SHDR adapter; <paramref name="id"/> is the per-connection identifier the listener assigns.</summary>
    public delegate void AgentConnectedHandler(string id, TcpClient client);

    /// <summary>Delegate raised when an agent's TCP connection is closed or times out.</summary>
    public delegate void AgentDisconnectedHandler(string id);

    /// <summary>Delegate raised for each PING received from or PONG sent to the agent identified by <paramref name="id"/>.</summary>
    public delegate void AgentResponseHandler(string id);

    /// <summary>Delegate raised when the underlying <see cref="TcpListener"/> throws (port in use, socket error).</summary>
    public delegate void AgentListenerErrorHandler(Exception exception);

    /// <summary>Delegate raised when the heartbeat loop for a specific agent connection throws; the connection is normally torn down afterwards.</summary>
    public delegate void AgentConnectionErrorHandler(string id, Exception exception);


    /// <summary>
    /// Listens for new Agent TCP Connections and handles the Adapter to Agent Heartbeat
    /// </summary>
    public class AgentClientConnectionListener
    {
        private CancellationTokenSource _stop;
        private TcpListener _listener;
        private ConcurrentDictionary<string, TcpClient> _clientConnections = new ConcurrentDictionary<string, TcpClient>();

        /// <summary>
        /// The Port used to listen for connections
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// The Heartbeat interval (in milliseconds)
        /// </summary>
        public int Heartbeat { get; set; }


        /// <summary>Raised when a new agent TCP connection is accepted.</summary>
        public event AgentConnectedHandler ClientConnected;

        /// <summary>Raised when an agent connection closes; the per-connection id is no longer valid afterwards.</summary>
        public event AgentDisconnectedHandler ClientDisconnected;

        /// <summary>Raised for each SHDR PING received from a connected agent.</summary>
        public event AgentResponseHandler ClientPingReceived;

        /// <summary>Raised after the listener writes the matching SHDR PONG back to the connected agent.</summary>
        public event AgentResponseHandler ClientPongSent;

        /// <summary>Raised when the underlying <see cref="TcpListener"/> throws (typically a port-in-use or socket failure).</summary>
        public event AgentListenerErrorHandler ListenerErrorReceived;

        /// <summary>Raised when an individual agent connection's heartbeat loop throws.</summary>
        public event AgentConnectionErrorHandler ConnectionErrorReceived;


        /// <summary>Constructs the listener with the TCP <paramref name="port"/> to bind to and the heartbeat interval in milliseconds.</summary>
        public AgentClientConnectionListener(int port, int heartbeat)
        {
            Port = port;
            Heartbeat = heartbeat;
        }


        /// <summary>Starts the listener on a background task. The supplied <paramref name="cancel"/> token also triggers <see cref="Stop"/> when cancelled.</summary>
        public void Start(CancellationToken cancel)
        {
            _stop = new CancellationTokenSource();
            cancel.Register(() => { Stop(); });

            _ = Task.Run(() => ClientListener(_stop.Token));
        }

        /// <summary>Stops the listener, closes every connected agent socket, and clears the connection table.</summary>
        public void Stop()
        {
            if (_stop != null) _stop.Cancel();

            if (_listener != null)
            {
                try
                {
                    _listener.Stop();
                }
                catch (SocketException ex)
                {
                    if (ListenerErrorReceived != null) ListenerErrorReceived.Invoke(ex);
                }
            }

            if (!_clientConnections.IsNullOrEmpty())
            {
                foreach (var clientConnection in _clientConnections)
                {
                    var client = clientConnection.Value;
                    client.Close();
                }

                _clientConnections.Clear();
            }
        }


        private async Task ClientListener(CancellationToken cancel)
        {
            do
            {
                try
                {
                    _listener = new TcpListener(IPAddress.Any, Port);
                    _listener.Start();

                    // Enter the listening loop.
                    while (!cancel.IsCancellationRequested)
                    {
                        // Wait for new Client Connection
                        var client = await _listener.AcceptTcpClientAsync();
                        client.ReceiveTimeout = Heartbeat;
                        client.SendTimeout = Heartbeat;

                        // Generate Unique ID for Connection
                        var clientId = StringFunctions.RandomString(10);

                        // Add to internal list of connected clients
                        _clientConnections.TryAdd(clientId, client);

                        if (ClientConnected != null) ClientConnected.Invoke(clientId, client);

                        // Start new Ping / Pong Task
                        _ = Task.Run(() => HeartbeatConnection(clientId, client, cancel));
                    }
                }
                catch (SocketException ex)
                {
                    if (ListenerErrorReceived != null) ListenerErrorReceived.Invoke(ex);
                }
                catch (Exception ex)
                {
                    if (ListenerErrorReceived != null) ListenerErrorReceived.Invoke(ex);
                }

                if (!cancel.IsCancellationRequested) await Task.Delay(1000);

            } while (!cancel.IsCancellationRequested);
        }


        private async Task HeartbeatConnection(string clientId, TcpClient client, CancellationToken cancel)
        {
            var connectionAttempts = 0;
            var maxConnectionAttempts = 2;

            // Create PONG Message
            var message = $"* PONG {Heartbeat} \n";
            var messageBytes = Encoding.ASCII.GetBytes(message);

            try
            {
                while (!cancel.IsCancellationRequested && client.Connected)
                {
                    connectionAttempts++;

                    try
                    {
                        // Buffer to store the response bytes.
                        var buffer = new byte[4096];

                        // Read PING from stream
                        var stream = client.GetStream();
                        stream.ReadTimeout = Heartbeat;

                        // Read the next bytes from the stream response
                        var responseBytes = await stream.ReadAsync(buffer, 0, buffer.Length, cancel);

                        if (ClientPingReceived != null) ClientPingReceived.Invoke(clientId);

                        // Write the PONG Message back to the stream
                        await stream.WriteAsync(messageBytes, 0, messageBytes.Length);

                        if (ClientPongSent != null) ClientPongSent.Invoke(clientId);

                        connectionAttempts = 0;
                    }
                    catch (IOException ex)
                    {
                        if (ConnectionErrorReceived != null) ConnectionErrorReceived.Invoke(clientId, ex);
                        break; // If IOException (Stream Timeout) then close the connection
                    }
                    catch (ArgumentNullException ex)
                    {
                        if (ConnectionErrorReceived != null) ConnectionErrorReceived.Invoke(clientId, ex);
                    }
                    catch (SocketException ex)
                    {
                        if (ConnectionErrorReceived != null) ConnectionErrorReceived.Invoke(clientId, ex);
                    }
                    catch (Exception ex)
                    {
                        if (ConnectionErrorReceived != null) ConnectionErrorReceived.Invoke(clientId, ex);
                    }
                    finally
                    {
                        if (connectionAttempts >= maxConnectionAttempts) client.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                if (ConnectionErrorReceived != null) ConnectionErrorReceived.Invoke(clientId, ex);
            }
            finally
            {
                if (client != null) client.Close();
                _clientConnections.TryRemove(clientId, out _);
                if (ClientDisconnected != null) ClientDisconnected.Invoke(clientId);
            }
        }
    }
}