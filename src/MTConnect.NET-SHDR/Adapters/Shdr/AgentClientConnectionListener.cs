// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Adapters.Shdr
{
    public delegate void AgentConnectedHandler(string id, TcpClient client);

    public delegate void AgentDisconnectedHandler(string id);

    public delegate void AgentResponseHandler(string id);

    public delegate void AgentListenerErrorHandler(Exception exception);

    public delegate void AgentConnectionErrorHandler(string id, Exception exception);


    /// <summary>
    /// Listens for new Agent TCP Connections and handles the Adapter to Agent Heartbeat
    /// </summary>
    class AgentClientConnectionListener
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


        public AgentConnectedHandler ClientConnected;

        public AgentDisconnectedHandler ClientDisconnected;

        public AgentResponseHandler ClientPingReceived;

        public AgentResponseHandler ClientPongSent;

        public AgentListenerErrorHandler ListenerErrorReceived;

        public AgentConnectionErrorHandler ConnectionErrorReceived;


        public AgentClientConnectionListener(int port, int heartbeat)
        {
            Port = port;
            Heartbeat = heartbeat;
        }


        public void Start(CancellationToken cancel)
        {
            _stop = new CancellationTokenSource();
            cancel.Register(() => { Stop(); });

            _= Task.Run(() => ClientListener(_stop.Token));
        }

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
                        _= Task.Run(() => HeartbeatConnection(clientId, client, cancel));
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
