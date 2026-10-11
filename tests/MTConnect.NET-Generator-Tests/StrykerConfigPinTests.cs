// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Text.Json;
using NUnit.Framework;

namespace MTConnect.NET_Generator_Tests
{
    /// <summary>
    /// Pins the shape of <c>stryker-config.json</c> so a later PR cannot
    /// silently regress or over-tighten the mutation-score gate without also
    /// editing this test alongside the config change.
    ///
    /// <para>
    /// The pilot Stryker.NET run on PR #233 established a 7.75% baseline
    /// mutation score for MTConnect.NET-Common on 2026-08-20 (Stryker.NET
    /// v4.16.0, Regex mutator ignored per the preceding <c>chore(tests):
    /// exclude Stryker Regex mutator</c> commit). Thresholds were pinned to
    /// <c>{ high: 8, low: 5, break: 5 }</c> — below the baseline so this PR
    /// does not regress, and low enough that subsequent PRs inherit a pass-
    /// through gate until the dedicated coverage-quality campaign
    /// (TrakHound/MTConnect.NET#242) raises the floor in phased steps
    /// (20 -&gt; 40 -&gt; 60 -&gt; 80%+).
    /// </para>
    ///
    /// <para>
    /// These tests guard three orthogonal invariants:
    /// <list type="number">
    ///   <item>The file must parse as JSONC (JSON with <c>//</c> line
    ///     comments) — Stryker.NET's native config format. A silent switch
    ///     to strict JSON would strip the baseline-rationale comment
    ///     block.</item>
    ///   <item>The threshold triple must sit inside the ratchet window
    ///     <c>break in [5, 8]</c> and <c>break &lt;= low &lt;= high &lt;=
    ///     100</c>. A drop below the pinned floor is a coverage regression;
    ///     a jump to 100 without a matching baseline-lift commit is a
    ///     phantom ratchet the pin catches.</item>
    ///   <item>The <c>ignore-mutations</c> list must still contain
    ///     <c>Regex</c> — pinning the upstream-bug workaround per the
    ///     preceding chore commit.</item>
    /// </list>
    /// </para>
    /// </summary>
    [TestFixture]
    public class StrykerConfigPinTests
    {
        private const string SlnFileName = "MTConnect.NET.sln";
        private const string StrykerConfigFileName = "stryker-config.json";

        // Ratchet window — cycle-2 pins { high: 8, low: 5, break: 5 } at the
        // 7.75% baseline. Any change that moves `break` outside [5, 8] must
        // also edit these constants and the surrounding rationale in a
        // matching baseline-lift commit per TrakHound/MTConnect.NET#242.
        private const int BreakThresholdMin = 5;
        private const int BreakThresholdMax = 8;

        [Test]
        public void StrykerConfig_parses_as_JSONC_with_line_comments()
        {
            var raw = File.ReadAllText(StrykerConfigPath());

            // The file MUST start with the '//' baseline-rationale comment
            // block; strict-JSON parsing would reject it.
            Assert.That(raw.TrimStart(), Does.StartWith("//"),
                "stryker-config.json must open with a '//' JSONC comment " +
                "(the baseline rationale). If the comment block was " +
                "removed, restore it — Stryker.NET reads JSONC natively " +
                "and the rationale belongs in the config file.");

            var options = new JsonDocumentOptions
            {
                CommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
            };

            Assert.DoesNotThrow(() =>
            {
                using var _ = JsonDocument.Parse(raw, options);
            }, "stryker-config.json must parse as JSONC (System.Text.Json " +
               "with CommentHandling=Skip). If a syntax error slipped in, " +
               "Stryker.NET will fail at run time with the same class of " +
               "error — this test surfaces it at build time instead.");
        }

        [Test]
        public void Threshold_break_sits_inside_ratchet_window()
        {
            var thresholds = ReadThresholds();

            Assert.That(thresholds.Break, Is.GreaterThanOrEqualTo(BreakThresholdMin),
                "Stryker `break` threshold dropped below the pinned floor. " +
                "The 7.75% baseline (2026-08-20, PR #233) means anything " +
                "below 5 is a silent coverage regression. Either restore " +
                "the floor or land a matching baseline-recompute commit " +
                "and bump `BreakThresholdMin` in this test.");

            Assert.That(thresholds.Break, Is.LessThanOrEqualTo(BreakThresholdMax),
                "Stryker `break` threshold jumped above the ratchet " +
                "ceiling without a matching kill-test campaign commit. " +
                "The phased raise plan (see TrakHound/MTConnect.NET#242) " +
                "is 20 -> 40 -> 60 -> 80%+ — each step must land its own " +
                "kill tests before this pin's ceiling is lifted.");
        }

        [Test]
        public void Threshold_triple_is_monotonically_ordered()
        {
            var thresholds = ReadThresholds();

            Assert.Multiple(() =>
            {
                Assert.That(thresholds.Break, Is.LessThanOrEqualTo(thresholds.Low),
                    "Stryker convention: break <= low. A `break` above `low` " +
                    "is a Stryker-config error — the run would fail before " +
                    "any mutation was scored.");

                Assert.That(thresholds.Low, Is.LessThanOrEqualTo(thresholds.High),
                    "Stryker convention: low <= high. A `low` above `high` " +
                    "is a Stryker-config error — the run would fail before " +
                    "any mutation was scored.");

                Assert.That(thresholds.High, Is.LessThanOrEqualTo(100),
                    "Stryker `high` threshold is a percentage; a value " +
                    "above 100 is out of range.");

                Assert.That(thresholds.Break, Is.GreaterThanOrEqualTo(0),
                    "Stryker `break` threshold is a percentage; a value " +
                    "below 0 is out of range.");
            });
        }

        [Test]
        public void Regex_mutator_stays_on_the_ignore_list()
        {
            using var doc = ParseStrykerConfig();
            var root = doc.RootElement.GetProperty("stryker-config");

            Assert.That(root.TryGetProperty("ignore-mutations", out var ignored), Is.True,
                "`ignore-mutations` node missing — the Regex-mutator " +
                "workaround (preceding chore commit) was removed. " +
                "Restore it until upstream Stryker.NET fixes the Regex " +
                "mutator crash, or land an issue reference explaining why " +
                "the workaround is no longer needed.");

            Assert.That(ignored.ValueKind, Is.EqualTo(JsonValueKind.Array),
                "`ignore-mutations` must be a JSON array.");

            var foundRegex = false;
            foreach (var entry in ignored.EnumerateArray())
            {
                if (entry.ValueKind == JsonValueKind.String &&
                    string.Equals(entry.GetString(), "Regex", StringComparison.Ordinal))
                {
                    foundRegex = true;
                    break;
                }
            }

            Assert.That(foundRegex, Is.True,
                "`Regex` must remain in `ignore-mutations` — the mutator " +
                "trips an upstream Stryker.NET crash on the pilot " +
                "assembly. Removing it without an upstream fix will " +
                "break the mutation run.");
        }

        [Test]
        public void Pilot_project_stays_pinned_to_MTConnect_NET_Common()
        {
            using var doc = ParseStrykerConfig();
            var root = doc.RootElement.GetProperty("stryker-config");

            Assert.That(root.GetProperty("project").GetString(),
                Is.EqualTo("MTConnect.NET-Common.csproj"),
                "The 7.75% baseline was measured on MTConnect.NET-Common. " +
                "Swapping the pilot project silently invalidates the " +
                "baseline; land a matching baseline-recompute commit and " +
                "update this pin at the same time.");

            var testProjects = root.GetProperty("test-projects");
            Assert.That(testProjects.ValueKind, Is.EqualTo(JsonValueKind.Array));
            Assert.That(testProjects.GetArrayLength(), Is.EqualTo(1),
                "Only one test project should feed the pilot run — the " +
                "baseline was measured against MTConnect.NET-Common-Tests " +
                "alone.");
            Assert.That(testProjects[0].GetString(),
                Is.EqualTo("tests/MTConnect.NET-Common-Tests/MTConnect.NET-Common-Tests.csproj"));
        }

        private static (int High, int Low, int Break) ReadThresholds()
        {
            using var doc = ParseStrykerConfig();
            var thresholds = doc.RootElement
                .GetProperty("stryker-config")
                .GetProperty("thresholds");

            return (
                High: thresholds.GetProperty("high").GetInt32(),
                Low: thresholds.GetProperty("low").GetInt32(),
                Break: thresholds.GetProperty("break").GetInt32());
        }

        private static JsonDocument ParseStrykerConfig()
        {
            var raw = File.ReadAllText(StrykerConfigPath());
            var options = new JsonDocumentOptions
            {
                CommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
            };
            return JsonDocument.Parse(raw, options);
        }

        private static string StrykerConfigPath()
            => Path.Combine(FindRepoRoot(), StrykerConfigFileName);

        private static string FindRepoRoot()
        {
            var current = new DirectoryInfo(AppContext.BaseDirectory);
            while (current != null)
            {
                if (File.Exists(Path.Combine(current.FullName, SlnFileName)))
                    return current.FullName;
                current = current.Parent;
            }
            throw new DirectoryNotFoundException(
                $"Could not locate {SlnFileName} in any ancestor of {AppContext.BaseDirectory}.");
        }
    }
}
