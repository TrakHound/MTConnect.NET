// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MTConnect.Configurations;
using MTConnect.Http;
using MTConnect.Servers.Http;
using MTConnect.Tests.Http.TestHelpers;
using NUnit.Framework;

namespace MTConnect.Tests.Http.Servers
{
    /// <summary>
    /// End-to-end behaviour of <see cref="MTConnectHttpServer"/> on a loopback port: error status codes,
    /// request validation, POST authorization, response compression, streaming (headers flushed, path filter,
    /// heartbeat, no byte order mark), default version and Accept negotiation.
    /// </summary>
    [TestFixture]
    [Category("HttpServerBehavior")]
    public class HttpServerBehaviorTests
    {
        private LoopbackAgentServer _server = null!;
        private HttpClient _http = null!;


        /// <summary>
        /// Helper for the tests in this fixture: set up.
        /// </summary>
        [OneTimeSetUp]
        public void SetUp()
        {
            _server = new LoopbackAgentServer(configuration =>
            {
                configuration.AllowPut = false;
                configuration.DefaultVersion = "2.0";
                configuration.ResponseCompression = new[] { HttpResponseCompression.Gzip, HttpResponseCompression.Br, HttpResponseCompression.Deflate };
            });

            // No automatic decompression, so Content-Encoding and the raw bytes are visible
            _http = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.None }) { Timeout = TimeSpan.FromSeconds(20) };
        }

        /// <summary>
        /// Helper for the tests in this fixture: tear down.
        /// </summary>
        [OneTimeTearDown]
        public void TearDown()
        {
            _http.Dispose();
            _server.Dispose();
        }


        private async Task<(HttpStatusCode Status, string Body)> GetAsync(string path)
        {
            using var response = await _http.GetAsync(_server.BaseUrl + path);
            return (response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        // Reads the multipart stream for the given time and returns everything received
        private async Task<byte[]> ReadStreamAsync(string path, TimeSpan duration, Func<byte[], bool>? stopWhen = null)
        {
            using var cts = new CancellationTokenSource(duration);
            using var request = new HttpRequestMessage(HttpMethod.Get, _server.BaseUrl + path);
            using var response = await _http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cts.Token);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var received = new MemoryStream();
            using var stream = await response.Content.ReadAsStreamAsync();
            var buffer = new byte[8192];
            try
            {
                while (true)
                {
                    var read = await stream.ReadAsync(buffer, 0, buffer.Length, cts.Token);
                    if (read <= 0) break;
                    received.Write(buffer, 0, read);
                    if (stopWhen != null && stopWhen(received.ToArray())) break;
                }
            }
            catch (OperationCanceledException) { }
            catch (IOException) { }

            return received.ToArray();
        }

        private static int CountOccurrences(string text, string value)
        {
            var count = 0;
            var index = 0;
            while ((index = text.IndexOf(value, index, StringComparison.Ordinal)) >= 0) { count++; index += value.Length; }
            return count;
        }


        /// <summary>
        /// Pins the behaviour expressed by the test name: error document keeps its status code.
        /// </summary>
        [Test]
        public async Task ErrorDocument_KeepsItsStatusCode()
        {
            var (status, body) = await GetAsync("/NoSuchDevice/current");

            Assert.That(status, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(body, Does.Contain("MTConnectError").And.Contain("NO_DEVICE"));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: invalid parameters return invalid request.
        /// </summary>
        [TestCase("/sample?count=-10")]
        [TestCase("/sample?count=abc")]
        [TestCase("/sample?from=-1")]
        [TestCase("/current?at=abc")]
        [TestCase("/current?at=1&interval=1000")]
        [TestCase("/current?interval=soon")]
        public async Task InvalidParameters_ReturnInvalidRequest(string path)
        {
            var (status, body) = await GetAsync(path);

            Assert.That(status, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(body, Does.Contain("INVALID_REQUEST"));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: count larger than buffer returns out of range.
        /// </summary>
        [Test]
        public async Task CountLargerThanBuffer_ReturnsOutOfRange()
        {
            var (status, body) = await GetAsync("/sample?count=999999999");

            Assert.That(status, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(body, Does.Contain("OUT_OF_RANGE"));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: post when put is not allowed is forbidden.
        /// </summary>
        [Test]
        public async Task Post_WhenPutIsNotAllowed_IsForbidden()
        {
            using var content = new StringContent("<File assetId=\"A1\" />", Encoding.UTF8, "text/xml");
            using var response = await _http.PostAsync(_server.BaseUrl + "/A1?device=d1&type=File", content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
            Assert.That(_server.Agent.GetAssetsResponseDocument().Assets, Is.Empty);
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: compressed response has body.
        /// </summary>
        [TestCase("br")]
        [TestCase("deflate")]
        [TestCase("gzip")]
        public async Task CompressedResponse_HasBody(string encoding)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, _server.BaseUrl + "/probe");
            request.Headers.TryAddWithoutValidation("Accept-Encoding", encoding);
            using var response = await _http.SendAsync(request);

            Assert.That(response.Content.Headers.ContentEncoding, Does.Contain(encoding));

            var compressed = await response.Content.ReadAsByteArrayAsync();
            Assert.That(compressed, Is.Not.Empty);

            using var input = new MemoryStream(compressed);
            using Stream decoder = encoding == "br" ? new BrotliStream(input, CompressionMode.Decompress)
                : encoding == "deflate" ? new DeflateStream(input, CompressionMode.Decompress)
                : new GZipStream(input, CompressionMode.Decompress);
            using var reader = new StreamReader(decoder);
            Assert.That(reader.ReadToEnd(), Does.Contain("MTConnectDevices"));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: current stream delivers chunks without byte order mark.
        /// </summary>
        [Test]
        public async Task CurrentStream_DeliversChunksWithoutByteOrderMark()
        {
            var bytes = await ReadStreamAsync("/current?interval=100", TimeSpan.FromSeconds(8),
                received => CountOccurrences(Encoding.UTF8.GetString(received), "Content-length:") >= 2);

            var text = Encoding.UTF8.GetString(bytes);
            Assert.That(CountOccurrences(text, "Content-length:"), Is.GreaterThanOrEqualTo(2), "current stream chunks must reach the client as they are produced");
            Assert.That(bytes.Take(2).Select(b => (char)b), Is.EqualTo(new[] { '-', '-' }), "a chunk must start with its boundary, not a UTF-8 byte order mark");
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: agent wide sample stream applies path filter.
        /// </summary>
        [Test]
        public async Task AgentWideSampleStream_AppliesPathFilter()
        {
            var path = Uri.EscapeDataString("//DataItem[@id=\"p1\"]");
            var bytes = await ReadStreamAsync($"/sample?interval=100&path={path}", TimeSpan.FromSeconds(8),
                received => Encoding.UTF8.GetString(received).Contains("</MTConnectStreams>"));

            var text = Encoding.UTF8.GetString(bytes);
            Assert.That(text, Does.Contain("dataItemId=\"p1\""));
            Assert.That(text, Does.Not.Contain("dataItemId=\"p2\""));
            Assert.That(text, Does.Not.Contain("dataItemId=\"q1\""));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: long heartbeat does not send a heartbeat every interval.
        /// </summary>
        [Test]
        public async Task LongHeartbeat_DoesNotSendAHeartbeatEveryInterval()
        {
            // heartbeat * 10000 overflowed int above ~214 seconds, so a heartbeat went out on every interval
            var bytes = await ReadStreamAsync("/d1/sample?interval=50&heartbeat=300000", TimeSpan.FromSeconds(2));

            var text = Encoding.UTF8.GetString(bytes);
            Assert.That(CountOccurrences(text, "Content-length:"), Is.LessThanOrEqualTo(2));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: default version applies to sample and assets.
        /// </summary>
        [Test]
        public async Task DefaultVersion_AppliesToSampleAndAssets()
        {
            var (_, sample) = await GetAsync("/sample");
            var (_, assets) = await GetAsync("/assets");

            Assert.That(sample, Does.Contain("urn:mtconnect.org:MTConnectStreams:2.0"));
            Assert.That(assets, Does.Contain("urn:mtconnect.org:MTConnectAssets:2.0"));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: accept header with several media ranges selects first match.
        /// </summary>
        [Test]
        public async Task AcceptHeaderWithSeveralMediaRanges_SelectsFirstMatch()
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, _server.BaseUrl + "/probe");
            request.Headers.TryAddWithoutValidation("Accept", "application/json, */*;q=0.8");
            using var response = await _http.SendAsync(request);

            Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("application/json"));
        }


        /// <summary>
        /// Pins the behaviour expressed by the test name: static file directory resolves with platform separators.
        /// </summary>
        [Test]
        public void StaticFileDirectory_ResolvesWithPlatformSeparators()
        {
            var configuration = new HttpServerConfiguration
            {
                Files = new List<FileConfiguration> { new FileConfiguration { Location = "styles", Path = "wwwroot\\styles" } }
            };

            var handlerType = typeof(MTConnectHttpServer).Assembly.GetType("MTConnect.Servers.MTConnectStaticResponseHandler", throwOnError: true)!;
            var handler = Activator.CreateInstance(handlerType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new object[] { configuration, _server.Agent }, null);
            var resolve = handlerType.GetMethod("ResolveLocalPath", BindingFlags.Instance | BindingFlags.NonPublic)!;

            var localPath = (string)resolve.Invoke(handler, new object[] { "/styles/a.css" })!;

            Assert.That(localPath, Is.EqualTo(Path.Combine("wwwroot", "styles", "a.css")));
        }
    }
}
