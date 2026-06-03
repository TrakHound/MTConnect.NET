// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Clients;
using NUnit.Framework;
using System;
using System.Threading;

namespace MTConnect.Tests.Http.Clients
{
    /// <summary>
    /// Pins multicast-isolation across the events on <see cref="MTConnectHttpClient"/>
    /// whose raise-sites still used the unsafe <c>?.Invoke</c> pattern after the
    /// sibling <see cref="MulticastIsolationTests"/> work landed: the non-generic
    /// <see cref="EventHandler"/> lifecycle events (ClientStarting, ClientStarted,
    /// ClientStopping, ClientStopped, ResponseReceived); the stream lifecycle
    /// events that fire from the existing run path (StreamStarted, StreamStopped);
    /// and ConnectionError, drivable from a closed loopback port. Two halves of
    /// the contract per event: a throwing subscriber must not starve later
    /// subscribers in the invocation list, and a fault from the InternalError
    /// handler itself must also be swallowed so the fan-out continues.
    /// </summary>
    /// <remarks>
    /// StreamStarting, StreamStopping, MTConnectError, FormatError, and
    /// InternalError raise-sites are not driven end-to-end here. StreamStarting
    /// and StreamStopping are unreachable from the existing client code path
    /// (the outer client never calls <c>_stream.Start()</c> or
    /// <c>_stream.Stop()</c>); MTConnectError needs an MTConnect protocol-error
    /// document that the healthy embedded agent does not produce; FormatError
    /// needs a malformed wire response with the same constraint; InternalError's
    /// only raise-site is the Worker's catch-all and lacks a deterministic
    /// injection path. All five raise-sites route through the same shared
    /// private RaiseEvent helper exercised by the events covered here; the fix
    /// at each site is the identical one-line swap from <c>?.Invoke</c> to
    /// <c>RaiseEvent</c>.
    /// </remarks>
    [TestFixture]
    public class NonGenericMulticastIsolationTests : HttpClientFixture
    {
        // Generous CI-safe bounds. Lifecycle events resolve well under a second;
        // the stream lifecycle requires the probe + current round trip plus the
        // stream run to begin, which the existing fixture pins under thirty
        // seconds. The same envelope covers the bad-port ConnectionError tests,
        // which fall through the OS connect-refused path inside Timeout.
        private const int EventWaitTimeoutMs = 30000;

        // Hostname intentionally distinct from the healthy fixture so the
        // ConnectionError tests do not race a half-bound listener. 127.0.0.1
        // with a port the OS never assigned will refuse instantly.
        private const string UnreachableHost = "127.0.0.1";
        private const int UnreachablePort = 1; // privileged port, never bound by the fixture.


        // ---------------------------------------------------------------------
        // Lifecycle events on the client itself. ClientStarting and
        // ClientStopping fire synchronously on the calling thread; ClientStarted
        // and ClientStopped fire from the Worker. Multicast isolation is the
        // raise-site invariant under test.
        // ---------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: client starting fires for all subscribers when one throws.</summary>
        [Test]
        public void ClientStartingFiresForAllSubscribersWhenOneThrows()
        {
            var client = new MTConnectHttpClient(Hostname, Port);

            client.ClientStarting += (_, _) => throw new InvalidOperationException("first handler throws");

            using var recorded = new ManualResetEventSlim(false);
            client.ClientStarting += (_, _) => recorded.Set();

            try
            {
                client.Start();

                Assert.That(recorded.Wait(EventWaitTimeoutMs), Is.True,
                    "subscribers after a throwing one must still receive ClientStarting");
            }
            finally
            {
                client.Stop();
            }
        }

        /// <summary>Pins the behavior expressed by the test name: internal error handler throwing does not break client starting fan out.</summary>
        [Test]
        public void InternalErrorHandlerThrowingDoesNotBreakClientStartingFanOut()
        {
            var client = new MTConnectHttpClient(Hostname, Port);

            client.InternalError += (_, _) => throw new InvalidOperationException("InternalError throws");
            client.ClientStarting += (_, _) => throw new InvalidOperationException("first ClientStarting throws");

            using var recorded = new ManualResetEventSlim(false);
            client.ClientStarting += (_, _) => recorded.Set();

            try
            {
                client.Start();

                Assert.That(recorded.Wait(EventWaitTimeoutMs), Is.True,
                    "InternalError throwing must not break the ClientStarting fan-out");
            }
            finally
            {
                client.Stop();
            }
        }


        /// <summary>Pins the behavior expressed by the test name: client started fires for all subscribers when one throws.</summary>
        [Test]
        public void ClientStartedFiresForAllSubscribersWhenOneThrows()
        {
            var client = new MTConnectHttpClient(Hostname, Port);

            client.ClientStarted += (_, _) => throw new InvalidOperationException("first handler throws");

            using var recorded = new ManualResetEventSlim(false);
            client.ClientStarted += (_, _) => recorded.Set();

            try
            {
                client.Start();

                Assert.That(recorded.Wait(EventWaitTimeoutMs), Is.True,
                    "subscribers after a throwing one must still receive ClientStarted");
            }
            finally
            {
                client.Stop();
            }
        }

        /// <summary>Pins the behavior expressed by the test name: internal error handler throwing does not break client started fan out.</summary>
        [Test]
        public void InternalErrorHandlerThrowingDoesNotBreakClientStartedFanOut()
        {
            var client = new MTConnectHttpClient(Hostname, Port);

            client.InternalError += (_, _) => throw new InvalidOperationException("InternalError throws");
            client.ClientStarted += (_, _) => throw new InvalidOperationException("first ClientStarted throws");

            using var recorded = new ManualResetEventSlim(false);
            client.ClientStarted += (_, _) => recorded.Set();

            try
            {
                client.Start();

                Assert.That(recorded.Wait(EventWaitTimeoutMs), Is.True,
                    "InternalError throwing must not break the ClientStarted fan-out");
            }
            finally
            {
                client.Stop();
            }
        }


        /// <summary>Pins the behavior expressed by the test name: client stopping fires for all subscribers when one throws.</summary>
        [Test]
        public void ClientStoppingFiresForAllSubscribersWhenOneThrows()
        {
            var client = new MTConnectHttpClient(Hostname, Port);

            client.ClientStopping += (_, _) => throw new InvalidOperationException("first handler throws");

            using var recorded = new ManualResetEventSlim(false);
            client.ClientStopping += (_, _) => recorded.Set();

            client.Start();
            client.Stop();

            Assert.That(recorded.Wait(EventWaitTimeoutMs), Is.True,
                "subscribers after a throwing one must still receive ClientStopping");
        }

        /// <summary>Pins the behavior expressed by the test name: internal error handler throwing does not break client stopping fan out.</summary>
        [Test]
        public void InternalErrorHandlerThrowingDoesNotBreakClientStoppingFanOut()
        {
            var client = new MTConnectHttpClient(Hostname, Port);

            client.InternalError += (_, _) => throw new InvalidOperationException("InternalError throws");
            client.ClientStopping += (_, _) => throw new InvalidOperationException("first ClientStopping throws");

            using var recorded = new ManualResetEventSlim(false);
            client.ClientStopping += (_, _) => recorded.Set();

            client.Start();
            client.Stop();

            Assert.That(recorded.Wait(EventWaitTimeoutMs), Is.True,
                "InternalError throwing must not break the ClientStopping fan-out");
        }


        /// <summary>Pins the behavior expressed by the test name: client stopped fires for all subscribers when one throws.</summary>
        [Test]
        public void ClientStoppedFiresForAllSubscribersWhenOneThrows()
        {
            var client = new MTConnectHttpClient(Hostname, Port);

            client.ClientStopped += (_, _) => throw new InvalidOperationException("first handler throws");

            using var recorded = new ManualResetEventSlim(false);
            client.ClientStopped += (_, _) => recorded.Set();

            // Wait for ClientStarted before stopping so the Worker has actually
            // entered the loop — calling Stop() before the Task.Run launches the
            // Worker leaves ClientStopped permanently un-raised.
            using var clientStarted = new ManualResetEventSlim(false);
            client.ClientStarted += (_, _) => clientStarted.Set();

            client.Start();
            Assert.That(clientStarted.Wait(EventWaitTimeoutMs), Is.True,
                "Worker did not start before the test could stop it");
            client.Stop();

            Assert.That(recorded.Wait(EventWaitTimeoutMs), Is.True,
                "subscribers after a throwing one must still receive ClientStopped");
        }

        /// <summary>Pins the behavior expressed by the test name: internal error handler throwing does not break client stopped fan out.</summary>
        [Test]
        public void InternalErrorHandlerThrowingDoesNotBreakClientStoppedFanOut()
        {
            var client = new MTConnectHttpClient(Hostname, Port);

            client.InternalError += (_, _) => throw new InvalidOperationException("InternalError throws");
            client.ClientStopped += (_, _) => throw new InvalidOperationException("first ClientStopped throws");

            using var recorded = new ManualResetEventSlim(false);
            client.ClientStopped += (_, _) => recorded.Set();

            using var clientStarted = new ManualResetEventSlim(false);
            client.ClientStarted += (_, _) => clientStarted.Set();

            client.Start();
            Assert.That(clientStarted.Wait(EventWaitTimeoutMs), Is.True,
                "Worker did not start before the test could stop it");
            client.Stop();

            Assert.That(recorded.Wait(EventWaitTimeoutMs), Is.True,
                "InternalError throwing must not break the ClientStopped fan-out");
        }


        /// <summary>Pins the behavior expressed by the test name: response received fires for all subscribers when one throws.</summary>
        [Test]
        public void ResponseReceivedFiresForAllSubscribersWhenOneThrows()
        {
            var client = new MTConnectHttpClient(Hostname, Port);

            client.ResponseReceived += (_, _) => throw new InvalidOperationException("first handler throws");

            using var recorded = new ManualResetEventSlim(false);
            client.ResponseReceived += (_, _) => recorded.Set();

            try
            {
                client.Start();

                Assert.That(recorded.Wait(EventWaitTimeoutMs), Is.True,
                    "subscribers after a throwing one must still receive ResponseReceived");
            }
            finally
            {
                client.Stop();
            }
        }

        /// <summary>Pins the behavior expressed by the test name: internal error handler throwing does not break response received fan out.</summary>
        [Test]
        public void InternalErrorHandlerThrowingDoesNotBreakResponseReceivedFanOut()
        {
            var client = new MTConnectHttpClient(Hostname, Port);

            client.InternalError += (_, _) => throw new InvalidOperationException("InternalError throws");
            client.ResponseReceived += (_, _) => throw new InvalidOperationException("first ResponseReceived throws");

            using var recorded = new ManualResetEventSlim(false);
            client.ResponseReceived += (_, _) => recorded.Set();

            try
            {
                client.Start();

                Assert.That(recorded.Wait(EventWaitTimeoutMs), Is.True,
                    "InternalError throwing must not break the ResponseReceived fan-out");
            }
            finally
            {
                client.Stop();
            }
        }


        // ---------------------------------------------------------------------
        // Stream lifecycle events. The inner _stream forwards each of these
        // through a one-liner lambda on the outer client; those lambdas were the
        // remaining ?.Invoke raise-sites covered by this PR. Only StreamStarted
        // and StreamStopped fire from the existing _stream.Run path
        // (MTConnectHttpClient never calls _stream.Start() or _stream.Stop(),
        // so the Starting / Stopping raise-sites are unreachable from this code
        // path); their multicast-isolation contract is exercised here, while
        // the Starting / Stopping helper swap is verified by the same shared
        // RaiseEvent helper.
        // ---------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: stream started fires for all subscribers when one throws.</summary>
        [Test]
        public void StreamStartedFiresForAllSubscribersWhenOneThrows()
        {
            var client = new MTConnectHttpClient(Hostname, Port);

            client.StreamStarted += (_, _) => throw new InvalidOperationException("first handler throws");

            using var recorded = new ManualResetEventSlim(false);
            client.StreamStarted += (_, url) =>
            {
                if (!string.IsNullOrEmpty(url)) recorded.Set();
            };

            try
            {
                client.Start();

                Assert.That(recorded.Wait(EventWaitTimeoutMs), Is.True,
                    "subscribers after a throwing one must still receive StreamStarted");
            }
            finally
            {
                client.Stop();
            }
        }

        /// <summary>Pins the behavior expressed by the test name: internal error handler throwing does not break stream started fan out.</summary>
        [Test]
        public void InternalErrorHandlerThrowingDoesNotBreakStreamStartedFanOut()
        {
            var client = new MTConnectHttpClient(Hostname, Port);

            client.InternalError += (_, _) => throw new InvalidOperationException("InternalError throws");
            client.StreamStarted += (_, _) => throw new InvalidOperationException("first StreamStarted throws");

            using var recorded = new ManualResetEventSlim(false);
            client.StreamStarted += (_, url) =>
            {
                if (!string.IsNullOrEmpty(url)) recorded.Set();
            };

            try
            {
                client.Start();

                Assert.That(recorded.Wait(EventWaitTimeoutMs), Is.True,
                    "InternalError throwing must not break the StreamStarted fan-out");
            }
            finally
            {
                client.Stop();
            }
        }


        /// <summary>Pins the behavior expressed by the test name: stream stopped fires for all subscribers when one throws.</summary>
        [Test]
        public void StreamStoppedFiresForAllSubscribersWhenOneThrows()
        {
            var client = new MTConnectHttpClient(Hostname, Port);

            client.StreamStopped += (_, _) => throw new InvalidOperationException("first handler throws");

            using var recorded = new ManualResetEventSlim(false);
            client.StreamStopped += (_, url) =>
            {
                if (!string.IsNullOrEmpty(url)) recorded.Set();
            };

            using var streamStarted = new ManualResetEventSlim(false);
            client.StreamStarted += (_, _) => streamStarted.Set();

            client.Start();
            Assert.That(streamStarted.Wait(EventWaitTimeoutMs), Is.True,
                "Stream did not start before the test could stop it");
            client.Stop();

            Assert.That(recorded.Wait(EventWaitTimeoutMs), Is.True,
                "subscribers after a throwing one must still receive StreamStopped");
        }

        /// <summary>Pins the behavior expressed by the test name: internal error handler throwing does not break stream stopped fan out.</summary>
        [Test]
        public void InternalErrorHandlerThrowingDoesNotBreakStreamStoppedFanOut()
        {
            var client = new MTConnectHttpClient(Hostname, Port);

            client.InternalError += (_, _) => throw new InvalidOperationException("InternalError throws");
            client.StreamStopped += (_, _) => throw new InvalidOperationException("first StreamStopped throws");

            using var recorded = new ManualResetEventSlim(false);
            client.StreamStopped += (_, url) =>
            {
                if (!string.IsNullOrEmpty(url)) recorded.Set();
            };

            using var streamStarted = new ManualResetEventSlim(false);
            client.StreamStarted += (_, _) => streamStarted.Set();

            client.Start();
            Assert.That(streamStarted.Wait(EventWaitTimeoutMs), Is.True,
                "Stream did not start before the test could stop it");
            client.Stop();

            Assert.That(recorded.Wait(EventWaitTimeoutMs), Is.True,
                "InternalError throwing must not break the StreamStopped fan-out");
        }


        // ---------------------------------------------------------------------
        // ConnectionError. Pointing the client at a port the fixture never
        // bound forces the probe sub-client's connect to refuse; the forwarding
        // lambda on the outer client (one of the remaining ?.Invoke raise-sites)
        // then fans out ConnectionError.
        // ---------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: connection error fires for all subscribers when one throws.</summary>
        [Test]
        public void ConnectionErrorFiresForAllSubscribersWhenOneThrows()
        {
            var client = new MTConnectHttpClient(UnreachableHost, UnreachablePort);
            client.Timeout = 2000;

            client.ConnectionError += (_, _) => throw new InvalidOperationException("first handler throws");

            using var recorded = new ManualResetEventSlim(false);
            client.ConnectionError += (_, ex) =>
            {
                if (ex != null) recorded.Set();
            };

            // Synchronous GetProbe drives the probe sub-client and exercises
            // its forwarding lambda without spinning the Worker.
            client.GetProbe();

            Assert.That(recorded.Wait(EventWaitTimeoutMs), Is.True,
                "subscribers after a throwing one must still receive ConnectionError");
        }

        /// <summary>Pins the behavior expressed by the test name: internal error handler throwing does not break connection error fan out.</summary>
        [Test]
        public void InternalErrorHandlerThrowingDoesNotBreakConnectionErrorFanOut()
        {
            var client = new MTConnectHttpClient(UnreachableHost, UnreachablePort);
            client.Timeout = 2000;

            client.InternalError += (_, _) => throw new InvalidOperationException("InternalError throws");
            client.ConnectionError += (_, _) => throw new InvalidOperationException("first ConnectionError throws");

            using var recorded = new ManualResetEventSlim(false);
            client.ConnectionError += (_, ex) =>
            {
                if (ex != null) recorded.Set();
            };

            client.GetProbe();

            Assert.That(recorded.Wait(EventWaitTimeoutMs), Is.True,
                "InternalError throwing must not break the ConnectionError fan-out");
        }


        // MTConnectError and FormatError are not exercised directly here. The
        // embedded agent does not return an MTConnect protocol-error document
        // for a missing device (the probe sub-client routes that through
        // ConnectionError, already covered above), and FormatError requires a
        // malformed wire response that the healthy fixture cannot produce. Both
        // raise-sites route through the shared private RaiseEvent helper exercised
        // by every other event covered here; the fix at each site is the
        // identical one-line swap from ?.Invoke to RaiseEvent.
    }
}
