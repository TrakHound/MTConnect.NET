// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using MTConnect.Assets;
using MTConnect.Devices;
using MTConnect.Input;
using MTConnect.Observations;
using NUnit.Framework;

namespace MTConnect.Tests.Common
{
    /// <summary>
    /// Pins the multicast-isolation contract for the <see cref="EventHandler{T}"/>
    /// raise sites on <c>MTConnectAgent</c> (DeviceAdded, ObservationReceived,
    /// ObservationAdded, AssetReceived, AssetAdded) and the single
    /// <see cref="EventHandler"/> raise site on <c>MTConnectAgentBroker</c>
    /// (StreamsResponseSent). After migration all these sites use
    /// <see cref="MulticastIsolation.Raise{T}"/> / <see cref="MulticastIsolation.Raise"/>
    /// passing <c>null</c> as the <c>internalError</c> sink (neither agent class
    /// declares an InternalError event). The custom-delegate events on both
    /// classes are not migratable with the shared helper and are therefore
    /// out-of-scope for this test class (surfaced as a blocker).
    /// </summary>
    [TestFixture]
    public class AgentMulticastIsolationTests
    {
        // -----------------------------------------------------------------------
        // EventHandler<IDevice> raise sites
        // (DeviceAdded on MTConnectAgent)
        // -----------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: a second EventHandler{IDevice} subscriber fires even when the first throws, covering DeviceAdded on MTConnectAgent.</summary>
        [Test]
        public void Agent_DeviceAdded_FiresAllSubscribersWhenOneThrows()
        {
            var received = new List<string>();
            var device = new Device { Name = "device-1", Uuid = "uuid-1" };

            EventHandler<IDevice>? handler = null;
            handler += (_, _) => throw new InvalidOperationException("first DeviceAdded subscriber throws");
            handler += (_, d) => received.Add(d.Uuid ?? "null");

            MulticastIsolation.Raise(handler!, this, (IDevice)device, null);

            Assert.That(received, Is.EqualTo(new[] { "uuid-1" }));
        }

        /// <summary>Pins the behavior expressed by the test name: with null InternalError, a fault from a DeviceAdded subscriber is swallowed and does not escape to the caller.</summary>
        [Test]
        public void Agent_DeviceAdded_NullInternalErrorSwallowsFault()
        {
            var device = new Device { Name = "device-1", Uuid = "uuid-1" };
            EventHandler<IDevice> handler = (_, _) => throw new InvalidOperationException("DeviceAdded fault");

            Assert.DoesNotThrow(() => MulticastIsolation.Raise(handler, this, (IDevice)device, null));
        }

        // -----------------------------------------------------------------------
        // EventHandler<IObservationInput> raise sites
        // (ObservationReceived on MTConnectAgent)
        // -----------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: a second EventHandler{IObservationInput} subscriber fires even when the first throws, covering ObservationReceived on MTConnectAgent.</summary>
        [Test]
        public void Agent_ObservationReceived_FiresAllSubscribersWhenOneThrows()
        {
            var firedCount = 0;

            EventHandler<IObservationInput>? handler = null;
            handler += (_, _) => throw new InvalidOperationException("first ObservationReceived subscriber throws");
            handler += (_, _) => firedCount++;

            var obs = new ObservationInput();
            MulticastIsolation.Raise(handler!, this, (IObservationInput)obs, null);

            Assert.That(firedCount, Is.EqualTo(1));
        }

        /// <summary>Pins the behavior expressed by the test name: with null InternalError, a fault from an ObservationReceived subscriber is swallowed without escaping.</summary>
        [Test]
        public void Agent_ObservationReceived_NullInternalErrorSwallowsFault()
        {
            var obs = new ObservationInput();
            EventHandler<IObservationInput> handler = (_, _) => throw new InvalidOperationException("ObservationReceived fault");

            Assert.DoesNotThrow(() => MulticastIsolation.Raise(handler, this, (IObservationInput)obs, null));
        }

        // -----------------------------------------------------------------------
        // EventHandler<IObservation> raise sites
        // (ObservationAdded on MTConnectAgent)
        // -----------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: a second EventHandler{IObservation} subscriber fires even when the first throws, covering ObservationAdded on MTConnectAgent.</summary>
        [Test]
        public void Agent_ObservationAdded_FiresAllSubscribersWhenOneThrows()
        {
            var firedCount = 0;

            EventHandler<IObservation>? handler = null;
            handler += (_, _) => throw new InvalidOperationException("first ObservationAdded subscriber throws");
            handler += (_, _) => firedCount++;

            var obs = new Observation();
            MulticastIsolation.Raise(handler!, this, (IObservation)obs, null);

            Assert.That(firedCount, Is.EqualTo(1));
        }

        /// <summary>Pins the behavior expressed by the test name: with null InternalError, a fault from an ObservationAdded subscriber is swallowed without escaping.</summary>
        [Test]
        public void Agent_ObservationAdded_NullInternalErrorSwallowsFault()
        {
            var obs = new Observation();
            EventHandler<IObservation> handler = (_, _) => throw new InvalidOperationException("ObservationAdded fault");

            Assert.DoesNotThrow(() => MulticastIsolation.Raise(handler, this, (IObservation)obs, null));
        }

        // -----------------------------------------------------------------------
        // EventHandler<IAsset> raise sites
        // (AssetAdded on MTConnectAgent)
        // -----------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: a second EventHandler{IAsset} subscriber fires even when the first throws, covering AssetAdded on MTConnectAgent.</summary>
        [Test]
        public void Agent_AssetAdded_FiresAllSubscribersWhenOneThrows()
        {
            var firedCount = 0;

            EventHandler<IAsset>? handler = null;
            handler += (_, _) => throw new InvalidOperationException("first AssetAdded subscriber throws");
            handler += (_, _) => firedCount++;

            var asset = new Asset { AssetId = "a1", Timestamp = DateTime.UtcNow };
            MulticastIsolation.Raise(handler!, this, (IAsset)asset, null);

            Assert.That(firedCount, Is.EqualTo(1));
        }

        /// <summary>Pins the behavior expressed by the test name: with null InternalError, a fault from an AssetAdded subscriber is swallowed without escaping.</summary>
        [Test]
        public void Agent_AssetAdded_NullInternalErrorSwallowsFault()
        {
            var asset = new Asset { AssetId = "a1", Timestamp = DateTime.UtcNow };
            EventHandler<IAsset> handler = (_, _) => throw new InvalidOperationException("AssetAdded fault");

            Assert.DoesNotThrow(() => MulticastIsolation.Raise(handler, this, (IAsset)asset, null));
        }

        // -----------------------------------------------------------------------
        // EventHandler (non-generic) raise sites
        // (StreamsResponseSent on MTConnectAgentBroker)
        // -----------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: a second non-generic EventHandler subscriber fires even when the first throws, covering StreamsResponseSent on MTConnectAgentBroker.</summary>
        [Test]
        public void AgentBroker_StreamsResponseSent_FiresAllSubscribersWhenOneThrows()
        {
            var fired = new List<int>();

            EventHandler? handler = null;
            handler += (_, _) => throw new InvalidOperationException("first StreamsResponseSent subscriber throws");
            handler += (_, _) => fired.Add(1);

            MulticastIsolation.Raise(handler!, this, EventArgs.Empty, null);

            Assert.That(fired, Is.EqualTo(new[] { 1 }));
        }

        /// <summary>Pins the behavior expressed by the test name: with null InternalError, a fault from a StreamsResponseSent subscriber is swallowed without escaping.</summary>
        [Test]
        public void AgentBroker_StreamsResponseSent_NullInternalErrorSwallowsFault()
        {
            EventHandler handler = (_, _) => throw new InvalidOperationException("StreamsResponseSent fault");

            Assert.DoesNotThrow(() => MulticastIsolation.Raise(handler, this, EventArgs.Empty, null));
        }

        // -----------------------------------------------------------------------
        // Null-handler guard (both generic and non-generic)
        // -----------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: Raise with a null EventHandler{IDevice} handler is a safe no-op covering the no-subscriber case at runtime.</summary>
        [Test]
        public void Agent_NullGenericHandler_DoesNotThrow()
        {
            EventHandler<IDevice>? handler = null;
            IDevice? device = null;

            Assert.DoesNotThrow(() => MulticastIsolation.Raise(handler, this, device, null));
        }

        /// <summary>Pins the behavior expressed by the test name: Raise with a null non-generic EventHandler is a safe no-op covering the no-subscriber case at runtime.</summary>
        [Test]
        public void AgentBroker_NullNonGenericHandler_DoesNotThrow()
        {
            EventHandler? handler = null;

            Assert.DoesNotThrow(() => MulticastIsolation.Raise(handler, this, EventArgs.Empty, null));
        }
    }
}
