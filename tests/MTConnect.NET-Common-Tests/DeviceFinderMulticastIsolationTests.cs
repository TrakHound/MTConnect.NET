// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using NUnit.Framework;

namespace MTConnect.Tests.Common
{
    /// <summary>
    /// Pins the multicast-isolation contract for the custom-delegate events
    /// declared on <c>MTConnectDeviceFinder</c> and <c>PingQueue</c>. Neither
    /// class can use the typed <see cref="EventHandler"/> /
    /// <see cref="EventHandler{T}"/> overloads because every event is declared
    /// with a custom delegate signature (<c>DeviceHandler</c>,
    /// <c>PingSentHandler</c>, <c>PingReceivedHandler</c>,
    /// <c>PortRequestHandler</c>, <c>ProbeRequestHandler</c>,
    /// <c>RequestStatusHandler</c>, <c>CompletedHandler</c>). After migration
    /// every raise site uses
    /// <see cref="MulticastIsolation.Raise{TDelegate}(TDelegate, Action{TDelegate}, EventHandler{Exception}, object)"/>
    /// passing the per-subscriber invocation lambda inline and <c>null</c> as
    /// the <c>internalError</c> sink (neither class declares an InternalError
    /// event). These tests redeclare each delegate shape locally and verify
    /// the isolation guarantee holds for every signature; the helper contract
    /// is independent of the originating class.
    /// </summary>
    [TestFixture]
    public class DeviceFinderMulticastIsolationTests
    {
        // -----------------------------------------------------------------------
        // Local delegate shapes mirroring the production declarations on
        // MTConnectDeviceFinder and PingQueue. Same signatures; redeclared
        // locally to keep this test project free of a DeviceFinder project ref.
        // -----------------------------------------------------------------------

        /// <summary>Mirror of <c>MTConnectDeviceFinder.DeviceHandler</c> — fired by DeviceFound when an MTConnect agent is positively identified at an address/port pair.</summary>
        private delegate void TestDeviceHandler(object sender, object device);

        /// <summary>Mirror of <c>MTConnectDeviceFinder.RequestStatusHandler</c> — fired by SearchCompleted; carries the elapsed milliseconds of the search.</summary>
        private delegate void TestRequestStatusHandler(object sender, long milliseconds);

        /// <summary>Mirror of <c>MTConnectDeviceFinder.PingSentHandler</c> — fired once a ping has been dispatched to <paramref name="address"/>.</summary>
        private delegate void TestPingSentHandlerOnFinder(object sender, IPAddress address);

        /// <summary>Mirror of <c>MTConnectDeviceFinder.PingReceivedHandler</c> — fired when a ping reply returns from <paramref name="address"/>.</summary>
        private delegate void TestPingReceivedHandlerOnFinder(object sender, IPAddress address, PingReply reply);

        /// <summary>Mirror of <c>MTConnectDeviceFinder.PortRequestHandler</c> — fired by PortOpened/PortClosed to report the state of a TCP port at <paramref name="address"/>.</summary>
        private delegate void TestPortRequestHandler(object sender, IPAddress address, int port);

        /// <summary>Mirror of <c>MTConnectDeviceFinder.ProbeRequestHandler</c> — fired by ProbeSent/ProbeSuccessful/ProbeError for each MTConnect probe attempt.</summary>
        private delegate void TestProbeRequestHandler(object sender, IPAddress address, int port);

        /// <summary>Mirror of <c>PingQueue.PingSentHandler</c> — fired once a ping has been dispatched to <paramref name="address"/>; no sender argument.</summary>
        private delegate void TestPingSentHandlerOnQueue(IPAddress address);

        /// <summary>Mirror of <c>PingQueue.PingReceivedHandler</c> — fired when a ping reply returns; no sender argument.</summary>
        private delegate void TestPingReceivedHandlerOnQueue(IPAddress address, PingReply reply);

        /// <summary>Mirror of <c>PingQueue.CompletedHandler</c> — fired when the queue drains, with the list of successful addresses.</summary>
        private delegate void TestCompletedHandler(List<IPAddress> successfulAddresses);

        // -----------------------------------------------------------------------
        // MTConnectDeviceFinder.DeviceFound (DeviceHandler)
        // -----------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: a second DeviceHandler subscriber fires even when the first throws, covering DeviceFound on MTConnectDeviceFinder.</summary>
        [Test]
        public void DeviceFinder_DeviceFound_FiresAllSubscribersWhenOneThrows()
        {
            var seen = new List<object>();
            TestDeviceHandler? handler = null;
            handler += (_, _) => throw new InvalidOperationException("first DeviceFound subscriber throws");
            handler += (_, d) => seen.Add(d);

            MulticastIsolation.Raise(handler!, h => h(this, "device-1"));

            Assert.That(seen, Is.EqualTo(new object[] { "device-1" }));
        }

        /// <summary>Pins the behavior expressed by the test name: with null InternalError, a fault from a DeviceFound subscriber is swallowed without escaping.</summary>
        [Test]
        public void DeviceFinder_DeviceFound_NullInternalErrorSwallowsFault()
        {
            TestDeviceHandler handler = (_, _) => throw new InvalidOperationException("DeviceFound fault");

            Assert.DoesNotThrow(() => MulticastIsolation.Raise(handler, h => h(this, "device-1")));
        }

        // -----------------------------------------------------------------------
        // MTConnectDeviceFinder.SearchCompleted (RequestStatusHandler)
        // -----------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: a second RequestStatusHandler subscriber fires even when the first throws, covering SearchCompleted on MTConnectDeviceFinder.</summary>
        [Test]
        public void DeviceFinder_SearchCompleted_FiresAllSubscribersWhenOneThrows()
        {
            var seen = new List<long>();
            TestRequestStatusHandler? handler = null;
            handler += (_, _) => throw new InvalidOperationException("first SearchCompleted subscriber throws");
            handler += (_, ms) => seen.Add(ms);

            MulticastIsolation.Raise(handler!, h => h(this, 42L));

            Assert.That(seen, Is.EqualTo(new[] { 42L }));
        }

        /// <summary>Pins the behavior expressed by the test name: with null InternalError, a fault from a SearchCompleted subscriber is swallowed without escaping.</summary>
        [Test]
        public void DeviceFinder_SearchCompleted_NullInternalErrorSwallowsFault()
        {
            TestRequestStatusHandler handler = (_, _) => throw new InvalidOperationException("SearchCompleted fault");

            Assert.DoesNotThrow(() => MulticastIsolation.Raise(handler, h => h(this, 0L)));
        }

        // -----------------------------------------------------------------------
        // MTConnectDeviceFinder.PingSent (PingSentHandler, finder-shape)
        // -----------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: a second PingSentHandler subscriber fires even when the first throws, covering PingSent on MTConnectDeviceFinder.</summary>
        [Test]
        public void DeviceFinder_PingSent_FiresAllSubscribersWhenOneThrows()
        {
            var seen = new List<IPAddress>();
            var addr = IPAddress.Loopback;
            TestPingSentHandlerOnFinder? handler = null;
            handler += (_, _) => throw new InvalidOperationException("first PingSent subscriber throws");
            handler += (_, a) => seen.Add(a);

            MulticastIsolation.Raise(handler!, h => h(this, addr));

            Assert.That(seen, Is.EqualTo(new[] { addr }));
        }

        /// <summary>Pins the behavior expressed by the test name: with null InternalError, a fault from a PingSent subscriber on the finder is swallowed without escaping.</summary>
        [Test]
        public void DeviceFinder_PingSent_NullInternalErrorSwallowsFault()
        {
            TestPingSentHandlerOnFinder handler = (_, _) => throw new InvalidOperationException("PingSent fault");

            Assert.DoesNotThrow(() => MulticastIsolation.Raise(handler, h => h(this, IPAddress.Loopback)));
        }

        // -----------------------------------------------------------------------
        // MTConnectDeviceFinder.PingReceived (PingReceivedHandler, finder-shape)
        // -----------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: a second PingReceivedHandler subscriber fires even when the first throws, covering PingReceived on MTConnectDeviceFinder.</summary>
        [Test]
        public void DeviceFinder_PingReceived_FiresAllSubscribersWhenOneThrows()
        {
            var seen = new List<IPAddress>();
            var addr = IPAddress.Loopback;
            TestPingReceivedHandlerOnFinder? handler = null;
            handler += (_, _, _) => throw new InvalidOperationException("first PingReceived subscriber throws");
            handler += (_, a, _) => seen.Add(a);

            MulticastIsolation.Raise(handler!, h => h(this, addr, null!));

            Assert.That(seen, Is.EqualTo(new[] { addr }));
        }

        /// <summary>Pins the behavior expressed by the test name: with null InternalError, a fault from a PingReceived subscriber on the finder is swallowed without escaping.</summary>
        [Test]
        public void DeviceFinder_PingReceived_NullInternalErrorSwallowsFault()
        {
            TestPingReceivedHandlerOnFinder handler = (_, _, _) => throw new InvalidOperationException("PingReceived fault");

            Assert.DoesNotThrow(() => MulticastIsolation.Raise(handler, h => h(this, IPAddress.Loopback, null!)));
        }

        // -----------------------------------------------------------------------
        // MTConnectDeviceFinder.PortOpened / PortClosed (PortRequestHandler)
        // -----------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: a second PortRequestHandler subscriber fires even when the first throws, covering PortOpened/PortClosed on MTConnectDeviceFinder.</summary>
        [Test]
        public void DeviceFinder_PortRequest_FiresAllSubscribersWhenOneThrows()
        {
            var seen = new List<int>();
            TestPortRequestHandler? handler = null;
            handler += (_, _, _) => throw new InvalidOperationException("first PortRequest subscriber throws");
            handler += (_, _, p) => seen.Add(p);

            MulticastIsolation.Raise(handler!, h => h(this, IPAddress.Loopback, 5000));

            Assert.That(seen, Is.EqualTo(new[] { 5000 }));
        }

        /// <summary>Pins the behavior expressed by the test name: with null InternalError, a fault from a PortRequestHandler subscriber is swallowed without escaping.</summary>
        [Test]
        public void DeviceFinder_PortRequest_NullInternalErrorSwallowsFault()
        {
            TestPortRequestHandler handler = (_, _, _) => throw new InvalidOperationException("PortRequest fault");

            Assert.DoesNotThrow(() => MulticastIsolation.Raise(handler, h => h(this, IPAddress.Loopback, 5000)));
        }

        // -----------------------------------------------------------------------
        // MTConnectDeviceFinder.ProbeSent / ProbeSuccessful (ProbeRequestHandler)
        // -----------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: a second ProbeRequestHandler subscriber fires even when the first throws, covering ProbeSent/ProbeSuccessful on MTConnectDeviceFinder.</summary>
        [Test]
        public void DeviceFinder_ProbeRequest_FiresAllSubscribersWhenOneThrows()
        {
            var seen = new List<int>();
            TestProbeRequestHandler? handler = null;
            handler += (_, _, _) => throw new InvalidOperationException("first ProbeRequest subscriber throws");
            handler += (_, _, p) => seen.Add(p);

            MulticastIsolation.Raise(handler!, h => h(this, IPAddress.Loopback, 5000));

            Assert.That(seen, Is.EqualTo(new[] { 5000 }));
        }

        /// <summary>Pins the behavior expressed by the test name: with null InternalError, a fault from a ProbeRequestHandler subscriber is swallowed without escaping.</summary>
        [Test]
        public void DeviceFinder_ProbeRequest_NullInternalErrorSwallowsFault()
        {
            TestProbeRequestHandler handler = (_, _, _) => throw new InvalidOperationException("ProbeRequest fault");

            Assert.DoesNotThrow(() => MulticastIsolation.Raise(handler, h => h(this, IPAddress.Loopback, 5000)));
        }

        // -----------------------------------------------------------------------
        // PingQueue.PingSent (PingSentHandler, queue-shape: single IPAddress arg)
        // -----------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: a second queue-shape PingSentHandler subscriber fires even when the first throws, covering PingSent on PingQueue.</summary>
        [Test]
        public void PingQueue_PingSent_FiresAllSubscribersWhenOneThrows()
        {
            var seen = new List<IPAddress>();
            var addr = IPAddress.Loopback;
            TestPingSentHandlerOnQueue? handler = null;
            handler += _ => throw new InvalidOperationException("first PingQueue.PingSent subscriber throws");
            handler += a => seen.Add(a);

            MulticastIsolation.Raise(handler!, h => h(addr));

            Assert.That(seen, Is.EqualTo(new[] { addr }));
        }

        /// <summary>Pins the behavior expressed by the test name: with null InternalError, a fault from a queue-shape PingSent subscriber is swallowed without escaping.</summary>
        [Test]
        public void PingQueue_PingSent_NullInternalErrorSwallowsFault()
        {
            TestPingSentHandlerOnQueue handler = _ => throw new InvalidOperationException("PingQueue.PingSent fault");

            Assert.DoesNotThrow(() => MulticastIsolation.Raise(handler, h => h(IPAddress.Loopback)));
        }

        // -----------------------------------------------------------------------
        // PingQueue.PingReceived (PingReceivedHandler, queue-shape)
        // -----------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: a second queue-shape PingReceivedHandler subscriber fires even when the first throws, covering PingReceived on PingQueue.</summary>
        [Test]
        public void PingQueue_PingReceived_FiresAllSubscribersWhenOneThrows()
        {
            var seen = new List<IPAddress>();
            var addr = IPAddress.Loopback;
            TestPingReceivedHandlerOnQueue? handler = null;
            handler += (_, _) => throw new InvalidOperationException("first PingQueue.PingReceived subscriber throws");
            handler += (a, _) => seen.Add(a);

            MulticastIsolation.Raise(handler!, h => h(addr, null!));

            Assert.That(seen, Is.EqualTo(new[] { addr }));
        }

        /// <summary>Pins the behavior expressed by the test name: with null InternalError, a fault from a queue-shape PingReceived subscriber is swallowed without escaping.</summary>
        [Test]
        public void PingQueue_PingReceived_NullInternalErrorSwallowsFault()
        {
            TestPingReceivedHandlerOnQueue handler = (_, _) => throw new InvalidOperationException("PingQueue.PingReceived fault");

            Assert.DoesNotThrow(() => MulticastIsolation.Raise(handler, h => h(IPAddress.Loopback, null!)));
        }

        // -----------------------------------------------------------------------
        // PingQueue.Completed (CompletedHandler)
        // -----------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: a second CompletedHandler subscriber fires even when the first throws, covering Completed on PingQueue.</summary>
        [Test]
        public void PingQueue_Completed_FiresAllSubscribersWhenOneThrows()
        {
            var seen = new List<int>();
            var addresses = new List<IPAddress> { IPAddress.Loopback };
            TestCompletedHandler? handler = null;
            handler += _ => throw new InvalidOperationException("first PingQueue.Completed subscriber throws");
            handler += list => seen.Add(list.Count);

            MulticastIsolation.Raise(handler!, h => h(addresses));

            Assert.That(seen, Is.EqualTo(new[] { 1 }));
        }

        /// <summary>Pins the behavior expressed by the test name: with null InternalError, a fault from a Completed subscriber on PingQueue is swallowed without escaping.</summary>
        [Test]
        public void PingQueue_Completed_NullInternalErrorSwallowsFault()
        {
            TestCompletedHandler handler = _ => throw new InvalidOperationException("PingQueue.Completed fault");

            Assert.DoesNotThrow(() => MulticastIsolation.Raise(handler, h => h(new List<IPAddress>())));
        }

        // -----------------------------------------------------------------------
        // Null-handler guard for the generic delegate overload
        // -----------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: Raise with a null custom-delegate handler is a safe no-op covering the no-subscriber case at runtime.</summary>
        [Test]
        public void DeviceFinder_NullCustomDelegateHandler_DoesNotThrow()
        {
            TestDeviceHandler? handler = null;

            Assert.DoesNotThrow(() => MulticastIsolation.Raise(handler!, h => h(this, "any")));
        }
    }
}
