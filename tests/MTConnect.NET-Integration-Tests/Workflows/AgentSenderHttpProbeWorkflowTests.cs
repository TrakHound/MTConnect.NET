// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Servers.Http;
using Xunit;

namespace MTConnect.Tests.Integration.Workflows
{
    /// <summary>
    /// End-to-end workflow tests for the operator-authored
    /// <see cref="AgentConfiguration.Sender"/> surface. Each test boots an
    /// in-process <see cref="MTConnectAgentBroker"/> plus embedded
    /// <see cref="MTConnectHttpServer"/>, performs a real HTTP GET on
    /// <c>/probe</c>, and asserts the emitted
    /// <c>MTConnectDevices/Header/@sender</c> attribute matches the
    /// operator-authored value; the negative path asserts the pre-existing
    /// <see cref="Dns.GetHostName"/> fallback still fires when the config
    /// value is absent.
    /// </summary>
    /// <remarks>
    /// Sources:
    /// <list type="bullet">
    /// <item>Prose — MTConnect Part 1 §7 defines <c>Header/@sender</c> as
    /// "An identification defining where the Agent that published the
    /// Response Document is installed or hosted."</item>
    /// <item>XSD — <c>schemas.mtconnect.org/schemas/MTConnectDevices_2.7.xsd</c>
    /// declares the <c>sender</c> attribute on the <c>Header</c> element.</item>
    /// </list>
    /// </remarks>
    [Trait("Category", "E2E")]
    public sealed class AgentSenderHttpProbeWorkflowTests
    {
        private static int s_nextPort = 6400 + (System.Security.Cryptography.RandomNumberGenerator.GetInt32(0, 1000));

        private static int AllocatePort() => Interlocked.Increment(ref s_nextPort);

        /// <summary>Operator-authored <see cref="AgentConfiguration.Sender"/>
        /// flows through the broker + HTTP server to appear as the
        /// <c>Header/@sender</c> attribute in the /probe response body.</summary>
        [Fact]
        public async Task Probe_response_Header_sender_matches_configured_Sender()
        {
            const string PinnedSender = "foo-plant-a";
            var port = AllocatePort();

            var configuration = new AgentConfiguration
            {
                Sender = PinnedSender
            };

            var body = await FetchProbeBodyAsync(configuration, port);

            Assert.Contains("MTConnectDevices", body);
            Assert.Contains($"sender=\"{PinnedSender}\"", body);
        }

        /// <summary>When <see cref="AgentConfiguration.Sender"/> is not set, the
        /// /probe response's <c>Header/@sender</c> falls back to
        /// <see cref="Dns.GetHostName"/> — the pre-existing behaviour before
        /// the operator surface was added.</summary>
        [Fact]
        public async Task Probe_response_Header_sender_falls_back_to_hostname_when_Sender_absent()
        {
            var port = AllocatePort();

            var configuration = new AgentConfiguration();

            var body = await FetchProbeBodyAsync(configuration, port);
            var expected = Dns.GetHostName();

            Assert.Contains("MTConnectDevices", body);
            Assert.Contains($"sender=\"{expected}\"", body);
        }

        private static async Task<string> FetchProbeBodyAsync(AgentConfiguration configuration, int port)
        {
            var agent = new MTConnectAgentBroker(configuration);
            agent.Start();
            try
            {
                var serverConfig = new HttpServerConfiguration
                {
                    Port = port,
                    Server = "127.0.0.1"
                };
                Exception? startupException = null;
                using var server = new MTConnectHttpServer(serverConfig, agent);
                server.ServerException += (_, ex) => startupException ??= ex;
                server.Start();
                try
                {
                    WaitForListener("127.0.0.1", port, TimeSpan.FromSeconds(30), () => startupException);

                    using var http = new HttpClient
                    {
                        BaseAddress = new Uri($"http://127.0.0.1:{port}/"),
                        Timeout = TimeSpan.FromSeconds(15)
                    };

                    var response = await http.GetAsync("probe");

                    Assert.True(
                        response.IsSuccessStatusCode,
                        $"/probe returned {(int)response.StatusCode} {response.ReasonPhrase}");

                    return await response.Content.ReadAsStringAsync();
                }
                finally
                {
                    server.Stop();
                }
            }
            finally
            {
                agent.Stop();
                Thread.Sleep(150);
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
                    // Not listening yet; keep polling.
                }

                Thread.Sleep(100);
            }

            throw new TimeoutException(
                $"HTTP listener did not bind to {host}:{port} within {timeout.TotalSeconds}s.");
        }
    }
}
