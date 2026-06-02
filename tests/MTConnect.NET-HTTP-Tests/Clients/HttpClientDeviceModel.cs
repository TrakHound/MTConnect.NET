// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Clients;
using MTConnect.Devices;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MTConnect.Tests.Http.Clients
{
    /// <summary>
    /// Drives the full MTConnectHttpClient (the streaming client, not the
    /// single-shot probe client) against the real embedded MTConnectHttpServer
    /// started by AgentRunner, pinning the two surfaces issue #176 added:
    /// the public Devices snapshot accessor (which exposes the post-probe
    /// device cache the client already maintains internally) and the
    /// DeviceReceived event (which historically built an empty list and
    /// iterated it, so it never fired in the field). Both invariants are
    /// exercised end to end through Start()/Stop(), so the probe round trip
    /// and the ProcessProbeDocument hand-off both run.
    /// </summary>
    [TestFixture]
    public class HttpClientDeviceModel : HttpClientFixture
    {
        // Generous CI-safe bounds. The streaming client raises DeviceReceived
        // and populates the snapshot from the first probe, which AgentRunner
        // guarantees is answerable before the fixture returns from Start().
        private const int ProbeWaitTimeoutMs = 15000;
        private const int ProbeWaitPollMs = 50;

        /// <summary>Pins the behaviour expressed by the test name: devices accessor is empty before probe.</summary>
        [Test]
        public void DevicesAccessorIsEmptyBeforeProbe()
        {
            var client = new MTConnectHttpClient(Hostname, Port);

            Assert.That(client.Devices, Is.Not.Null, "Devices accessor returned null");
            Assert.That(client.Devices.Count, Is.EqualTo(0), "Devices accessor was non-empty before any probe ran");
        }

        /// <summary>Pins the behaviour expressed by the test name: devices accessor is populated after probe.</summary>
        [Test]
        public void DevicesAccessorIsPopulatedAfterProbe()
        {
            var client = new MTConnectHttpClient(Hostname, Port);

            using var probeSignal = new ManualResetEventSlim(false);
            client.ProbeReceived += (_, _) => probeSignal.Set();

            try
            {
                client.Start();

                Assert.That(probeSignal.Wait(ProbeWaitTimeoutMs), Is.True, "ProbeReceived did not fire within the timeout");

                // The accessor returns a snapshot, so capture once and assert
                // against the captured dictionary.
                var snapshot = client.Devices;

                Assert.That(snapshot, Is.Not.Null, "Devices accessor returned null after probe");
                Assert.That(snapshot.Count, Is.EqualTo(ExpectedDocumentEntryCount), "Devices accessor did not surface every probed device");
                Assert.That(snapshot.Values.Any(d => d.Name == DeviceName), Is.True, $"Devices accessor did not surface the {DeviceName} device");
            }
            finally
            {
                client.Stop();
            }
        }

        /// <summary>Pins the behaviour expressed by the test name: devices accessor returns snapshot not live view.</summary>
        [Test]
        public void DevicesAccessorReturnsSnapshotNotLiveView()
        {
            var client = new MTConnectHttpClient(Hostname, Port);

            using var probeSignal = new ManualResetEventSlim(false);
            client.ProbeReceived += (_, _) => probeSignal.Set();

            try
            {
                client.Start();

                Assert.That(probeSignal.Wait(ProbeWaitTimeoutMs), Is.True, "ProbeReceived did not fire within the timeout");

                var snapshot = client.Devices;

                // The accessor must hand back an independent snapshot, not a
                // reference to the live cache — mutating the returned
                // dictionary must not be possible, and successive reads must
                // not alias one another.
                Assert.That(snapshot, Is.InstanceOf<IReadOnlyDictionary<string, IDevice>>(), "Devices accessor did not return an IReadOnlyDictionary");
                Assert.That(ReferenceEquals(snapshot, client.Devices), Is.False, "Devices accessor handed back an aliased instance instead of a snapshot");
            }
            finally
            {
                client.Stop();
            }
        }

        /// <summary>Pins the behaviour expressed by the test name: device received fires once per device on first probe.</summary>
        [Test]
        public void DeviceReceivedFiresOncePerDeviceOnFirstProbe()
        {
            var client = new MTConnectHttpClient(Hostname, Port);

            // Capture both the count and the carried IDevice instances so we
            // can assert the wired model is delivered, not a placeholder.
            var receivedDevices = new List<IDevice>();
            var receivedLock = new object();
            client.DeviceReceived += (_, device) =>
            {
                lock (receivedLock) { receivedDevices.Add(device); }
            };

            using var probeSignal = new ManualResetEventSlim(false);
            client.ProbeReceived += (_, _) => probeSignal.Set();

            try
            {
                client.Start();

                Assert.That(probeSignal.Wait(ProbeWaitTimeoutMs), Is.True, "ProbeReceived did not fire within the timeout");

                // The event is raised from inside ProcessProbeDocument, which
                // runs on the worker thread; ProbeReceived fires at the end of
                // that same method, so once it has been observed, every
                // DeviceReceived invocation for this probe has already
                // happened.
                List<IDevice> snapshot;
                lock (receivedLock) { snapshot = new List<IDevice>(receivedDevices); }

                Assert.That(snapshot.Count, Is.EqualTo(ExpectedDocumentEntryCount), "DeviceReceived did not fire once per probed device");
                Assert.That(snapshot.All(d => d != null), Is.True, "DeviceReceived carried a null device");
                Assert.That(snapshot.Any(d => d.Name == DeviceName), Is.True, $"DeviceReceived did not deliver the {DeviceName} device");
            }
            finally
            {
                client.Stop();
            }
        }

        /// <summary>Pins the behaviour expressed by the test name: device received carries wired data item back pointers.</summary>
        [Test]
        public void DeviceReceivedCarriesWiredDataItemBackPointers()
        {
            var client = new MTConnectHttpClient(Hostname, Port);

            IDevice? targetDevice = null;
            var targetLock = new object();
            client.DeviceReceived += (_, device) =>
            {
                if (device?.Name == DeviceName)
                {
                    lock (targetLock) { targetDevice = device; }
                }
            };

            using var probeSignal = new ManualResetEventSlim(false);
            client.ProbeReceived += (_, _) => probeSignal.Set();

            try
            {
                client.Start();

                Assert.That(probeSignal.Wait(ProbeWaitTimeoutMs), Is.True, "ProbeReceived did not fire within the timeout");

                IDevice? captured;
                lock (targetLock) { captured = targetDevice; }

                Assert.That(captured, Is.Not.Null, $"DeviceReceived did not deliver the {DeviceName} device");

                // Walk the DataItem tree and assert the back-pointers the
                // issue calls out are wired through to the caller — the whole
                // point of surfacing the cached IDevice is access to this
                // ancestry without re-parsing.
                var dataItems = captured!.GetDataItems().ToList();
                Assert.That(dataItems, Is.Not.Empty, $"Device {DeviceName} carried no DataItems");

                foreach (var dataItem in dataItems)
                {
                    Assert.That(dataItem.Device, Is.Not.Null, $"DataItem {dataItem.Id} lost its Device back-pointer");
                    Assert.That(dataItem.Container, Is.Not.Null, $"DataItem {dataItem.Id} lost its Container back-pointer");
                }
            }
            finally
            {
                client.Stop();
            }
        }
    }
}
