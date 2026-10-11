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

        /// <summary>Renders a three-property fixture and pins the exact
        /// separator shape between consecutive property blocks: each
        /// property's closing <c>{ get; set; }</c> line is followed by
        /// exactly one blank line before the next property's XML doc
        /// summary. This is the core invariant the PR restructured
        /// Model.scriban's property loop to guarantee — the pre-fix
        /// template emitted either zero or two blank lines depending on
        /// which loop-iteration branch fired.</summary>
        [Test]
        public void Rendered_output_separates_consecutive_properties_with_exactly_one_blank_line()
        {
            var output = RenderThreePropertyModel(parentName: null);

            // "{ get; set; }\n\n        /// <summary>" must appear
            // exactly twice (three properties → two inter-property gaps).
            var betweenPropsPattern = "{ get; set; }\n\n        /// <summary>";
            Assert.That(CountOccurrences(output, betweenPropsPattern), Is.EqualTo(2),
                "Every consecutive-property boundary must be exactly one blank "
                + "line. Rendered output:\n" + output);

            // Reject any double blank between consecutive properties
            // (the pre-fix template's regression signature).
            var doubleBlankBetweenProps = "{ get; set; }\n\n\n        /// <summary>";
            Assert.That(output, Does.Not.Contain(doubleBlankBetweenProps),
                "Rendered output must not contain a double blank line "
                + "between consecutive properties.");

            // Reject any zero-blank between consecutive properties
            // (the other pre-fix failure mode).
            var noBlankBetweenProps = "{ get; set; }\n        /// <summary>";
            Assert.That(output, Does.Not.Contain(noBlankBetweenProps),
                "Rendered output must not run consecutive properties "
                + "together without a separating blank line.");
        }

        /// <summary>A property with <c>IsInherited=true</c> renders with
        /// the <c>new</c> modifier so C# hiding compiles cleanly (CS0108
        /// otherwise). Covers the class-side inheritance arm the
        /// TemplateRenderer's <c>classOnlyNames</c> switch feeds into.</summary>
        [Test]
        public void Rendered_output_emits_new_modifier_for_inherited_scalar_property()
        {
            var output = RenderThreePropertyModel(parentName: "BaseClass");

            // Beta is IsInherited=true, IsOptional=true, DataType=int
            Assert.That(output, Does.Contain("public new int? Beta { get; set; }"),
                "Inherited scalar property must render `public new int? Beta { get; set; }`. "
                + "Rendered output:\n" + output);
        }

        /// <summary>A property with <c>IsInherited=false</c> renders WITHOUT
        /// the <c>new</c> modifier — the fresh-introduction case that
        /// ToolingMeasurement.Code exemplifies. Emitting <c>new</c> here
        /// would raise CS0109 at compile time.</summary>
        [Test]
        public void Rendered_output_omits_new_modifier_for_fresh_scalar_property()
        {
            var output = RenderThreePropertyModel(parentName: "BaseClass");

            // Alpha is IsInherited=false, IsOptional=false, DataType=string
            Assert.That(output, Does.Contain("public string Alpha { get; set; }"),
                "Fresh (non-inherited) scalar property must render without `new` — "
                + "emitting `new` on a fresh property raises CS0109. Rendered output:\n"
                + output);
            Assert.That(output, Does.Not.Contain("public new string Alpha"),
                "Fresh (non-inherited) scalar property must not carry the `new` modifier.");
        }

        /// <summary>A property with <c>IsArray=true</c> renders as an
        /// <c>IEnumerable&lt;T&gt;</c> — pins the array branch of the
        /// property loop, exercised nowhere else in this fixture.</summary>
        [Test]
        public void Rendered_output_emits_IEnumerable_for_array_property()
        {
            var output = RenderThreePropertyModel(parentName: null);

            // Gamma is IsArray=true, DataType=double, IsInherited=false
            Assert.That(output, Does.Contain(
                "public System.Collections.Generic.IEnumerable<double> Gamma { get; set; }"),
                "Array property must render as IEnumerable<T>. Rendered output:\n"
                + output);
            // The scalar branch must NOT also fire for an array property.
            Assert.That(output, Does.Not.Contain("public double Gamma"),
                "Array property must not also render via the scalar branch.");
        }

        /// <summary>When <c>parent_name</c> is present the
        /// <c>DescriptionText</c> constant carries the <c>new</c>
        /// modifier — pins the parent-set arm of the DescriptionText
        /// emission.</summary>
        [Test]
        public void Rendered_output_emits_new_DescriptionText_when_parent_name_present()
        {
            var output = RenderThreePropertyModel(parentName: "BaseClass");

            Assert.That(output, Does.Contain(
                "public new const string DescriptionText = "),
                "With parent_name set, DescriptionText must carry `new`. "
                + "Rendered output:\n" + output);
            Assert.That(output, Does.Contain("class FixtureClass : BaseClass, IFixtureClass"),
                "With parent_name set, the class header must list the parent "
                + "before the marker interface.");
        }

        /// <summary>When <c>parent_name</c> is null the <c>DescriptionText</c>
        /// constant is emitted without the <c>new</c> modifier — pins the
        /// parent-null arm of the DescriptionText emission (the arm the
        /// existing fixture happens to exercise, now made explicit).</summary>
        [Test]
        public void Rendered_output_omits_new_DescriptionText_when_parent_name_absent()
        {
            var output = RenderMinimalModel();

            Assert.That(output, Does.Contain(
                "public const string DescriptionText = "),
                "With parent_name null, DescriptionText must NOT carry `new`.");
            Assert.That(output, Does.Not.Contain(
                "public new const string DescriptionText"),
                "With parent_name null, the `new` modifier on DescriptionText "
                + "would raise CS0109 (nothing to hide).");
        }

        /// <summary>ToolingMeasurement's <c>Code</c> semantic — a fresh
        /// scalar property named <c>Code</c> renders without <c>new</c>
        /// even though the concrete class has a parent (Measurement). This
        /// is the exact rendering shape the TemplateRenderer change
        /// (dropping <c>classOnlyNames.Add("Code")</c>) makes possible;
        /// with the seed still present the property would be flagged
        /// inherited and render `public new string Code`, producing
        /// CS0109 against the v2.7 Measurement base.</summary>
        [Test]
        public void Rendered_output_ToolingMeasurement_like_Code_property_has_no_new_modifier()
        {
            var template = Template.Parse(LoadTemplate());
            Assert.That(template.HasErrors, Is.False);

            var model = new
            {
                UmlId = "_tooling",
                Namespace = "MTConnect.Assets.CuttingTools",
                Description = "Tooling measurement fixture.",
                IsAbstract = false,
                IsPartial = false,
                Name = "ToolingMeasurement",
                ParentName = (string?)"Measurement",
                AdditionalParentNames = new string[0],
                Properties = new[]
                {
                    // Code is a fresh introduction per SysML v2.7 → is_inherited=false
                    new
                    {
                        Name = "Code",
                        Description = "The code identifying the measurement.",
                        DataType = "string",
                        IsArray = false,
                        IsInherited = false,
                        IsOptional = false,
                    },
                }
            };

            var output = template.Render(model);

            Assert.That(output, Does.Contain("public string Code { get; set; }"),
                "ToolingMeasurement.Code must render as a fresh property "
                + "(no `new` modifier). Rendered output:\n" + output);
            Assert.That(output, Does.Not.Contain("public new string Code"),
                "ToolingMeasurement.Code must NOT carry the `new` modifier — "
                + "the SysML v2.7 XMI relocates Code onto ToolingMeasurement, "
                + "so the parent Measurement declares no Code slot. Emitting "
                + "`new` here would raise CS0109.");
        }

        /// <summary>A model with zero properties still renders a valid
        /// class body — pins the empty-loop boundary the property loop
        /// must handle.</summary>
        [Test]
        public void Rendered_output_handles_zero_properties_without_error()
        {
            var template = Template.Parse(LoadTemplate());
            Assert.That(template.HasErrors, Is.False);

            var model = new
            {
                UmlId = "_empty",
                Namespace = "MTConnect.Tests.Fixture",
                Description = "Empty fixture.",
                IsAbstract = false,
                IsPartial = false,
                Name = "EmptyClass",
                ParentName = (string?)null,
                AdditionalParentNames = new string[0],
                Properties = new object[0],
            };

            var output = template.Render(model);

            Assert.That(output, Does.Contain("public class EmptyClass : IEmptyClass"));
            // No property blocks
            Assert.That(output, Does.Not.Contain("{ get; set; }"),
                "An empty property collection must not emit any property "
                + "blocks. Rendered output:\n" + output);
            // Still ends with exactly one newline
            Assert.That(output[output.Length - 1], Is.EqualTo('\n'));
            if (output.Length >= 2)
            {
                Assert.That(output[output.Length - 2], Is.Not.EqualTo('\n'));
            }
        }

        /// <summary>Renders a three-property fixture exercising every
        /// property-loop arm: fresh scalar, inherited optional scalar,
        /// fresh array. The <paramref name="parentName"/> parameter
        /// selects whether the class-header parent-projection arm and
        /// the <c>DescriptionText</c> <c>new</c>-modifier arm fire.</summary>
        private static string RenderThreePropertyModel(string? parentName)
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
                ParentName = parentName,
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
                        IsInherited = true,
                        IsOptional = true,
                    },
                    new
                    {
                        Name = "Gamma",
                        Description = "Third property.",
                        DataType = "double",
                        IsArray = true,
                        IsInherited = false,
                        IsOptional = false,
                    },
                }
            };

            return template.Render(model);
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
