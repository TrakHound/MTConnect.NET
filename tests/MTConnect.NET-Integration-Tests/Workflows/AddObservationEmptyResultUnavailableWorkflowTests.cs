using System;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using MTConnect;
using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using MTConnect.Servers.Http;
using Xunit;

namespace MTConnect.Tests.Integration.Workflows
{
    // Wire-level E2E for MTConnect Part 1 Observation Information Model — Representation —
    // Observation Values: a null, empty, or whitespace-only Result value MUST surface as
    // "UNAVAILABLE" on the wire, not as the empty value verbatim.
    //
    // The agent's AddObservation(deviceKey, dataItemKey, value, timestamp) path is exercised
    // with an empty-string Result; an HTTP GET /current is issued against the loopback-bound
    // MTConnectHttpServer the agent feeds. The rendered XML envelope is inspected directly —
    // the assertion is that the AVAILABILITY data item carries value="UNAVAILABLE" rather
    // than an empty value attribute. This pins the SDK's coerce all the way through to the
    // wire, so a future regression in MTConnectAgent.AddObservation, the Common -> HTTP -> XML
    // formatter chain, or anything between the buffer and the wire is caught here.
    //
    // Spec authority: MTConnect Standard, Part 1 - Devices Information Model, Observation
    // Information Model - Representation - Observation Values:
    //   "If an Agent cannot determine a Valid Data Value for a DataItem, the value returned
    //    for the Result for the Data Entity MUST be reported as UNAVAILABLE."
    /// <summary>Represents the empty-result coerce workflow tests.</summary>
    [Trait("Category", "E2E")]
    public sealed class AddObservationEmptyResultUnavailableWorkflowTests : IDisposable
    {
        private const string DeviceUuid = "AvailabilityCoerce-DEVICE";
        private const string DeviceName = "AvailabilityCoerce";
        private const string DeviceId = "availability-coerce-device";
        private const string DataItemId = "availability-coerce-availability";

        private readonly IMTConnectAgentBroker _agent;
        private readonly MTConnectHttpServer _server;
        private readonly int _port;

        /// <summary>Initialises a new instance of the empty-result coerce workflow tests type.</summary>
        public AddObservationEmptyResultUnavailableWorkflowTests()
        {
            _port = AllocateLoopbackPort();

            var agentConfig = new AgentConfiguration
            {
                DefaultVersion = MTConnectVersions.Version25,
            };
            _agent = new MTConnectAgentBroker(agentConfig);
            _agent.Start();

            var device = new Device
            {
                Id = DeviceId,
                Name = DeviceName,
                Uuid = DeviceUuid,
            };
            device.AddDataItem(new AvailabilityDataItem(DeviceId) { Id = DataItemId });

            var added = _agent.AddDevice(device);
            Assert.NotNull(added);

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

        /// <summary>Runs the dispose operation.</summary>
        public void Dispose()
        {
            _server?.Stop();
            _agent?.Stop();
        }

        /// <summary>Pins the behaviour expressed by the test name across the full null / empty / whitespace family: every member is coerced to UNAVAILABLE on the current envelope.</summary>
        /// <param name="value">The pre-coerce Result the caller forwards in.</param>
        /// <param name="label">Human-readable label for the case (drives the test display name).</param>
        /// <returns>The result of the operation.</returns>
        [Theory]
        [InlineData(null, "null")]
        [InlineData("", "empty")]
        [InlineData("   ", "spaces")]
        [InlineData("\t", "tab")]
        [InlineData("\n", "newline")]
        [InlineData("\r\n", "crlf")]
        public async Task AddObservation_with_invalid_result_renders_UNAVAILABLE_on_current_envelope(string? value, string label)
        {
            var added = _agent.AddObservation(
                DeviceUuid,
                DataItemId,
                (object)value!,
                DateTime.UtcNow);
            Assert.True(added, $"[{label}] non-Valid-Data-Value AddObservation must reach the buffer post-coerce");

            using var http = new HttpClient
            {
                BaseAddress = new Uri($"http://127.0.0.1:{_port}/"),
                Timeout = TimeSpan.FromSeconds(15),
            };

            var response = await http.GetAsync("current");

            Assert.True(
                response.IsSuccessStatusCode,
                $"[{label}] /current returned {(int)response.StatusCode} {response.ReasonPhrase}");

            var body = await response.Content.ReadAsStringAsync();

            // The streaming envelope renders the AVAILABILITY observation as
            //   <Availability dataItemId="..." ...>UNAVAILABLE</Availability>
            // (the value lives in the element body, not in a value="..." attribute).
            // Parse the response and locate the specific element whose dataItemId
            // matches the device's AVAILABILITY data item, then assert its body
            // is the UNAVAILABLE sentinel. A naive substring check is unsafe — the
            // envelope carries other UNAVAILABLE entries for every uninitialised
            // sibling data item (AssetChanged, AssetRemoved, etc.) and would yield
            // a false positive against an empty-body Availability element.
            var availability = FindObservationByDataItemId(body, DataItemId);
            Assert.NotNull(availability);
            Assert.Equal("UNAVAILABLE", availability!.Value);
        }

        /// <summary>Pins the behaviour expressed by the test name: concrete value survives the wire round-trip verbatim.</summary>
        /// <returns>The result of the operation.</returns>
        [Fact]
        public async Task AddObservation_with_concrete_result_renders_value_verbatim()
        {
            var added = _agent.AddObservation(
                DeviceUuid,
                DataItemId,
                (object)"AVAILABLE",
                DateTime.UtcNow);
            Assert.True(added);

            using var http = new HttpClient
            {
                BaseAddress = new Uri($"http://127.0.0.1:{_port}/"),
                Timeout = TimeSpan.FromSeconds(15),
            };

            var response = await http.GetAsync("current");

            Assert.True(response.IsSuccessStatusCode);

            var body = await response.Content.ReadAsStringAsync();

            // Locate the specific AVAILABILITY element by dataItemId attribute, then
            // assert its body is "AVAILABLE" — pinning that the coerce did NOT
            // substitute UNAVAILABLE for the valid concrete value. The substring
            // approach is unsafe because the envelope's other (uninitialised) data
            // items carry UNAVAILABLE in their bodies.
            var availability = FindObservationByDataItemId(body, DataItemId);
            Assert.NotNull(availability);
            Assert.Equal("AVAILABLE", availability!.Value);
        }

        /// <summary>Pins the behaviour expressed by the test name: a concrete-then-empty sequence renders UNAVAILABLE on /current (the latest observation wins).</summary>
        /// <returns>The result of the operation.</returns>
        [Fact]
        public async Task AddObservation_concrete_then_empty_renders_UNAVAILABLE_on_current_envelope()
        {
            var t0 = DateTime.UtcNow;
            Assert.True(_agent.AddObservation(DeviceUuid, DataItemId, (object)"AVAILABLE", t0));
            Assert.True(_agent.AddObservation(DeviceUuid, DataItemId, (object)string.Empty, t0.AddSeconds(1)));

            using var http = new HttpClient
            {
                BaseAddress = new Uri($"http://127.0.0.1:{_port}/"),
                Timeout = TimeSpan.FromSeconds(15),
            };

            var response = await http.GetAsync("current");
            Assert.True(response.IsSuccessStatusCode);

            var body = await response.Content.ReadAsStringAsync();
            // /current reflects the latest observation only; the second AddObservation
            // (empty Result) coerced to UNAVAILABLE must overwrite the concrete value
            // posted first.
            var availability = FindObservationByDataItemId(body, DataItemId);
            Assert.NotNull(availability);
            Assert.Equal("UNAVAILABLE", availability!.Value);
        }

        /// <summary>Pins the behaviour expressed by the test name: under InputValidationLevel.Strict an empty Result coerces and lands on /current rather than being silently dropped pre-fix.</summary>
        /// <returns>The result of the operation.</returns>
        [Fact]
        public async Task AddObservation_with_empty_result_under_Strict_validation_lands_on_current_envelope()
        {
            using var strict = StrictHarness.Create();

            var added = strict.Agent.AddObservation(
                StrictHarness.Uuid,
                StrictHarness.DataItemId,
                (object)string.Empty,
                DateTime.UtcNow);
            Assert.True(added, "Strict must coerce to UNAVAILABLE — never silently drop an empty Result");

            using var http = new HttpClient
            {
                BaseAddress = new Uri($"http://127.0.0.1:{strict.Port}/"),
                Timeout = TimeSpan.FromSeconds(15),
            };

            var response = await http.GetAsync("current");
            Assert.True(response.IsSuccessStatusCode);

            var body = await response.Content.ReadAsStringAsync();
            var availability = FindObservationByDataItemId(body, StrictHarness.DataItemId);
            Assert.NotNull(availability);
            Assert.Equal("UNAVAILABLE", availability!.Value);
        }

        // Per-test harness for the Strict-level case: the default fixture-level
        // agent runs under InputValidationLevel.Warning so it can't exercise the
        // pre-fix silent-drop pathology. The Strict harness spins its own agent
        // + server + loopback port and disposes them when the test ends.
        private sealed class StrictHarness : IDisposable
        {
            public const string Uuid = "AvailabilityCoerce-STRICT";
            public const string DataItemId = "availability-coerce-strict";
            private const string DeviceName = "AvailabilityCoerceStrict";
            private const string DeviceId = "availability-coerce-strict-device";

            public IMTConnectAgentBroker Agent { get; }
            public MTConnectHttpServer Server { get; }
            public int Port { get; }

            private StrictHarness(IMTConnectAgentBroker agent, MTConnectHttpServer server, int port)
            {
                Agent = agent;
                Server = server;
                Port = port;
            }

            public static StrictHarness Create()
            {
                var port = AllocateLoopbackPort();
                var agentConfig = new AgentConfiguration
                {
                    DefaultVersion = MTConnectVersions.Version25,
                    InputValidationLevel = InputValidationLevel.Strict,
                };
                var agent = new MTConnectAgentBroker(agentConfig);
                agent.Start();

                var device = new Device
                {
                    Id = DeviceId,
                    Name = DeviceName,
                    Uuid = Uuid,
                };
                device.AddDataItem(new AvailabilityDataItem(DeviceId) { Id = DataItemId });
                Assert.NotNull(agent.AddDevice(device));

                var serverConfig = new HttpServerConfiguration
                {
                    Port = port,
                    Server = "127.0.0.1",
                };
                var server = new MTConnectHttpServer(serverConfig, agent);
                Exception? startupException = null;
                server.ServerException += (_, ex) => startupException ??= ex;
                server.Start();
                WaitForListener("127.0.0.1", port, TimeSpan.FromSeconds(30), () => startupException);

                return new StrictHarness(agent, server, port);
            }

            public void Dispose()
            {
                Server.Stop();
                Agent.Stop();
            }
        }

        private static XElement? FindObservationByDataItemId(string envelopeXml, string dataItemId)
        {
            var doc = XDocument.Parse(envelopeXml);
            return doc.Descendants()
                .FirstOrDefault(e =>
                    string.Equals(
                        (string?)e.Attribute("dataItemId"),
                        dataItemId,
                        StringComparison.Ordinal));
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
