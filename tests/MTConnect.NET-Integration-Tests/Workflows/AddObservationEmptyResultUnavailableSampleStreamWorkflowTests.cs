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
    // The sibling fixture AddObservationEmptyResultUnavailableWorkflowTests pins the same
    // contract against the /current envelope. This fixture extends that coverage to the
    // /sample stream — every observation written to the buffer post-coerce must surface
    // as UNAVAILABLE in the sample envelope, and a concrete-then-empty sequence must
    // produce two distinct observations in the stream (concrete first, UNAVAILABLE
    // second). /sample preserves history (vs /current which is keyed by latest), so it
    // is the stronger pyramid signal for any future regression that re-renders or
    // rewrites buffered observations during stream emission.
    //
    // Spec authority: MTConnect Standard, Part 1 - Devices Information Model, Observation
    // Information Model - Representation - Observation Values:
    //   "If an Agent cannot determine a Valid Data Value for a DataItem, the value returned
    //    for the Result for the Data Entity MUST be reported as UNAVAILABLE."
    /// <summary>Represents the empty-result coerce wire-level E2E fixture for the /sample stream.</summary>
    [Trait("Category", "E2E")]
    public sealed class AddObservationEmptyResultUnavailableSampleStreamWorkflowTests : IDisposable
    {
        private const string DeviceUuid = "AvailabilityCoerce-SAMPLE-DEVICE";
        private const string DeviceName = "AvailabilityCoerceSample";
        private const string DeviceId = "availability-coerce-sample-device";
        private const string DataItemId = "availability-coerce-sample-availability";

        private readonly IMTConnectAgentBroker _agent;
        private readonly MTConnectHttpServer _server;
        private readonly int _port;

        /// <summary>Initialises a new instance of the /sample stream empty-Result coerce fixture.</summary>
        public AddObservationEmptyResultUnavailableSampleStreamWorkflowTests()
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

        /// <summary>Pins the behaviour expressed by the test name across the full null / empty / whitespace family: every member is coerced to UNAVAILABLE on the /sample stream.</summary>
        /// <param name="value">The pre-coerce Result the caller forwards in.</param>
        /// <param name="label">Human-readable label for the case.</param>
        /// <returns>The result of the operation.</returns>
        [Theory]
        [InlineData(null, "null")]
        [InlineData("", "empty")]
        [InlineData("   ", "spaces")]
        [InlineData("\t", "tab")]
        [InlineData("\n", "newline")]
        [InlineData("\r\n", "crlf")]
        public async Task AddObservation_with_invalid_result_renders_UNAVAILABLE_on_sample_stream(string? value, string label)
        {
            var added = _agent.AddObservation(
                DeviceUuid,
                DataItemId,
                (object)value!,
                DateTime.UtcNow);
            Assert.True(added, $"[{label}] non-Valid-Data-Value AddObservation must reach the buffer post-coerce");

            var availabilities = await FetchAvailabilityElementsAsync();

            // The default device pre-seeds an UNAVAILABLE sample at sequence 1, then the
            // AddObservation above writes the coerced UNAVAILABLE at sequence 2. The
            // latest observation we explicitly wrote MUST surface as UNAVAILABLE.
            Assert.NotEmpty(availabilities);
            var latest = availabilities[^1];
            Assert.Equal("UNAVAILABLE", latest.Value);
        }

        /// <summary>Pins the behaviour expressed by the test name: a concrete-then-empty sequence renders two distinct samples on /sample (concrete first, then UNAVAILABLE), proving the coerce did not silently drop the second observation.</summary>
        /// <returns>The result of the operation.</returns>
        [Fact]
        public async Task AddObservation_concrete_then_empty_renders_distinct_samples_on_sample_stream()
        {
            var t0 = DateTime.UtcNow;
            Assert.True(_agent.AddObservation(DeviceUuid, DataItemId, (object)"AVAILABLE", t0));
            Assert.True(_agent.AddObservation(DeviceUuid, DataItemId, (object)string.Empty, t0.AddSeconds(1)));

            var availabilities = await FetchAvailabilityElementsAsync();

            // Filter out the agent's pre-seed UNAVAILABLE at sequence 1 (added when
            // AddDevice runs) — the two AddObservation calls above produce the LAST two
            // observations in sequence order. Their values must read AVAILABLE then
            // UNAVAILABLE, in that order; the second observation is the coerce surfacing
            // on the wire and must NOT be dropped.
            Assert.True(availabilities.Count >= 2, $"expected at least 2 Availability samples, got {availabilities.Count}");
            Assert.Equal("AVAILABLE", availabilities[^2].Value);
            Assert.Equal("UNAVAILABLE", availabilities[^1].Value);
        }

        /// <summary>Pins the behaviour expressed by the test name: concrete value survives the /sample stream round-trip verbatim — the coerce must not substitute for a Valid Data Value.</summary>
        /// <returns>The result of the operation.</returns>
        [Fact]
        public async Task AddObservation_with_concrete_result_renders_value_verbatim_on_sample_stream()
        {
            var added = _agent.AddObservation(
                DeviceUuid,
                DataItemId,
                (object)"AVAILABLE",
                DateTime.UtcNow);
            Assert.True(added);

            var availabilities = await FetchAvailabilityElementsAsync();

            Assert.NotEmpty(availabilities);
            Assert.Equal("AVAILABLE", availabilities[^1].Value);
        }

        private async Task<System.Collections.Generic.List<XElement>> FetchAvailabilityElementsAsync()
        {
            using var http = new HttpClient
            {
                BaseAddress = new Uri($"http://127.0.0.1:{_port}/"),
                Timeout = TimeSpan.FromSeconds(15),
            };

            var response = await http.GetAsync("sample?from=0&count=100");
            Assert.True(
                response.IsSuccessStatusCode,
                $"/sample returned {(int)response.StatusCode} {response.ReasonPhrase}");

            var body = await response.Content.ReadAsStringAsync();

            // The streaming envelope renders each observation under <Streams> /
            // <DeviceStream> / <ComponentStream> / <Events> as a dedicated <Availability>
            // element with its own dataItemId + sequence attributes. Locate every element
            // matching the fixture's DataItemId, ordered by their document position
            // (sequence order, oldest first) — a naive substring or single-Descendants
            // call would miss the multi-sample semantics of /sample.
            var doc = XDocument.Parse(body);
            return doc.Descendants()
                .Where(e => string.Equals(
                    (string?)e.Attribute("dataItemId"),
                    DataItemId,
                    StringComparison.Ordinal))
                .ToList();
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
