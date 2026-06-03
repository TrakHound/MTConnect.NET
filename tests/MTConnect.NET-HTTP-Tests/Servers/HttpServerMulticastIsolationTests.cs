// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using MTConnect.Http;
using MTConnect.Servers.Http;
using NUnit.Framework;

namespace MTConnect.Tests.Http.Servers
{
    /// <summary>
    /// Pins the multicast-isolation contract for the delegate shapes used by
    /// <c>MTConnectHttpResponseHandler</c> (ResponseSent, ClientConnected,
    /// ClientDisconnected, ClientException) and <c>MTConnectHttpServerStream</c>
    /// (StreamStarted, StreamStopped, StreamException, DocumentReceived,
    /// HeartbeatReceived). Both classes declare their events as
    /// <see cref="EventHandler{T}"/> or <see cref="EventHandler"/>; after
    /// migration all raise sites use
    /// <see cref="MulticastIsolation.Raise{T}"/> / <see cref="MulticastIsolation.Raise"/>.
    ///
    /// <para>
    /// <c>MTConnectHttpResponseHandler</c> is <c>internal abstract</c>; these
    /// tests verify the isolation guarantee by exercising the helper directly
    /// with the same delegate signatures that the handler's raise sites use,
    /// without requiring a concrete subclass or a live HTTP connection.
    /// </para>
    /// </summary>
    [TestFixture]
    public class HttpServerMulticastIsolationTests
    {
        // -----------------------------------------------------------------------
        // MTConnectHttpResponse raise sites
        // (ResponseSent on MTConnectHttpResponseHandler)
        // -----------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: a second EventHandler{MTConnectHttpResponse} subscriber fires even when the first throws, covering ResponseSent on MTConnectHttpResponseHandler.</summary>
        [Test]
        public void HttpResponseHandler_ResponseSent_FiresAllSubscribersWhenOneThrows()
        {
            var received = new List<string>();
            var response = new MTConnectHttpResponse { ContentType = "application/xml" };

            EventHandler<MTConnectHttpResponse>? handler = null;
            handler += (_, _) => throw new InvalidOperationException("first ResponseSent subscriber throws");
            handler += (_, r) => received.Add(r.ContentType ?? "null");

            MulticastIsolation.Raise(handler!, this, response, null);

            Assert.That(received, Is.EqualTo(new[] { "application/xml" }));
        }

        /// <summary>Pins the behavior expressed by the test name: with null InternalError, a fault from a ResponseSent subscriber is swallowed without escaping to the caller.</summary>
        [Test]
        public void HttpResponseHandler_ResponseSent_NullInternalErrorSwallowsFault()
        {
            var response = new MTConnectHttpResponse { ContentType = "application/xml" };
            EventHandler<MTConnectHttpResponse> handler = (_, _) => throw new InvalidOperationException("ResponseSent fault");

            Assert.DoesNotThrow(() => MulticastIsolation.Raise(handler, this, response, null));
        }

        // -----------------------------------------------------------------------
        // IHttpRequest raise sites
        // (ClientConnected on MTConnectHttpResponseHandler)
        // -----------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: a second EventHandler{IHttpRequest} subscriber fires even when the first throws, covering ClientConnected on MTConnectHttpResponseHandler.</summary>
        [Test]
        public void HttpResponseHandler_ClientConnected_FiresAllSubscribersWhenOneThrows()
        {
            var firedCount = 0;

            EventHandler<IHttpRequest>? handler = null;
            handler += (_, _) => throw new InvalidOperationException("first ClientConnected subscriber throws");
            handler += (_, _) => firedCount++;

            MulticastIsolation.Raise(handler!, this, (IHttpRequest)null, null);

            Assert.That(firedCount, Is.EqualTo(1));
        }

        // -----------------------------------------------------------------------
        // ClientDisconnected raise sites
        // (EventHandler<string> on MTConnectHttpResponseHandler)
        // -----------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: a second EventHandler{string} subscriber fires even when the first throws, covering ClientDisconnected on MTConnectHttpResponseHandler.</summary>
        [Test]
        public void HttpResponseHandler_ClientDisconnected_FiresAllSubscribersWhenOneThrows()
        {
            var received = new List<string>();

            EventHandler<string>? handler = null;
            handler += (_, _) => throw new InvalidOperationException("first ClientDisconnected subscriber throws");
            handler += (_, endpoint) => received.Add(endpoint ?? "null");

            MulticastIsolation.Raise(handler!, this, "127.0.0.1:5000", null);

            Assert.That(received, Is.EqualTo(new[] { "127.0.0.1:5000" }));
        }

        // -----------------------------------------------------------------------
        // ClientException raise sites
        // (EventHandler<Exception> on MTConnectHttpResponseHandler)
        // -----------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: a second EventHandler{Exception} subscriber fires even when the first throws, covering ClientException on MTConnectHttpResponseHandler.</summary>
        [Test]
        public void HttpResponseHandler_ClientException_FiresAllSubscribersWhenOneThrows()
        {
            var received = new List<string>();
            var payload = new InvalidOperationException("http-context-error");

            EventHandler<Exception>? handler = null;
            handler += (_, _) => throw new InvalidOperationException("first ClientException subscriber throws");
            handler += (_, ex) => received.Add(ex.Message);

            MulticastIsolation.Raise(handler!, this, payload, null);

            Assert.That(received, Is.EqualTo(new[] { "http-context-error" }));
        }

        // -----------------------------------------------------------------------
        // StreamStarted / StreamStopped raise sites
        // (EventHandler<string> on MTConnectHttpServerStream)
        // -----------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: a second EventHandler{string} subscriber fires even when the first throws, covering StreamStarted and StreamStopped on MTConnectHttpServerStream.</summary>
        [Test]
        public void HttpServerStream_StreamStarted_FiresAllSubscribersWhenOneThrows()
        {
            var received = new List<string>();

            EventHandler<string>? handler = null;
            handler += (_, _) => throw new InvalidOperationException("first StreamStarted subscriber throws");
            handler += (_, id) => received.Add(id ?? "null");

            MulticastIsolation.Raise(handler!, this, "stream-id-1", null);

            Assert.That(received, Is.EqualTo(new[] { "stream-id-1" }));
        }

        /// <summary>Pins the behavior expressed by the test name: with null InternalError, a fault from a StreamStopped subscriber is swallowed without escaping.</summary>
        [Test]
        public void HttpServerStream_StreamStopped_NullInternalErrorSwallowsFault()
        {
            EventHandler<string> handler = (_, _) => throw new InvalidOperationException("StreamStopped fault");

            Assert.DoesNotThrow(() => MulticastIsolation.Raise(handler, this, "stream-id-1", null));
        }

        // -----------------------------------------------------------------------
        // StreamException raise sites
        // (EventHandler<Exception> on MTConnectHttpServerStream — serves as its own error sink)
        // -----------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: a second EventHandler{Exception} subscriber fires even when the first throws, covering StreamException on MTConnectHttpServerStream.</summary>
        [Test]
        public void HttpServerStream_StreamException_FiresAllSubscribersWhenOneThrows()
        {
            var received = new List<string>();
            var payload = new InvalidOperationException("stream-broke");

            EventHandler<Exception>? handler = null;
            handler += (_, _) => throw new InvalidOperationException("first StreamException subscriber throws");
            handler += (_, ex) => received.Add(ex.Message);

            MulticastIsolation.Raise(handler!, this, payload, null);

            Assert.That(received, Is.EqualTo(new[] { "stream-broke" }));
        }

        // -----------------------------------------------------------------------
        // DocumentReceived / HeartbeatReceived raise sites
        // (EventHandler<MTConnectHttpStreamArgs> on MTConnectHttpServerStream)
        // -----------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: a second EventHandler{MTConnectHttpStreamArgs} subscriber fires even when the first throws, covering DocumentReceived and HeartbeatReceived on MTConnectHttpServerStream.</summary>
        [Test]
        public void HttpServerStream_DocumentReceived_FiresAllSubscribersWhenOneThrows()
        {
            var received = new List<string>();
            var args = new MTConnectHttpStreamArgs("stream-id-2", System.IO.Stream.Null, 42.5);

            EventHandler<MTConnectHttpStreamArgs>? handler = null;
            handler += (_, _) => throw new InvalidOperationException("first DocumentReceived subscriber throws");
            handler += (_, a) => received.Add(a.StreamId);

            MulticastIsolation.Raise(handler!, this, args, null);

            Assert.That(received, Is.EqualTo(new[] { "stream-id-2" }));
        }

        /// <summary>Pins the behavior expressed by the test name: with null InternalError, a fault from a HeartbeatReceived subscriber is swallowed without escaping.</summary>
        [Test]
        public void HttpServerStream_HeartbeatReceived_NullInternalErrorSwallowsFault()
        {
            var args = new MTConnectHttpStreamArgs("stream-id-2", System.IO.Stream.Null, 42.5);
            EventHandler<MTConnectHttpStreamArgs> handler = (_, _) => throw new InvalidOperationException("HeartbeatReceived fault");

            Assert.DoesNotThrow(() => MulticastIsolation.Raise(handler, this, args, null));
        }

        // -----------------------------------------------------------------------
        // Null-handler guard
        // -----------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: Raise with a null EventHandler{string} handler is a safe no-op covering the no-subscriber case for stream events at runtime.</summary>
        [Test]
        public void HttpServerStream_NullGenericHandler_DoesNotThrow()
        {
            EventHandler<string>? handler = null;

            Assert.DoesNotThrow(() => MulticastIsolation.Raise(handler, this, "x", null));
        }
    }
}
