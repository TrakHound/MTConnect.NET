using System;
using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using NUnit.Framework;

namespace MTConnect.NET_Common_Tests.V2_6_V2_7
{
    // Pins every DataItem type the v2.6 SysML XMI introduces.
    //
    //   - XMI: mtconnect/mtconnect_sysml_model @ v2.6 (SHA 08185447bf86…)
    //          UML classes:
    //            * `AssetAddedDataItem`        — xmi:id _2024x_68e0225_1744799118784_270323_23376
    //            * `AssociatedAssetIdDataItem` — xmi:id _2024x_68e0225_1744800465544_…
    //            * `AssetChangedDataItem`      — description rewritten in v2.6
    //   - XSD: schemas.mtconnect.org/schemas/MTConnectStreams_2.6.xsd
    //          (the EVENT category for both new types is encoded in the
    //          MTConnectStreams XSD's enumerations.)
    //   - Prose: MTConnect Standard Part_2.0_Streams_v2.6 section 11.5 "Asset events"
    //          (clarifies the v2.5 → v2.6 split — `AssetChanged` narrowed to
    //          changes only; `AssetAdded` introduced for additions.)
    [TestFixture]
    public class V2_6DataItemTypeTests
    {
        // Source: XMI v2.6 UML class `AssetAddedDataItem`; XSD v2.6 enum `EventEnum`
        // value `ASSET_ADDED`.
        [Test]
        public void AssetAddedDataItem_constructs_with_event_metadata()
        {
            var d = new AssetAddedDataItem();
            Assert.That(d.Type, Is.EqualTo("ASSET_ADDED"));
            Assert.That(d.Name, Is.EqualTo("assetAdded"));
            Assert.That(d.Category, Is.EqualTo(DataItemCategory.EVENT));
            Assert.That(AssetAddedDataItem.TypeId, Is.EqualTo("ASSET_ADDED"));
            Assert.That(AssetAddedDataItem.NameId, Is.EqualTo("assetAdded"));
            Assert.That(AssetAddedDataItem.CategoryId, Is.EqualTo(DataItemCategory.EVENT));
        }

        // Source: XMI v2.6 — `DataItem.id` formation rule via parent device.
        [Test]
        public void AssetAddedDataItem_with_deviceId_produces_qualified_id()
        {
            var d = new AssetAddedDataItem("dev01");
            Assert.That(d.Id, Is.Not.Null.And.Not.Empty);
            Assert.That(d.Id, Does.Contain("dev01"));
            Assert.That(d.Type, Is.EqualTo("ASSET_ADDED"));
        }

        // Source: XMI v2.6 UML class `AssociatedAssetIdDataItem`; XSD v2.6
        // EventEnum value `ASSOCIATED_ASSET_ID`.
        [Test]
        public void AssociatedAssetIdDataItem_constructs_with_event_metadata()
        {
            var d = new AssociatedAssetIdDataItem();
            Assert.That(d.Type, Is.EqualTo(AssociatedAssetIdDataItem.TypeId));
            Assert.That(d.Name, Is.EqualTo(AssociatedAssetIdDataItem.NameId));
            Assert.That(d.Category, Is.EqualTo(AssociatedAssetIdDataItem.CategoryId));
            Assert.That(AssociatedAssetIdDataItem.TypeId, Is.EqualTo("ASSOCIATED_ASSET_ID"));
            Assert.That(AssociatedAssetIdDataItem.CategoryId, Is.EqualTo(DataItemCategory.EVENT));
        }

        // Source: XMI v2.6 — generalization of `AssetAddedDataItem` is `DataItem`.
        [Test]
        public void AssetAddedDataItem_inherits_from_DataItem()
        {
            Assert.That(typeof(AssetAddedDataItem).BaseType, Is.EqualTo(typeof(DataItem)));
        }

        // Source: XMI v2.6 — generalization of `AssociatedAssetIdDataItem` is `DataItem`.
        [Test]
        public void AssociatedAssetIdDataItem_inherits_from_DataItem()
        {
            Assert.That(typeof(AssociatedAssetIdDataItem).BaseType, Is.EqualTo(typeof(DataItem)));
        }

        // Source: XMI v2.6 description on `AssetChangedDataItem` (was "added or
        // changed" in v2.5; now "changed" only). Prose confirms in
        // Part_2.0_Streams_v2.6 section 11.5.
        [Test]
        public void AssetChangedDataItem_description_narrowed_in_v2_6()
        {
            Assert.That(AssetChangedDataItem.DescriptionText,
                Is.EqualTo("AssetId of the Asset that has been changed."),
                "AssetChangedDataItem description must reflect the v2.6 split " +
                "where 'added' moved to AssetAddedDataItem");
        }
    }
}
