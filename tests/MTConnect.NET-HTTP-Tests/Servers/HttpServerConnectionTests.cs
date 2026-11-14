// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MTConnect.Tests.Http.TestHelpers;
using NUnit.Framework;

namespace MTConnect.Tests.Http.Servers
{
    /// <summary>
    /// Connection handling of <see cref="MTConnect.Servers.Http.MTConnectHttpServer"/> over raw sockets: HTTP/1.1
    /// keep-alive without a Connection header, explicit close, and bursts of simultaneous connections.
    /// </summary>
    [TestFixture]
    [Category("HttpServerConnection")]
    public class HttpServerConnectionTests
    {
        private LoopbackAgentServer _server = null!;


        /// <summary>
        /// Helper for the tests in this fixture: set up.
        /// </summary>
        [OneTimeSetUp]
        public void SetUp() => _server = new LoopbackAgentServer();

        /// <summary>
        /// Helper for the tests in this fixture: tear down.
        /// </summary>
        [OneTimeTearDown]
        public void TearDown() => _server.Dispose();


        /// <summary>Pins the behaviour expressed by the test name: HTTP/1.1 connections are persistent by default.</summary>
        [Test]
        public void Http11_requests_without_connection_header_reuse_the_connection()
        {
            using var client = Connect();
            var stream = client.GetStream();

            foreach (var path in new[] { "/current", "/probe", "/sample?count=10", "/current" })
            {
                var response = SendRequest(stream, $"GET {path} HTTP/1.1\r\nHost: localhost\r\n\r\n");
                Assert.That(response.StatusLine, Does.Contain(" 200 "), path);
                Assert.That(response.Connection, Is.EqualTo("keep-alive").IgnoreCase, path);
            }
        }

        /// <summary>Pins the behaviour expressed by the test name: "Connection: close" closes after the response.</summary>
        [Test]
        public void Connection_close_closes_after_the_response()
        {
            using var client = Connect();
            var stream = client.GetStream();

            var response = SendRequest(stream, "GET /current HTTP/1.1\r\nHost: localhost\r\nConnection: close\r\n\r\n");
            Assert.That(response.StatusLine, Does.Contain(" 200 "));
            Assert.That(response.Connection, Is.EqualTo("close").IgnoreCase);
            Assert.That(ReadsToEnd(stream), Is.True, "The server must close the connection.");
        }

        /// <summary>Pins the behaviour expressed by the test name: a burst of connections is queued, not refused.</summary>
        /// <remarks>
        /// The requests use async socket I/O. The server runs in this process and shares its thread pool, and on
        /// Linux its socket completions run on pool worker threads, so 200 clients blocked in synchronous reads
        /// starved the server until its 60 s idle timeout reset the connections (Windows completes socket I/O on
        /// separate I/O threads, which hid this).
        /// </remarks>
        [Test]
        public async Task Burst_of_simultaneous_connections_is_accepted()
        {
            const int connections = 200;

            var clients = await Task.WhenAll(Enumerable.Range(0, connections).Select(async _ =>
            {
                var client = new TcpClient();
                await client.ConnectAsync(IPAddress.Loopback, _server.Port);
                return client;
            }));

            try
            {
                var responses = await Task.WhenAll(clients.Select(async c =>
                    (await SendRequestAsync(c.GetStream(), "GET /probe HTTP/1.1\r\nHost: localhost\r\n\r\n")).StatusLine))
                    .WaitAsync(TimeSpan.FromSeconds(30));

                Assert.That(responses, Has.All.Contain(" 200 "));
            }
            finally
            {
                foreach (var client in clients) client.Dispose();
            }
        }


        /// <summary>Pins the behaviour expressed by the test name: with no address configured, IPv4 and IPv6 clients are both accepted.</summary>
        [Test]
        public void Default_server_accepts_IPv4_and_IPv6_clients()
        {
            if (!Socket.OSSupportsIPv6) Assert.Ignore("IPv6 is not available");

            // No Server address configured (the agent's default): listens on every IPv4 and IPv6 address
            using var server = new LoopbackAgentServer(configuration => configuration.Server = null);

            foreach (var address in new[] { IPAddress.Loopback, IPAddress.IPv6Loopback })
            {
                using var client = new TcpClient(address.AddressFamily);
                client.Connect(address, server.Port);
                client.ReceiveTimeout = 10000;

                var response = SendRequest(client.GetStream(), "GET /probe HTTP/1.1\r\nHost: localhost\r\n\r\n");
                Assert.That(response.StatusLine, Does.Contain(" 200 "), address.ToString());
            }
        }


        private TcpClient Connect()
        {
            var client = new TcpClient();
            client.Connect(IPAddress.Loopback, _server.Port);
            client.ReceiveTimeout = 10000;
            return client;
        }

        private sealed class Response
        {
            public string StatusLine = string.Empty;
            public string? Connection;
        }

        private static Response SendRequest(Stream stream, string request) =>
            SendRequestAsync(stream, request).GetAwaiter().GetResult();

        private static async Task<Response> SendRequestAsync(Stream stream, string request)
        {
            var bytes = Encoding.ASCII.GetBytes(request);
            await stream.WriteAsync(bytes, 0, bytes.Length);

            var response = new Response { StatusLine = await ReadLineAsync(stream) };
            var length = 0;
            string line;
            while ((line = await ReadLineAsync(stream)).Length > 0)
            {
                var separator = line.IndexOf(':');
                if (separator < 0) continue;
                var name = line.Substring(0, separator).Trim();
                var value = line.Substring(separator + 1).Trim();
                if (name.Equals("Content-Length", StringComparison.OrdinalIgnoreCase)) length = int.Parse(value);
                if (name.Equals("Connection", StringComparison.OrdinalIgnoreCase)) response.Connection = value;
            }

            var body = new byte[length];
            var offset = 0;
            while (offset < length)
            {
                var read = await stream.ReadAsync(body, offset, length - offset);
                if (read == 0) throw new EndOfStreamException("Connection closed before the response body was read");
                offset += read;
            }

            return response;
        }

        private static async Task<string> ReadLineAsync(Stream stream)
        {
            var line = new StringBuilder();
            var one = new byte[1];
            int b = -1;
            while (await stream.ReadAsync(one, 0, 1) == 1 && (b = one[0]) != '\n')
            {
                line.Append((char)b);
                b = -1;
            }
            if (b < 0 && line.Length == 0) throw new EndOfStreamException("Connection closed");
            return line.ToString().TrimEnd('\r');
        }

        private static bool ReadsToEnd(Stream stream)
        {
            try { return stream.ReadByte() < 0; }
            catch (IOException) { return true; }
        }
    }
}
