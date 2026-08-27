// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.IO;
using System.Linq;
using MTConnect.Tests.SysMLImport.TestHelpers;
using NUnit.Framework;

namespace MTConnect.Tests.SysMLImport
{
    /// <summary>
    /// Pins the semantic outcome of PR 216's ToolingMeasurement /
    /// Measurement <c>Code</c> relocation. The SysML v2.7 XMI moved the
    /// <c>Code</c> property off the abstract <c>Measurement</c> base and
    /// onto <c>ToolingMeasurement</c> — the concrete child that
    /// CuttingTool assets actually carry. The generator was updated to
    /// remove the historical <c>classOnlyNames.Add("Code")</c> seed
    /// (which was a workaround for the pre-v2.7 shape) so the current
    /// output is:
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><c>Measurement.g.cs</c> does NOT declare <c>Code</c>.</item>
    /// <item><c>ToolingMeasurement.g.cs</c> declares <c>Code</c> WITHOUT
    /// the <c>new</c> modifier (fresh introduction — parent no longer
    /// carries a hidden slot; <c>new</c> here would raise CS0109).</item>
    /// <item><c>TemplateRenderer.cs</c> no longer seeds
    /// <c>classOnlyNames.Add("Code")</c> in the ToolingMeasurement
    /// switch arm.</item>
    /// </list>
    /// If the SysML model regresses or a template flow re-adds the
    /// legacy seed, these assertions fail loudly rather than letting
    /// the semantic drift slip through as a diff-only chore.
    /// </remarks>
    [TestFixture]
    public class MeasurementCodeSemanticsTests
    {
        private static readonly string s_repoRoot = RepoRootLocator.LocateRoot();

        private static string ReadFile(params string[] relativePath)
        {
            var full = Path.Combine(new[] { s_repoRoot }.Concat(relativePath).ToArray());
            Assert.That(File.Exists(full), Is.True,
                $"Expected file not found: {full}");
            return File.ReadAllText(full);
        }

        /// <summary>The abstract-base <c>Measurement.g.cs</c> under
        /// CuttingTools no longer declares a <c>Code</c> property.</summary>
        [Test]
        public void CuttingTools_Measurement_g_cs_has_no_Code_property()
        {
            var text = ReadFile(
                "libraries", "MTConnect.NET-Common", "Assets", "CuttingTools", "Measurement.g.cs");

            Assert.That(text, Does.Not.Match(@"\bpublic\s+(new\s+)?string\s+Code\s*\{"),
                "Measurement.g.cs must not declare `Code`. The SysML v2.7 "
                + "XMI relocates Code onto ToolingMeasurement; a Code slot "
                + "on the base would either duplicate it or force a `new` "
                + "modifier on the child.");
        }

        /// <summary>The abstract-base <c>IMeasurement.g.cs</c> under
        /// CuttingTools also carries no <c>Code</c> declaration — the
        /// interface tree mirrors the class tree.</summary>
        [Test]
        public void CuttingTools_IMeasurement_g_cs_has_no_Code_property()
        {
            var text = ReadFile(
                "libraries", "MTConnect.NET-Common", "Assets", "CuttingTools", "IMeasurement.g.cs");

            Assert.That(text, Does.Not.Match(@"\bstring\s+Code\s*\{"),
                "IMeasurement.g.cs must not declare `Code`. The interface "
                + "tree mirrors the class tree; adding Code here would "
                + "re-introduce the pre-v2.7 shape.");
        }

        /// <summary>The concrete <c>ToolingMeasurement.g.cs</c> DOES declare
        /// <c>Code</c> — and it must NOT carry the <c>new</c> modifier
        /// because the parent Measurement no longer hides anything.</summary>
        [Test]
        public void ToolingMeasurement_g_cs_declares_Code_without_new_modifier()
        {
            var text = ReadFile(
                "libraries", "MTConnect.NET-Common", "Assets", "CuttingTools", "ToolingMeasurement.g.cs");

            Assert.That(text, Does.Match(@"\bpublic\s+string\s+Code\s*\{"),
                "ToolingMeasurement.g.cs must declare `Code` (SysML v2.7 "
                + "relocates it here from the abstract base).");
            Assert.That(text, Does.Not.Match(@"\bpublic\s+new\s+string\s+Code\s*\{"),
                "ToolingMeasurement.Code must NOT carry the `new` modifier "
                + "— the parent Measurement no longer declares Code, so "
                + "`new` would raise CS0109 as a fresh introduction.");
        }

        /// <summary>The concrete <c>IToolingMeasurement.g.cs</c> DOES declare
        /// <c>Code</c> — the interface tree mirrors the class tree; the
        /// interface partial has no <c>new</c> distinction, so we only
        /// assert presence.</summary>
        [Test]
        public void IToolingMeasurement_g_cs_declares_Code()
        {
            var text = ReadFile(
                "libraries", "MTConnect.NET-Common", "Assets", "CuttingTools", "IToolingMeasurement.g.cs");

            Assert.That(text, Does.Match(@"\bstring\s+Code\s*\{"),
                "IToolingMeasurement.g.cs must declare `Code` (SysML v2.7 "
                + "relocates it here from the abstract base).");
        }

        /// <summary>The template renderer's <c>ToolingMeasurement</c> switch
        /// arm no longer contains the legacy <c>classOnlyNames.Add("Code")</c>
        /// seed. If a later refactor re-adds it, this test fires so the
        /// intent (documented in the switch's comment block) is preserved.</summary>
        [Test]
        public void TemplateRenderer_ToolingMeasurement_arm_no_longer_seeds_Code_classOnlyName()
        {
            var text = ReadFile(
                "build", "MTConnect.NET-SysML-Import", "CSharp", "TemplateRenderer.cs");

            Assert.That(text, Does.Not.Match(@"classOnlyNames\.Add\s*\(\s*""Code""\s*\)"),
                "TemplateRenderer.cs must not seed `classOnlyNames.Add(\"Code\")` "
                + "for ToolingMeasurement. The SysML v2.7 XMI relocates Code "
                + "onto ToolingMeasurement so no hidden-slot marker is needed; "
                + "the seed would inject a spurious `new` modifier that "
                + "produces CS0109 at compile time.");
        }
    }
}
