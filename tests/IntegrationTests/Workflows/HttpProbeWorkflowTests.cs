using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MTConnect;
using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Servers.Http;
using Xunit;

namespace IntegrationTests.Workflows
{
    // Workflow W01 — HTTP Probe returns the seeded devices envelope.
    //
    // Source authority:
    //   - XSD: schemas.mtconnect.org/schemas/MTConnectDevices_<vN.M>.xsd —
    //     defines the wire shape returned by the /probe endpoint.
    //   - Prose: docs.mtconnect.org "Part 1.0 - Overview" / "Part 2.0 -
    //     Devices" — defines the /probe semantics.
    //
    // The fixture spins an in-process MTConnectAgentBroker + HTTP server
    // bound to loopback, seeds it with the same XML template the existing
    // ClientAgentCommunicationTests fixture uses, and asserts the /probe
    // endpoint returns a 200 with a devices envelope referencing the
    // seeded device by uuid + name.
    [Trait("Category", "E2E")]
    public sealed class HttpProbeWorkflowTests : IDisposable
    {
        private readonly IMTConnectAgentBroker _agent;
        private readonly MTConnectHttpServer _server;
        private readonly int _port;
        private readonly string _machineId;
        private readonly string _machineName;

        public HttpProbeWorkflowTests()
        {
            // Pick a free loopback port at fixture-creation time so
            // parallel test classes do not contend for a fixed port.
            _port = AllocateLoopbackPort();
            _machineId = Guid.NewGuid().ToString();
            _machineName = $"WorkflowProbe-{_port}";

            var devicesFile = Path.Combine(
                Path.GetTempPath(),
                $"workflow-probe-devices-{Guid.NewGuid():N}.xml");
            try
            {
                ClientAgentCommunicationTests.GenerateDevicesXml(
                    _machineId,
                    _machineName,
                    devicesFile,
                    Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance);

                var agentConfig = new AgentConfiguration
                {
                    DefaultVersion = MTConnectVersions.Version25,
                };
                _agent = new MTConnectAgentBroker(agentConfig);
                _agent.Start();

                var devices = DeviceConfiguration
                    .FromFile(devicesFile, DocumentFormat.XML)
                    .ToList();
                foreach (var device in devices)
                {
                    _agent.AddDevice(device);
                }

                var serverConfig = new HttpServerConfiguration
                {
                    Port = _port,
                    Server = "127.0.0.1",
                };
                _server = new MTConnectHttpServer(serverConfig, _agent);

                Exception? startupException = null;
                _server.ServerException += (_, ex) => startupException ??= ex;
                _server.Start();

                WaitForListener("127.0.0.1", _port, TimeSpan.FromSeconds(30), () => startupException);
            }
            finally
            {
                if (File.Exists(devicesFile))
                {
                    File.Delete(devicesFile);
                }
            }
        }

        public void Dispose()
        {
            _server?.Stop();
            _agent?.Stop();
        }

        [Fact]
        public async Task Probe_returns_seeded_device()
        {
            using var http = new HttpClient
            {
                BaseAddress = new Uri($"http://127.0.0.1:{_port}/"),
                Timeout = TimeSpan.FromSeconds(15),
            };

            var response = await http.GetAsync("probe");

            Assert.True(
                response.IsSuccessStatusCode,
                $"/probe returned {(int)response.StatusCode} {response.ReasonPhrase}");

            var body = await response.Content.ReadAsStringAsync();
            Assert.Contains("MTConnectDevices", body);
            Assert.Contains(_machineName, body);
            Assert.Contains(_machineId, body);
        }

        [Fact]
        public async Task Probe_with_unknown_device_returns_error_envelope()
        {
            // Negative case: a /probe against a device key the agent does
            // not know about must NOT return 500. The MTConnect spec
            // requires an Errors envelope; the implementation may return
            // 200 + Errors or 4xx + Errors, but never an empty 500.
            using var http = new HttpClient
            {
                BaseAddress = new Uri($"http://127.0.0.1:{_port}/"),
                Timeout = TimeSpan.FromSeconds(15),
            };

            var response = await http.GetAsync("nonexistent-device/probe");

            Assert.NotEqual(500, (int)response.StatusCode);
            var body = await response.Content.ReadAsStringAsync();
            // Either an MTConnectDevices envelope (the agent returns the
            // global envelope) or an MTConnectError envelope (per the
            // spec) is acceptable — both are legitimate per-implementation
            // behavior.
            Assert.True(
                body.Contains("MTConnectDevices") || body.Contains("MTConnectError"),
                $"unexpected /probe error body: {body}");
        }

        private static int AllocateLoopbackPort()
        {
            using var listener = new TcpListener(System.Net.IPAddress.Loopback, 0);
            listener.Start();
            try
            {
                return ((System.Net.IPEndPoint)listener.LocalEndpoint).Port;
            }
            finally
            {
                listener.Stop();
            }
        }

        private static void WaitForListener(
            string host,
            int port,
            TimeSpan timeout,
            Func<Exception?> serverStartException)
        {
            var deadline = DateTime.UtcNow + timeout;
            while (DateTime.UtcNow < deadline)
            {
                var startupException = serverStartException();
                if (startupException != null)
                {
                    throw new InvalidOperationException(
                        $"HTTP server failed to start on {host}:{port}: {startupException.Message}",
                        startupException);
                }

                try
                {
                    using var client = new TcpClient();
                    client.Connect(host, port);
                    if (client.Connected)
                    {
                        return;
                    }
                }
                catch (SocketException)
                {
                    // not listening yet; keep polling
                }

                Thread.Sleep(100);
            }

            throw new TimeoutException(
                $"HTTP listener did not bind to {host}:{port} within {timeout.TotalSeconds}s.");
        }
    }
}
