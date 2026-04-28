// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MTConnect.Assets;
using NUnit.Framework;

namespace MTConnect.NET_Common_Tests.Reflection
{
    // Per-category parametric coverage for the regenerated Asset
    // subtypes under MTConnect.Assets and its sub-namespaces. The broad
    // RegeneratedTypesCoverageTests fixture exercises the bare ctor +
    // property round-trip contract; this fixture adds the Asset-specific
    // contract:
    //
    //   - Every concrete Asset subtype is constructible.
    //   - The DescriptionText surface is non-null for every concrete
    //     subtype (the regenerator template emits a string field even when
    //     the SysML XMI ships an empty value).
    //   - The Asset base properties (AssetId, Timestamp, DeviceUuid) are
    //     read-write and round-trip through the inherited setter chain
    //     without throw on every concrete subtype.
    //
    // Source authority:
    //   - SysML XMI: https://github.com/mtconnect/mtconnect_sysml_model
    //     (per-version tag) — defines the Asset class hierarchy and the
    //     DescriptionText values emitted by the regenerator.
    //   - XSD: https://schemas.mtconnect.org/schemas/MTConnectAssets_<vN.M>.xsd
    //     plus per-asset-family XSDs (CuttingTools, Pallet, Fixture).
    //   - Prose: docs.mtconnect.org "Part 4.0 - Assets Information Model"
    //     §"Asset" — defines the asset-id / timestamp / deviceUuid
    //     wire-shape contract.
    [TestFixture]
    public class RegeneratedAssetsCoverageTests
    {
        private static readonly string[] AssetNamespaces =
        {
            "MTConnect.Assets",
            "MTConnect.Assets.ComponentConfigurationParameters",
            "MTConnect.Assets.CuttingTools",
            "MTConnect.Assets.CuttingTools.Measurements",
            "MTConnect.Assets.Files",
            "MTConnect.Assets.Fixture",
            "MTConnect.Assets.Pallet",
            "MTConnect.Assets.QIF",
            "MTConnect.Assets.RawMaterials",
        };

        private static IEnumerable<Type> EnumerateAssetSubtypes()
        {
            return typeof(Asset).Assembly.GetTypes()
                .Where(t => t.IsPublic)
                .Where(t => !t.IsAbstract)
                .Where(t => !t.IsInterface)
                .Where(t => !t.IsGenericTypeDefinition)
                .Where(t => t.Namespace != null && AssetNamespaces.Contains(t.Namespace))
                .Where(t => typeof(Asset).IsAssignableFrom(t))
                .Where(t => t.GetConstructor(Type.EmptyTypes) != null)
                .OrderBy(t => t.FullName, StringComparer.Ordinal);
        }

        public static IEnumerable<TestCaseData> AssetSubtypes()
        {
            foreach (var type in EnumerateAssetSubtypes())
            {
                yield return new TestCaseData(type)
                    .SetName($"Asset_{type.Name}");
            }
        }

        [Test]
        public void Catalogue_enumerates_at_least_one_asset_subtype()
        {
            // Smoke-test the catalogue itself so a regenerator regression
            // that drops every Asset subtype produces a failing row, not a
            // silent green sweep.
            Assert.That(EnumerateAssetSubtypes().Count(), Is.GreaterThan(0),
                "MTConnect.Assets produced zero concrete subtypes — regenerator regression?");
        }

        [Test]
        [TestCaseSource(nameof(AssetSubtypes))]
        public void Asset_subtype_is_constructible(Type type)
        {
            object? instance = null;
            Assert.DoesNotThrow(
                () => instance = Activator.CreateInstance(type),
                $"{type.FullName} parameterless ctor threw");
            Assert.That(instance, Is.Not.Null,
                $"{type.FullName} parameterless ctor returned null");
        }

        [Test]
        [TestCaseSource(nameof(AssetSubtypes))]
        public void Asset_subtype_inherits_AssetId_round_trip(Type type)
        {
            // The base Asset.AssetId / Timestamp / DeviceUuid surface is
            // the wire-shape pin per Part 4.0. Every concrete subtype
            // inherits these; round-tripping a sentinel through each
            // property must not throw and must echo the value.
            var asset = (Asset)Activator.CreateInstance(type)!;
            const string sentinel = "ASSET-SENTINEL-1";
            asset.AssetId = sentinel;

            Assert.That(asset.AssetId, Is.EqualTo(sentinel),
                $"{type.FullName}.AssetId did not round-trip");
        }

        [Test]
        [TestCaseSource(nameof(AssetSubtypes))]
        public void Asset_subtype_inherits_DeviceUuid_round_trip(Type type)
        {
            var asset = (Asset)Activator.CreateInstance(type)!;
            const string sentinel = "DEVICE-UUID-SENTINEL";
            asset.DeviceUuid = sentinel;

            Assert.That(asset.DeviceUuid, Is.EqualTo(sentinel),
                $"{type.FullName}.DeviceUuid did not round-trip");
        }

        [Test]
        [TestCaseSource(nameof(AssetSubtypes))]
        public void Asset_subtype_inherits_Timestamp_round_trip(Type type)
        {
            var asset = (Asset)Activator.CreateInstance(type)!;
            var sentinel = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            asset.Timestamp = sentinel;

            Assert.That(asset.Timestamp, Is.EqualTo(sentinel),
                $"{type.FullName}.Timestamp did not round-trip");
        }

        [Test]
        [TestCaseSource(nameof(AssetSubtypes))]
        public void Asset_subtype_exposes_DescriptionText_field(Type type)
        {
            // The regenerator template always emits a static
            // DescriptionText field for every concrete Asset subtype.
            // A subtype missing the field is a generator-side defect.
            var field = type.GetField("DescriptionText",
                BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            Assert.That(field, Is.Not.Null,
                $"{type.FullName} is missing the DescriptionText field");
        }
    }
}
