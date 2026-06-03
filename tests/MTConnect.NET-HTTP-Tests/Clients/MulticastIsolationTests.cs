// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using MTConnect.Clients;
using MTConnect.Observations.Events;
using NUnit.Framework;
using System;
using System.Threading;

namespace MTConnect.Tests.Http.Clients
{
    /// <summary>
    /// Drives the streaming MTConnectHttpClient against the real embedded
    /// MTConnectHttpServer started by AgentRunner and pins, for every sibling
    /// event the client raises (CurrentReceived, SampleReceived,
    /// ObservationReceived, AssetReceived, ProbeReceived, AssetsReceived), the
    /// two halves of the multicast-isolation contract: a subscriber that throws
    /// cannot starve later subscribers in the invocation list, and a fault
    /// raised by the InternalError handler itself must also be swallowed so the
    /// fan-out keeps going. Mirrors the DeviceReceived coverage already in
    /// HttpClientDeviceModel.
    /// </summary>
    [TestFixture]
    public class MulticastIsolationTests : HttpClientFixture
    {
        // Generous CI-safe bounds. The streaming client raises ProbeReceived,
        // DeviceReceived, and CurrentReceived from the first probe and current
        // round trip respectively; AgentRunner guarantees both are answerable
        // before Start() returns. SampleReceived / ObservationReceived /
        // AssetReceived require a follow-on broker push, so the deadline
        // covers the streamed round trip.
        private const int EventWaitTimeoutMs = 30000;

        private const string AssetDataItemId = "dev1_asset_chg";
        private const string AvailabilityDataItemId = "L2avail";


        // ---------------------------------------------------------------------
        // ProbeReceived
        // ---------------------------------------------------------------------

        /// <summary>Pins the behaviour expressed by the test name: probe received fires for all subscribers when one throws.</summary>
        [Test]
        public void ProbeReceivedFiresForAllSubscribersWhenOneThrows()
        {
            var client = new MTConnectHttpClient(Hostname, Port);

            client.ProbeReceived += (_, _) => throw new InvalidOperationException("first handler throws");

            using var recorded = new ManualResetEventSlim(false);
            client.ProbeReceived += (_, doc) =>
            {
                if (doc != null) recorded.Set();
            };

            try
            {
                client.Start();

                Assert.That(recorded.Wait(EventWaitTimeoutMs), Is.True,
                    "subscribers after a throwing one must still receive ProbeReceived");
            }
            finally
            {
                client.Stop();
            }
        }

        /// <summary>Pins the behaviour expressed by the test name: internal error handler throwing does not break probe received fan out.</summary>
        [Test]
        public void InternalErrorHandlerThrowingDoesNotBreakProbeReceivedFanOut()
        {
            var client = new MTConnectHttpClient(Hostname, Port);

            client.InternalError += (_, _) => throw new InvalidOperationException("InternalError throws");
            client.ProbeReceived += (_, _) => throw new InvalidOperationException("first ProbeReceived throws");

            using var recorded = new ManualResetEventSlim(false);
            client.ProbeReceived += (_, doc) =>
            {
                if (doc != null) recorded.Set();
            };

            try
            {
                client.Start();

                Assert.That(recorded.Wait(EventWaitTimeoutMs), Is.True,
                    "InternalError throwing must not break the ProbeReceived fan-out");
            }
            finally
            {
                client.Stop();
            }
        }


        // ---------------------------------------------------------------------
        // CurrentReceived
        // ---------------------------------------------------------------------

        /// <summary>Pins the behaviour expressed by the test name: current received fires for all subscribers when one throws.</summary>
        [Test]
        public void CurrentReceivedFiresForAllSubscribersWhenOneThrows()
        {
            var client = new MTConnectHttpClient(Hostname, Port);

            client.CurrentReceived += (_, _) => throw new InvalidOperationException("first handler throws");

            using var recorded = new ManualResetEventSlim(false);
            client.CurrentReceived += (_, doc) =>
            {
                if (doc != null) recorded.Set();
            };

            try
            {
                client.Start();

                Assert.That(recorded.Wait(EventWaitTimeoutMs), Is.True,
                    "subscribers after a throwing one must still receive CurrentReceived");
            }
            finally
            {
                client.Stop();
            }
        }

        /// <summary>Pins the behaviour expressed by the test name: internal error handler throwing does not break current received fan out.</summary>
        [Test]
        public void InternalErrorHandlerThrowingDoesNotBreakCurrentReceivedFanOut()
        {
            var client = new MTConnectHttpClient(Hostname, Port);

            client.InternalError += (_, _) => throw new InvalidOperationException("InternalError throws");
            client.CurrentReceived += (_, _) => throw new InvalidOperationException("first CurrentReceived throws");

            using var recorded = new ManualResetEventSlim(false);
            client.CurrentReceived += (_, doc) =>
            {
                if (doc != null) recorded.Set();
            };

            try
            {
                client.Start();

                Assert.That(recorded.Wait(EventWaitTimeoutMs), Is.True,
                    "InternalError throwing must not break the CurrentReceived fan-out");
            }
            finally
            {
                client.Stop();
            }
        }


        // ---------------------------------------------------------------------
        // SampleReceived
        // ---------------------------------------------------------------------

        /// <summary>Pins the behaviour expressed by the test name: sample received fires for all subscribers when one throws.</summary>
        [Test]
        public void SampleReceivedFiresForAllSubscribersWhenOneThrows()
        {
            var client = new MTConnectHttpClient(Hostname, Port);

            client.SampleReceived += (_, _) => throw new InvalidOperationException("first handler throws");

            using var recorded = new ManualResetEventSlim(false);
            client.SampleReceived += (_, doc) =>
            {
                if (doc != null) recorded.Set();
            };

            // SampleReceived fires on streamed sample responses, which only
            // start after the initial Current round trip. Wait for the seed
            // CurrentReceived before pushing the observation so the streaming
            // sample loop is established and ready to deliver.
            using var currentSeed = new ManualResetEventSlim(false);
            client.CurrentReceived += (_, _) => currentSeed.Set();

            try
            {
                client.Start();

                Assert.That(currentSeed.Wait(EventWaitTimeoutMs), Is.True,
                    "Streamed Current did not deliver the seed before the test could push a sample");

                PushAvailabilityTransition();

                Assert.That(recorded.Wait(EventWaitTimeoutMs), Is.True,
                    "subscribers after a throwing one must still receive SampleReceived");
            }
            finally
            {
                client.Stop();
            }
        }

        /// <summary>Pins the behaviour expressed by the test name: internal error handler throwing does not break sample received fan out.</summary>
        [Test]
        public void InternalErrorHandlerThrowingDoesNotBreakSampleReceivedFanOut()
        {
            var client = new MTConnectHttpClient(Hostname, Port);

            client.InternalError += (_, _) => throw new InvalidOperationException("InternalError throws");
            client.SampleReceived += (_, _) => throw new InvalidOperationException("first SampleReceived throws");

            using var recorded = new ManualResetEventSlim(false);
            client.SampleReceived += (_, doc) =>
            {
                if (doc != null) recorded.Set();
            };

            using var currentSeed = new ManualResetEventSlim(false);
            client.CurrentReceived += (_, _) => currentSeed.Set();

            try
            {
                client.Start();

                Assert.That(currentSeed.Wait(EventWaitTimeoutMs), Is.True,
                    "Streamed Current did not deliver the seed before the test could push a sample");

                PushAvailabilityTransition();

                Assert.That(recorded.Wait(EventWaitTimeoutMs), Is.True,
                    "InternalError throwing must not break the SampleReceived fan-out");
            }
            finally
            {
                client.Stop();
            }
        }

        // AVAILABILITY is an EVENT data item; the broker dedupes consecutive
        // identical pushes, so a single AVAILABLE push after an earlier test
        // already left AVAILABLE in the buffer would be a no-op and the
        // streamed Sample loop would never observe a new sequence. Pushing an
        // UNAVAILABLE → AVAILABLE transition guarantees at least one new
        // observation in the buffer regardless of prior fixture state.
        private void PushAvailabilityTransition()
        {
            AgentRunner.Agent.AddObservation(DeviceUuid, AvailabilityDataItemId, Availability.UNAVAILABLE);
            AgentRunner.Agent.AddObservation(DeviceUuid, AvailabilityDataItemId, Availability.AVAILABLE);
        }


        // ---------------------------------------------------------------------
        // ObservationReceived
        // ---------------------------------------------------------------------

        /// <summary>Pins the behaviour expressed by the test name: observation received fires for all subscribers when one throws.</summary>
        [Test]
        public void ObservationReceivedFiresForAllSubscribersWhenOneThrows()
        {
            var client = new MTConnectHttpClient(Hostname, Port);

            client.ObservationReceived += (_, _) => throw new InvalidOperationException("first handler throws");

            using var recorded = new ManualResetEventSlim(false);
            client.ObservationReceived += (_, observation) =>
            {
                if (observation != null) recorded.Set();
            };

            try
            {
                client.Start();

                Assert.That(recorded.Wait(EventWaitTimeoutMs), Is.True,
                    "subscribers after a throwing one must still receive ObservationReceived");
            }
            finally
            {
                client.Stop();
            }
        }

        /// <summary>Pins the behaviour expressed by the test name: internal error handler throwing does not break observation received fan out.</summary>
        [Test]
        public void InternalErrorHandlerThrowingDoesNotBreakObservationReceivedFanOut()
        {
            var client = new MTConnectHttpClient(Hostname, Port);

            client.InternalError += (_, _) => throw new InvalidOperationException("InternalError throws");
            client.ObservationReceived += (_, _) => throw new InvalidOperationException("first ObservationReceived throws");

            using var recorded = new ManualResetEventSlim(false);
            client.ObservationReceived += (_, observation) =>
            {
                if (observation != null) recorded.Set();
            };

            try
            {
                client.Start();

                Assert.That(recorded.Wait(EventWaitTimeoutMs), Is.True,
                    "InternalError throwing must not break the ObservationReceived fan-out");
            }
            finally
            {
                client.Stop();
            }
        }


        // ---------------------------------------------------------------------
        // AssetsReceived / AssetReceived
        //
        // Triggered through CheckAssetChanged: the streaming client sees an
        // AssetChanged observation in the streamed Current/Sample envelope and
        // issues a GetAsset request whose response raises AssetsReceived (the
        // envelope) and AssetReceived (each contained asset). The seed Okuma
        // device file carries an ASSET_CHANGED data item (id = dev1_asset_chg)
        // and the broker accepts the matching observation.
        // ---------------------------------------------------------------------

        /// <summary>Pins the behaviour expressed by the test name: assets received fires for all subscribers when one throws.</summary>
        [Test]
        public void AssetsReceivedFiresForAllSubscribersWhenOneThrows()
        {
            var client = new MTConnectHttpClient(Hostname, Port);

            client.AssetsReceived += (_, _) => throw new InvalidOperationException("first handler throws");

            using var recorded = new ManualResetEventSlim(false);
            client.AssetsReceived += (_, doc) =>
            {
                if (doc != null) recorded.Set();
            };

            try
            {
                client.Start();

                SeedAssetAndFireAssetChanged();

                Assert.That(recorded.Wait(EventWaitTimeoutMs), Is.True,
                    "subscribers after a throwing one must still receive AssetsReceived");
            }
            finally
            {
                client.Stop();
            }
        }

        /// <summary>Pins the behaviour expressed by the test name: internal error handler throwing does not break assets received fan out.</summary>
        [Test]
        public void InternalErrorHandlerThrowingDoesNotBreakAssetsReceivedFanOut()
        {
            var client = new MTConnectHttpClient(Hostname, Port);

            client.InternalError += (_, _) => throw new InvalidOperationException("InternalError throws");
            client.AssetsReceived += (_, _) => throw new InvalidOperationException("first AssetsReceived throws");

            using var recorded = new ManualResetEventSlim(false);
            client.AssetsReceived += (_, doc) =>
            {
                if (doc != null) recorded.Set();
            };

            try
            {
                client.Start();

                SeedAssetAndFireAssetChanged();

                Assert.That(recorded.Wait(EventWaitTimeoutMs), Is.True,
                    "InternalError throwing must not break the AssetsReceived fan-out");
            }
            finally
            {
                client.Stop();
            }
        }

        /// <summary>Pins the behaviour expressed by the test name: asset received fires for all subscribers when one throws.</summary>
        [Test]
        public void AssetReceivedFiresForAllSubscribersWhenOneThrows()
        {
            var client = new MTConnectHttpClient(Hostname, Port);

            client.AssetReceived += (_, _) => throw new InvalidOperationException("first handler throws");

            using var recorded = new ManualResetEventSlim(false);
            client.AssetReceived += (_, asset) =>
            {
                if (asset != null) recorded.Set();
            };

            try
            {
                client.Start();

                SeedAssetAndFireAssetChanged();

                Assert.That(recorded.Wait(EventWaitTimeoutMs), Is.True,
                    "subscribers after a throwing one must still receive AssetReceived");
            }
            finally
            {
                client.Stop();
            }
        }

        /// <summary>Pins the behaviour expressed by the test name: internal error handler throwing does not break asset received fan out.</summary>
        [Test]
        public void InternalErrorHandlerThrowingDoesNotBreakAssetReceivedFanOut()
        {
            var client = new MTConnectHttpClient(Hostname, Port);

            client.InternalError += (_, _) => throw new InvalidOperationException("InternalError throws");
            client.AssetReceived += (_, _) => throw new InvalidOperationException("first AssetReceived throws");

            using var recorded = new ManualResetEventSlim(false);
            client.AssetReceived += (_, asset) =>
            {
                if (asset != null) recorded.Set();
            };

            try
            {
                client.Start();

                SeedAssetAndFireAssetChanged();

                Assert.That(recorded.Wait(EventWaitTimeoutMs), Is.True,
                    "InternalError throwing must not break the AssetReceived fan-out");
            }
            finally
            {
                client.Stop();
            }
        }


        // ---------------------------------------------------------------------
        // Asset seeding helper. Adds a CuttingTool asset to the broker against
        // the seed device's UUID, then pushes an AssetChanged observation that
        // names it; the streamed observation reaches the client and drives
        // CheckAssetChanged into GetAssetAsync, which raises AssetsReceived
        // and AssetReceived.
        // ---------------------------------------------------------------------
        private void SeedAssetAndFireAssetChanged()
        {
            var assetId = $"asset-{Guid.NewGuid():N}";

            var asset = new CuttingToolAsset
            {
                AssetId = assetId,
                ToolId = "T1",
                Timestamp = DateTime.UtcNow,
                DeviceUuid = DeviceUuid,
            };
            AgentRunner.Agent.AddAsset(DeviceUuid, asset);

            // The AssetChanged data item lives on the Okuma seed device; the
            // broker accepts the observation by device UUID + data item id.
            AgentRunner.Agent.AddObservation(DeviceUuid, AssetDataItemId, assetId);
        }
    }
}
