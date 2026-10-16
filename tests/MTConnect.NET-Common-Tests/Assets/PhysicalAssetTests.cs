// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Linq;
using MTConnect.Agents;
using MTConnect.Assets;
using MTConnect.Assets.Fixture;
using MTConnect.Assets.Pallet;
using MTConnect.Configurations;
using MTConnect.Devices;
using NUnit.Framework;

namespace MTConnect.Tests.Common.Assets
{
    /// <summary>
    /// Pins the physical assets (<see cref="PalletAsset"/> and <see cref="FixtureAsset"/>, MTConnect 2.5+)
    /// as real assets: they implement <see cref="IAsset"/>, carry their type name (the pallet's own type
    /// is <see cref="PalletAsset.PalletType"/>, not <see cref="IAsset.Type"/>), are created by name,
    /// are excluded before 2.5 and can be held and served by an agent.
    /// </summary>
    [TestFixture]
    [Category("PhysicalAsset")]
    public class PhysicalAssetTests
    {
        [Test]
        public void Pallet_is_an_asset_whose_type_is_Pallet()
        {
            var pallet = new PalletAsset { AssetId = "P1", PalletType = "Machine" };

            IAsset asset = pallet;
            Assert.That(asset, Is.InstanceOf<IPhysicalAsset>());
            Assert.That(asset.Type, Is.EqualTo(PalletAsset.TypeId));
            Assert.That(((IPalletAsset)pallet).PalletType, Is.EqualTo("Machine"));
        }

        [Test]
        public void Fixture_is_an_asset_whose_type_is_Fixture()
        {
            IAsset asset = new FixtureAsset { AssetId = "X1" };

            Assert.That(asset, Is.InstanceOf<IPhysicalAsset>());
            Assert.That(asset.Type, Is.EqualTo(FixtureAsset.TypeId));
        }

        [Test]
        public void Asset_Create_makes_pallets_and_fixtures()
        {
            Assert.That(Asset.Create(PalletAsset.TypeId), Is.InstanceOf<PalletAsset>());
            Assert.That(Asset.Create(FixtureAsset.TypeId), Is.InstanceOf<FixtureAsset>());
        }

        [Test]
        public void Physical_assets_are_excluded_before_2_5()
        {
            var pallet = new PalletAsset { AssetId = "P1" };
            var fixture = new FixtureAsset { AssetId = "X1" };

            Assert.That(pallet.Process(MTConnectVersions.Version24), Is.Null);
            Assert.That(fixture.Process(MTConnectVersions.Version24), Is.Null);
            Assert.That(pallet.Process(MTConnectVersions.Version25), Is.InstanceOf<PalletAsset>());
            Assert.That(fixture.Process(MTConnectVersions.Version25), Is.InstanceOf<FixtureAsset>());
        }

        [Test]
        public void Agent_holds_and_serves_physical_assets()
        {
            var broker = new MTConnectAgentBroker(new AgentConfiguration(), initializeAgentDevice: false);
            broker.AddDevice(new Device { Id = "d1", Name = "d1", Uuid = "d1" });

            var pallet = new PalletAsset { AssetId = "P1", PalletId = "pallet-1" };
            pallet.Measurements = new IMeasurement[] { new WeightMeasurement(12.5) };
            broker.AddAsset("d1", pallet);
            broker.AddAsset("d1", new FixtureAsset { AssetId = "X1", FixtureId = "fixture-1" });

            var assets = broker.GetAssetsResponseDocument().Assets.ToList();
            Assert.That(assets.Select(o => o.Type), Is.EquivalentTo(new[] { PalletAsset.TypeId, FixtureAsset.TypeId }));

            var served = (IPalletAsset)assets.Single(o => o.Type == PalletAsset.TypeId);
            Assert.That(served.PalletId, Is.EqualTo("pallet-1"));
            Assert.That(served.Measurements.Single().Value, Is.EqualTo(12.5));

            Assert.That(broker.GetAssetsResponseDocument(type: PalletAsset.TypeId).Assets.Select(o => o.AssetId), Is.EqualTo(new[] { "P1" }));
        }
    }
}
