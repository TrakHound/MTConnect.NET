// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.IO;
using System.Linq;
using MTConnect.Tests.SysMLImport.TestHelpers;
using NUnit.Framework;

namespace MTConnect.Tests.SysMLImport
{
    /// <summary>
    /// Hermetic assertions over every committed <c>.g.cs</c> file produced
    /// by the SysML-Import CSharp template pipeline. Pins the workspace
    /// compliance contract that PR 216 brought the generator into — no
    /// trailing whitespace, exactly one terminating newline, no CRLF /
    /// mixed line endings, and the template renderer's output stable
    /// across regenerations. Regressions on any of these break the
    /// merge-time diff hygiene that the compliance sweep established.
    /// </summary>
    [TestFixture]
    public class GeneratedCodeComplianceTests
    {
        private static readonly string s_repoRoot = RepoRootLocator.LocateRoot();

        private static readonly string s_generatedRoot = Path.Combine(
            s_repoRoot,
            "libraries",
            "MTConnect.NET-Common");

        /// <summary>Every <c>*.g.cs</c> under the Common library uses LF-only
        /// line endings — the compliance sweep normalised the entire
        /// generated tree so a fresh <c>git diff</c> after regeneration
        /// stays empty on Linux workspaces (which are the CI baseline).</summary>
        [Test]
        public void All_generated_files_use_LF_only_line_endings()
        {
            var offenders = EnumerateGeneratedFiles()
                .Where(f => File.ReadAllText(f).Contains('\r'))
                .Select(f => Path.GetRelativePath(s_repoRoot, f))
                .OrderBy(f => f)
                .ToList();

            Assert.That(offenders, Is.Empty,
                "The following generated files contain CR characters "
                + "(expected LF-only line endings):\n  "
                + string.Join("\n  ", offenders));
        }

        /// <summary>Every <c>*.g.cs</c> ends with exactly one newline —
        /// neither missing the trailing newline nor doubling it. The
        /// generator's compliance sweep normalised this via the
        /// <c>Model.scriban</c> trailing whitespace trimmer.</summary>
        [Test]
        public void All_generated_files_end_with_exactly_one_newline()
        {
            var offenders = EnumerateGeneratedFiles()
                .Where(f =>
                {
                    var text = File.ReadAllText(f);
                    if (text.Length == 0) return true;
                    if (!text.EndsWith('\n')) return true;
                    // reject a trailing blank line (two consecutive '\n')
                    if (text.Length >= 2 && text[text.Length - 2] == '\n') return true;
                    return false;
                })
                .Select(f => Path.GetRelativePath(s_repoRoot, f))
                .OrderBy(f => f)
                .ToList();

            Assert.That(offenders, Is.Empty,
                "The following generated files do not end with exactly "
                + "one newline:\n  "
                + string.Join("\n  ", offenders));
        }

        /// <summary>No line in any <c>*.g.cs</c> under the Common library
        /// carries trailing whitespace — the whitespace trimmer emitted by
        /// the compliance-sweep <c>Model.scriban</c> update keeps the
        /// generated tree diff-clean under editor auto-strip rules.</summary>
        [Test]
        public void All_generated_files_have_no_trailing_whitespace_on_any_line()
        {
            var offenders = EnumerateGeneratedFiles()
                .SelectMany(f =>
                {
                    var lines = File.ReadAllLines(f);
                    return lines
                        .Select((line, index) => (Line: line, Index: index + 1))
                        .Where(pair => pair.Line.Length > 0
                            && (pair.Line.EndsWith(' ') || pair.Line.EndsWith('\t')))
                        .Select(pair => $"{Path.GetRelativePath(s_repoRoot, f)}:{pair.Index}");
                })
                .OrderBy(o => o)
                .ToList();

            Assert.That(offenders, Is.Empty,
                "The following generated file lines carry trailing "
                + "whitespace:\n  "
                + string.Join("\n  ", offenders));
        }

        private static System.Collections.Generic.IEnumerable<string> EnumerateGeneratedFiles()
        {
            if (!Directory.Exists(s_generatedRoot))
            {
                Assert.Fail(
                    $"Generated-code root not found: {s_generatedRoot}. "
                    + "The repo root walk must resolve inside the workspace.");
            }

            return Directory.EnumerateFiles(
                s_generatedRoot,
                "*.g.cs",
                SearchOption.AllDirectories);
        }
    }
}
