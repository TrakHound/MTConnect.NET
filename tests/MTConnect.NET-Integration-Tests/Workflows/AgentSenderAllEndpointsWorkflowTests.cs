// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using MTConnect.Agents;
using MTConnect.Assets.CuttingTools;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Servers.Http;
using Xunit;

namespace MTConnect.Tests.Integration.Workflows
{
    /// <summary>
    /// End-to-end companion to
    /// <see cref="AgentSenderHttpProbeWorkflowTests"/> that widens the
    /// pinning surface from <c>/probe</c> alone to every MTConnect wire
    /// endpoint that carries a <c>Header</c> element in its response
    /// document. MTConnect Part 1 §7 defines the <c>sender</c> attribute
    /// as an identification of the host / installation that emitted the
    /// document, and the XSDs declare it on every top-level response
    /// header — <c>MTConnectDevicesType/Header</c> (/probe),
    /// <c>MTConnectStreamsType/Header</c> (/current + /sample) and
    /// <c>MTConnectAssetsType/Header</c> (/asset). The operator-authored
    /// <see cref="AgentConfiguration.Sender"/> flows through the broker
    /// into every one of them without special-casing per endpoint.
    /// </summary>
    /// <remarks>
    /// A shared fixture boots the agent + HTTP server once per test class
    /// and re-uses them across the four endpoint assertions, keeping the
    /// end-to-end pinning cheap under the RequiresDocker / E2E category.
    /// </remarks>
    [Trait("Category", "E2E")]
    public sealed class AgentSenderAllEndpointsWorkflowTests : IDisposable
    {
        private const string PinnedSender = "sender-all-endpoints-fixture";
        private const string DeviceUuid = "sender-all-endpoints-device";
        private const string DeviceName = "SenderAllEndpointsDevice";
        private const string AssetId = "SENDER-ALL-ENDPOINTS-ASSET-1";

        private readonly IMTConnectAgentBroker _agent;
        private readonly MTConnectHttpServer _server;
        private readonly int _port;

        /// <summary>Boots the agent + HTTP server, seeds a Device and an
        /// asset so the four endpoint tests each have a valid document to
        /// fetch.</summary>
        public AgentSenderAllEndpointsWorkflowTests()
        {
            _port = AllocateLoopbackPort();

            var configuration = new AgentConfiguration
            {
                Sender = PinnedSender,
                DefaultVersion = MTConnectVersions.Version25,
            };
            _agent = new MTConnectAgentBroker(configuration);
            _agent.Start();

            var device = new Device
            {
                Id = "senderAllEndpointsDeviceId",
                Uuid = DeviceUuid,
                Name = DeviceName,
            };
            device.AddDataItem(new DataItem(DataItemCategory.EVENT, "AVAILABILITY", null, "avail"));
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
            Exception? startupException = null;
            _server = new MTConnectHttpServer(serverConfig, _agent);
            _server.ServerException += (_, ex) => startupException ??= ex;
            _server.Start();
            WaitForListener("127.0.0.1", _port, TimeSpan.FromSeconds(30), () => startupException);
        }

        /// <summary>Tears down the fixture — stops the HTTP server and the
        /// broker, then briefly yields so port + broker background threads
        /// unwind before the next fixture allocates a new port.</summary>
        public void Dispose()
        {
            try { _server?.Stop(); } catch { /* swallow — stop is best-effort */ }
            try { _agent?.Stop(); } catch { /* swallow — stop is best-effort */ }
            Thread.Sleep(150);
        }

        /// <summary>/probe response's <c>MTConnectDevices/Header/@sender</c>
        /// carries the operator-authored <see cref="AgentConfiguration.Sender"/>
        /// verbatim, matching the MTConnectDevices XSD's declaration.</summary>
        [Fact]
        public async Task Probe_Header_sender_matches_configured_Sender()
        {
            var body = await GetAsync("probe");
            Assert.Contains("MTConnectDevices", body);
            Assert.Contains($"sender=\"{PinnedSender}\"", body);
        }

        /// <summary>/current response's <c>MTConnectStreams/Header/@sender</c>
        /// carries the operator-authored <see cref="AgentConfiguration.Sender"/>
        /// verbatim, matching the MTConnectStreams XSD's declaration.</summary>
        [Fact]
        public async Task Current_Header_sender_matches_configured_Sender()
        {
            var body = await GetAsync("current");
            Assert.Contains("MTConnectStreams", body);
            Assert.Contains($"sender=\"{PinnedSender}\"", body);
        }

        /// <summary>/sample response's <c>MTConnectStreams/Header/@sender</c>
        /// carries the operator-authored <see cref="AgentConfiguration.Sender"/>
        /// verbatim — the same streams envelope as /current but populated
        /// from the observation buffer.</summary>
        [Fact]
        public async Task Sample_Header_sender_matches_configured_Sender()
        {
            var body = await GetAsync("sample");
            Assert.Contains("MTConnectStreams", body);
            Assert.Contains($"sender=\"{PinnedSender}\"", body);
        }

        /// <summary>/assets response's <c>MTConnectAssets/Header/@sender</c>
        /// carries the operator-authored <see cref="AgentConfiguration.Sender"/>
        /// verbatim, matching the MTConnectAssets XSD's declaration.</summary>
        [Fact]
        public async Task Assets_Header_sender_matches_configured_Sender()
        {
            var body = await GetAsync("assets");
            Assert.Contains("MTConnectAssets", body);
            Assert.Contains(AssetId, body);
            Assert.Contains($"sender=\"{PinnedSender}\"", body);
        }

        /// <summary>/asset/{id} response for a single asset also carries the
        /// operator-authored <see cref="AgentConfiguration.Sender"/> in the
        /// MTConnectAssets header — the single-asset endpoint reuses the
        /// same envelope shape as the /assets collection endpoint.</summary>
        [Fact]
        public async Task SingleAsset_Header_sender_matches_configured_Sender()
        {
            var body = await GetAsync($"asset/{AssetId}");
            Assert.Contains("MTConnectAssets", body);
            Assert.Contains($"sender=\"{PinnedSender}\"", body);
        }

        // ---------------- helpers ----------------

        private async Task<string> GetAsync(string path)
        {
            using var http = new HttpClient
            {
                BaseAddress = new Uri($"http://127.0.0.1:{_port}/"),
                Timeout = TimeSpan.FromSeconds(15),
            };

            var response = await http.GetAsync(path);
            Assert.True(
                response.IsSuccessStatusCode,
                $"/{path} returned {(int)response.StatusCode} {response.ReasonPhrase}");
            return await response.Content.ReadAsStringAsync();
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
                    if (client.Connected) return;
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
