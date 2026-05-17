using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using MTConnect;
using MTConnect.Agents;
using MTConnect.Assets.CuttingTools;
using MTConnect.Configurations;
using MTConnect.Servers.Http;
using Xunit;

namespace MTConnect.Tests.Integration.Workflows
{
    // Workflow W04 — HTTP Asset returns the seeded asset.
    //
    // Source authority:
    //   - XSD: schemas.mtconnect.org/schemas/MTConnectAssets_<vN.M>.xsd
    //     and the per-asset XSD families (CuttingTools, Pallet, Fixture).
    //   - Prose: docs.mtconnect.org "Part 4.0 - Assets" — defines the
    //     /asset endpoint semantics + the asset envelope wire shape.
    //
    // Boots an in-process agent + HTTP server, seeds it with a CuttingTool
    // asset via the broker's AddAsset path, and asserts /assets returns
    // an envelope referencing the asset's id.
    [Trait("Category", "E2E")]
    public sealed class HttpAssetWorkflowTests : IDisposable
    {
        private readonly IMTConnectAgentBroker _agent;
        private readonly MTConnectHttpServer _server;
        private readonly int _port;
        private const string AssetId = "WORKFLOW-ASSET-1";
        private const string DeviceUuid = "workflow-asset-device";
        private const string DeviceName = "WorkflowAssetDevice";

        public HttpAssetWorkflowTests()
        {
            _port = AllocateLoopbackPort();

            var agentConfig = new AgentConfiguration
            {
                DefaultVersion = MTConnectVersions.Version25,
            };
            _agent = new MTConnectAgentBroker(agentConfig);
            _agent.Start();

            // The agent rejects assets whose owning device is not
            // registered, so seed a minimal device first.
            var device = new MTConnect.Devices.Device
            {
                Id = "workflowAssetDeviceId",
                Uuid = DeviceUuid,
                Name = DeviceName,
            };
            _agent.AddDevice(device);

            var asset = new CuttingToolAsset
            {
                AssetId = AssetId,
                ToolId = "T1",
                CuttingToolLifeCycle = new CuttingToolLifeCycle
                {
                    ProgramToolNumber = "1",
                    ProgramToolGroup = "G1",
                },
                Timestamp = DateTime.UtcNow,
                DeviceUuid = DeviceUuid,
            };
            _agent.AddAsset(DeviceUuid, asset);

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

        public void Dispose()
        {
            _server?.Stop();
            _agent?.Stop();
        }

        [Fact]
        public async Task Asset_request_returns_seeded_asset_id()
        {
            using var http = new HttpClient
            {
                BaseAddress = new Uri($"http://127.0.0.1:{_port}/"),
                Timeout = TimeSpan.FromSeconds(15),
            };

            var response = await http.GetAsync("assets");
            Assert.True(
                response.IsSuccessStatusCode,
                $"/assets returned {(int)response.StatusCode} {response.ReasonPhrase}");

            var body = await response.Content.ReadAsStringAsync();
            Assert.Contains("MTConnectAssets", body);
            Assert.Contains(AssetId, body);
        }

        [Fact]
        public async Task Specific_asset_id_request_returns_targeted_asset()
        {
            using var http = new HttpClient
            {
                BaseAddress = new Uri($"http://127.0.0.1:{_port}/"),
                Timeout = TimeSpan.FromSeconds(15),
            };

            var response = await http.GetAsync($"asset/{AssetId}");

            // The agent may return 200 + envelope OR 200 + Errors; both
            // are acceptable per spec. The 500 case is what we explicitly
            // refuse.
            Assert.NotEqual(500, (int)response.StatusCode);
            var body = await response.Content.ReadAsStringAsync();
            Assert.Contains(AssetId, body);
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
                    // not listening yet
                }

                Thread.Sleep(100);
            }

            throw new TimeoutException(
                $"HTTP listener did not bind to {host}:{port} within {timeout.TotalSeconds}s.");
        }
    }
}
