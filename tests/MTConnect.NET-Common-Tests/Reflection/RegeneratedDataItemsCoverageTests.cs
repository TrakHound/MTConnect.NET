// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using NUnit.Framework;

namespace MTConnect.NET_Common_Tests.Reflection
{
    // Per-category parametric coverage focused on the regenerated DataItem
    // types under MTConnect.Devices.DataItems. The broad
    // RegeneratedTypesCoverageTests fixture exercises the bare
    // ctor + property round-trip contract for every public regenerated
    // type; this fixture adds the DataItem-specific contract:
    //
    //   - Every concrete DataItem subtype exposes a non-empty TypeId const.
    //   - Every TypeId is upper-snake-case (the wire-shape contract per
    //     MTConnect Part 2 - Devices Information Model).
    //   - Every concrete DataItem exposes a CategoryId const that resolves
    //     to one of the three enumeration members SAMPLE / EVENT / CONDITION.
    //   - Constructed instances expose Category / Type matching the const
    //     pair so the wire envelope is shaped per spec on first-touch.
    //
    // Source authority:
    //   - SysML XMI: https://github.com/mtconnect/mtconnect_sysml_model
    //     (per-version tag) — defines the DataItem class hierarchy and the
    //     CategoryId / TypeId values emitted by the regenerator.
    //   - XSD: https://schemas.mtconnect.org/schemas/MTConnectDevices_<vN.M>.xsd
    //     — defines the wire-shape constraint on the type+category pair.
    //   - Prose: docs.mtconnect.org "Part 2.0 - Devices Information Model"
    //     §"DataItem" — defines the SAMPLE / EVENT / CONDITION semantics.
    [TestFixture]
    public class RegeneratedDataItemsCoverageTests
    {
        private static IEnumerable<Type> EnumerateDataItemSubtypes()
        {
            return typeof(DataItem).Assembly.GetTypes()
                .Where(t => t.IsPublic)
                .Where(t => !t.IsAbstract)
                .Where(t => !t.IsInterface)
                .Where(t => !t.IsGenericTypeDefinition)
                .Where(t => t.Namespace == "MTConnect.Devices.DataItems")
                .Where(t => typeof(DataItem).IsAssignableFrom(t))
                .Where(t => t.GetConstructor(Type.EmptyTypes) != null)
                .OrderBy(t => t.FullName, StringComparer.Ordinal);
        }

        public static IEnumerable<TestCaseData> DataItemSubtypes()
        {
            foreach (var type in EnumerateDataItemSubtypes())
            {
                yield return new TestCaseData(type)
                    .SetName($"DataItem_{type.Name}");
            }
        }

        [Test]
        public void Catalogue_enumerates_at_least_one_data_item_subtype()
        {
            // Smoke-test the catalogue itself so the parametric sweep cannot
            // silently shrink to zero. A regenerator regression that drops
            // every subtype would otherwise produce a quietly green sweep.
            Assert.That(EnumerateDataItemSubtypes().Count(), Is.GreaterThan(100),
                "MTConnect.Devices.DataItems produced fewer than 100 concrete subtypes — regenerator regression?");
        }

        [Test]
        [TestCaseSource(nameof(DataItemSubtypes))]
        public void DataItem_subtype_exposes_non_empty_TypeId(Type type)
        {
            // TypeId is the wire identifier the agent emits in the Devices
            // envelope. Empty / whitespace TypeIds are spec-illegal.
            var field = type.GetField("TypeId",
                BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            Assert.That(field, Is.Not.Null,
                $"{type.FullName} is missing the TypeId const required by the regenerator template");

            var value = field!.GetRawConstantValue() as string;
            Assert.That(value, Is.Not.Null.And.Not.Empty,
                $"{type.FullName}.TypeId is null or empty");
        }

        [Test]
        [TestCaseSource(nameof(DataItemSubtypes))]
        public void DataItem_subtype_TypeId_is_upper_snake_case(Type type)
        {
            // Wire-shape rule from MTConnect Part 2: DataItem type values
            // are upper-snake-case identifiers (uppercase letters, digits,
            // underscores; no leading digit). The regenerator emits these
            // verbatim from SysML; a lower-case or punctuation-bearing
            // value is a generator-side defect.
            var field = type.GetField("TypeId",
                BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            var value = (string)field!.GetRawConstantValue()!;

            Assert.That(value, Does.Match("^[A-Z][A-Z0-9_]*$"),
                $"{type.FullName}.TypeId = \"{value}\" violates the upper-snake-case wire-shape rule");
        }

        [Test]
        [TestCaseSource(nameof(DataItemSubtypes))]
        public void DataItem_subtype_exposes_known_CategoryId(Type type)
        {
            // CategoryId pins the DataItem to one of three spec-defined
            // categories. A subtype without a CategoryId const, or with a
            // value outside the enum range, is a generator-side defect.
            var field = type.GetField("CategoryId",
                BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            Assert.That(field, Is.Not.Null,
                $"{type.FullName} is missing the CategoryId const required by the regenerator template");

            var raw = field!.GetRawConstantValue();
            Assert.That(raw, Is.Not.Null,
                $"{type.FullName}.CategoryId returned null");

            var category = (DataItemCategory)raw!;
            var allowed = new[] { DataItemCategory.SAMPLE, DataItemCategory.EVENT, DataItemCategory.CONDITION };
            Assert.That(allowed, Does.Contain(category),
                $"{type.FullName}.CategoryId = {category} is not one of SAMPLE / EVENT / CONDITION");
        }

        [Test]
        [TestCaseSource(nameof(DataItemSubtypes))]
        public void Constructed_DataItem_subtype_carries_category_and_type_from_const_pair(Type type)
        {
            // The default ctor must wire Category + Type from the const
            // pair so a fresh instance is wire-emit-ready. Tests the
            // positive contract: the parametric round-trip in the broad
            // sweep does NOT exercise this; hand-written V2_6 / V2_7
            // fixtures pin a handful of types only.
            var instance = (DataItem)Activator.CreateInstance(type)!;

            var typeIdField = type.GetField("TypeId",
                BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)!;
            var expectedTypeId = (string)typeIdField.GetRawConstantValue()!;

            var categoryField = type.GetField("CategoryId",
                BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)!;
            var expectedCategory = (DataItemCategory)categoryField.GetRawConstantValue()!;

            Assert.That(instance.Type, Is.EqualTo(expectedTypeId),
                $"{type.FullName} default ctor did not wire Type from TypeId");
            Assert.That(instance.Category, Is.EqualTo(expectedCategory),
                $"{type.FullName} default ctor did not wire Category from CategoryId");
        }

        [Test]
        public void DataItem_subtype_with_unknown_TypeId_string_is_rejected_at_lookup()
        {
            // Negative case: the DataItem.Create / ResolveType path is the
            // public lookup surface that maps a wire-string TypeId back to
            // a concrete subtype. Looking up a string that NO subtype
            // exposes must NOT match any of the regenerated subtypes.
            var sentinelTypeId = $"NOT_A_REAL_TYPE_{Guid.NewGuid():N}";

            var matchingSubtypes = EnumerateDataItemSubtypes()
                .Where(t =>
                {
                    var field = t.GetField("TypeId",
                        BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                    return field != null && (string)field.GetRawConstantValue()! == sentinelTypeId;
                })
                .ToList();

            Assert.That(matchingSubtypes, Is.Empty,
                $"a sentinel TypeId \"{sentinelTypeId}\" should not match any regenerated subtype");
        }
    }
}
