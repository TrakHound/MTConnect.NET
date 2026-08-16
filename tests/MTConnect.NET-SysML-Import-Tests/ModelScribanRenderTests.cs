// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.IO;
using System.Linq;
using MTConnect.Tests.SysMLImport.TestHelpers;
using NUnit.Framework;
using Scriban;

namespace MTConnect.Tests.SysMLImport
{
    /// <summary>
    /// Renders the <c>Model.scriban</c> template in-process against a
    /// minimal in-memory fixture and pins the compliance guarantees the
    /// template must uphold: no trailing whitespace on any emitted line,
    /// no trailing blank line, exactly one terminating newline, and a
    /// correctly-shaped namespace / class / property layout. This is the
    /// hermetic smoke test that would have caught the historical
    /// whitespace + double-newline regression that PR 216 addressed.
    /// </summary>
    /// <remarks>
    /// The test loads <c>Model.scriban</c> straight from the source tree
    /// (walked to via <see cref="RepoRootLocator.LocateRoot"/>) and
    /// renders it with an anonymous-object fixture whose PascalCase
    /// members Scriban's default snake-case renamer maps to the
    /// template's snake_case placeholders (<c>{{name}}</c>,
    /// <c>{{namespace}}</c>, etc.).
    /// </remarks>
    [TestFixture]
    public class ModelScribanRenderTests
    {
        private static readonly string s_repoRoot = RepoRootLocator.LocateRoot();

        private static string LoadTemplate() =>
            File.ReadAllText(Path.Combine(
                s_repoRoot,
                "build",
                "MTConnect.NET-SysML-Import",
                "CSharp",
                "Templates",
                "Model.scriban"));

        private static string RenderMinimalModel()
        {
            var template = Template.Parse(LoadTemplate());
            Assert.That(template.HasErrors, Is.False,
                "Model.scriban failed to parse: "
                + string.Join("\n", template.Messages));

            var model = new
            {
                UmlId = "_fixture-uml-id",
                Namespace = "MTConnect.Tests.Fixture",
                Description = "Fixture class for template smoke test.",
                IsAbstract = false,
                IsPartial = false,
                Name = "FixtureClass",
                ParentName = (string?)null,
                AdditionalParentNames = new string[0],
                Properties = new[]
                {
                    new
                    {
                        Name = "Alpha",
                        Description = "First property.",
                        DataType = "string",
                        IsArray = false,
                        IsInherited = false,
                        IsOptional = false,
                    },
                    new
                    {
                        Name = "Beta",
                        Description = "Second property.",
                        DataType = "int",
                        IsArray = false,
                        IsInherited = false,
                        IsOptional = true,
                    },
                }
            };

            // No custom renamer — Scriban's default StandardMemberRenamer
            // maps the anonymous object's PascalCase members onto the
            // template's snake_case placeholders, matching production
            // ClassModel.Render(this) call sites.
            return template.Render(model);
        }

        /// <summary>The rendered output contains the fixture's namespace,
        /// class name, and interface projection (<c>: IFixtureClass</c>) —
        /// pinning that the template still emits the class + interface
        /// linkage after PR 216's compliance sweep.</summary>
        [Test]
        public void Rendered_output_carries_namespace_and_class_and_interface()
        {
            var output = RenderMinimalModel();

            Assert.That(output, Does.Contain("namespace MTConnect.Tests.Fixture"));
            Assert.That(output, Does.Contain("public class FixtureClass : IFixtureClass"));
        }

        /// <summary>Every emitted property block appears exactly once in the
        /// rendered output — the properties loop does not duplicate or drop
        /// entries.</summary>
        [Test]
        public void Rendered_output_carries_each_property_exactly_once()
        {
            var output = RenderMinimalModel();

            Assert.That(CountOccurrences(output, "public string Alpha"), Is.EqualTo(1));
            // The Beta property is is_optional=true so its data_type gets a `?` suffix.
            Assert.That(CountOccurrences(output, "public int? Beta"), Is.EqualTo(1));
        }

        /// <summary>The rendered output ends with exactly one newline —
        /// neither missing the trailing newline nor doubling it. This is
        /// the invariant that PR 216 pinned in the Model.scriban trim.</summary>
        [Test]
        public void Rendered_output_ends_with_exactly_one_newline()
        {
            var output = RenderMinimalModel();

            Assert.That(output.Length, Is.GreaterThan(0));
            Assert.That(output[output.Length - 1], Is.EqualTo('\n'),
                "Rendered template must end with a newline character.");
            if (output.Length >= 2)
            {
                Assert.That(output[output.Length - 2], Is.Not.EqualTo('\n'),
                    "Rendered template must not end with a trailing blank line.");
            }
        }

        /// <summary>No emitted line carries trailing whitespace — matches
        /// the workspace's editor-strip convention and the compliance
        /// contract PR 216 pinned.</summary>
        [Test]
        public void Rendered_output_has_no_trailing_whitespace_on_any_line()
        {
            var output = RenderMinimalModel();
            var lines = output.Split('\n');
            var offenders = lines
                .Select((line, index) => (Line: line, Index: index + 1))
                .Where(pair => pair.Line.Length > 0
                    && (pair.Line.EndsWith(' ') || pair.Line.EndsWith('\t')))
                .Select(pair => $"line {pair.Index}: \"{pair.Line}\"")
                .ToList();

            Assert.That(offenders, Is.Empty,
                "Rendered template output carries trailing whitespace on:\n  "
                + string.Join("\n  ", offenders));
        }

        /// <summary>The template loads and parses without diagnostics —
        /// pins that the Scriban syntax stays valid across future edits.</summary>
        [Test]
        public void Model_scriban_template_parses_without_errors()
        {
            var template = Template.Parse(LoadTemplate());
            Assert.That(template.HasErrors, Is.False,
                "Model.scriban must parse without Scriban syntax errors. "
                + "Diagnostics:\n" + string.Join("\n",
                    template.Messages.Select(m => m.ToString())));
        }

        private static int CountOccurrences(string haystack, string needle)
        {
            var count = 0;
            var index = 0;
            while ((index = haystack.IndexOf(needle, index, System.StringComparison.Ordinal)) >= 0)
            {
                count++;
                index += needle.Length;
            }
            return count;
        }
    }
}
