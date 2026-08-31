// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace MTConnect.NET_Common_Tests.DryGenerator
{
    // Parity guard for the DRY-generator campaign's Phase 1 migration.
    //
    // The migration collapses the per-version fixture family under
    //   tests/MTConnect.NET-Common-Tests/V2_6_V2_7/
    // into single-topic fixtures at their canonical location:
    //   tests/MTConnect.NET-Common-Tests/Devices/DataItems/DataItemTypeTests.cs
    //   tests/MTConnect.NET-Common-Tests/Devices/Components/ComponentTests.cs
    //   tests/MTConnect.NET-Common-Tests/Devices/Configurations/ConfigurationTests.cs
    //   tests/MTConnect.NET-Common-Tests/Observations/SampleObservationTests.cs
    //   tests/MTConnect.NET-Common-Tests/Enums/EnumArmTests.cs
    //   tests/MTConnect.NET-Common-Tests/MTConnectVersionsTests.cs
    //
    // Every assertion the pre-migration fixtures carried MUST re-appear at
    // its post-migration home. This fixture asserts that invariant by
    // walking a hardcoded baseline snapshot of the 34 pre-migration
    // [Test] / [TestCase] method entries (captured 2026-08-19 from
    // extra-files.user/plans/dry-generator-phase0/baseline-assertions-2026-08-19.txt)
    // against the live reflection view of the test assembly.
    //
    // States:
    //   - RED (pre-migration): the new topic fixtures do not exist yet;
    //     the baseline entries have no post-migration home. Every entry
    //     surfaces as a missing target.
    //   - GREEN (post-migration): every baseline entry resolves to a
    //     method that carries [Test] / [TestCase] / [TestCaseSource] and
    //     lives OUTSIDE the deprecated V2_6_V2_7 namespace. The V2_6_V2_7
    //     folder itself is deleted; PerVersionFolderProhibitionTests
    //     enforces the deletion permanently.
    //
    // Renames are declared inline via MigrationMap below. The gitignored
    // extra-files.user/plans/dry-generator-phase0/renames.tsv artefact is a
    // human-facing audit trail; the assertion source of truth lives in this
    // fixture so the test is portable across clones.
    /// <summary>Pins the behavior expressed by the test name: assertion parity tests.</summary>
    [TestFixture]
    public class AssertionParityTests
    {
        // Every pre-migration method's expected post-migration name.
        // Identity entries (OldMethod == NewMethod) migrate under the same
        // name; renames carry a NewMethod that strips the `_in_v2_6`
        // suffix or the `V2_7_` prefix, since the version is now a
        // property of the [TestCaseSource(MTConnectVersionMatrix.All)]
        // matrix rather than encoded in the method name.
        private static readonly (string OldFile, string OldMethod, string NewMethod)[] MigrationMap =
        {
            // V2_6ComponentAndEnumTests.cs (3 methods)
            ("V2_6ComponentAndEnumTests.cs", "CuttingTorchComponent_constructs_with_correct_type", "CuttingTorchComponent_constructs_with_correct_type"),
            ("V2_6ComponentAndEnumTests.cs", "ElectrodeComponent_constructs_with_correct_type", "ElectrodeComponent_constructs_with_correct_type"),
            ("V2_6ComponentAndEnumTests.cs", "MediaType_QIF_MBD_value_present_in_v2_6", "MediaType_QIF_MBD_value_present"),

            // V2_6DataItemTypeTests.cs (6 methods)
            ("V2_6DataItemTypeTests.cs", "AssetAddedDataItem_constructs_with_event_metadata", "AssetAddedDataItem_constructs_with_event_metadata"),
            ("V2_6DataItemTypeTests.cs", "AssetAddedDataItem_with_deviceId_produces_qualified_id", "AssetAddedDataItem_with_deviceId_produces_qualified_id"),
            ("V2_6DataItemTypeTests.cs", "AssociatedAssetIdDataItem_constructs_with_event_metadata", "AssociatedAssetIdDataItem_constructs_with_event_metadata"),
            ("V2_6DataItemTypeTests.cs", "AssetAddedDataItem_inherits_from_DataItem", "AssetAddedDataItem_inherits_from_DataItem"),
            ("V2_6DataItemTypeTests.cs", "AssociatedAssetIdDataItem_inherits_from_DataItem", "AssociatedAssetIdDataItem_inherits_from_DataItem"),
            ("V2_6DataItemTypeTests.cs", "AssetChangedDataItem_description_narrowed_in_v2_6", "AssetChangedDataItem_description_narrowed"),

            // V2_7DataItemTypeTests.cs (1 method, 8 [TestCase] rows)
            ("V2_7DataItemTypeTests.cs", "V2_7_DataItem_constructs_with_correct_metadata", "DataItem_constructs_with_correct_metadata"),

            // MTConnectVersionsTests.cs (5 methods — kept plain [Test] since
            // these test constant-value invariants, not per-version behavior)
            ("MTConnectVersionsTests.cs", "Version26_constant_equals_2_6", "Version26_constant_equals_2_6"),
            ("MTConnectVersionsTests.cs", "Version27_constant_equals_2_7", "Version27_constant_equals_2_7"),
            ("MTConnectVersionsTests.cs", "Max_equals_Version27", "Max_equals_Version27"),
            ("MTConnectVersionsTests.cs", "Every_published_version_constant_is_distinct_and_monotonic", "Every_published_version_constant_is_distinct_and_monotonic"),
            ("MTConnectVersionsTests.cs", "Version19_field_does_not_exist", "Version19_field_does_not_exist"),

            // V2_7ComponentTests.cs (2 methods)
            ("V2_7ComponentTests.cs", "PinToolComponent_constructs_with_correct_type", "PinToolComponent_constructs_with_correct_type"),
            ("V2_7ComponentTests.cs", "ToolHolderComponent_constructs_with_correct_type", "ToolHolderComponent_constructs_with_correct_type"),

            // V2_7ConfigurationDataSetTests.cs (16 methods)
            ("V2_7ConfigurationDataSetTests.cs", "DataSet_base_constructs_and_implements_IDataSet", "DataSet_base_constructs_and_implements_IDataSet"),
            ("V2_7ConfigurationDataSetTests.cs", "AxisDataSet_has_xyz_fields_and_implements_IDataSet", "AxisDataSet_has_xyz_fields_and_implements_IDataSet"),
            ("V2_7ConfigurationDataSetTests.cs", "OriginDataSet_has_xyz_fields_and_implements_IDataSet", "OriginDataSet_has_xyz_fields_and_implements_IDataSet"),
            ("V2_7ConfigurationDataSetTests.cs", "RotationDataSet_has_abc_fields_and_implements_IDataSet", "RotationDataSet_has_abc_fields_and_implements_IDataSet"),
            ("V2_7ConfigurationDataSetTests.cs", "ScaleDataSet_implements_IDataSet", "ScaleDataSet_implements_IDataSet"),
            ("V2_7ConfigurationDataSetTests.cs", "TranslationDataSet_implements_IDataSet", "TranslationDataSet_implements_IDataSet"),
            ("V2_7ConfigurationDataSetTests.cs", "Axis_inherits_AbstractAxis_and_constructs", "Axis_inherits_AbstractAxis_and_constructs"),
            ("V2_7ConfigurationDataSetTests.cs", "Origin_inherits_AbstractOrigin", "Origin_inherits_AbstractOrigin"),
            ("V2_7ConfigurationDataSetTests.cs", "Rotation_inherits_AbstractRotation", "Rotation_inherits_AbstractRotation"),
            ("V2_7ConfigurationDataSetTests.cs", "Scale_inherits_AbstractScale", "Scale_inherits_AbstractScale"),
            ("V2_7ConfigurationDataSetTests.cs", "Translation_inherits_AbstractTranslation", "Translation_inherits_AbstractTranslation"),
            ("V2_7ConfigurationDataSetTests.cs", "AbstractAxis_is_abstract", "AbstractAxis_is_abstract"),
            ("V2_7ConfigurationDataSetTests.cs", "AbstractOrigin_is_abstract", "AbstractOrigin_is_abstract"),
            ("V2_7ConfigurationDataSetTests.cs", "AbstractRotation_is_abstract", "AbstractRotation_is_abstract"),
            ("V2_7ConfigurationDataSetTests.cs", "AbstractScale_is_abstract", "AbstractScale_is_abstract"),
            ("V2_7ConfigurationDataSetTests.cs", "AbstractTranslation_is_abstract", "AbstractTranslation_is_abstract"),

            // V2_7SampleObservationTests.cs (1 method)
            ("V2_7SampleObservationTests.cs", "WaterHardness_sample_observation_round_trip", "WaterHardness_sample_observation_round_trip"),
        };

        /// <summary>Pins the invariant: every baseline assertion has a post-migration home.</summary>
        [Test]
        public void Every_baseline_assertion_has_a_post_migration_home()
        {
            var postMigrationMethods = EnumeratePostMigrationTestMethods();
            var missing = new List<string>();

            foreach (var (oldFile, oldMethod, newMethod) in MigrationMap)
            {
                if (!postMigrationMethods.Contains(newMethod))
                {
                    missing.Add($"{oldFile}::{oldMethod} -> {newMethod}");
                }
            }

            Assert.That(missing, Is.Empty,
                "Baseline assertions missing a post-migration home:\n  "
                + string.Join("\n  ", missing));
        }

        /// <summary>Pins the invariant: the migration map covers every baseline entry.</summary>
        [Test]
        public void Migration_map_covers_the_full_baseline_of_34_entries()
        {
            // Guard against silent shrinkage of the map itself. The Phase 0
            // baseline captured exactly 34 [Test] / [TestCase] method
            // entries; if a future edit trims the map below that floor, the
            // parity guard is inspecting less than the full baseline and
            // this fixture must fail loudly.
            Assert.That(MigrationMap.Length, Is.EqualTo(34),
                "MigrationMap has drifted from the 34-entry baseline captured "
                + "on 2026-08-19. Re-verify against "
                + "extra-files.user/plans/dry-generator-phase0/baseline-assertions-2026-08-19.txt "
                + "before editing.");
        }

        // Reflect over the test assembly and return every method name that
        // carries [Test], [TestCase], or [TestCaseSource] and lives OUTSIDE
        // the deprecated V2_6_V2_7 namespace. The name-only granularity
        // matches the plan's Phase 1.4 assertion-diff shape.
        private static ISet<string> EnumeratePostMigrationTestMethods()
        {
            var assembly = typeof(AssertionParityTests).Assembly;
            return assembly.GetTypes()
                .Where(t => t.Namespace != null
                    && !t.Namespace.Contains("V2_6_V2_7", StringComparison.Ordinal))
                .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance))
                .Where(HasNUnitTestAttribute)
                .Select(m => m.Name)
                .ToHashSet(StringComparer.Ordinal);
        }

        private static bool HasNUnitTestAttribute(MethodInfo method)
        {
            foreach (var attribute in method.GetCustomAttributes(inherit: false))
            {
                if (attribute is TestAttribute
                    || attribute is TestCaseAttribute
                    || attribute is TestCaseSourceAttribute)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
