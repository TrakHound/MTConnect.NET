// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using MTConnect.Agents;
using MTConnect.Assets.Files;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using MTConnect.Observations;
using MTConnect.Observations.Output;
using MTConnect.Streams.Output;
using NUnit.Framework;

namespace MTConnect.Tests.Common.Agents
{
    /// <summary>
    /// Pins <see cref="MTConnectAgentBroker"/> fixes: DataItem filters on every Streams overload,
    /// AssetCount bookkeeping, per-Device buffer key allocation past 9,999 total DataItems,
    /// read paths that do not allocate indexes, requested-version quality output and
    /// current-value preservation when a Device model changes after the archive wraps.
    /// </summary>
    [TestFixture]
    [Category("MTConnectAgentBroker")]
    public class MTConnectAgentBrokerTests
    {
        private static MTConnectAgentBroker CreateBroker(uint bufferSize = 1000, bool enableValidation = false)
        {
            var configuration = new AgentConfiguration
            {
                ObservationBufferSize = bufferSize,
                EnableValidation = enableValidation
            };

            return new MTConnectAgentBroker(configuration, initializeAgentDevice: false);
        }

        private static Device CreateDevice(string uuid, params string[] dataItemIds)
        {
            return new Device
            {
                Id = uuid,
                Name = uuid,
                Uuid = uuid,
                DataItems = dataItemIds.Select(id => (IDataItem)new ProgramDataItem { Id = id }).ToList()
            };
        }

        private static IEnumerable<IObservationOutput> AllObservations(IStreamsResponseOutputDocument? document)
        {
            if (document == null || document.Streams == null) return Enumerable.Empty<IObservationOutput>();

            return document.Streams
                .SelectMany(s => s.ComponentStreams ?? Array.Empty<IComponentStreamOutput>())
                .SelectMany(c => c.Observations ?? Array.Empty<IObservationOutput>());
        }

        private static string? Result(IObservationOutput observation)
        {
            return observation.Values?.FirstOrDefault(v => v.Key == ValueKeys.Result).Value;
        }


        /// <summary>
        /// Pins the behaviour expressed by the test name: all devices sample with data item filter and from to returns only requested data items.
        /// </summary>
        [Test]
        public void AllDevicesSample_WithDataItemFilterAndFromTo_ReturnsOnlyRequestedDataItems()
        {
            using var broker = CreateBroker();
            broker.AddDevice(CreateDevice("d1", "p1", "p2"));
            broker.AddObservation("d1", "p1", "a");
            broker.AddObservation("d1", "p2", "b");

            var document = broker.GetDeviceStreamsResponseDocument(new[] { "p1" }, 1UL, broker.LastSequence);

            var observations = AllObservations(document).ToList();
            Assert.That(observations, Is.Not.Empty);
            Assert.That(observations.Select(o => o.DataItemId).Distinct(), Is.EqualTo(new[] { "p1" }));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: sample returns each data items observations in sequence order.
        /// </summary>
        [Test]
        public void Sample_ReturnsEachDataItemsObservationsInSequenceOrder()
        {
            using var broker = CreateBroker();
            broker.AddDevice(CreateDevice("d1", "p1", "p2"));
            for (var i = 0; i < 3; i++)
            {
                broker.AddObservation("d1", "p1", "a" + i);
                broker.AddObservation("d1", "p2", "b" + i);
            }

            var observations = AllObservations(broker.GetDeviceStreamsResponseDocument("d1", 1UL, broker.LastSequence)).ToList();

            var p1 = observations.Where(o => o.DataItemId == "p1").ToList();
            Assert.That(p1.Select(Result), Is.EqualTo(new[] { "UNAVAILABLE", "a0", "a1", "a2" }));
            Assert.That(p1.Select(o => o.Sequence), Is.Ordered.Ascending);
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: asset count after removing one of two and adding another counts two.
        /// </summary>
        [Test]
        public void AssetCount_AfterRemovingOneOfTwoAndAddingAnother_CountsTwo()
        {
            using var broker = CreateBroker();
            broker.AddDevice(CreateDevice("d1", "p1"));

            broker.AddAsset("d1", NewFileAsset("a1"));
            broker.AddAsset("d1", NewFileAsset("a2"));
            broker.RemoveAsset("a1");
            broker.AddAsset("d1", NewFileAsset("a3"));

            Assert.That(FileAssetCount(broker, "d1"), Is.EqualTo("2"));
        }

        private static FileAsset NewFileAsset(string assetId)
        {
            // Size and VersionId are required for the Asset to appear in an MTConnectAssets response
            return new FileAsset { AssetId = assetId, Size = 1, VersionId = "1" };
        }

        private static string? FileAssetCount(MTConnectAgentBroker broker, string deviceKey)
        {
            var assetCount = AllObservations(broker.GetDeviceStreamsResponseDocument(deviceKey))
                .Single(o => o.Type == AssetCountDataItem.TypeId);

            return assetCount.Values?.FirstOrDefault(v => v.Key == ValueKeys.CreateDataSetValueKey(FileAsset.TypeId)).Value;
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: asset count updating an existing asset does not increment.
        /// </summary>
        [Test]
        public void AssetCount_UpdatingAnExistingAsset_DoesNotIncrement()
        {
            using var broker = CreateBroker();
            broker.AddDevice(CreateDevice("d1", "p1"));

            broker.AddAsset("d1", NewFileAsset("a1"));
            broker.AddAsset("d1", NewFileAsset("a1"));
            broker.AddAsset("d1", NewFileAsset("a1"));

            Assert.That(FileAssetCount(broker, "d1"), Is.EqualTo("1"));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: asset count re adding a removed asset counts it again.
        /// </summary>
        [Test]
        public void AssetCount_ReAddingARemovedAsset_CountsItAgain()
        {
            using var broker = CreateBroker();
            broker.AddDevice(CreateDevice("d1", "p1"));

            broker.AddAsset("d1", NewFileAsset("a1"));
            broker.AddAsset("d1", NewFileAsset("a2"));
            broker.RemoveAsset("a1");
            broker.AddAsset("d1", NewFileAsset("a1"));

            Assert.That(FileAssetCount(broker, "d1"), Is.EqualTo("2"));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: asset count removing the same asset twice decrements once.
        /// </summary>
        [Test]
        public void AssetCount_RemovingTheSameAssetTwice_DecrementsOnce()
        {
            using var broker = CreateBroker();
            broker.AddDevice(CreateDevice("d1", "p1"));

            broker.AddAsset("d1", NewFileAsset("a1"));
            broker.AddAsset("d1", NewFileAsset("a2"));
            broker.AddAsset("d1", NewFileAsset("a3"));

            Assert.That(broker.RemoveAsset("a1"), Is.True);
            Assert.That(broker.RemoveAsset("a1"), Is.False);

            Assert.That(FileAssetCount(broker, "d1"), Is.EqualTo("2"));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: asset count overflow eviction decrements.
        /// </summary>
        [Test]
        public void AssetCount_OverflowEviction_Decrements()
        {
            using var broker = new MTConnectAgentBroker(new AgentConfiguration { AssetBufferSize = 2 }, initializeAgentDevice: false);
            broker.AddDevice(CreateDevice("d1", "p1"));

            broker.AddAsset("d1", NewFileAsset("a1"));
            broker.AddAsset("d1", NewFileAsset("a2"));
            broker.AddAsset("d1", NewFileAsset("a3")); // Evicts a1

            Assert.That(FileAssetCount(broker, "d1"), Is.EqualTo("2"));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: asset count evicting an already removed asset does not decrement again.
        /// </summary>
        [Test]
        public void AssetCount_EvictingAnAlreadyRemovedAsset_DoesNotDecrementAgain()
        {
            using var broker = new MTConnectAgentBroker(new AgentConfiguration { AssetBufferSize = 3 }, initializeAgentDevice: false);
            broker.AddDevice(CreateDevice("d1", "p1"));

            broker.AddAsset("d1", NewFileAsset("a1"));
            broker.AddAsset("d1", NewFileAsset("a2"));
            broker.AddAsset("d1", NewFileAsset("a3"));
            broker.RemoveAsset("a2");                                    // Removed assets stay in the buffer, moved to the end: a1, a3, a2
            broker.AddAsset("d1", NewFileAsset("a4"));     // Evicts a1 (still counted): a3, a2, a4
            broker.AddAsset("d1", NewFileAsset("a5"));     // Evicts a3 (still counted): a2, a4, a5
            broker.AddAsset("d1", NewFileAsset("a6"));     // Evicts a2 (already removed): a4, a5, a6

            Assert.That(FileAssetCount(broker, "d1"), Is.EqualTo("3"));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: remove all assets removes every asset of the type.
        /// </summary>
        [Test]
        public void RemoveAllAssets_RemovesEveryAssetOfTheType()
        {
            using var broker = CreateBroker();
            broker.AddDevice(CreateDevice("d1", "p1"));

            broker.AddAsset("d1", NewFileAsset("a1"));
            broker.AddAsset("d1", NewFileAsset("a2"));

            Assert.That(broker.RemoveAllAssets(FileAsset.TypeId), Is.True);
            Assert.That(broker.GetAssetsResponseDocument(type: FileAsset.TypeId).Assets, Is.Empty);

            broker.AddAsset("d1", NewFileAsset("a3"));
            Assert.That(FileAssetCount(broker, "d1"), Is.EqualTo("1"));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: get assets response document default count returns all assets.
        /// </summary>
        [Test]
        public void GetAssetsResponseDocument_DefaultCount_ReturnsAllAssets()
        {
            using var broker = CreateBroker();
            broker.AddDevice(CreateDevice("d1", "p1"));

            broker.AddAsset("d1", NewFileAsset("a1"));
            broker.AddAsset("d1", NewFileAsset("a2"));

            Assert.That(broker.GetAssetsResponseDocument().Assets.Count(), Is.EqualTo(2));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: current more than9999 data items across devices returns every data item.
        /// </summary>
        [Test]
        public void Current_MoreThan9999DataItemsAcrossDevices_ReturnsEveryDataItem()
        {
            // Two Devices x 6,000 DataItems: a shared index counter would pass 9,999 and collide / decode wrongly
            const int perDevice = 6000;
            var d1Ids = Enumerable.Range(1, perDevice).Select(i => "d1_p" + i).ToArray();
            var d2Ids = Enumerable.Range(1, perDevice).Select(i => "d2_p" + i).ToArray();

            using var broker = CreateBroker(bufferSize: 20000);
            broker.AddDevice(CreateDevice("d1", d1Ids));
            broker.AddDevice(CreateDevice("d2", d2Ids));

            var lastId = d2Ids[perDevice - 1];
            broker.AddObservation("d2", lastId, "hello");

            var observations = AllObservations(broker.GetDeviceStreamsResponseDocument("d2")).ToList();

            Assert.That(observations.Count(o => o.DataItemId.StartsWith("d2_p")), Is.EqualTo(perDevice));
            Assert.That(observations.Where(o => o.DataItemId == lastId).Select(Result), Is.EqualTo(new[] { "hello" }));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: data item indexes are allocated per device.
        /// </summary>
        [Test]
        public void DataItemIndexes_AreAllocatedPerDevice()
        {
            using var broker = CreateBroker();

            Assert.That(broker.GetDataItemIndex("d1", "x"), Is.EqualTo(1));
            Assert.That(broker.GetDataItemIndex("d1", "y"), Is.EqualTo(2));
            Assert.That(broker.GetDataItemIndex("d2", "x"), Is.EqualTo(1));
            Assert.That(broker.GetDataItemIndex("d1", "x"), Is.EqualTo(1));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: initialize data item index drops legacy indexes above9999 and resolves by buffer key.
        /// </summary>
        [Test]
        public void InitializeDataItemIndex_DropsLegacyIndexesAbove9999AndResolvesByBufferKey()
        {
            using var broker = CreateBroker();
            broker.InitializeDeviceIndex(new Dictionary<string, int> { { "d1", 1 }, { "d2", 2 } });
            broker.InitializeDataItemIndex(new Dictionary<string, int> { { "d1:a", 1 }, { "d2:b", 2 }, { "d2:c", 12000 } });

            var indexes = broker.DataItemIndexes;
            Assert.That(indexes.ContainsKey("d2:c"), Is.False);
            Assert.That(broker.GetDataItemIdFromBufferKey((2 * 10000) + 2), Is.EqualTo("b"));
            Assert.That(broker.GetDataItemIdFromBufferKey((1 * 10000) + 2), Is.Null);

            // The dropped DataItem gets the next free index for its Device
            Assert.That(broker.GetDataItemIndex("d2", "c"), Is.EqualTo(3));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: streams requests for unknown data item ids do not allocate indexes.
        /// </summary>
        [Test]
        public void StreamsRequests_ForUnknownDataItemIds_DoNotAllocateIndexes()
        {
            using var broker = CreateBroker();
            broker.AddDevice(CreateDevice("d1", "p1"));
            broker.AddDevice(CreateDevice("d2", "q1"));

            var before = broker.DataItemIndexes.Count;

            broker.GetDeviceStreamsResponseDocument("d1", new[] { "nope1", "nope2" });
            broker.GetDeviceStreamsResponseDocument(new[] { "p1", "nope3" });
            broker.GetDeviceStreamsResponseDocument(new[] { "p1" }, 1UL, broker.LastSequence);

            Assert.That(broker.DataItemIndexes.Count, Is.EqualTo(before));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: quality follows requested version.
        /// </summary>
        [Test]
        public void Quality_FollowsRequestedVersion()
        {
            using var broker = CreateBroker(enableValidation: true);
            broker.AddDevice(CreateDevice("d1", "p1"));
            broker.AddObservation("d1", "p1", "x");

            var v25 = AllObservations(broker.GetDeviceStreamsResponseDocument("d1", mtconnectVersion: MTConnectVersions.Version25)).Single(o => o.DataItemId == "p1");
            var v20 = AllObservations(broker.GetDeviceStreamsResponseDocument("d1", mtconnectVersion: MTConnectVersions.Version20)).Single(o => o.DataItemId == "p1");

            Assert.That(v25.Quality, Is.EqualTo(Quality.VALID));
            Assert.That(v20.Quality, Is.EqualTo(Quality.UNVERIFIABLE));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: device model change after archive wraps keeps current values.
        /// </summary>
        [Test]
        public void DeviceModelChange_AfterArchiveWraps_KeepsCurrentValues()
        {
            using var broker = CreateBroker(bufferSize: 10);
            broker.AddDevice(CreateDevice("d1", "p1", "p2"));
            broker.AddObservation("d1", "p1", "keep");

            // Push p1's observation out of the archive
            for (var i = 0; i < 20; i++) broker.AddObservation("d1", "p2", "v" + i);

            // Changing the model re-runs InitializeDataItems
            broker.AddDevice(CreateDevice("d1", "p1", "p2", "p3"));

            var p1 = AllObservations(broker.GetDeviceStreamsResponseDocument("d1")).Single(o => o.DataItemId == "p1");
            Assert.That(Result(p1), Is.EqualTo("keep"));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: index properties return snapshots.
        /// </summary>
        [Test]
        public void IndexProperties_ReturnSnapshots()
        {
            using var broker = CreateBroker();
            broker.AddDevice(CreateDevice("d1", "p1"));

            broker.DeviceIndexes.Clear();
            broker.DataItemIndexes.Clear();

            Assert.That(broker.DeviceIndexes, Is.Not.Empty);
            Assert.That(broker.DataItemIndexes, Is.Not.Empty);
        }
    }
}
