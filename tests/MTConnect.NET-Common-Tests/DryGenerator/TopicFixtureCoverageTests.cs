// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace MTConnect.NET_Common_Tests.DryGenerator
{
    // Phase 2.2 topic-fixture coverage guard (DRY-generator campaign plan
    // §2.2 — "Extend RegeneratedTypesCoverageTests with per-topic coverage").
    //
    // For every spec-anchor type migrated out of the deprecated per-version
    // fixture family (tests/MTConnect.NET-Common-Tests/V2_6_V2_7/) the
    // canonical topic-fixture file MUST name the type at least once. This
    // is a permanent guard: a future edit that renames a topic fixture or
    // deletes an anchor assertion without a replacement fires RED here.
    //
    // Complementarity with the sibling guards:
    //   - AssertionParityTests holds the 34-entry method-name migration
    //     map and asserts every entry resolves to a live [Test] method.
    //     It does NOT check that the resolved method actually references
    //     the anchor type (a rename that changed the fixture location AND
    //     replaced the assertion body would slip past a name-only guard).
    //   - PerVersionFolderProhibitionTests asserts no V<N_M>/ topology
    //     regrowth. It says nothing about coverage of anchor types.
    //   - TopicFixtureCoverageTests (this file) asserts every anchor type
    //     name appears in its designated topic-fixture source file. This
    //     is the source-text cross-check the plan calls for.
    //
    // Source of truth for the anchor list: the migration renames.tsv
    // artefact under extra-files.user/plans/dry-generator-phase0/
    // (gitignored — the tsv is the audit trail; the C# entries below
    // are the assertion source). Every anchor type mentioned in a
    // renames.tsv row lands here with its topic-fixture destination.
    //
    // Substring-match risk: the scan uses a whole-token regex
    // (\b<TypeName>\b) so a shorter type name (e.g. Axis) does not falsely
    // match a longer one (AxisDataSet, AbstractAxis). Comments inside the
    // topic fixture count as valid mentions — the guard is that the type
    // name is present in the source text, not that a specific attribute
    // shape references it.
    //
    // Source authority:
    //   - SysML XMI: https://github.com/mtconnect/mtconnect_sysml_model
    //     (per-version tag). Every anchor type maps to a UML class that
    //     the SysML importer emits into MTConnect.NET-Common.
    //   - MTConnect Standard Part 2 — Devices Information Model /
    //     Part 3 — Streams / Part 4 — Assets. Defines the topic
    //     hierarchy the topic-fixture files mirror.
    /// <summary>Pins the invariant: every migrated spec-anchor type is named in its designated topic-fixture source file.</summary>
    [TestFixture]
    public class TopicFixtureCoverageTests
    {
        // (anchor_type_name, topic_fixture_relative_path) pairs sourced
        // verbatim from renames.tsv. New spec-version bumps append rows
        // here alongside the topic-fixture edit; this map is the ONE
        // place the anchor-set is versioned.
        //
        // Path is relative to the tests/MTConnect.NET-Common-Tests/
        // project root. Forward slashes match POSIX conventions; the
        // path resolver below normalises for Windows.
        private static readonly (string AnchorType, string TopicFixtureRelativePath)[] TopicAnchors =
        {
            // --- Component types (2 v2.6, 2 v2.7) ---------------------
            ("CuttingTorchComponent", "Devices/Components/ComponentTests.cs"),
            ("ElectrodeComponent",   "Devices/Components/ComponentTests.cs"),
            ("PinToolComponent",     "Devices/Components/ComponentTests.cs"),
            ("ToolHolderComponent",  "Devices/Components/ComponentTests.cs"),

            // --- DataItem types (v2.6 anchor set) ---------------------
            ("AssetAddedDataItem",         "Devices/DataItems/DataItemTypeTests.cs"),
            ("AssociatedAssetIdDataItem",  "Devices/DataItems/DataItemTypeTests.cs"),
            ("AssetChangedDataItem",       "Devices/DataItems/DataItemTypeTests.cs"),

            // --- Configuration DataSet types (v2.7 anchor set) --------
            ("DataSet",              "Devices/Configurations/ConfigurationTests.cs"),
            ("AxisDataSet",          "Devices/Configurations/ConfigurationTests.cs"),
            ("OriginDataSet",        "Devices/Configurations/ConfigurationTests.cs"),
            ("RotationDataSet",      "Devices/Configurations/ConfigurationTests.cs"),
            ("ScaleDataSet",         "Devices/Configurations/ConfigurationTests.cs"),
            ("TranslationDataSet",   "Devices/Configurations/ConfigurationTests.cs"),
            ("AbstractAxis",         "Devices/Configurations/ConfigurationTests.cs"),
            ("AbstractOrigin",       "Devices/Configurations/ConfigurationTests.cs"),
            ("AbstractRotation",     "Devices/Configurations/ConfigurationTests.cs"),
            ("AbstractScale",        "Devices/Configurations/ConfigurationTests.cs"),
            ("AbstractTranslation",  "Devices/Configurations/ConfigurationTests.cs"),

            // --- Sample observation (v2.7 anchor) ---------------------
            ("WaterHardness", "Observations/SampleObservationTests.cs"),

            // --- Enum arm (v2.6 anchor) -------------------------------
            ("MediaType", "Enums/EnumArmTests.cs"),
            ("QIF_MBD",   "Enums/EnumArmTests.cs"),

            // --- MTConnectVersions constants (v2.6/v2.7 anchors) ------
            ("Version26", "MTConnectVersionsTests.cs"),
            ("Version27", "MTConnectVersionsTests.cs"),
        };

        /// <summary>Produces one test-case row per (anchor_type, topic_fixture) pair.</summary>
        /// <returns>Enumeration of NUnit TestCaseData rows keyed by anchor type name.</returns>
        public static IEnumerable<TestCaseData> Anchors()
        {
            foreach (var (anchorType, topicFixtureRelativePath) in TopicAnchors)
            {
                yield return new TestCaseData(anchorType, topicFixtureRelativePath)
                    .SetName($"Topic_fixture_names_{anchorType}");
            }
        }

        /// <summary>Pins the invariant: the designated topic fixture source references the anchor type by name at least once.</summary>
        /// <param name="anchorType">The spec-anchor type name (as it appears in generated C# sources).</param>
        /// <param name="topicFixtureRelativePath">Path to the topic-fixture file, relative to the test project root; forward-slash separator.</param>
        [Test]
        [TestCaseSource(nameof(Anchors))]
        public void Topic_fixture_source_references_anchor_type(string anchorType, string topicFixtureRelativePath)
        {
            var testsRoot = LocateTestProjectRoot();
            var absolutePath = Path.Combine(testsRoot,
                topicFixtureRelativePath.Replace('/', Path.DirectorySeparatorChar));

            Assert.That(File.Exists(absolutePath), Is.True,
                $"Topic fixture file '{topicFixtureRelativePath}' is missing under '{testsRoot}'. "
                + "The topic-first convention requires every anchor type to live in its "
                + "canonical topic fixture; adding an anchor row to TopicAnchors and then "
                + "renaming/deleting the target file is the failure mode this guard catches.");

            var source = File.ReadAllText(absolutePath);
            // Whole-word match so that a shorter anchor (e.g. Axis) does not
            // spuriously match a longer type name (AbstractAxis, AxisDataSet).
            var pattern = new Regex($@"\b{Regex.Escape(anchorType)}\b", RegexOptions.CultureInvariant);
            Assert.That(pattern.IsMatch(source), Is.True,
                $"Topic fixture '{topicFixtureRelativePath}' does not reference the anchor "
                + $"type '{anchorType}'. The DRY-generator Phase 1 migration pinned this "
                + $"type at this topic-fixture home; a coverage-parity regression happens when "
                + "the anchor is silently removed. Restore the assertion (or move the anchor "
                + "row in TopicFixtureCoverageTests.TopicAnchors to a different topic fixture "
                + "AND leave a rationale) before landing the change.");
        }

        /// <summary>Pins the smoke-invariant: the TopicAnchors map does not silently shrink below the migrated baseline.</summary>
        [Test]
        public void TopicAnchors_covers_at_least_the_full_migrated_baseline()
        {
            // The Phase 1 migration surfaced 23 distinct anchor types across
            // six topic fixtures (4 Components + 3 DataItems + 11 Configuration
            // + 1 WaterHardness + 2 Enum + 2 Version). The map above
            // enumerates them explicitly. A future edit that changes the
            // anchor list must land alongside a rationale in the topic
            // fixture AND update this pinned count with the same rationale.
            // Exact-equality matches the AssertionParityTests pattern (which
            // pins the migration map at exactly 34) so a silent drop of one
            // row cannot slip past a "≥ baseline" smoke floor.
            Assert.That(TopicAnchors.Length, Is.EqualTo(23),
                $"TopicAnchors is at {TopicAnchors.Length} entries — the Phase 1 "
                + "migration baseline is exactly 23 entries. Restore the anchor rows "
                + "or, if the change is intentional, update this pinned count with a "
                + "rationale that cross-references the topic fixture change.");
        }

        /// <summary>Pins the smoke-invariant: every distinct topic fixture named in TopicAnchors is present on disk.</summary>
        [Test]
        public void Every_topic_fixture_named_in_TopicAnchors_exists_on_disk()
        {
            var testsRoot = LocateTestProjectRoot();
            var missing = TopicAnchors
                .Select(row => row.TopicFixtureRelativePath)
                .Distinct(StringComparer.Ordinal)
                .Where(rel => !File.Exists(Path.Combine(testsRoot,
                    rel.Replace('/', Path.DirectorySeparatorChar))))
                .OrderBy(x => x, StringComparer.Ordinal)
                .ToList();

            Assert.That(missing, Is.Empty,
                "Topic fixture files named in TopicFixtureCoverageTests.TopicAnchors do not "
                + "exist on disk. Restore the file or repoint the anchor rows to the "
                + "correct topic-fixture home. Missing files:\n  "
                + string.Join("\n  ", missing));
        }

        // Locate the test project's source root by walking up from the
        // test binary's directory. The test project's .csproj lives at
        // the root. This walker mirrors the pattern used in
        // PerVersionFolderProhibitionTests so both guards resolve the
        // same root under bin/Debug/net8.0/, bin/Release/net8.0/, and
        // any runsettings-overridden test directory.
        private static string LocateTestProjectRoot()
        {
            var dir = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
            while (dir != null)
            {
                if (dir.EnumerateFiles("MTConnect.NET-Common-Tests.csproj").Any())
                    return dir.FullName;
                dir = dir.Parent;
            }

            throw new InvalidOperationException(
                "Could not locate MTConnect.NET-Common-Tests.csproj by walking up from "
                + $"'{TestContext.CurrentContext.TestDirectory}'. TopicFixtureCoverageTests "
                + "needs the source-tree root to open topic-fixture files for scanning.");
        }
    }
}
