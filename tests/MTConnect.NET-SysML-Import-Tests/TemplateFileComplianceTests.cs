// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using MTConnect.Tests.SysMLImport.TestHelpers;
using NUnit.Framework;

namespace MTConnect.Tests.SysMLImport
{
    /// <summary>
    /// Pins the on-disk compliance shape of every <c>.scriban</c> template
    /// under <c>build/MTConnect.NET-SysML-Import/</c>. The compliance sweep
    /// in this PR added a trailing newline to every C# + XML template
    /// (<c>\ No newline at end of file</c> → <c>\n</c>) and stripped
    /// stray CR characters. Rather than pin each template file
    /// individually, this fixture walks the whole tree — any future
    /// template addition is automatically enrolled in the guarantee.
    /// </summary>
    /// <remarks>
    /// The walk is restricted to source-tree templates
    /// (<c>build/MTConnect.NET-SysML-Import/{CSharp,Xml,Json-cppagent}/Templates/</c>);
    /// build output copies under <c>bin/</c> and <c>obj/</c> are excluded
    /// so a stale build artefact cannot mask a source-tree regression.
    /// </remarks>
    [TestFixture]
    public class TemplateFileComplianceTests
    {
        private static readonly string s_repoRoot = RepoRootLocator.LocateRoot();

        private static readonly string s_templatesRoot = Path.Combine(
            s_repoRoot, "build", "MTConnect.NET-SysML-Import");

        private static IReadOnlyList<string> LoadTemplateFiles()
        {
            // Enumerate every .scriban under the three source-tree template
            // subdirectories, excluding bin/ and obj/ copies.
            var subdirs = new[] { "CSharp", "Xml", "Json-cppagent" };
            var files = new List<string>();
            foreach (var sub in subdirs)
            {
                var templatesDir = Path.Combine(s_templatesRoot, sub, "Templates");
                if (!Directory.Exists(templatesDir)) continue;
                files.AddRange(Directory.EnumerateFiles(
                    templatesDir, "*.scriban", SearchOption.TopDirectoryOnly));
            }
            return files;
        }

        /// <summary>Sanity canary — the walker must find at least one
        /// template. If empty, the tree layout has drifted and every other
        /// assertion in this fixture would trivially pass on an empty set.</summary>
        [Test]
        public void Walker_finds_at_least_one_source_tree_scriban_template()
        {
            var files = LoadTemplateFiles();
            Assert.That(files, Is.Not.Empty,
                "Expected at least one .scriban template under "
                + $"'{s_templatesRoot}'. If empty, the template subdirectory "
                + "layout has changed and this fixture needs to be updated.");
        }

        /// <summary>Every source-tree <c>.scriban</c> ends with exactly
        /// one <c>\n</c> — matches the sweep the PR performed across all
        /// 17 templates (C# + XML + Json-cppagent).</summary>
        [Test]
        public void Every_source_template_ends_with_exactly_one_newline()
        {
            var offenders = LoadTemplateFiles()
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
                "The following .scriban templates do not end with exactly "
                + "one newline (either missing a trailing newline or ending "
                + "with a blank line):\n  "
                + string.Join("\n  ", offenders));
        }

        /// <summary>No source-tree <c>.scriban</c> contains a CR (<c>\r</c>)
        /// character — the sweep normalised every template to LF-only line
        /// endings.</summary>
        [Test]
        public void Every_source_template_uses_LF_only_line_endings()
        {
            var offenders = LoadTemplateFiles()
                .Where(f => File.ReadAllText(f).Contains('\r'))
                .Select(f => Path.GetRelativePath(s_repoRoot, f))
                .OrderBy(f => f)
                .ToList();

            Assert.That(offenders, Is.Empty,
                "The following .scriban templates contain CR characters "
                + "(expected LF-only line endings):\n  "
                + string.Join("\n  ", offenders));
        }
    }
}
