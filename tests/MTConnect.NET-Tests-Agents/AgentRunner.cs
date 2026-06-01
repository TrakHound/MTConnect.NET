// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Agents;
using MTConnect.Clients;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Servers.Http;
using System.Net;
using System.Net.Sockets;

namespace MTConnect.Tests.Agents
{
    /// <summary>Represents the agent runner.</summary>
    public class AgentRunner : IDisposable
    {
        // Generous, CI-safe bounds. The embedded HTTP server's Start() is
        // fire-and-forget (it spins the listener up on a background Task),
        // so a client request issued immediately after Start() can race the
        // socket bind. WaitForReady() polls the real Probe endpoint until it
        // answers, which is the only reliable "ready to serve" signal.
        private const int ServerReadyTimeoutMs = 30000;
        private const int ServerReadyPollMs = 100;

        private readonly MTConnectAgentBroker _agent;
        private readonly MTConnectHttpServer _httpServer;
        private readonly IEnumerable<IDevice> _devices;
        private readonly string _hostname;
        private readonly int _port;

        /// <summary>Gets or sets the agent.</summary>
        public MTConnectAgentBroker Agent => _agent;

        /// <summary>Gets or sets the http server.</summary>
        public MTConnectHttpServer HttpServer => _httpServer;

        /// <summary>Gets or sets the devices.</summary>
        public IEnumerable<IDevice> Devices => _devices;

        /// <summary>Gets or sets the hostname.</summary>
        public string Hostname => _hostname;

        /// <summary>Gets or sets the port.</summary>
        public int Port => _port;


        /// <summary>Initialises a new instance of the agent runner type.</summary>
        /// <param name="port">The port.</param>
        /// <param name="mtconnectVersion">The mtconnect version.</param>
        public AgentRunner(int port, Version? mtconnectVersion = null)
            : this(IPAddress.Loopback.ToString(), port, mtconnectVersion) { }

        /// <summary>Initialises a new instance of the agent runner type.</summary>
        /// <param name="hostname">The hostname.</param>
        /// <param name="port">The port.</param>
        /// <param name="mtconnectVersion">The mtconnect version.</param>
        public AgentRunner(string hostname, int port = 5000, Version? mtconnectVersion = null)
        {
            _hostname = hostname;
            _port = port;

            // HTTP server configuration. DefaultVersion is a string on
            // HttpServerConfiguration; only set it when a version was
            // requested so the agent keeps its built-in default otherwise.
            var configuration = new HttpServerConfiguration();
            configuration.Server = hostname;
            configuration.Port = port;
            if (mtconnectVersion != null) configuration.DefaultVersion = mtconnectVersion.ToString();

            // Create MTConnect Agent
            _agent = new MTConnectAgentBroker();

            var devices = new List<IDevice>();

            // Read Device Files
            var devicesDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "devices");
            var deviceFiles = Directory.GetFiles(devicesDir);
            foreach (var deviceFile in deviceFiles)
            {
                var fileDevices = DeviceConfiguration.FromFile(deviceFile, DocumentFormat.XML);
                if (!fileDevices.IsNullOrEmpty())
                {
                    foreach (var device in fileDevices)
                    {
                        devices.Add(device);
                        _agent.AddDevice(device);
                    }
                }
            }
            _devices = devices;

            // Start the Http Server
            _httpServer = new MTConnectHttpServer(configuration, _agent);
        }

        // Picks a TCP port the OS confirms is free, so concurrently
        // constructed fixtures never collide on a hard-coded port.
        /// <summary>Runs the get free port operation.</summary>
        /// <returns>The result of the operation.</returns>
        public static int GetFreePort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            try
            {
                return ((IPEndPoint)listener.LocalEndpoint).Port;
            }
            finally
            {
                listener.Stop();
            }
        }

        /// <summary>Runs the start operation.</summary>
        public void Start()
        {
            _agent.Start();
            _httpServer.Start();
            WaitForReady();
        }

        // Blocks until the embedded HTTP server answers a Probe request, or a
        // CI-safe bound elapses. Polling the actual endpoint proves end to end
        // that the listener is bound and the agent is serving, removing the
        // start-up race that otherwise lets a client's first request fail.
        /// <summary>Runs the wait for ready operation.</summary>
        public void WaitForReady()
        {
            var deadline = DateTime.UtcNow.AddMilliseconds(ServerReadyTimeoutMs);
            Exception? lastError = null;

            while (DateTime.UtcNow < deadline)
            {
                try
                {
                    var probeClient = new MTConnectHttpProbeClient(_hostname, _port)
                    {
                        Timeout = ServerReadyPollMs * 8
                    };

                    var document = probeClient.Get();
                    if (document != null && !document.Devices.IsNullOrEmpty()) return;
                }
                catch (Exception ex)
                {
                    lastError = ex;
                }

                Thread.Sleep(ServerReadyPollMs);
            }

            throw new TimeoutException(
                $"Embedded HTTP server on {_hostname}:{_port} did not become ready within "
                + $"{ServerReadyTimeoutMs} ms"
                + (lastError != null ? $" (last error: {lastError.Message})." : "."));
        }

        /// <summary>Runs the stop operation.</summary>
        public void Stop()
        {
            _agent.Stop();
            _httpServer.Stop();

            // Stop() only signals cancellation; the listener socket closes
            // asynchronously inside the accept loop. Block until the port
            // stops accepting connections so the next fixture (which may bind
            // a port) does not start against a half-open listener.
            WaitForPortReleased(_port);
        }

        private static void WaitForPortReleased(int port)
        {
            var deadline = DateTime.UtcNow.AddMilliseconds(ServerReadyTimeoutMs);

            while (DateTime.UtcNow < deadline)
            {
                using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    socket.Connect(IPAddress.Loopback, port);
                    socket.Close();
                }
                catch (SocketException)
                {
                    // Connection refused: the listener is gone.
                    return;
                }

                Thread.Sleep(ServerReadyPollMs);
            }
        }

        /// <summary>Runs the dispose operation.</summary>
        public void Dispose()
        {
            _httpServer.Dispose();
        }
    }
}
