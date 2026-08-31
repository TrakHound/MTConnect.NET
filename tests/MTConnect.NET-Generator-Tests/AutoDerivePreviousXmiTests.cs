// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace MTConnect.NET_Generator_Tests
{
    /// <summary>
    /// Zero-config PREV_VERSION auto-derive coverage for the SysML importer
    /// (task #408 amendment to PR #233 Phase 4).
    ///
    /// <para>
    /// When neither <c>--previous-xmi</c> nor <c>--full-tree</c> is supplied,
    /// the importer parses <c>MTConnectVersions.Max</c> from
    /// <c>libraries/MTConnect.NET-Common/MTConnectVersions.cs</c> under
    /// <c>--output</c> and resolves the prior-version XMI in this priority
    /// order:
    /// <list type="number">
    ///   <item>
    ///     Strategy B (primary):
    ///     <c>build/.cache/sysml-prev/MTConnectSysMLModel_v${PREV_VERSION}.xml</c>.
    ///   </item>
    ///   <item>
    ///     Strategy A (fallback):
    ///     <c>build/sysml-model/MTConnectSysMLModel.xml</c>, gated on
    ///     <c>git -C build/sysml-model describe --exact-match --tags HEAD</c>
    ///     returning <c>v${PREV_VERSION}</c> exactly.
    ///   </item>
    ///   <item>
    ///     Strategy C (fail-hard): throw with an actionable message when
    ///     neither resolves.
    ///   </item>
    /// </list>
    /// </para>
    ///
    /// <para>
    /// Every case here is exercised end-to-end via <c>dotnet run --no-build
    /// --project build/MTConnect.NET-SysML-Import</c> against a synthetic
    /// <c>--output</c> scratch tree that mimics the repo layout so the
    /// auto-derive resolver sees a controlled world: a pinned
    /// <c>MTConnectVersions.cs</c>, a curated cache directory, and (where
    /// needed) a synthetic git-tagged <c>build/sysml-model</c>. The
    /// assertions bind to the CLI contract, not to any internal helper.
    /// </para>
    /// </summary>
    [TestFixture]
    public class AutoDerivePreviousXmiTests
    {
        private const string SlnFileName = "MTConnect.NET.sln";
        private const string GeneratorProject = "build/MTConnect.NET-SysML-Import";
        private const string RealXmiRelativePath = "build/sysml-model/MTConnectSysMLModel.xml";
        private const string ScratchRoot = ".claude/gen-test-out/auto-derive";

        // A minimal MTConnectVersions.cs skeleton — enough for the auto-derive
        // regex to lock onto `public static Version Max => VersionXY;` and
        // `public static readonly Version VersionXY = new Version(X, Y);`. The
        // constants below cover the Max we pin the tests against; the
        // `Version27` constant matches the current-tree Max at #233 landing
        // so the tests stay in step with the shipped fixture XMI.
        private const string SyntheticVersionsCs = @"// Copyright (c) 2026 TrakHound Inc.

using System;

namespace MTConnect
{
    public static class MTConnectVersions
    {
        public static Version Max => Version27;

        public static readonly Version Version26 = new Version(2, 6);
        public static readonly Version Version27 = new Version(2, 7);
    }
}
";

        [Test]
        public void Auto_derive_from_MTConnectVersionsMax_uses_cache_when_present()
        {
            var repoRoot = FindRepoRoot();
            var realXmi = Path.Combine(repoRoot, RealXmiRelativePath);
            Assert.That(File.Exists(realXmi), Is.True,
                $"XMI snapshot missing at {realXmi}. Is the build/sysml-model submodule initialized?");

            var scratch = InitScratchRepoLayout("cache-primary");
            WriteSyntheticVersionsCs(scratch);

            // Strategy B setup: populate the cache path with the current-tree
            // XMI as a stand-in for the prior-version XMI. Using the same bytes
            // for --new-xmi and the cache produces a "same XMI on both sides"
            // delta — every emitted file lands in the UNCHANGED-concentrated
            // partition, so the stdout stats line is grep-able for
            // `unchanged-concentrated=N>0` and the Compat file appears at the
            // expected auto-derived label path.
            var cacheDir = Path.Combine(scratch, "build", ".cache", "sysml-prev");
            Directory.CreateDirectory(cacheDir);
            var cachePath = Path.Combine(cacheDir, "MTConnectSysMLModel_v2.7.xml");
            File.Copy(realXmi, cachePath);

            var (exitCode, stdout, stderr) = RunAutoDerive(realXmi, scratch);

            Assert.That(exitCode, Is.Zero,
                $"Auto-derive with cache present should succeed.\nstdout:\n{stdout}\nstderr:\n{stderr}");
            Assert.That(stdout, Does.Contain("auto-derived from MTConnectVersions.Max"),
                "stdout must announce that PREV_VERSION was auto-derived so the operator sees which strategy fired.");
            Assert.That(stdout, Does.Contain("MTConnectSysMLModel_v2.7.xml"),
                "stdout must echo the resolved cache path so the operator can verify Strategy B ran.");
            Assert.That(stdout, Does.Contain("Delta emission:"),
                "Auto-derive must reach the delta emitter, not the full-tree branch.");

            // The auto-derived Compat label is `v2_7` (from Max = Version27),
            // and same-XMI-on-both-sides forces every file into the UNCHANGED
            // partition — so exactly one Compat/v2_7.g.cs lands per library.
            var compatFiles = Directory
                .EnumerateFiles(scratch, "v2_7.g.cs", SearchOption.AllDirectories)
                .Select(p => p.Replace('\\', '/'))
                .Where(p => p.Contains("/Compat/"))
                .ToList();
            Assert.That(compatFiles.Count, Is.EqualTo(3),
                "One auto-labeled Compat file per library (three libraries): "
                + string.Join(", ", compatFiles));
        }

        [Test]
        public void Auto_derive_from_MTConnectVersionsMax_falls_back_to_submodule_tag_when_cache_absent()
        {
            var repoRoot = FindRepoRoot();
            var realXmi = Path.Combine(repoRoot, RealXmiRelativePath);
            Assert.That(File.Exists(realXmi), Is.True,
                $"XMI snapshot missing at {realXmi}. Is the build/sysml-model submodule initialized?");

            var scratch = InitScratchRepoLayout("submodule-fallback");
            WriteSyntheticVersionsCs(scratch);
            // No cache path populated — Strategy B misses. Strategy A must fire.

            // Strategy A setup: build a synthetic git repo at
            // <scratch>/build/sysml-model, drop the XMI in, and tag HEAD as
            // v2.7 (matching MTConnectVersions.Max in the synthetic
            // MTConnectVersions.cs). The auto-derive resolver runs
            // `git -C build/sysml-model describe --exact-match --tags HEAD`
            // and accepts the tree only when the tag matches exactly.
            var submoduleDir = Path.Combine(scratch, "build", "sysml-model");
            Directory.CreateDirectory(submoduleDir);
            File.Copy(realXmi, Path.Combine(submoduleDir, "MTConnectSysMLModel.xml"));
            InitGitRepoWithTag(submoduleDir, "v2.7");

            var (exitCode, stdout, stderr) = RunAutoDerive(realXmi, scratch);

            Assert.That(exitCode, Is.Zero,
                $"Auto-derive with only the submodule-tag path available should succeed.\n"
                + $"stdout:\n{stdout}\nstderr:\n{stderr}");
            Assert.That(stdout, Does.Contain("auto-derived from MTConnectVersions.Max"),
                "stdout must announce the auto-derive.");
            Assert.That(stdout, Does.Contain(Path.Combine(submoduleDir, "MTConnectSysMLModel.xml")),
                "stdout must echo the resolved submodule XMI path so the operator can verify Strategy A ran.");
            Assert.That(stdout, Does.Contain("Delta emission:"),
                "Strategy A must reach the delta emitter, not the full-tree branch.");
        }

        [Test]
        public void Auto_derive_from_MTConnectVersionsMax_fails_hard_when_neither_cache_nor_tag_resolves()
        {
            var repoRoot = FindRepoRoot();
            var realXmi = Path.Combine(repoRoot, RealXmiRelativePath);

            var scratch = InitScratchRepoLayout("fail-hard");
            WriteSyntheticVersionsCs(scratch);
            // No cache populated. No submodule directory populated.

            var (exitCode, _, stderr) = RunAutoDerive(realXmi, scratch);

            Assert.That(exitCode, Is.Not.Zero,
                "Neither Strategy B nor Strategy A resolving must fail the invocation, not silently no-op.");
            Assert.That(stderr, Does.Contain("PREV_VERSION auto-derivation"),
                "stderr must name the auto-derive failure class so the operator knows which resolver aborted.");
            Assert.That(stderr, Does.Contain("MTConnectVersions.Max = 2.7"),
                "stderr must state the resolved PREV_VERSION so the operator can cross-check the current Max.");
            Assert.That(stderr, Does.Contain("MTConnectSysMLModel_v2.7.xml"),
                "stderr must name the probed cache path so the operator can drop the file in.");
            Assert.That(stderr, Does.Contain("v2.7"),
                "stderr must name the expected submodule tag so the operator can check the submodule tip.");
            Assert.That(stderr, Does.Contain("--previous-xmi"),
                "stderr must direct the operator to the explicit-override flag.");
            Assert.That(stderr, Does.Contain("--full-tree"),
                "stderr must direct the operator to the delta-disable escape hatch.");
        }

        [Test]
        public void Auto_derived_label_carries_the_auto_derived_suffix_on_stdout()
        {
            // Label-lie guard positive branch (F-IMP cycle 4): when the
            // Compat label is genuinely auto-derived (no explicit
            // --compat-version-label passed, zero-config prev-XMI resolved),
            // the stdout `Label:` line must carry the "(auto-derived)"
            // suffix so the operator sees at a glance which resolution
            // strategy the CLI took. The `compatLabelIsAutoDerived` bool
            // in Program.cs is TRUE on this branch.
            var repoRoot = FindRepoRoot();
            var realXmi = Path.Combine(repoRoot, RealXmiRelativePath);

            var scratch = InitScratchRepoLayout("label-auto-derived-suffix");
            WriteSyntheticVersionsCs(scratch);
            var cacheDir = Path.Combine(scratch, "build", ".cache", "sysml-prev");
            Directory.CreateDirectory(cacheDir);
            File.Copy(realXmi, Path.Combine(cacheDir, "MTConnectSysMLModel_v2.7.xml"));

            var (exitCode, stdout, stderr) = RunAutoDerive(realXmi, scratch);
            Assert.That(exitCode, Is.Zero, $"stdout:\n{stdout}\nstderr:\n{stderr}");
            Assert.That(stdout, Does.Contain("Label:  v2_7 (auto-derived)"),
                "Auto-derived label must be announced with the \"(auto-derived)\" suffix — "
                + "positive branch of the compatLabelIsAutoDerived ternary in Program.cs.");
        }

        [Test]
        public void Explicit_label_alongside_zero_config_prev_xmi_does_not_get_auto_derived_suffix()
        {
            // Label-lie guard negative branch (F-IMP cycle 4): the
            // operator can pass an explicit --compat-version-label
            // ALONGSIDE the zero-config prev-XMI path. The explicit label
            // wins the `??=` default; annotating it "(auto-derived)"
            // would be a lie. Pre-fix, the stdout unconditionally
            // suffixed "(auto-derived)" whenever the delta mode
            // announcement fired without --previous-xmi; the fix
            // introduced a `compatLabelIsAutoDerived` bool so only the
            // genuinely auto-derived branch appends the suffix.
            var repoRoot = FindRepoRoot();
            var realXmi = Path.Combine(repoRoot, RealXmiRelativePath);

            var scratch = InitScratchRepoLayout("label-explicit-no-suffix");
            WriteSyntheticVersionsCs(scratch);
            var cacheDir = Path.Combine(scratch, "build", ".cache", "sysml-prev");
            Directory.CreateDirectory(cacheDir);
            File.Copy(realXmi, Path.Combine(cacheDir, "MTConnectSysMLModel_v2.7.xml"));

            const string explicitLabel = "Custom-Release-Label";
            var (exitCode, stdout, stderr) = RunGenerator(scratch,
                "--new-xmi", realXmi,
                "--output", scratch,
                "--compat-version-label", explicitLabel);

            Assert.That(exitCode, Is.Zero, $"stdout:\n{stdout}\nstderr:\n{stderr}");
            Assert.That(stdout, Does.Contain($"Label:  {explicitLabel}"),
                "The explicit --compat-version-label must appear on the Label: line.");
            Assert.That(stdout, Does.Not.Contain($"Label:  {explicitLabel} (auto-derived)"),
                "The explicit label must NOT carry the \"(auto-derived)\" suffix — the "
                + "operator supplied it themselves, so the suffix would misattribute "
                + "authorship. This is the negative branch of the compatLabelIsAutoDerived "
                + "ternary and the direct pin for the cycle-4 label-lie fix.");
            Assert.That(stdout, Does.Contain("Mode:   delta (zero-config)"),
                "The zero-config delta path must still fire — the auto-derive resolver "
                + "runs (the cache is resolved), only the label default is bypassed.");
        }

        [Test]
        public void Explicit_previous_xmi_wins_over_auto_derive()
        {
            var repoRoot = FindRepoRoot();
            var realXmi = Path.Combine(repoRoot, RealXmiRelativePath);

            var scratch = InitScratchRepoLayout("explicit-wins");
            WriteSyntheticVersionsCs(scratch);

            // Populate the cache with a MUTATED copy of the XMI. If the
            // resolver picked the cache (Strategy B) over the explicit
            // --previous-xmi, the delta would surface CoordinateSystem-shaped
            // changes (from the mutation) instead of zero-change output. The
            // explicit --previous-xmi points at the pristine XMI, matching
            // --new-xmi bit-for-bit, so a correctly-prioritized resolver
            // produces `changed=0` while a broken one produces `changed>0`.
            var cacheDir = Path.Combine(scratch, "build", ".cache", "sysml-prev");
            Directory.CreateDirectory(cacheDir);
            var cachePath = Path.Combine(cacheDir, "MTConnectSysMLModel_v2.7.xml");
            var mutated = File.ReadAllText(realXmi)
                .Replace(
                    "unchangeable coordinate system that has machine zero as its origin.",
                    "OVERRIDE_TEST_MARKER coordinate system that has machine zero as its origin.");
            File.WriteAllText(cachePath, mutated);

            var (exitCode, stdout, stderr) = RunWithExplicitPrevious(realXmi, previousXmi: realXmi, scratch);

            Assert.That(exitCode, Is.Zero,
                $"Explicit --previous-xmi should succeed even when the cache carries a different XMI.\n"
                + $"stdout:\n{stdout}\nstderr:\n{stderr}");
            Assert.That(stdout, Does.Not.Contain("auto-derived from MTConnectVersions.Max"),
                "Explicit --previous-xmi must skip the auto-derive announcement — the delta is operator-directed.");
            Assert.That(stdout, Does.Contain("--previous-xmi override"),
                "stdout must announce the explicit-override mode so the operator sees which path fired.");

            // If the resolver had picked the cache, the mutation would surface
            // as CHANGED files. With the explicit prev matching the new XMI,
            // changed=0.
            var stats = ParseChanged(stdout);
            Assert.That(stats, Is.Zero,
                "Explicit --previous-xmi matched --new-xmi bit-for-bit; changed must be zero. "
                + "A non-zero count means the cache leaked into the delta — the explicit override lost.");
        }

        [Test]
        public void Missing_MTConnectVersions_cs_fails_with_actionable_message()
        {
            // ReadMTConnectVersionsMax throws FileNotFoundException when the
            // versions file is absent under --output. The top-level try/catch
            // in Program.cs (lines 172-182) maps that to stderr `error: ...`
            // + exit 1. Pin the actionable message the operator sees so a
            // later refactor of the error text still names all four
            // recovery paths (probed file path, --previous-xmi override,
            // --full-tree escape hatch, expected file location).
            var repoRoot = FindRepoRoot();
            var realXmi = Path.Combine(repoRoot, RealXmiRelativePath);

            var scratch = InitScratchRepoLayout("versions-cs-missing");
            // Deliberately do NOT write MTConnectVersions.cs — the guard
            // must fire before the resolver reaches Strategy A/B/C.

            var (exitCode, _, stderr) = RunAutoDerive(realXmi, scratch);

            Assert.That(exitCode, Is.EqualTo(1),
                $"Missing MTConnectVersions.cs must exit 1 via the top-level catch, not stack-trace.\nstderr:\n{stderr}");
            Assert.That(stderr, Does.Contain("MTConnectVersions.cs not found"),
                "stderr must name the missing file so the operator can locate it.");
            Assert.That(stderr, Does.Contain("--previous-xmi"),
                "stderr must direct the operator to the explicit-override flag.");
            Assert.That(stderr, Does.Contain("--full-tree"),
                "stderr must direct the operator to the delta-disable escape hatch.");
        }

        [Test]
        public void MTConnectVersions_cs_without_Max_declaration_fails_hard()
        {
            // The Max regex miss surfaces as InvalidOperationException →
            // top-level catch → exit 1 with a message that pinpoints the
            // convention the parser expects. Write a syntactically-valid
            // C# file that carries no `public static Version Max => ...`
            // property so the regex miss fires; the parser must reject
            // rather than silently no-op or default to a wrong version.
            var repoRoot = FindRepoRoot();
            var realXmi = Path.Combine(repoRoot, RealXmiRelativePath);

            var scratch = InitScratchRepoLayout("versions-cs-no-max");
            var targetPath = Path.Combine(
                scratch, "libraries", "MTConnect.NET-Common", "MTConnectVersions.cs");
            File.WriteAllText(targetPath, @"// Missing Max property; the parser must reject this file.
using System;
namespace MTConnect
{
    public static class MTConnectVersions
    {
        public static readonly Version Version27 = new Version(2, 7);
    }
}
");

            var (exitCode, _, stderr) = RunAutoDerive(realXmi, scratch);

            Assert.That(exitCode, Is.EqualTo(1),
                $"Missing Max property must exit 1 via the top-level catch.\nstderr:\n{stderr}");
            Assert.That(stderr, Does.Contain("Could not locate"),
                "stderr must announce a parse-shape failure, not a resolver failure.");
            Assert.That(stderr, Does.Contain("Max"),
                "stderr must name the missing convention element so the operator knows what to restore.");
            Assert.That(stderr, Does.Contain("--previous-xmi"),
                "stderr must direct the operator to the explicit-override flag.");
        }

        [Test]
        public void MTConnectVersions_cs_without_const_table_entry_fails_hard()
        {
            // The Max property resolves to a VersionXY constant that must
            // exist in the file's const table. If Max => VersionNN but no
            // `public static readonly Version VersionNN = new Version(...)`,
            // the resolver throws InvalidOperationException. This exercises
            // the second `if (!constMatch.Success)` branch, distinct from
            // the Max-regex-miss branch above.
            var repoRoot = FindRepoRoot();
            var realXmi = Path.Combine(repoRoot, RealXmiRelativePath);

            var scratch = InitScratchRepoLayout("versions-cs-no-const");
            var targetPath = Path.Combine(
                scratch, "libraries", "MTConnect.NET-Common", "MTConnectVersions.cs");
            File.WriteAllText(targetPath, @"// Max points at Version99 which is not declared.
using System;
namespace MTConnect
{
    public static class MTConnectVersions
    {
        public static Version Max => Version99;
        public static readonly Version Version27 = new Version(2, 7);
    }
}
");

            var (exitCode, _, stderr) = RunAutoDerive(realXmi, scratch);

            Assert.That(exitCode, Is.EqualTo(1),
                $"Missing const-table entry must exit 1 via the top-level catch.\nstderr:\n{stderr}");
            Assert.That(stderr, Does.Contain("Version99"),
                "stderr must name the un-resolvable constant so the operator can add it.");
            Assert.That(stderr, Does.Contain("Could not locate"),
                "stderr must carry the parse-shape failure fingerprint.");
        }

        [Test]
        public void Commented_out_Max_declaration_does_not_confuse_the_parser()
        {
            // Regression pin (F-IMP-401, dime cycle 3): a stale
            // `// public static Version Max => Version27;` line commented out
            // above the LIVE `public static Version Max => Version29;` line
            // would win the first-match regex without a comment-strip pass,
            // pinning PREV_VERSION to the wrong version (v2.7 not v2.9). This
            // fixture writes such a file with an OLD version commented out
            // above a NEW version live, then populates the cache path for the
            // NEW version. Auto-derive must pick up the NEW version (v2.9),
            // resolving the NEW cache path and NOT the OLD one.
            var repoRoot = FindRepoRoot();
            var realXmi = Path.Combine(repoRoot, RealXmiRelativePath);

            var scratch = InitScratchRepoLayout("commented-max-decoy");

            // MTConnectVersions.cs with a commented-out decoy Max line above
            // the live Max line. Both line comments (`//`) and a block-comment
            // (`/* ... */`) decoy are exercised so the strip covers both
            // shapes.
            var versionsPath = Path.Combine(
                scratch, "libraries", "MTConnect.NET-Common", "MTConnectVersions.cs");
            File.WriteAllText(versionsPath, @"// Copyright (c) 2026 TrakHound Inc.

using System;

namespace MTConnect
{
    public static class MTConnectVersions
    {
        // Historical decoy — the pre-bump Max line, kept as documentation.
        // public static Version Max => Version27;

        /* Alternative shape decoy retained for reference:
           public static Version Max => Version28;
         */

        public static Version Max => Version29;

        public static readonly Version Version27 = new Version(2, 7);
        public static readonly Version Version28 = new Version(2, 8);
        public static readonly Version Version29 = new Version(2, 9);
    }
}
");

            // Populate the cache for v2.9 ONLY. If the parser is fooled by
            // either comment-out decoy, it will look for v2.7 or v2.8 cache
            // paths (which are absent), fall through to Strategy C, and
            // fail-hard with a version-mismatched fingerprint.
            var cacheDir = Path.Combine(scratch, "build", ".cache", "sysml-prev");
            Directory.CreateDirectory(cacheDir);
            File.Copy(realXmi, Path.Combine(cacheDir, "MTConnectSysMLModel_v2.9.xml"));

            var (exitCode, stdout, stderr) = RunAutoDerive(realXmi, scratch);

            Assert.That(exitCode, Is.Zero,
                $"Comment-stripped parse must pick the live Max = Version29 and hit the v2.9 cache.\n"
                + $"stdout:\n{stdout}\nstderr:\n{stderr}");
            Assert.That(stdout, Does.Contain("MTConnectSysMLModel_v2.9.xml"),
                "The comment-stripped parse must resolve the v2.9 cache path (live Max), "
                + "not the v2.7 / v2.8 decoy paths.");
            Assert.That(stdout, Does.Not.Contain("MTConnectSysMLModel_v2.7.xml"),
                "The commented-out `Max => Version27` decoy must not fool the parser.");
            Assert.That(stdout, Does.Not.Contain("MTConnectSysMLModel_v2.8.xml"),
                "The block-commented `Max => Version28` decoy must not fool the parser.");
        }

        [Test]
        public void Prev_equals_new_warns_and_no_ops_when_new_xmi_filename_encodes_current_max()
        {
            // PREV == NEW guard: when the new XMI's filename encodes the same
            // version as the auto-derived PREV_VERSION (from MTConnectVersions.Max),
            // the delta is empty by construction — the max already matches the
            // version being generated. Exit 0 + a warning on stderr, no delta
            // emit. Filename convention is `MTConnectSysMLModel_v<major>.<minor>.xml`.
            var repoRoot = FindRepoRoot();
            var realXmi = Path.Combine(repoRoot, RealXmiRelativePath);
            Assert.That(File.Exists(realXmi), Is.True,
                $"XMI snapshot missing at {realXmi}. Is the build/sysml-model submodule initialized?");

            var scratch = InitScratchRepoLayout("prev-eq-new-guard");
            WriteSyntheticVersionsCs(scratch);

            // Populate the cache so Strategy B resolves — the guard runs AFTER
            // ResolvePreviousXmi succeeds. Without a cache, Strategy C would
            // fire and exit 1 before the guard could evaluate.
            var cacheDir = Path.Combine(scratch, "build", ".cache", "sysml-prev");
            Directory.CreateDirectory(cacheDir);
            File.Copy(realXmi, Path.Combine(cacheDir, "MTConnectSysMLModel_v2.7.xml"));

            // Copy realXmi to a filename that matches MTConnectVersions.Max (v2.7)
            // so the guard's filename regex matches and the versions equate.
            var newXmiVersioned = Path.Combine(scratch, "MTConnectSysMLModel_v2.7.xml");
            File.Copy(realXmi, newXmiVersioned);

            var (exitCode, stdout, stderr) = RunAutoDerive(newXmiVersioned, scratch);

            Assert.That(exitCode, Is.Zero,
                $"PREV==NEW must exit 0 with warning, not fail.\nstdout:\n{stdout}\nstderr:\n{stderr}");
            Assert.That(stderr, Does.Contain("already supported by MTConnectVersions.Max"),
                "stderr must announce the no-delta-to-derive warning so the operator sees why nothing was emitted.");
            Assert.That(stderr, Does.Contain("v2.7"),
                "stderr must name the version so the operator can verify the guard fired on the intended version.");
            Assert.That(stdout, Does.Not.Contain("Delta emission:"),
                "PREV==NEW must skip the delta emitter — no-op semantics.");
        }

        [Test]
        public void Prev_equals_new_guard_stays_silent_when_new_xmi_filename_has_no_version_suffix()
        {
            // The PREV==NEW guard predicates on the new XMI filename encoding a
            // version via the `_v<major>.<minor>.xml` suffix. A filename without
            // that suffix (the default `MTConnectSysMLModel.xml` snapshot shape)
            // must fall through to the normal delta emit — no warning, no early
            // return, even when MTConnectVersions.Max would numerically match
            // the underlying XMI's version. This preserves the default Phase 3
            // workflow where the newXmi is the un-suffixed submodule snapshot.
            var repoRoot = FindRepoRoot();
            var realXmi = Path.Combine(repoRoot, RealXmiRelativePath);

            var scratch = InitScratchRepoLayout("prev-eq-new-unsuffixed");
            WriteSyntheticVersionsCs(scratch);

            var cacheDir = Path.Combine(scratch, "build", ".cache", "sysml-prev");
            Directory.CreateDirectory(cacheDir);
            File.Copy(realXmi, Path.Combine(cacheDir, "MTConnectSysMLModel_v2.7.xml"));

            // newXmi is realXmi at its default un-suffixed path — guard's regex
            // does not match, guard stays silent, delta emitter runs.
            var (exitCode, stdout, stderr) = RunAutoDerive(realXmi, scratch);

            Assert.That(exitCode, Is.Zero,
                $"Un-suffixed new-xmi filename must NOT trigger the guard.\nstdout:\n{stdout}\nstderr:\n{stderr}");
            Assert.That(stderr, Does.Not.Contain("already supported by MTConnectVersions.Max"),
                "Un-suffixed filename must not trigger the PREV==NEW warning.");
            Assert.That(stdout, Does.Contain("Delta emission:"),
                "Un-suffixed filename must reach the delta emitter, not the guard's early return.");
        }

        [Test]
        public void Submodule_dir_without_git_repo_falls_through_to_fail_hard()
        {
            // Strategy A gates on TryGetSubmoduleTag returning a matching
            // tag. When the submodule dir exists and holds an XMI but is
            // NOT a git repository, `git describe` fails and
            // TryGetSubmoduleTag returns null — Strategy A rejects, and
            // Strategy C fires. Distinct from the "no submodule dir at all"
            // path already covered by the fail-hard fixture.
            var repoRoot = FindRepoRoot();
            var realXmi = Path.Combine(repoRoot, RealXmiRelativePath);

            var scratch = InitScratchRepoLayout("submodule-not-git");
            WriteSyntheticVersionsCs(scratch);

            var submoduleDir = Path.Combine(scratch, "build", "sysml-model");
            Directory.CreateDirectory(submoduleDir);
            File.Copy(realXmi, Path.Combine(submoduleDir, "MTConnectSysMLModel.xml"));
            // NO git init — TryGetSubmoduleTag must return null.

            var (exitCode, _, stderr) = RunAutoDerive(realXmi, scratch);

            Assert.That(exitCode, Is.EqualTo(1),
                $"A non-git submodule dir must fall through to Strategy C, not Strategy A.\nstderr:\n{stderr}");
            Assert.That(stderr, Does.Contain("PREV_VERSION auto-derivation"),
                "stderr must announce the Strategy-C fail-hard, not accept the un-tagged tree.");
            Assert.That(stderr, Does.Contain("v2.7"),
                "stderr must state the expected submodule tag so the operator sees which tag was required.");
        }

        [Test]
        public void Submodule_git_repo_with_wrong_tag_falls_through_to_fail_hard()
        {
            // Strategy A accepts only an EXACT-match tag. A git repo tagged
            // v9.9 (not v2.7 = MTConnectVersions.Max) must reject and fall
            // through to Strategy C. This exercises the branch where
            // TryGetSubmoduleTag returns a non-null string that fails the
            // Ordinal comparison against expectedTag.
            var repoRoot = FindRepoRoot();
            var realXmi = Path.Combine(repoRoot, RealXmiRelativePath);

            var scratch = InitScratchRepoLayout("submodule-wrong-tag");
            WriteSyntheticVersionsCs(scratch);

            var submoduleDir = Path.Combine(scratch, "build", "sysml-model");
            Directory.CreateDirectory(submoduleDir);
            File.Copy(realXmi, Path.Combine(submoduleDir, "MTConnectSysMLModel.xml"));
            InitGitRepoWithTag(submoduleDir, "v9.9");

            var (exitCode, _, stderr) = RunAutoDerive(realXmi, scratch);

            Assert.That(exitCode, Is.EqualTo(1),
                $"A wrong-tagged submodule must fall through to Strategy C.\nstderr:\n{stderr}");
            Assert.That(stderr, Does.Contain("PREV_VERSION auto-derivation"),
                "stderr must announce the Strategy-C fail-hard, not silently accept the wrong tag.");
            Assert.That(stderr, Does.Contain("v2.7"),
                "stderr must state the expected tag (v2.7) so the operator sees the mismatch.");
        }

        [Test]
        public void Full_tree_flag_disables_delta_mode()
        {
            var repoRoot = FindRepoRoot();
            var realXmi = Path.Combine(repoRoot, RealXmiRelativePath);

            var scratch = InitScratchRepoLayout("full-tree");
            WriteSyntheticVersionsCs(scratch);

            // Populate the cache so auto-derive WOULD succeed if it were
            // allowed to run. --full-tree must skip both delta paths and
            // trigger the full-tree branch instead, producing no Compat
            // file and no `Delta emission:` stats line.
            var cacheDir = Path.Combine(scratch, "build", ".cache", "sysml-prev");
            Directory.CreateDirectory(cacheDir);
            File.Copy(realXmi, Path.Combine(cacheDir, "MTConnectSysMLModel_v2.7.xml"));

            var (exitCode, stdout, stderr) = RunWithFullTree(realXmi, scratch);

            Assert.That(exitCode, Is.Zero,
                $"--full-tree must succeed against a valid tree.\nstdout:\n{stdout}\nstderr:\n{stderr}");
            Assert.That(stdout, Does.Contain("full-tree"),
                "stdout must announce that the full-tree path fired.");
            Assert.That(stdout, Does.Not.Contain("Delta emission:"),
                "--full-tree must skip the delta emitter's stats line — the delta path is fully disabled.");
            Assert.That(stdout, Does.Not.Contain("auto-derived from MTConnectVersions.Max"),
                "--full-tree must short-circuit before the auto-derive resolver runs.");

            var compatFiles = Directory
                .EnumerateFiles(scratch, "*.g.cs", SearchOption.AllDirectories)
                .Where(p => p.Replace('\\', '/').Contains("/Compat/"))
                .ToList();
            Assert.That(compatFiles, Is.Empty,
                "--full-tree must emit zero Compat/*.g.cs files (Compat is delta-mode-only). "
                + "Unexpected Compat files:\n  " + string.Join("\n  ", compatFiles));

            // Full-tree emits the whole tree — at least the current-XMI
            // baseline count of files. The committed tree ships ~892 .g.cs
            // files at v2.7 landing (2026-08-20); pin a floor of 700 so
            // ordinary spec-shrink drift (a version dropping ~15 types) is
            // tolerated but a delta-mode leakage (which would emit only the
            // ~10-file diff, not the full tree) trips the guard loudly.
            // A previous `>100` threshold accepted any partial emission
            // including the delta subset.
            var emittedFiles = Directory
                .EnumerateFiles(scratch, "*.g.cs", SearchOption.AllDirectories)
                .Count();
            Assert.That(emittedFiles, Is.GreaterThan(700),
                "--full-tree must emit the whole generated tree, not the delta subset. "
                + $"Actual .g.cs count: {emittedFiles}. A count in the ~10-100 range "
                + "signals a delta-mode leak; a count under 700 signals substantial spec "
                + "shrink and should ratchet this floor after human review.");
        }

        // --- helpers -----------------------------------------------------

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

        // Creates the scratch dir with a repo-like layout: the three library
        // subdirectories the renderers guard against, plus the build/ tree
        // ancestor that the cache and submodule strategies probe under.
        private static string InitScratchRepoLayout(string suffix)
        {
            var repoRoot = FindRepoRoot();
            var path = Path.Combine(repoRoot, ScratchRoot, suffix);
            if (Directory.Exists(path))
            {
                // A prior test run may have left a synthetic git repo behind
                // whose .git/objects tree resists a plain recursive delete on
                // some filesystems. Two-pass delete: first try recursive,
                // then if that fails, chmod the tree writable and retry.
                TryDeleteTree(path);
            }
            Directory.CreateDirectory(path);
            Directory.CreateDirectory(Path.Combine(path, "libraries", "MTConnect.NET-Common"));
            Directory.CreateDirectory(Path.Combine(path, "libraries", "MTConnect.NET-JSON-cppagent"));
            Directory.CreateDirectory(Path.Combine(path, "libraries", "MTConnect.NET-XML"));
            Directory.CreateDirectory(Path.Combine(path, "build"));
            return path;
        }

        private static void TryDeleteTree(string path)
        {
            try
            {
                Directory.Delete(path, recursive: true);
            }
            catch (UnauthorizedAccessException)
            {
                // Loose read-only bits on git pack files trip the plain delete
                // on Windows; clear them and retry once.
                foreach (var f in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
                {
                    try { File.SetAttributes(f, FileAttributes.Normal); } catch { }
                }
                Directory.Delete(path, recursive: true);
            }
        }

        private static void WriteSyntheticVersionsCs(string scratch)
        {
            var target = Path.Combine(
                scratch, "libraries", "MTConnect.NET-Common", "MTConnectVersions.cs");
            File.WriteAllText(target, SyntheticVersionsCs);
        }

        // Bootstraps a minimal git repo at `dir`, stages every present file,
        // commits, and tags the commit `tagName`. The auto-derive Strategy A
        // path runs `git -C <dir> describe --exact-match --tags HEAD`; this
        // helper produces the shape that lookup expects.
        //
        // Every git config that could pull in a signing hook is disabled
        // per-repo (commit.gpgsign, tag.gpgsign, tag.forceSignAnnotated) so the
        // helper works on a developer host with the tester's global-config
        // signing hooks (ottobolyos runs `commit.gpgsign=true` + `tag.gpgsign=true`
        // globally — those defaults would abort the synthetic tag on a host
        // without a matching GPG key context).
        private static void InitGitRepoWithTag(string dir, string tagName)
        {
            RunGit(dir, "init", "-q");
            RunGit(dir, "config", "user.email", "auto-derive-test@example.invalid");
            RunGit(dir, "config", "user.name", "Auto Derive Test");
            RunGit(dir, "config", "commit.gpgsign", "false");
            RunGit(dir, "config", "tag.gpgsign", "false");
            RunGit(dir, "config", "tag.forceSignAnnotated", "false");
            RunGit(dir, "add", "-A");
            RunGit(dir, "commit", "-q", "-m", "synthetic sysml-model snapshot for auto-derive test");
            // Explicit lightweight tag — no `-a`, no `-s`, no message — so the
            // synthetic tag lands regardless of tester-side GPG state. The
            // per-repo `tag.gpgsign=false` above is defense-in-depth for the
            // same concern.
            RunGit(dir, "tag", tagName);
        }

        private static void RunGit(string workingDir, params string[] args)
        {
            var psi = new ProcessStartInfo("git")
            {
                WorkingDirectory = workingDir,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            foreach (var a in args)
                psi.ArgumentList.Add(a);

            using var proc = Process.Start(psi)
                ?? throw new InvalidOperationException($"Failed to start git {string.Join(' ', args)}.");
            var stdoutTask = proc.StandardOutput.ReadToEndAsync();
            var stderrTask = proc.StandardError.ReadToEndAsync();
            System.Threading.Tasks.Task.WhenAll(stdoutTask, stderrTask).GetAwaiter().GetResult();
            proc.WaitForExit();
            if (proc.ExitCode != 0)
            {
                throw new InvalidOperationException(
                    $"git {string.Join(' ', args)} exited {proc.ExitCode} in {workingDir}. " +
                    $"stderr:\n{stderrTask.Result}");
            }
        }

        // Auto-derive invocation shape: only --new-xmi + --output. No
        // --previous-xmi, no --full-tree — this is exactly the zero-config
        // form Phase 3 of the version-bump plan calls.
        private static (int ExitCode, string Stdout, string Stderr) RunAutoDerive(
            string newXmi, string output)
        {
            return RunGenerator(output, "--new-xmi", newXmi, "--output", output);
        }

        private static (int ExitCode, string Stdout, string Stderr) RunWithExplicitPrevious(
            string newXmi, string previousXmi, string output)
        {
            return RunGenerator(output,
                "--new-xmi", newXmi,
                "--previous-xmi", previousXmi,
                "--output", output);
        }

        private static (int ExitCode, string Stdout, string Stderr) RunWithFullTree(
            string newXmi, string output)
        {
            return RunGenerator(output,
                "--new-xmi", newXmi,
                "--output", output,
                "--full-tree");
        }

        private static (int ExitCode, string Stdout, string Stderr) RunGenerator(
            string outputRootForCwd, params string[] cliArgs)
        {
            var repoRoot = FindRepoRoot();
            var psi = new ProcessStartInfo("dotnet")
            {
                // Run `dotnet run` from the REAL repo root so the generator
                // project builds correctly (the ProjectReference on the test
                // csproj already built it, and --no-build below reuses that
                // output). The generator's --output points at the SCRATCH
                // dir, so all path probes land there.
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            psi.ArgumentList.Add("run");
            psi.ArgumentList.Add("--no-build");
            psi.ArgumentList.Add("--project");
            psi.ArgumentList.Add(GeneratorProject);
            psi.ArgumentList.Add("--");
            foreach (var arg in cliArgs)
                psi.ArgumentList.Add(arg);

            using var proc = Process.Start(psi)
                ?? throw new InvalidOperationException("Failed to start dotnet run for the generator.");

            // Drain stdout and stderr concurrently — see ByteIdenticalRegenTests
            // for the deadlock defense this pattern encodes.
            var stdoutTask = proc.StandardOutput.ReadToEndAsync();
            var stderrTask = proc.StandardError.ReadToEndAsync();
            System.Threading.Tasks.Task.WhenAll(stdoutTask, stderrTask).GetAwaiter().GetResult();
            proc.WaitForExit();
            return (proc.ExitCode, stdoutTask.Result, stderrTask.Result);
        }

        // Extracts the `changed=N` value from the delta stats line so the
        // explicit-override test can pin the CHANGED count. Returns -1 on
        // absent stats line (which is a distinct failure mode from
        // changed=0).
        private static int ParseChanged(string stdout)
        {
            var match = System.Text.RegularExpressions.Regex.Match(
                stdout, @"Delta emission:.*?changed=(?<c>\d+)");
            if (!match.Success)
                throw new AssertionException(
                    "stdout does not carry the expected 'Delta emission: ... changed=N ...' stats line.\n"
                    + stdout);
            return int.Parse(match.Groups["c"].Value);
        }
    }
}
