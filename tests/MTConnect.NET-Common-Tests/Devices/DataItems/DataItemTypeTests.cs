// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using MTConnect.Tests.Common.TestHelpers;
using NUnit.Framework;

namespace MTConnect.NET_Common_Tests.Devices.DataItems
{
    // Version-gated shape assertions for every DataItem type the v2.6 and
    // v2.7 SysML XMI introduces.
    //
    //   - XMI: https://github.com/mtconnect/mtconnect_sysml_model tags
    //          v2.6 (SHA 08185447bf86…):
    //            * AssetAddedDataItem        — xmi:id _2024x_68e0225_1744799118784_270323_23376
    //            * AssociatedAssetIdDataItem — xmi:id _2024x_68e0225_1744800465544_…
    //            * AssetChangedDataItem      — description rewritten in v2.6
    //          v2.7 (SHA 25796ac591bb…) — Observation Types package:
    //            * BindingState (Event), Depth (Event), FixtureAssetId (Event),
    //              SwingAngle (Event), SwingDiameter (Event), SwingRadius (Event),
    //              TaskAssetId (Event), WaterHardness (Sample).
    //   - XSD: https://schemas.mtconnect.org/schemas/MTConnectStreams_2.6.xsd
    //                                                 MTConnectStreams_2.7.xsd
    //          (each TypeId is encoded in the EventEnum / SampleEnum
    //          enumerations.)
    //   - Prose: MTConnect Standard Part_2.0_Streams_v2.6 section 11.5 "Asset
    //          events" (asset-event split rationale);
    //          Part_2.0_Streams_v2.7 sections 11/13 "Event/Sample types"
    //          (v2.7 additions).
    //
    // Every fixture below is matrix-parameterised over
    // MTConnectVersionMatrix.All per plan Design Decision D1
    // (2026-08-19). Assume.That gates each assertion to versions where
    // the spec introduced the type; rows below the floor surface as
    // Inconclusive in the test explorer.
    /// <summary>Pins the behavior expressed by the test name: data item type tests.</summary>
    [TestFixture]
    public class DataItemTypeTests
    {
        // Source: XMI v2.6 UML class `AssetAddedDataItem`; XSD v2.6 enum
        // `EventEnum` value `ASSET_ADDED`.
        /// <summary>Pins the behavior expressed by the test name: asset added data item constructs with event metadata.</summary>
        /// <param name="v">The MTConnect Standard version under test.</param>
        [TestCaseSource(typeof(MTConnectVersionMatrix), nameof(MTConnectVersionMatrix.All))]
        public void AssetAddedDataItem_constructs_with_event_metadata(Version v)
        {
            Assume.That(v, Is.GreaterThanOrEqualTo(MTConnectVersions.Version26),
                "AssetAddedDataItem was introduced in MTConnect v2.6.");

            var d = new AssetAddedDataItem();
            Assert.That(d.Type, Is.EqualTo("ASSET_ADDED"));
            Assert.That(d.Name, Is.EqualTo("assetAdded"));
            Assert.That(d.Category, Is.EqualTo(DataItemCategory.EVENT));
            Assert.That(AssetAddedDataItem.TypeId, Is.EqualTo("ASSET_ADDED"));
            Assert.That(AssetAddedDataItem.NameId, Is.EqualTo("assetAdded"));
            Assert.That(AssetAddedDataItem.CategoryId, Is.EqualTo(DataItemCategory.EVENT));
        }

        // Source: XMI v2.6 — `DataItem.id` formation rule via parent device.
        /// <summary>Pins the behavior expressed by the test name: asset added data item with device id produces qualified id.</summary>
        /// <param name="v">The MTConnect Standard version under test.</param>
        [TestCaseSource(typeof(MTConnectVersionMatrix), nameof(MTConnectVersionMatrix.All))]
        public void AssetAddedDataItem_with_deviceId_produces_qualified_id(Version v)
        {
            Assume.That(v, Is.GreaterThanOrEqualTo(MTConnectVersions.Version26),
                "AssetAddedDataItem was introduced in MTConnect v2.6.");

            var d = new AssetAddedDataItem("dev01");
            Assert.That(d.Id, Is.Not.Null.And.Not.Empty);
            Assert.That(d.Id, Does.Contain("dev01"));
            Assert.That(d.Type, Is.EqualTo("ASSET_ADDED"));
        }

        // Source: XMI v2.6 UML class `AssociatedAssetIdDataItem`; XSD v2.6
        // EventEnum value `ASSOCIATED_ASSET_ID`.
        /// <summary>Pins the behavior expressed by the test name: associated asset id data item constructs with event metadata.</summary>
        /// <param name="v">The MTConnect Standard version under test.</param>
        [TestCaseSource(typeof(MTConnectVersionMatrix), nameof(MTConnectVersionMatrix.All))]
        public void AssociatedAssetIdDataItem_constructs_with_event_metadata(Version v)
        {
            Assume.That(v, Is.GreaterThanOrEqualTo(MTConnectVersions.Version26),
                "AssociatedAssetIdDataItem was introduced in MTConnect v2.6.");

            var d = new AssociatedAssetIdDataItem();
            Assert.That(d.Type, Is.EqualTo(AssociatedAssetIdDataItem.TypeId));
            Assert.That(d.Name, Is.EqualTo(AssociatedAssetIdDataItem.NameId));
            Assert.That(d.Category, Is.EqualTo(AssociatedAssetIdDataItem.CategoryId));
            Assert.That(AssociatedAssetIdDataItem.TypeId, Is.EqualTo("ASSOCIATED_ASSET_ID"));
            Assert.That(AssociatedAssetIdDataItem.CategoryId, Is.EqualTo(DataItemCategory.EVENT));
        }

        // Source: XMI v2.6 — generalization of `AssetAddedDataItem` is `DataItem`.
        /// <summary>Pins the behavior expressed by the test name: asset added data item inherits from data item.</summary>
        /// <param name="v">The MTConnect Standard version under test.</param>
        [TestCaseSource(typeof(MTConnectVersionMatrix), nameof(MTConnectVersionMatrix.All))]
        public void AssetAddedDataItem_inherits_from_DataItem(Version v)
        {
            Assume.That(v, Is.GreaterThanOrEqualTo(MTConnectVersions.Version26),
                "AssetAddedDataItem was introduced in MTConnect v2.6.");

            Assert.That(typeof(AssetAddedDataItem).BaseType, Is.EqualTo(typeof(DataItem)));
        }

        // Source: XMI v2.6 — generalization of `AssociatedAssetIdDataItem` is `DataItem`.
        /// <summary>Pins the behavior expressed by the test name: associated asset id data item inherits from data item.</summary>
        /// <param name="v">The MTConnect Standard version under test.</param>
        [TestCaseSource(typeof(MTConnectVersionMatrix), nameof(MTConnectVersionMatrix.All))]
        public void AssociatedAssetIdDataItem_inherits_from_DataItem(Version v)
        {
            Assume.That(v, Is.GreaterThanOrEqualTo(MTConnectVersions.Version26),
                "AssociatedAssetIdDataItem was introduced in MTConnect v2.6.");

            Assert.That(typeof(AssociatedAssetIdDataItem).BaseType, Is.EqualTo(typeof(DataItem)));
        }

        // Source: XMI v2.6 description on `AssetChangedDataItem` (was "added or
        // changed" in v2.5; now "changed" only). Prose confirms in
        // Part_2.0_Streams_v2.6 section 11.5.
        /// <summary>Pins the behavior expressed by the test name: asset changed data item description narrowed.</summary>
        /// <param name="v">The MTConnect Standard version under test.</param>
        [TestCaseSource(typeof(MTConnectVersionMatrix), nameof(MTConnectVersionMatrix.All))]
        public void AssetChangedDataItem_description_narrowed(Version v)
        {
            Assume.That(v, Is.GreaterThanOrEqualTo(MTConnectVersions.Version26),
                "The narrowed description shipped in MTConnect v2.6.");

            Assert.That(AssetChangedDataItem.DescriptionText,
                Is.EqualTo("AssetId of the Asset that has been changed."),
                "AssetChangedDataItem description must reflect the v2.6 split " +
                "where 'added' moved to AssetAddedDataItem");
        }

        // Combined enumeration of the (Type, ExpectedTypeId, ExpectedCategory)
        // triples for the v2.7 DataItem additions, cross-multiplied with
        // MTConnectVersionMatrix.All so each row exercises the full 17-way
        // version matrix. Assume.That gates every row to v2.7.
        //
        // Categories match what the v2.7 SysML XMI declares — the spec
        // authority. Several types that look "measurement-y" (SwingAngle,
        // Depth, etc.) are EVENT in the spec rather than SAMPLE; locking
        // them so a future regen drift is caught immediately.
        /// <summary>Enumerates the (type, expected type id, expected category, version) rows for the v2.7 DataItem sweep.</summary>
        /// <returns>The parametric matrix.</returns>
        public static IEnumerable<TestCaseData> V27DataItemCases()
        {
            var kinds = new (Type Type, string TypeId, DataItemCategory Category)[]
            {
                (typeof(BindingStateDataItem), "BINDING_STATE", DataItemCategory.EVENT),
                (typeof(DepthDataItem), "DEPTH", DataItemCategory.EVENT),
                (typeof(FixtureAssetIdDataItem), "FIXTURE_ASSET_ID", DataItemCategory.EVENT),
                (typeof(SwingAngleDataItem), "SWING_ANGLE", DataItemCategory.EVENT),
                (typeof(SwingDiameterDataItem), "SWING_DIAMETER", DataItemCategory.EVENT),
                (typeof(SwingRadiusDataItem), "SWING_RADIUS", DataItemCategory.EVENT),
                (typeof(TaskAssetIdDataItem), "TASK_ASSET_ID", DataItemCategory.EVENT),
                (typeof(WaterHardnessDataItem), "WATER_HARDNESS", DataItemCategory.SAMPLE),
            };

            foreach (var v in MTConnectVersionMatrix.All)
            {
                foreach (var (type, typeId, category) in kinds)
                {
                    yield return new TestCaseData(type, typeId, category, v)
                        .SetName($"DataItem_constructs_with_correct_metadata({type.Name},{typeId},{category},{v})");
                }
            }
        }

        // Source: XMI v2.7 Observation Types package (each entry above).
        /// <summary>Pins the behavior expressed by the test name: data item constructs with correct metadata.</summary>
        /// <param name="dataItemType">The data item type.</param>
        /// <param name="expectedTypeId">The expected type id.</param>
        /// <param name="expectedCategory">The expected category.</param>
        /// <param name="v">The MTConnect Standard version under test.</param>
        [TestCaseSource(nameof(V27DataItemCases))]
        public void DataItem_constructs_with_correct_metadata(
            Type dataItemType,
            string expectedTypeId,
            DataItemCategory expectedCategory,
            Version v)
        {
            Assume.That(v, Is.GreaterThanOrEqualTo(MTConnectVersions.Version27),
                "These DataItem types were introduced in MTConnect v2.7.");

            // Wrap with Assert.DoesNotThrow so a missing parameterless ctor
            // surfaces as a clear NUnit failure with the offending type
            // name rather than a bare MissingMethodException.
            object? instance = null;
            Assert.DoesNotThrow(() => instance = Activator.CreateInstance(dataItemType),
                $"{dataItemType.Name} should have a public parameterless constructor");
            Assert.That(instance, Is.Not.Null);
            Assert.That(instance, Is.InstanceOf<DataItem>());

            var di = (DataItem)instance!;
            Assert.That(di.Type, Is.EqualTo(expectedTypeId),
                $"{dataItemType.Name}.Type should be the spec TypeId");
            Assert.That(di.Category, Is.EqualTo(expectedCategory),
                $"{dataItemType.Name}.Category should be {expectedCategory}");

            var typeIdConst = dataItemType.GetField("TypeId",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)?.GetRawConstantValue();
            Assert.That(typeIdConst, Is.EqualTo(expectedTypeId),
                $"{dataItemType.Name}.TypeId static const should match the spec TypeId");
        }
    }
}
