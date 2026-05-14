// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using NUnit.Framework;

namespace MTConnect.Tests.Common.Agents
{
    /// <summary>
    /// Regression pin for
    /// https://github.com/TrakHound/MTConnect.NET/issues/132 :
    /// every code path that auto-injects an AssetCountDataItem MUST
    /// stamp Representation = DATA_SET. The defect was an auto-generator
    /// that inherited the .g.cs DefaultRepresentation = VALUE; this
    /// fixture guards every auto-injection entry point we have access
    /// to so a future regenerated generator (or a refactor of
    /// NormalizeDevice) cannot reintroduce the bug silently.
    ///
    /// Spec: MTConnect Standard, Part 2 - Devices Information Model,
    /// ASSET_COUNT (UML _19_0_3_68e0225_1640602520420_217627_44).
    /// </summary>
    [TestFixture]
    [Category("AssetCountIsDataSet")]
    public class AssetCountFactoryDataSetGuardTests
    {
        private static readonly string[] _deviceIds = { "lathe-1", "mill-7", "robot-A" };

        [TestCaseSource(nameof(_deviceIds))]
        public void AddDevice_AutoInjects_AssetCount_With_DataSet_Representation(string deviceId)
        {
            using var agent = new MTConnectAgent(initializeAgentDevice: false);

            var device = new Device
            {
                Id = deviceId,
                Name = deviceId,
                Uuid = $"{deviceId}-uuid"
            };

            var added = agent.AddDevice(device, initializeDataItems: false);

            Assert.That(added, Is.Not.Null);

            var assetCount = added.DataItems
                .SingleOrDefault(d => d.Type == AssetCountDataItem.TypeId);

            Assert.That(assetCount, Is.Not.Null,
                "auto-generator must inject one ASSET_COUNT DataItem on every device");
            Assert.That(assetCount!.Representation,
                Is.EqualTo(DataItemRepresentation.DATA_SET));
        }

        [Test]
        public void AddDevices_AutoInjects_AssetCount_With_DataSet_Representation_For_Every_Device()
        {
            using var agent = new MTConnectAgent(initializeAgentDevice: false);

            var devices = _deviceIds.Select(id => new Device
            {
                Id = id,
                Name = id,
                Uuid = $"{id}-uuid"
            }).ToList<IDevice>();

            var added = agent.AddDevices(devices, initializeDataItems: false);

            Assert.That(added, Is.Not.Null);
            Assert.That(added.Count(), Is.EqualTo(_deviceIds.Length));

            foreach (var dev in added)
            {
                var assetCount = dev.DataItems
                    .SingleOrDefault(d => d.Type == AssetCountDataItem.TypeId);

                Assert.That(assetCount, Is.Not.Null,
                    $"device {dev.Id}: auto-generator must inject one ASSET_COUNT DataItem");
                Assert.That(assetCount!.Representation,
                    Is.EqualTo(DataItemRepresentation.DATA_SET),
                    $"device {dev.Id}: Representation must be DATA_SET");
            }
        }

        [Test]
        public void AddDevice_With_Configuration_AutoInjects_AssetCount_With_DataSet_Representation()
        {
            // Same agent path but constructed via the configuration overload
            // to keep both constructor paths covered.
            var configuration = new AgentConfiguration();
            using var agent = new MTConnectAgent(configuration, initializeAgentDevice: false);

            var device = new Device
            {
                Id = "configured-dev",
                Name = "configured-dev",
                Uuid = "configured-dev-uuid"
            };

            var added = agent.AddDevice(device, initializeDataItems: false);

            Assert.That(added, Is.Not.Null);

            var assetCount = added.DataItems
                .SingleOrDefault(d => d.Type == AssetCountDataItem.TypeId);

            Assert.That(assetCount, Is.Not.Null);
            Assert.That(assetCount!.Representation,
                Is.EqualTo(DataItemRepresentation.DATA_SET));
        }
    }
}
