// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using MTConnect.Assets;
using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using MTConnect.Errors;
using MTConnect.Input;
using MTConnect.Observations;
using NUnit.Framework;

namespace MTConnect.Tests.Common
{
    /// <summary>
    /// Pins the multicast-isolation contract for every event raised by
    /// <c>MTConnectAgent</c> and <c>MTConnectAgentBroker</c>:
    /// <see cref="EventHandler{T}"/> sites on the agent (DeviceAdded,
    /// ObservationReceived, ObservationAdded, AssetReceived, AssetAdded), the
    /// custom-delegate validation events on the agent (InvalidDeviceAdded,
    /// InvalidComponentAdded, InvalidCompositionAdded, InvalidDataItemAdded,
    /// InvalidObservationAdded, InvalidAssetAdded), the single
    /// <see cref="EventHandler"/> site on the broker (StreamsResponseSent),
    /// and the custom-delegate request / response events on the broker
    /// (DevicesRequestReceived, DevicesResponseSent, StreamsRequestReceived,
    /// AssetsRequestReceived, DeviceAssetsRequestReceived, AssetsResponseSent,
    /// ErrorResponseSent). After migration all these sites use
    /// <see cref="MulticastIsolation.Raise{T}(EventHandler{T}, object, T, EventHandler{Exception})"/> /
    /// <see cref="MulticastIsolation.Raise(EventHandler, object, EventArgs, EventHandler{Exception})"/> /
    /// <see cref="MulticastIsolation.Raise{TDelegate}(TDelegate, Action{TDelegate}, EventHandler{Exception}, object)"/>
    /// passing <c>null</c> as the <c>internalError</c> sink — neither agent
    /// class declares an InternalError event, so faults are swallowed at the
    /// per-delegate boundary (consistent with the pre-isolation null-conditional
    /// behaviour).
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

            handler!.Raise(this, (IDevice)device, null);

            Assert.That(received, Is.EqualTo(new[] { "uuid-1" }));
        }

        /// <summary>Pins the behavior expressed by the test name: with null InternalError, a fault from a DeviceAdded subscriber is swallowed and does not escape to the caller.</summary>
        [Test]
        public void Agent_DeviceAdded_NullInternalErrorSwallowsFault()
        {
            var device = new Device { Name = "device-1", Uuid = "uuid-1" };
            EventHandler<IDevice> handler = (_, _) => throw new InvalidOperationException("DeviceAdded fault");

            Assert.DoesNotThrow(() => handler.Raise(this, (IDevice)device, null));
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
            handler!.Raise(this, (IObservationInput)obs, null);

            Assert.That(firedCount, Is.EqualTo(1));
        }

        /// <summary>Pins the behavior expressed by the test name: with null InternalError, a fault from an ObservationReceived subscriber is swallowed without escaping.</summary>
        [Test]
        public void Agent_ObservationReceived_NullInternalErrorSwallowsFault()
        {
            var obs = new ObservationInput();
            EventHandler<IObservationInput> handler = (_, _) => throw new InvalidOperationException("ObservationReceived fault");

            Assert.DoesNotThrow(() => handler.Raise(this, (IObservationInput)obs, null));
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
            handler!.Raise(this, (IObservation)obs, null);

            Assert.That(firedCount, Is.EqualTo(1));
        }

        /// <summary>Pins the behavior expressed by the test name: with null InternalError, a fault from an ObservationAdded subscriber is swallowed without escaping.</summary>
        [Test]
        public void Agent_ObservationAdded_NullInternalErrorSwallowsFault()
        {
            var obs = new Observation();
            EventHandler<IObservation> handler = (_, _) => throw new InvalidOperationException("ObservationAdded fault");

            Assert.DoesNotThrow(() => handler.Raise(this, (IObservation)obs, null));
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
            handler!.Raise(this, (IAsset)asset, null);

            Assert.That(firedCount, Is.EqualTo(1));
        }

        /// <summary>Pins the behavior expressed by the test name: with null InternalError, a fault from an AssetAdded subscriber is swallowed without escaping.</summary>
        [Test]
        public void Agent_AssetAdded_NullInternalErrorSwallowsFault()
        {
            var asset = new Asset { AssetId = "a1", Timestamp = DateTime.UtcNow };
            EventHandler<IAsset> handler = (_, _) => throw new InvalidOperationException("AssetAdded fault");

            Assert.DoesNotThrow(() => handler.Raise(this, (IAsset)asset, null));
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

            handler!.Raise(this, EventArgs.Empty, null);

            Assert.That(fired, Is.EqualTo(new[] { 1 }));
        }

        /// <summary>Pins the behavior expressed by the test name: with null InternalError, a fault from a StreamsResponseSent subscriber is swallowed without escaping.</summary>
        [Test]
        public void AgentBroker_StreamsResponseSent_NullInternalErrorSwallowsFault()
        {
            EventHandler handler = (_, _) => throw new InvalidOperationException("StreamsResponseSent fault");

            Assert.DoesNotThrow(() => handler.Raise(this, EventArgs.Empty, null));
        }

        // -----------------------------------------------------------------------
        // Null-handler guard (both generic and non-generic)
        // -----------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: Raise with a null EventHandler{IDevice} handler is a safe no-op covering the no-subscriber case at runtime.</summary>
        [Test]
        public void Agent_NullGenericHandler_DoesNotThrow()
        {
            EventHandler<IDevice>? handler = null;
            var device = new Device { Name = "noop-device", Uuid = "noop-uuid" };

            Assert.DoesNotThrow(() => handler.Raise(this, (IDevice)device, null));
        }

        /// <summary>Pins the behavior expressed by the test name: Raise with a null non-generic EventHandler is a safe no-op covering the no-subscriber case at runtime.</summary>
        [Test]
        public void AgentBroker_NullNonGenericHandler_DoesNotThrow()
        {
            EventHandler? handler = null;

            Assert.DoesNotThrow(() => handler.Raise(this, EventArgs.Empty, null));
        }

        // =======================================================================
        // Custom-delegate raise sites — agent
        // =======================================================================

        // -----------------------------------------------------------------------
        // MTConnectDeviceValidationHandler (InvalidDeviceAdded on MTConnectAgent)
        // -----------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: a second MTConnectDeviceValidationHandler subscriber fires even when the first throws, covering InvalidDeviceAdded on MTConnectAgent.</summary>
        [Test]
        public void Agent_InvalidDeviceAdded_FiresAllSubscribersWhenOneThrows()
        {
            var seen = new List<string>();
            var device = new Device { Name = "d1", Uuid = "uuid-1" };
            var result = new ValidationResult(false, "bad device");

            MTConnectDeviceValidationHandler? handler = null;
            handler += (_, _) => throw new InvalidOperationException("first InvalidDeviceAdded subscriber throws");
            handler += (d, _) => seen.Add(d.Uuid ?? "null");

            MulticastIsolation.Raise(handler!, h => h(device, result));

            Assert.That(seen, Is.EqualTo(new[] { "uuid-1" }));
        }

        /// <summary>Pins the behavior expressed by the test name: with null InternalError, a fault from an InvalidDeviceAdded subscriber is swallowed without escaping.</summary>
        [Test]
        public void Agent_InvalidDeviceAdded_NullInternalErrorSwallowsFault()
        {
            var device = new Device { Name = "d1", Uuid = "uuid-1" };
            var result = new ValidationResult(false, "bad device");
            MTConnectDeviceValidationHandler handler = (_, _) => throw new InvalidOperationException("InvalidDeviceAdded fault");

            Assert.DoesNotThrow(() => MulticastIsolation.Raise(handler, h => h(device, result)));
        }

        // -----------------------------------------------------------------------
        // MTConnectComponentValidationHandler (InvalidComponentAdded on MTConnectAgent)
        // -----------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: a second MTConnectComponentValidationHandler subscriber fires even when the first throws, covering InvalidComponentAdded on MTConnectAgent.</summary>
        [Test]
        public void Agent_InvalidComponentAdded_FiresAllSubscribersWhenOneThrows()
        {
            var seen = new List<string>();
            var component = new Component { Id = "c1" };
            var result = new ValidationResult(false, "bad component");

            MTConnectComponentValidationHandler? handler = null;
            handler += (_, _, _) => throw new InvalidOperationException("first InvalidComponentAdded subscriber throws");
            handler += (uuid, _, _) => seen.Add(uuid);

            MulticastIsolation.Raise(handler!, h => h("uuid-1", component, result));

            Assert.That(seen, Is.EqualTo(new[] { "uuid-1" }));
        }

        /// <summary>Pins the behavior expressed by the test name: with null InternalError, a fault from an InvalidComponentAdded subscriber is swallowed without escaping.</summary>
        [Test]
        public void Agent_InvalidComponentAdded_NullInternalErrorSwallowsFault()
        {
            var component = new Component { Id = "c1" };
            var result = new ValidationResult(false, "bad component");
            MTConnectComponentValidationHandler handler = (_, _, _) => throw new InvalidOperationException("InvalidComponentAdded fault");

            Assert.DoesNotThrow(() => MulticastIsolation.Raise(handler, h => h("uuid-1", component, result)));
        }

        // -----------------------------------------------------------------------
        // MTConnectCompositionValidationHandler (InvalidCompositionAdded on MTConnectAgent)
        // -----------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: a second MTConnectCompositionValidationHandler subscriber fires even when the first throws, covering InvalidCompositionAdded on MTConnectAgent.</summary>
        [Test]
        public void Agent_InvalidCompositionAdded_FiresAllSubscribersWhenOneThrows()
        {
            var seen = new List<string>();
            var composition = new Composition { Id = "cm1" };
            var result = new ValidationResult(false, "bad composition");

            MTConnectCompositionValidationHandler? handler = null;
            handler += (_, _, _) => throw new InvalidOperationException("first InvalidCompositionAdded subscriber throws");
            handler += (uuid, _, _) => seen.Add(uuid);

            MulticastIsolation.Raise(handler!, h => h("uuid-1", composition, result));

            Assert.That(seen, Is.EqualTo(new[] { "uuid-1" }));
        }

        /// <summary>Pins the behavior expressed by the test name: with null InternalError, a fault from an InvalidCompositionAdded subscriber is swallowed without escaping.</summary>
        [Test]
        public void Agent_InvalidCompositionAdded_NullInternalErrorSwallowsFault()
        {
            var composition = new Composition { Id = "cm1" };
            var result = new ValidationResult(false, "bad composition");
            MTConnectCompositionValidationHandler handler = (_, _, _) => throw new InvalidOperationException("InvalidCompositionAdded fault");

            Assert.DoesNotThrow(() => MulticastIsolation.Raise(handler, h => h("uuid-1", composition, result)));
        }

        // -----------------------------------------------------------------------
        // MTConnectDataItemValidationHandler (InvalidDataItemAdded on MTConnectAgent)
        // -----------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: a second MTConnectDataItemValidationHandler subscriber fires even when the first throws, covering InvalidDataItemAdded on MTConnectAgent.</summary>
        [Test]
        public void Agent_InvalidDataItemAdded_FiresAllSubscribersWhenOneThrows()
        {
            var seen = new List<string>();
            var dataItem = new DataItem { Id = "di1" };
            var result = new ValidationResult(false, "bad data item");

            MTConnectDataItemValidationHandler? handler = null;
            handler += (_, _, _) => throw new InvalidOperationException("first InvalidDataItemAdded subscriber throws");
            handler += (uuid, _, _) => seen.Add(uuid);

            MulticastIsolation.Raise(handler!, h => h("uuid-1", dataItem, result));

            Assert.That(seen, Is.EqualTo(new[] { "uuid-1" }));
        }

        /// <summary>Pins the behavior expressed by the test name: with null InternalError, a fault from an InvalidDataItemAdded subscriber is swallowed without escaping.</summary>
        [Test]
        public void Agent_InvalidDataItemAdded_NullInternalErrorSwallowsFault()
        {
            var dataItem = new DataItem { Id = "di1" };
            var result = new ValidationResult(false, "bad data item");
            MTConnectDataItemValidationHandler handler = (_, _, _) => throw new InvalidOperationException("InvalidDataItemAdded fault");

            Assert.DoesNotThrow(() => MulticastIsolation.Raise(handler, h => h("uuid-1", dataItem, result)));
        }

        // -----------------------------------------------------------------------
        // MTConnectObservationValidationHandler (InvalidObservationAdded on MTConnectAgent)
        // -----------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: a second MTConnectObservationValidationHandler subscriber fires even when the first throws, covering InvalidObservationAdded on MTConnectAgent.</summary>
        [Test]
        public void Agent_InvalidObservationAdded_FiresAllSubscribersWhenOneThrows()
        {
            var seen = new List<string>();
            var result = new ValidationResult(false, "bad observation");

            MTConnectObservationValidationHandler? handler = null;
            handler += (_, _, _) => throw new InvalidOperationException("first InvalidObservationAdded subscriber throws");
            handler += (uuid, _, _) => seen.Add(uuid);

            MulticastIsolation.Raise(handler!, h => h("uuid-1", "key-1", result));

            Assert.That(seen, Is.EqualTo(new[] { "uuid-1" }));
        }

        /// <summary>Pins the behavior expressed by the test name: with null InternalError, a fault from an InvalidObservationAdded subscriber is swallowed without escaping.</summary>
        [Test]
        public void Agent_InvalidObservationAdded_NullInternalErrorSwallowsFault()
        {
            var result = new ValidationResult(false, "bad observation");
            MTConnectObservationValidationHandler handler = (_, _, _) => throw new InvalidOperationException("InvalidObservationAdded fault");

            Assert.DoesNotThrow(() => MulticastIsolation.Raise(handler, h => h("uuid-1", "key-1", result)));
        }

        // -----------------------------------------------------------------------
        // MTConnectAssetValidationHandler (InvalidAssetAdded on MTConnectAgent)
        // -----------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: a second MTConnectAssetValidationHandler subscriber fires even when the first throws, covering InvalidAssetAdded on MTConnectAgent.</summary>
        [Test]
        public void Agent_InvalidAssetAdded_FiresAllSubscribersWhenOneThrows()
        {
            var seen = new List<string>();
            var asset = new Asset { AssetId = "a1", Timestamp = DateTime.UtcNow };
            var result = new ValidationResult(false, "bad asset");

            MTConnectAssetValidationHandler? handler = null;
            handler += (_, _) => throw new InvalidOperationException("first InvalidAssetAdded subscriber throws");
            handler += (a, _) => seen.Add(a.AssetId ?? "null");

            MulticastIsolation.Raise(handler!, h => h(asset, result));

            Assert.That(seen, Is.EqualTo(new[] { "a1" }));
        }

        /// <summary>Pins the behavior expressed by the test name: with null InternalError, a fault from an InvalidAssetAdded subscriber is swallowed without escaping.</summary>
        [Test]
        public void Agent_InvalidAssetAdded_NullInternalErrorSwallowsFault()
        {
            var asset = new Asset { AssetId = "a1", Timestamp = DateTime.UtcNow };
            var result = new ValidationResult(false, "bad asset");
            MTConnectAssetValidationHandler handler = (_, _) => throw new InvalidOperationException("InvalidAssetAdded fault");

            Assert.DoesNotThrow(() => MulticastIsolation.Raise(handler, h => h(asset, result)));
        }

        // =======================================================================
        // Custom-delegate raise sites — broker
        // =======================================================================

        // -----------------------------------------------------------------------
        // MTConnectDevicesRequestedHandler (DevicesRequestReceived on MTConnectAgentBroker)
        // -----------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: a second MTConnectDevicesRequestedHandler subscriber fires even when the first throws, covering DevicesRequestReceived on MTConnectAgentBroker.</summary>
        [Test]
        public void AgentBroker_DevicesRequestReceived_FiresAllSubscribersWhenOneThrows()
        {
            var seen = new List<string?>();
            MTConnectDevicesRequestedHandler? handler = null;
            handler += _ => throw new InvalidOperationException("first DevicesRequestReceived subscriber throws");
            handler += u => seen.Add(u);

            MulticastIsolation.Raise(handler!, h => h("uuid-1"));

            Assert.That(seen, Is.EqualTo(new[] { "uuid-1" }));
        }

        /// <summary>Pins the behavior expressed by the test name: with null InternalError, a fault from a DevicesRequestReceived subscriber is swallowed without escaping.</summary>
        [Test]
        public void AgentBroker_DevicesRequestReceived_NullInternalErrorSwallowsFault()
        {
            MTConnectDevicesRequestedHandler handler = _ => throw new InvalidOperationException("DevicesRequestReceived fault");

            Assert.DoesNotThrow(() => MulticastIsolation.Raise(handler, h => h("uuid-1")));
        }

        // -----------------------------------------------------------------------
        // MTConnectDevicesHandler (DevicesResponseSent on MTConnectAgentBroker)
        // -----------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: a second MTConnectDevicesHandler subscriber fires even when the first throws, covering DevicesResponseSent on MTConnectAgentBroker.</summary>
        [Test]
        public void AgentBroker_DevicesResponseSent_FiresAllSubscribersWhenOneThrows()
        {
            var firedCount = 0;
            MTConnectDevicesHandler? handler = null;
            handler += _ => throw new InvalidOperationException("first DevicesResponseSent subscriber throws");
            handler += _ => firedCount++;

            MulticastIsolation.Raise(handler!, h => h(null!));

            Assert.That(firedCount, Is.EqualTo(1));
        }

        /// <summary>Pins the behavior expressed by the test name: with null InternalError, a fault from a DevicesResponseSent subscriber is swallowed without escaping.</summary>
        [Test]
        public void AgentBroker_DevicesResponseSent_NullInternalErrorSwallowsFault()
        {
            MTConnectDevicesHandler handler = _ => throw new InvalidOperationException("DevicesResponseSent fault");

            Assert.DoesNotThrow(() => MulticastIsolation.Raise(handler, h => h(null!)));
        }

        // -----------------------------------------------------------------------
        // MTConnectStreamsRequestedHandler (StreamsRequestReceived on MTConnectAgentBroker)
        // -----------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: a second MTConnectStreamsRequestedHandler subscriber fires even when the first throws, covering StreamsRequestReceived on MTConnectAgentBroker.</summary>
        [Test]
        public void AgentBroker_StreamsRequestReceived_FiresAllSubscribersWhenOneThrows()
        {
            var seen = new List<string?>();
            MTConnectStreamsRequestedHandler? handler = null;
            handler += _ => throw new InvalidOperationException("first StreamsRequestReceived subscriber throws");
            handler += u => seen.Add(u);

            MulticastIsolation.Raise(handler!, h => h("uuid-1"));

            Assert.That(seen, Is.EqualTo(new[] { "uuid-1" }));
        }

        /// <summary>Pins the behavior expressed by the test name: with null InternalError, a fault from a StreamsRequestReceived subscriber is swallowed without escaping.</summary>
        [Test]
        public void AgentBroker_StreamsRequestReceived_NullInternalErrorSwallowsFault()
        {
            MTConnectStreamsRequestedHandler handler = _ => throw new InvalidOperationException("StreamsRequestReceived fault");

            Assert.DoesNotThrow(() => MulticastIsolation.Raise(handler, h => h("uuid-1")));
        }

        // -----------------------------------------------------------------------
        // MTConnectAssetsRequestedHandler (AssetsRequestReceived on MTConnectAgentBroker)
        // -----------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: a second MTConnectAssetsRequestedHandler subscriber fires even when the first throws, covering AssetsRequestReceived on MTConnectAgentBroker.</summary>
        [Test]
        public void AgentBroker_AssetsRequestReceived_FiresAllSubscribersWhenOneThrows()
        {
            var seen = new List<int>();
            var ids = new[] { "asset-1" };
            MTConnectAssetsRequestedHandler? handler = null;
            handler += _ => throw new InvalidOperationException("first AssetsRequestReceived subscriber throws");
            handler += list => { foreach (var _ in list) seen.Add(1); };

            MulticastIsolation.Raise(handler!, h => h(ids));

            Assert.That(seen, Is.EqualTo(new[] { 1 }));
        }

        /// <summary>Pins the behavior expressed by the test name: with null InternalError, a fault from an AssetsRequestReceived subscriber is swallowed without escaping.</summary>
        [Test]
        public void AgentBroker_AssetsRequestReceived_NullInternalErrorSwallowsFault()
        {
            var ids = new[] { "asset-1" };
            MTConnectAssetsRequestedHandler handler = _ => throw new InvalidOperationException("AssetsRequestReceived fault");

            Assert.DoesNotThrow(() => MulticastIsolation.Raise(handler, h => h(ids)));
        }

        // -----------------------------------------------------------------------
        // MTConnectDeviceAssetsRequestedHandler (DeviceAssetsRequestReceived on MTConnectAgentBroker)
        // -----------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: a second MTConnectDeviceAssetsRequestedHandler subscriber fires even when the first throws, covering DeviceAssetsRequestReceived on MTConnectAgentBroker.</summary>
        [Test]
        public void AgentBroker_DeviceAssetsRequestReceived_FiresAllSubscribersWhenOneThrows()
        {
            var seen = new List<string?>();
            MTConnectDeviceAssetsRequestedHandler? handler = null;
            handler += _ => throw new InvalidOperationException("first DeviceAssetsRequestReceived subscriber throws");
            handler += u => seen.Add(u);

            MulticastIsolation.Raise(handler!, h => h("uuid-1"));

            Assert.That(seen, Is.EqualTo(new[] { "uuid-1" }));
        }

        /// <summary>Pins the behavior expressed by the test name: with null InternalError, a fault from a DeviceAssetsRequestReceived subscriber is swallowed without escaping.</summary>
        [Test]
        public void AgentBroker_DeviceAssetsRequestReceived_NullInternalErrorSwallowsFault()
        {
            MTConnectDeviceAssetsRequestedHandler handler = _ => throw new InvalidOperationException("DeviceAssetsRequestReceived fault");

            Assert.DoesNotThrow(() => MulticastIsolation.Raise(handler, h => h("uuid-1")));
        }

        // -----------------------------------------------------------------------
        // MTConnectAssetsHandler (AssetsResponseSent on MTConnectAgentBroker)
        // -----------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: a second MTConnectAssetsHandler subscriber fires even when the first throws, covering AssetsResponseSent on MTConnectAgentBroker.</summary>
        [Test]
        public void AgentBroker_AssetsResponseSent_FiresAllSubscribersWhenOneThrows()
        {
            var firedCount = 0;
            MTConnectAssetsHandler? handler = null;
            handler += _ => throw new InvalidOperationException("first AssetsResponseSent subscriber throws");
            handler += _ => firedCount++;

            MulticastIsolation.Raise(handler!, h => h(null!));

            Assert.That(firedCount, Is.EqualTo(1));
        }

        /// <summary>Pins the behavior expressed by the test name: with null InternalError, a fault from an AssetsResponseSent subscriber is swallowed without escaping.</summary>
        [Test]
        public void AgentBroker_AssetsResponseSent_NullInternalErrorSwallowsFault()
        {
            MTConnectAssetsHandler handler = _ => throw new InvalidOperationException("AssetsResponseSent fault");

            Assert.DoesNotThrow(() => MulticastIsolation.Raise(handler, h => h(null!)));
        }

        // -----------------------------------------------------------------------
        // MTConnectErrorHandler (ErrorResponseSent on MTConnectAgentBroker)
        // -----------------------------------------------------------------------

        /// <summary>Pins the behavior expressed by the test name: a second MTConnectErrorHandler subscriber fires even when the first throws, covering ErrorResponseSent on MTConnectAgentBroker.</summary>
        [Test]
        public void AgentBroker_ErrorResponseSent_FiresAllSubscribersWhenOneThrows()
        {
            var firedCount = 0;
            MTConnectErrorHandler? handler = null;
            handler += _ => throw new InvalidOperationException("first ErrorResponseSent subscriber throws");
            handler += _ => firedCount++;

            MulticastIsolation.Raise(handler!, h => h((IErrorResponseDocument)null!));

            Assert.That(firedCount, Is.EqualTo(1));
        }

        /// <summary>Pins the behavior expressed by the test name: with null InternalError, a fault from an ErrorResponseSent subscriber is swallowed without escaping.</summary>
        [Test]
        public void AgentBroker_ErrorResponseSent_NullInternalErrorSwallowsFault()
        {
            MTConnectErrorHandler handler = _ => throw new InvalidOperationException("ErrorResponseSent fault");

            Assert.DoesNotThrow(() => MulticastIsolation.Raise(handler, h => h((IErrorResponseDocument)null!)));
        }
    }
}
