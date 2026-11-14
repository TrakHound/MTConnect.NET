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


        /// <summary>
        /// Binds the listening socket and accepts connections on a background task. The port is bound before this
        /// method returns, so an Agent can connect as soon as it does (connections wait in the listen backlog until
        /// the background task accepts them). If the port cannot be bound, <see cref="ListenerErrorReceived"/> is
        /// raised and the background task keeps retrying. The supplied <paramref name="cancel"/> token also
        /// triggers <see cref="Stop"/> when cancelled.
        /// </summary>
        public void Start(CancellationToken cancel)
        {
            var stop = CancellationTokenSource.CreateLinkedTokenSource(cancel);
            var previous = Interlocked.Exchange(ref _stop, stop);
            previous?.Cancel(); // Synchronously releases the previous listener's port (see ClientListener)

            TcpListener listener = null;
            try
            {
                listener = BindListener();
            }
            catch (Exception ex)
            {
                if (ListenerErrorReceived != null) ListenerErrorReceived.Invoke(ex);
            }

            _ = Task.Run(() => ClientListener(listener, stop.Token));
        }

        /// <summary>Stops the listener, closes every connected agent socket, and clears the connection table.</summary>
        public void Stop()
        {
            if (_stop != null) _stop.Cancel();

            StopListener(Volatile.Read(ref _listener));

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

        private TcpListener BindListener()
        {
            var listener = new TcpListener(IPAddress.Any, Port);
            listener.Start();
            Volatile.Write(ref _listener, listener);
            return listener;
        }

        // Stops the given listener. _listener is only cleared when it still refers to this listener, so a loop
        // that is shutting down after a restart cannot stop the listener the new Start() just bound.
        private void StopListener(TcpListener listener)
        {
            if (listener == null) return;

            Interlocked.CompareExchange(ref _listener, null, listener);
            try
            {
                listener.Stop();
            }
            catch (SocketException ex)
            {
                if (ListenerErrorReceived != null) ListenerErrorReceived.Invoke(ex);
            }
        }


        // Accepts connections on the listener bound by Start(). If that bind failed, or accepting fails later,
        // the port is released and bound again after a delay until the listener is stopped.
        private async Task ClientListener(TcpListener listener, CancellationToken cancel)
        {
            // Stop() / cancellation closes the listening socket, which ends a pending accept
            using (cancel.Register(() => StopListener(Volatile.Read(ref listener))))
            {
                do
                {
                    try
                    {
                        if (listener == null) Volatile.Write(ref listener, BindListener());

                        // Cancelled while binding: the registration above may have run before the bind
                        if (cancel.IsCancellationRequested) break;

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

                    // Release the socket before binding the port again (a failed accept loop used to retry against
                    // its own still-bound listener forever)
                    StopListener(Volatile.Read(ref listener));
                    Volatile.Write(ref listener, null);

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

            StopListener(Volatile.Read(ref listener));
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
