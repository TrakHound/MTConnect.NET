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
                var responses = await Task.WhenAll(clients.Select(c => Task.Run(() =>
                    SendRequest(c.GetStream(), "GET /probe HTTP/1.1\r\nHost: localhost\r\n\r\n").StatusLine)));

                Assert.That(responses, Has.All.Contain(" 200 "));
            }
            finally
            {
                foreach (var client in clients) client.Dispose();
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

        private static Response SendRequest(Stream stream, string request)
        {
            var bytes = Encoding.ASCII.GetBytes(request);
            stream.Write(bytes, 0, bytes.Length);

            var response = new Response { StatusLine = ReadLine(stream) };
            var length = 0;
            string line;
            while ((line = ReadLine(stream)).Length > 0)
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
                var read = stream.Read(body, offset, length - offset);
                if (read == 0) throw new EndOfStreamException("Connection closed before the response body was read");
                offset += read;
            }

            return response;
        }

        private static string ReadLine(Stream stream)
        {
            var line = new StringBuilder();
            int b;
            while ((b = stream.ReadByte()) >= 0 && b != '\n') line.Append((char)b);
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
