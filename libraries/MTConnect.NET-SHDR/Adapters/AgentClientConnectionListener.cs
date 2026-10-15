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
            var stop = CancellationTokenSource.CreateLinkedTokenSource(cancel);
            var previous = Interlocked.Exchange(ref _stop, stop);
            previous?.Cancel();

            _ = Task.Run(() => ClientListener(stop.Token));
        }

        /// <summary>Stops the listener, closes every connected agent socket, and clears the connection table.</summary>
        public void Stop()
        {
            if (_stop != null) _stop.Cancel();

            StopListener();

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

        private void StopListener()
        {
            var listener = Interlocked.Exchange(ref _listener, null);
            if (listener != null)
            {
                try
                {
                    listener.Stop();
                }
                catch (SocketException ex)
                {
                    if (ListenerErrorReceived != null) ListenerErrorReceived.Invoke(ex);
                }
            }
        }


        private async Task ClientListener(CancellationToken cancel)
        {
            // Stop() closes the listening socket, which ends a pending accept
            using (cancel.Register(StopListener))
            {
                do
                {
                    try
                    {
                        // Release the previous socket before binding the port again (a failed accept loop used to
                        // retry against its own still-bound listener forever)
                        StopListener();

                        var listener = new TcpListener(IPAddress.Any, Port);
                        listener.Start();
                        _listener = listener;

                        // Enter the listening loop.
                        while (!cancel.IsCancellationRequested)
                        {
                            // Wait for new Client Connection
                            var client = await listener.AcceptTcpClientAsync().ConfigureAwait(false);
                            client.SendTimeout = Heartbeat;

                            // Generate Unique ID for Connection
                            var clientId = StringFunctions.RandomString(10);

                            // Add to internal list of connected clients
                            _clientConnections.TryAdd(clientId, client);

                            // Handle the connection off the accept loop, so a slow initial dump (or a failing
                            // handler) cannot hold up or end accepting other Agents
                            _ = Task.Run(() => HandleConnection(clientId, client, cancel));
                        }
                    }
                    catch (Exception ex) when (!cancel.IsCancellationRequested)
                    {
                        if (ListenerErrorReceived != null) ListenerErrorReceived.Invoke(ex);
                    }
                    catch (Exception) { /* Stopped */ }

                    if (!cancel.IsCancellationRequested)
                    {
                        try
                        {
                            await Task.Delay(1000, cancel).ConfigureAwait(false);
                        }
                        catch (OperationCanceledException) { }
                    }

                } while (!cancel.IsCancellationRequested);
            }

            StopListener();
        }

        private async Task HandleConnection(string clientId, TcpClient client, CancellationToken cancel)
        {
            try
            {
                if (ClientConnected != null) ClientConnected.Invoke(clientId, client);
            }
            catch (Exception ex)
            {
                if (ConnectionErrorReceived != null) ConnectionErrorReceived.Invoke(clientId, ex);
            }

            await HeartbeatConnection(clientId, client, cancel).ConfigureAwait(false);
        }


        // Answers each "* PING" with "* PONG <heartbeat>" and disconnects an Agent that closes the connection.
        // A silent Agent is not disconnected: Agents may skip PINGs while they are receiving data (MTConnect.NET's
        // IgnoreHeartbeatOnChange), and a dead connection is detected by the next data write failing.
        private async Task HeartbeatConnection(string clientId, TcpClient client, CancellationToken cancel)
        {
            // Create PONG Message
            var message = $"* PONG {Heartbeat}\n";
            var messageBytes = Encoding.ASCII.GetBytes(message);

            var buffer = new byte[4096];
            var pending = new StringBuilder();

            try
            {
                var stream = client.GetStream();

                // Stop() / cancellation closes the client, which ends a pending read on every target framework
                using (cancel.Register(() => { try { client.Close(); } catch { } }))
                while (!cancel.IsCancellationRequested && client.Connected)
                {
                    var count = await stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
                    if (count <= 0) break; // The Agent closed the connection

                    pending.Append(Encoding.ASCII.GetString(buffer, 0, count));

                    // Answer each complete PING line (other commands are ignored)
                    var text = pending.ToString();
                    var lastNewLine = text.LastIndexOf('\n');
                    if (lastNewLine < 0)
                    {
                        if (pending.Length > buffer.Length) pending.Clear();
                        continue;
                    }

                    pending.Remove(0, lastNewLine + 1);

                    foreach (var line in text.Substring(0, lastNewLine).Split('\n'))
                    {
                        if (line.Trim().StartsWith("* PING", StringComparison.OrdinalIgnoreCase))
                        {
                            if (ClientPingReceived != null) ClientPingReceived.Invoke(clientId);

                            // Write the PONG Message back to the stream (serialized with the Adapter's data writes)
                            lock (AgentClient.GetWriteLock(client))
                            {
                                stream.Write(messageBytes, 0, messageBytes.Length);
                            }

                            if (ClientPongSent != null) ClientPongSent.Invoke(clientId);
                        }
                    }
                }
            }
            catch (Exception ex) when (!cancel.IsCancellationRequested)
            {
                if (ConnectionErrorReceived != null) ConnectionErrorReceived.Invoke(clientId, ex);
            }
            catch (Exception) { /* Stopped */ }
            finally
            {
                if (client != null) client.Close();
                _clientConnections.TryRemove(clientId, out _);
                if (ClientDisconnected != null) ClientDisconnected.Invoke(clientId);
            }
        }
    }
}
