// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MTConnect.Agents;
using MTConnect.Clients;
using MTConnect.Errors;
using MTConnect.Formatters;
using MTConnect.Tests.Http.TestHelpers;
using NUnit.Framework;

namespace MTConnect.Tests.Http.Clients
{
    /// <summary>
    /// Behaviour of the HTTP clients: multipart stream framing (back-to-back parts, header order, truncated
    /// parts, Error parts), Error documents on 4xx responses, the Timeout property, ContentType following the
    /// document format, observations raised once per document, and restarting a running client.
    /// </summary>
    [TestFixture]
    [Category("HttpClientBehavior")]
    public class HttpClientBehaviorTests
    {
        private const string Boundary = "b0undary";

        private static byte[] StreamsXml()
        {
            var agent = new MTConnectAgentBroker();
            var document = agent.GetDeviceStreamsResponseDocument();
            var result = ResponseDocumentFormatter.Format(DocumentFormat.XML, ref document);
            return ReadAll(result.Content);
        }

        private static byte[] ErrorXml(ErrorCode errorCode)
        {
            var agent = new MTConnectAgentBroker();
            var result = ResponseDocumentFormatter.Format(DocumentFormat.XML, agent.GetErrorResponseDocument(errorCode, "test error"));
            return ReadAll(result.Content);
        }

        private static byte[] ReadAll(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            using var copy = new MemoryStream();
            stream.CopyTo(copy);
            return copy.ToArray();
        }

        private static byte[] Part(byte[] body, bool contentLengthFirst = false)
        {
            var headers = contentLengthFirst
                ? $"--{Boundary}\r\nContent-length: {body.Length}\r\nContent-type: text/xml\r\n\r\n"
                : $"--{Boundary}\r\nContent-type: text/xml\r\nContent-length: {body.Length}\r\n\r\n";
            return CannedHttpServer.Concat(Encoding.ASCII.GetBytes(headers), body, Encoding.ASCII.GetBytes("\r\n"));
        }


        /// <summary>
        /// Pins the behaviour expressed by the test name: stream back to back parts in any header order are all raised.
        /// </summary>
        [Test]
        [Timeout(15000)]
        public async Task Stream_BackToBackParts_InAnyHeaderOrder_AreAllRaised()
        {
            var xml = StreamsXml();
            var response = CannedHttpServer.Concat(CannedHttpServer.MultipartResponseHead(Boundary), Part(xml), Part(xml, contentLengthFirst: true), Part(xml));
            using var server = CannedHttpServer.Respond(response);

            var documents = 0;
            var stream = new MTConnectHttpClientStream(server.BaseUrl + "/sample");
            stream.DocumentReceived += (s, doc) => Interlocked.Increment(ref documents);

            await stream.Run(CancellationToken.None);

            Assert.That(documents, Is.EqualTo(3));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: stream truncated part ends instead of spinning.
        /// </summary>
        [Test]
        [Timeout(15000)]
        public async Task Stream_TruncatedPart_EndsInsteadOfSpinning()
        {
            // The Agent closes the connection part-way through a part
            var response = CannedHttpServer.Concat(
                CannedHttpServer.MultipartResponseHead(Boundary),
                Encoding.ASCII.GetBytes($"--{Boundary}\r\nContent-type: text/xml\r\nContent-length: 5000\r\n\r\n<MTConnectStreams"));
            using var server = CannedHttpServer.Respond(response);

            var stopped = false;
            var stream = new MTConnectHttpClientStream(server.BaseUrl + "/sample");
            stream.Stopped += (s, e) => stopped = true;

            await stream.Run(CancellationToken.None);

            Assert.That(stopped, Is.True);
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: stream error part raises error received.
        /// </summary>
        [Test]
        [Timeout(15000)]
        public async Task Stream_ErrorPart_RaisesErrorReceived()
        {
            var response = CannedHttpServer.Concat(CannedHttpServer.MultipartResponseHead(Boundary), Part(ErrorXml(ErrorCode.OUT_OF_RANGE)));
            using var server = CannedHttpServer.Respond(response);

            IErrorResponseDocument? error = null;
            var formatErrors = 0;
            var stream = new MTConnectHttpClientStream(server.BaseUrl + "/sample");
            stream.ErrorReceived += (s, doc) => error = doc;
            stream.FormatError += (s, r) => Interlocked.Increment(ref formatErrors);

            await stream.Run(CancellationToken.None);

            Assert.That(error, Is.Not.Null);
            Assert.That(error!.Errors.Single().ErrorCode, Is.EqualTo(ErrorCode.OUT_OF_RANGE));
            Assert.That(formatErrors, Is.EqualTo(0));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: probe client error document with404 raises MT connect error.
        /// </summary>
        [Test]
        [Timeout(15000)]
        public async Task ProbeClient_ErrorDocumentWith404_RaisesMTConnectError()
        {
            using var server = CannedHttpServer.Respond(CannedHttpServer.HttpResponse("404 Not Found", "application/xml", ErrorXml(ErrorCode.NO_DEVICE)));

            IErrorResponseDocument? error = null;
            Exception? connectionError = null;
            var client = new MTConnectHttpProbeClient(server.Authority, "nope");
            client.MTConnectError += (s, doc) => error = doc;
            client.ConnectionError += (s, ex) => connectionError = ex;

            var document = await client.GetAsync(CancellationToken.None);

            Assert.That(document, Is.Null);
            Assert.That(error, Is.Not.Null, connectionError?.Message);
            Assert.That(error!.Errors.Single().ErrorCode, Is.EqualTo(ErrorCode.NO_DEVICE));
            Assert.That(connectionError, Is.Null);
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: probe client sync get reports error document too.
        /// </summary>
        [Test]
        [Timeout(15000)]
        public void ProbeClient_SyncGet_ReportsErrorDocumentToo()
        {
            using var server = CannedHttpServer.Respond(CannedHttpServer.HttpResponse("404 Not Found", "application/xml", ErrorXml(ErrorCode.NO_DEVICE)));

            IErrorResponseDocument? error = null;
            var client = new MTConnectHttpProbeClient(server.Authority, "nope");
            client.MTConnectError += (s, doc) => error = doc;

            Assert.That(client.Get(), Is.Null);
            Assert.That(error, Is.Not.Null);
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: probe client honours timeout.
        /// </summary>
        [Test]
        [Timeout(15000)]
        public async Task ProbeClient_HonoursTimeout()
        {
            using var server = CannedHttpServer.NeverRespond();

            Exception? connectionError = null;
            var client = new MTConnectHttpProbeClient(server.Authority) { Timeout = 300 };
            client.ConnectionError += (s, ex) => connectionError = ex;

            var started = DateTime.UtcNow;
            var document = await client.GetAsync(CancellationToken.None);

            Assert.That(document, Is.Null);
            Assert.That(connectionError, Is.InstanceOf<TimeoutException>());
            Assert.That(DateTime.UtcNow - started, Is.LessThan(TimeSpan.FromSeconds(5)));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: http client content type follows document format.
        /// </summary>
        [Test]
        public void HttpClient_ContentType_FollowsDocumentFormat()
        {
            var client = new MTConnectHttpClient("127.0.0.1:5000", documentFormat: DocumentFormat.JSON);

            Assert.That(client.ContentType, Is.EqualTo("application/json"));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: http client raises each observation once per current.
        /// </summary>
        [Test]
        [Timeout(30000)]
        public void HttpClient_RaisesEachObservationOncePerCurrent()
        {
            using var server = new LoopbackAgentServer(); // two devices

            // Polling mode: each Current raises CurrentReceived, then ObservationReceived for its observations
            var firstCurrentObservations = -1;
            var raisedForFirst = 0;
            var currents = 0;
            var done = new ManualResetEventSlim();
            var gate = new object();

            var client = new MTConnectHttpClient(server.BaseUrl) { UseStreaming = false, Interval = 200 };
            client.CurrentReceived += (s, doc) =>
            {
                lock (gate)
                {
                    currents++;
                    if (currents == 1) firstCurrentObservations = doc.GetObservations().Count();
                    else if (currents == 2) done.Set();
                }
            };

            // Count the observations raised for the first Current, before the second one arrives
            client.ObservationReceived += (s, observation) => { lock (gate) { if (currents == 1) raisedForFirst++; } };

            client.Start();
            Assert.That(done.Wait(TimeSpan.FromSeconds(20)), Is.True, "two Current documents were not received");
            client.Stop();

            Assert.That(firstCurrentObservations, Is.GreaterThan(0));
            Assert.That(raisedForFirst, Is.EqualTo(firstCurrentObservations));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: http client starting again stops the previous worker.
        /// </summary>
        [Test]
        [Timeout(15000)]
        public void HttpClient_StartingAgain_StopsThePreviousWorker()
        {
            using var server = CannedHttpServer.NeverRespond();

            var stopped = 0;
            var client = new MTConnectHttpClient(server.Authority) { Timeout = 60000 };
            client.ClientStopped += (s, e) => Interlocked.Increment(ref stopped);

            client.Start();
            Thread.Sleep(300);
            client.Start();

            var deadline = DateTime.UtcNow.AddSeconds(5);
            while (Volatile.Read(ref stopped) == 0 && DateTime.UtcNow < deadline) Thread.Sleep(50);

            Assert.That(Volatile.Read(ref stopped), Is.EqualTo(1), "the first worker must stop when the client is started again");

            client.Stop();
        }
    }
}
