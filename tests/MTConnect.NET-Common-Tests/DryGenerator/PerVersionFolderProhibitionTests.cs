// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace MTConnect.NET_Common_Tests.DryGenerator
{
    // Permanent regression guard against the deprecated per-version
    // fixture-folder convention. Fires RED if any new V<N_M>/ directory
    // or V<N_M>*Tests fixture class returns to
    // tests/MTConnect.NET-Common-Tests/.
    //
    // Enforcement rules (plan §"Single-test-file-per-topic convention"):
    //   * No V<N_M>/ directory under tests/MTConnect.NET-Common-Tests/.
    //   * No fixture class matching the V<N_M>*Tests pattern in the
    //     assembly, except historical anchors listed in HistoricalAnchors.
    //   * No fixture file name matching V<N_M>*Tests.cs on disk.
    //
    // Historical anchors (e.g. CppAgentParityWorkflowTests pinned to
    // Version25) are NOT migrated — they document a deliberate,
    // permanent pin. Add such classes to HistoricalAnchors with a
    // rationale comment before the entry.
    /// <summary>Pins the behavior expressed by the test name: per version folder prohibition tests.</summary>
    [TestFixture]
    public class PerVersionFolderProhibitionTests
    {
        // Fixture full-class-names allowed to keep a V<N_M>* naming
        // convention (historical anchors that document a permanent
        // version pin). Each entry must include a rationale comment.
        private static readonly HashSet<string> HistoricalAnchors = new(StringComparer.Ordinal)
        {
            // No historical anchors at HEAD — this list exists so that a
            // future contributor introducing a deliberately-pinned
            // fixture (e.g. CppAgentParityWorkflowTests pinned to a
            // specific version for spec-fidelity reasons) can document
            // the pin here rather than trip the guard.
        };

        /// <summary>Pins the invariant: no V-N-M subdirectory exists under the test project.</summary>
        [Test]
        public void No_per_version_directory_exists_under_tests_MTConnect_NET_Common_Tests()
        {
            var testsRoot = LocateTestProjectRoot();
            var offenders = Directory.EnumerateDirectories(
                    testsRoot,
                    "V*",
                    SearchOption.AllDirectories)
                .Where(path => !IsUnderIgnoredDirectory(path))
                .Where(IsPerVersionDirectoryName)
                .Select(path => Path.GetRelativePath(testsRoot, path))
                .OrderBy(x => x, StringComparer.Ordinal)
                .ToList();

            Assert.That(offenders, Is.Empty,
                "Per-version fixture directories (V<N_M>/) are deprecated by the "
                + "DRY-generator campaign. Migrate the fixtures into a topic-first "
                + "layout (Devices/DataItems/, Devices/Components/, etc.) with "
                + "matrix-parameterised version-gated assertions. Offending directories:\n  "
                + string.Join("\n  ", offenders));
        }

        /// <summary>Pins the invariant: no V-N-M fixture file lives on disk under the test project.</summary>
        [Test]
        public void No_per_version_fixture_file_exists_under_tests_MTConnect_NET_Common_Tests()
        {
            var testsRoot = LocateTestProjectRoot();
            var offenders = Directory.EnumerateFiles(
                    testsRoot,
                    "V*Tests.cs",
                    SearchOption.AllDirectories)
                .Where(path => !IsUnderIgnoredDirectory(path))
                .Where(path => IsPerVersionFileName(Path.GetFileName(path)))
                .Select(path => Path.GetRelativePath(testsRoot, path))
                .OrderBy(x => x, StringComparer.Ordinal)
                .ToList();

            Assert.That(offenders, Is.Empty,
                "Per-version fixture files (V<N_M>*Tests.cs) are deprecated by the "
                + "DRY-generator campaign. Rename to a topic-first name (e.g. "
                + "V2_7DataItemTypeTests.cs -> DataItemTypeTests.cs). Offending files:\n  "
                + string.Join("\n  ", offenders));
        }

        /// <summary>Pins the invariant: no V-N-M fixture class exists in the test assembly.</summary>
        [Test]
        public void No_per_version_fixture_class_exists_in_the_test_assembly()
        {
            var assembly = typeof(PerVersionFolderProhibitionTests).Assembly;
            var offenders = assembly.GetTypes()
                .Where(t => t.IsPublic || t.IsNestedPublic)
                .Where(t => t.GetCustomAttribute<TestFixtureAttribute>() != null)
                .Where(t => IsPerVersionClassName(t.Name))
                .Where(t => !HistoricalAnchors.Contains(t.FullName ?? t.Name))
                .Select(t => t.FullName ?? t.Name)
                .OrderBy(x => x, StringComparer.Ordinal)
                .ToList();

            Assert.That(offenders, Is.Empty,
                "Per-version fixture classes (V<N_M>*Tests) are deprecated by the "
                + "DRY-generator campaign. If a class is a deliberate historical anchor "
                + "(e.g. a permanent version pin for spec-fidelity reasons), add its "
                + "full name to PerVersionFolderProhibitionTests.HistoricalAnchors with "
                + "a rationale comment. Offending classes:\n  "
                + string.Join("\n  ", offenders));
        }

        // Locate the test project's source root by walking up from the test
        // binary's directory. The test project's .csproj lives at the root.
        // This walker is resilient to being invoked from bin/Debug/net8.0/,
        // bin/Release/net8.0/, or a runsettings-overridden directory.
        private static string LocateTestProjectRoot()
        {
            var dir = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
            while (dir != null)
            {
                if (File.Exists(Path.Combine(dir.FullName, "MTConnect.NET-Common-Tests.csproj")))
                {
                    return dir.FullName;
                }
                dir = dir.Parent;
            }
            throw new InvalidOperationException(
                "Could not locate MTConnect.NET-Common-Tests.csproj from test directory: "
                + TestContext.CurrentContext.TestDirectory);
        }

        // Match V<digits>_<digits>[_<digits>_<digits>...] directory names —
        // e.g. V2_6, V2_6_V2_7, V1_8. Any leading-V-then-underscored-digits
        // sequence counts.
        private static bool IsPerVersionDirectoryName(string absolutePath)
        {
            var name = Path.GetFileName(absolutePath);
            return LooksLikePerVersionToken(name);
        }

        // Match V<digits>_<digits>*Tests.cs — the file naming convention
        // the migration retires. Excludes anything without the V-prefix
        // digit-underscored pattern.
        private static bool IsPerVersionFileName(string fileName)
        {
            if (!fileName.EndsWith("Tests.cs", StringComparison.Ordinal))
            {
                return false;
            }
            // Strip ".cs" and the "Tests" suffix; the head must still
            // start with a per-version token.
            var head = fileName.Substring(0, fileName.Length - "Tests.cs".Length);
            return LooksLikePerVersionToken(head);
        }

        // Match V<digits>_<digits>*Tests class names for the assembly
        // reflection sweep.
        private static bool IsPerVersionClassName(string className)
        {
            if (!className.EndsWith("Tests", StringComparison.Ordinal))
            {
                return false;
            }
            var head = className.Substring(0, className.Length - "Tests".Length);
            return LooksLikePerVersionToken(head);
        }

        // A per-version token starts with 'V', then one-or-more digits,
        // then an underscore, then one-or-more digits, then any suffix
        // (which may include additional V<n>_<n> segments).
        private static bool LooksLikePerVersionToken(string head)
        {
            if (string.IsNullOrEmpty(head) || head[0] != 'V')
            {
                return false;
            }
            int i = 1;
            // one or more digits after V
            if (i >= head.Length || !char.IsDigit(head[i])) return false;
            while (i < head.Length && char.IsDigit(head[i])) i++;
            // required underscore separator
            if (i >= head.Length || head[i] != '_') return false;
            i++;
            // one or more digits after the underscore
            if (i >= head.Length || !char.IsDigit(head[i])) return false;
            return true;
        }

        // The recursive directory walker crosses into bin/ and obj/ under
        // Debug builds; filter those out so the guard reflects the source
        // tree rather than build artefacts.
        private static bool IsUnderIgnoredDirectory(string absolutePath)
        {
            var normalised = absolutePath.Replace('\\', '/');
            return normalised.Contains("/bin/", StringComparison.Ordinal)
                || normalised.Contains("/obj/", StringComparison.Ordinal);
        }
    }
}
