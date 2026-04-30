// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using MTConnect.Agents;
using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using NUnit.Framework;

namespace MTConnect.Tests.Common.Agents
{
    /// <summary>
    /// Pins the contract that <see cref="MTConnectAgent.AddDevice"/> backfills
    /// the four required Device-level DataItems exactly once each, regardless
    /// of the starting state of <c>device.DataItems</c>:
    ///   - Availability
    ///   - AssetChanged
    ///   - AssetRemoved
    ///   - AssetCount
    ///
    /// Lets the inner-loop perf optimization (cast DataItems once and use a
    /// HashSet for type-id checks) refactor without regressing behavior.
    /// </summary>
    [TestFixture]
    public class NormalizeDeviceRequiredDataItemsTests
    {
        private static readonly string[] RequiredTypeIds =
        {
            AvailabilityDataItem.TypeId,
            AssetChangedDataItem.TypeId,
            AssetRemovedDataItem.TypeId,
            AssetCountDataItem.TypeId,
        };

        private static IEnumerable<IDataItem>? StartingDataItemsForCase(string startingState)
        {
            return startingState switch
            {
                "null" => null,
                "empty" => new List<IDataItem>(),
                "with_availability" => new List<IDataItem>
                {
                    new AvailabilityDataItem("dev1"),
                },
                "with_all_required" => new List<IDataItem>
                {
                    new AvailabilityDataItem("dev1"),
                    new AssetChangedDataItem("dev1"),
                    new AssetRemovedDataItem("dev1"),
                    new AssetCountDataItem("dev1"),
                },
                _ => null,
            };
        }

        [TestCase("null")]
        [TestCase("empty")]
        [TestCase("with_availability")]
        [TestCase("with_all_required")]
        public void AddDevice_backfills_all_required_dataItems_exactly_once(string startingState)
        {
            using var agent = new MTConnectAgent(uuid: "test-agent", initializeAgentDevice: false);
            var device = new Device
            {
                Id = "dev1",
                Uuid = "dev1-uuid",
                Name = "dev1",
                Type = Device.TypeId,
                DataItems = StartingDataItemsForCase(startingState),
            };

            var added = agent.AddDevice(device, initializeDataItems: false);

            Assert.That(added, Is.Not.Null, "AddDevice must return the normalized device.");
            Assert.That(added.DataItems, Is.Not.Null, "Normalized DataItems must not be null after backfill.");

            var types = added.DataItems!.Select(d => d.Type).ToList();
            foreach (var requiredType in RequiredTypeIds)
            {
                Assert.That(types.Count(t => t == requiredType), Is.EqualTo(1),
                    $"Required DataItem type '{requiredType}' must appear exactly once after AddDevice.");
            }
        }

        [Test]
        public void AddDevice_preserves_user_provided_dataItems_alongside_required_ones()
        {
            using var agent = new MTConnectAgent(uuid: "test-agent", initializeAgentDevice: false);
            var custom = new DataItem(DataItemCategory.EVENT, "PROGRAM", null, "dev1-program");
            var device = new Device
            {
                Id = "dev1",
                Uuid = "dev1-uuid",
                Name = "dev1",
                Type = Device.TypeId,
                DataItems = new List<IDataItem> { custom },
            };

            var added = agent.AddDevice(device, initializeDataItems: false);

            Assert.That(added!.DataItems, Is.Not.Null);
            Assert.That(added.DataItems!.Any(d => d.Id == "dev1-program"), Is.True,
                "User-provided DataItems must survive the required-DataItem backfill.");
            foreach (var requiredType in RequiredTypeIds)
            {
                Assert.That(added.DataItems!.Any(d => d.Type == requiredType), Is.True,
                    $"Required DataItem type '{requiredType}' must still be backfilled when custom DataItems are present.");
            }
        }
    }
}
