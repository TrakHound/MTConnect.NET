// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;

namespace MTConnect.NET_Generator_Tests
{
    /// <summary>
    /// CLI failure-path coverage for the SysML importer's <c>Program.cs</c>.
    ///
    /// <para>
    /// The importer's happy path (full-tree regen + delta regen) is covered
    /// by <see cref="ByteIdenticalRegenTests"/> and <see cref="DeltaRegenTests"/>.
    /// This fixture pins the EARLY-RETURN branches — every documented exit
    /// code, every invalid-input surface, and the two <c>RequireValue</c>
    /// throws that fire when a flag arrives without its trailing value.
    /// </para>
    ///
    /// <para>
    /// Every case is exercised end-to-end via <c>dotnet run --no-build
    /// --project build/MTConnect.NET-SysML-Import</c> so the assertions bind
    /// to the CLI contract the operator actually sees, not to an internal
    /// helper. Exit codes are documented in the <c>Program.cs</c> header:
    /// <list type="bullet">
    ///   <item><c>0</c> — success (including <c>--help</c> / <c>-h</c>).</item>
    ///   <item><c>1</c> — runtime failure (file not found, parse null, missing library subdir).</item>
    ///   <item><c>2</c> — usage failure (missing / unknown flag).</item>
    /// </list>
    /// </para>
    ///
    /// <para>
    /// Extra runtime failures (<c>RequireValue</c> throws on a dangling
    /// flag, <c>MTConnectModel.Parse</c> returns null on a malformed XMI,
    /// a missing library subdirectory throws <c>DirectoryNotFoundException</c>)
    /// surface as non-zero exit codes; the assertions there are on
    /// <c>ExitCode != 0</c> plus the stderr fingerprint, since the .NET
    /// runtime unhandled-exception exit code (0x80000000-ish, negative
    /// signed) is host-dependent.
    /// </para>
    /// </summary>
    [TestFixture]
    public class CliInvocationFailureTests
    {
        private const string SlnFileName = "MTConnect.NET.sln";
        private const string GeneratorProject = "build/MTConnect.NET-SysML-Import";
        private const string XmiRelativePath = "build/sysml-model/MTConnectSysMLModel.xml";
        private const string ScratchRoot = ".claude/gen-test-out/cli-failure";

        [Test]
        public void Unknown_flag_exits_2_and_stderr_names_the_flag()
        {
            var (exitCode, _, stderr) = Run("--not-a-real-flag");
            Assert.That(exitCode, Is.EqualTo(2),
                "Unknown flags are a usage error; exit 2 is the documented contract.");
            Assert.That(stderr, Does.Contain("Unknown argument"),
                "stderr should name the flag class so the operator sees a discoverable message.");
            Assert.That(stderr, Does.Contain("--not-a-real-flag"),
                "stderr should echo the offending flag verbatim.");
        }

        [Test]
        public void Missing_xmi_flag_exits_2_with_required_message()
        {
            var scratch = InitScratch("missing-xmi");
            var (exitCode, _, stderr) = Run("--output", scratch);
            Assert.That(exitCode, Is.EqualTo(2),
                "Missing --new-xmi is a usage error; exit 2.");
            Assert.That(stderr, Does.Contain("--new-xmi"),
                "stderr should identify which required flag is missing.");
            Assert.That(stderr, Does.Contain("--xmi"),
                "stderr should also mention the legacy --xmi alias so operators grepping for the pre-#233 flag name still see the required-flag hint.");
            Assert.That(stderr, Does.Contain("required"),
                "stderr should call out that the flag is required.");
        }

        [Test]
        public void Missing_output_flag_exits_2_with_required_message()
        {
            var repoRoot = FindRepoRoot();
            var xmi = Path.Combine(repoRoot, XmiRelativePath);
            var (exitCode, _, stderr) = Run("--xmi", xmi);
            Assert.That(exitCode, Is.EqualTo(2),
                "Missing --output is a usage error; exit 2.");
            Assert.That(stderr, Does.Contain("--output"),
                "stderr should identify which required flag is missing.");
        }

        [Test]
        public void Nonexistent_xmi_file_exits_1_with_not_found_message()
        {
            var scratch = InitScratch("nonexistent-xmi");
            var bogusXmi = Path.Combine(scratch, "does-not-exist.xml");
            var (exitCode, _, stderr) = Run("--xmi", bogusXmi, "--output", scratch);
            Assert.That(exitCode, Is.EqualTo(1),
                "Missing XMI file is a runtime failure; exit 1.");
            Assert.That(stderr, Does.Contain("XMI file not found"),
                "stderr should name the failure class so the operator can act.");
            Assert.That(stderr, Does.Contain(bogusXmi),
                "stderr should echo the resolved path so a typo is grep-able.");
        }

        [Test]
        public void Nonexistent_previous_xmi_file_exits_1_with_not_found_message()
        {
            var repoRoot = FindRepoRoot();
            var xmi = Path.Combine(repoRoot, XmiRelativePath);
            var scratch = InitScratch("nonexistent-previous-xmi");
            var bogusPrev = Path.Combine(scratch, "does-not-exist-prev.xml");
            var (exitCode, _, stderr) = Run("--xmi", xmi, "--previous-xmi", bogusPrev, "--output", scratch);
            Assert.That(exitCode, Is.EqualTo(1),
                "Missing --previous-xmi file is a runtime failure; exit 1.");
            Assert.That(stderr, Does.Contain("--previous-xmi file not found"),
                "stderr should name the exact flag whose target is missing.");
            Assert.That(stderr, Does.Contain(bogusPrev),
                "stderr should echo the resolved path.");
        }

        [Test]
        public void Nonexistent_output_root_exits_1_with_not_found_message()
        {
            var repoRoot = FindRepoRoot();
            var xmi = Path.Combine(repoRoot, XmiRelativePath);
            var bogusOutput = Path.Combine(repoRoot, ScratchRoot, "does-not-exist-output");
            // Deliberately do NOT create bogusOutput's directory.
            if (Directory.Exists(bogusOutput))
                Directory.Delete(bogusOutput, recursive: true);
            var (exitCode, _, stderr) = Run("--xmi", xmi, "--output", bogusOutput);
            Assert.That(exitCode, Is.EqualTo(1),
                "Missing output root is a runtime failure; exit 1.");
            Assert.That(stderr, Does.Contain("Output root not found"),
                "stderr should identify the output-root failure class.");
        }

        [Test]
        public void Help_flag_exits_0_and_stdout_carries_usage_banner()
        {
            var (exitCode, stdout, _) = Run("--help");
            Assert.That(exitCode, Is.Zero,
                "--help is a documented success path; exit 0.");
            Assert.That(stdout, Does.Contain("MTConnect.NET SysML Importer"),
                "Help output must carry the tool banner so the operator knows what they're using.");
            Assert.That(stdout, Does.Contain("--new-xmi"),
                "Help must list --new-xmi (preferred flag; task #408).");
            Assert.That(stdout, Does.Contain("--xmi"),
                "Help must also mention --xmi (legacy alias documented for pre-#408 callers).");
            Assert.That(stdout, Does.Contain("--previous-xmi"),
                "Help must list --previous-xmi (added in Phase 4.3).");
            Assert.That(stdout, Does.Contain("--compat-version-label"),
                "Help must list --compat-version-label (added in Phase 4.3).");
            Assert.That(stdout, Does.Contain("--full-tree"),
                "Help must list --full-tree (added in task #408 as the escape hatch that disables both delta paths).");
        }

        [Test]
        public void Short_help_flag_exits_0()
        {
            var (exitCode, stdout, _) = Run("-h");
            Assert.That(exitCode, Is.Zero, "-h is the short form of --help; exit 0.");
            Assert.That(stdout, Does.Contain("MTConnect.NET SysML Importer"),
                "-h must produce the same banner as --help.");
        }

        [Test]
        public void Missing_value_after_xmi_flag_exits_non_zero_with_argument_exception()
        {
            // RequireValue throws ArgumentException when the flag is the last
            // token and no value follows. The unhandled exception bubbles to
            // the CLR host and returns a non-zero exit code; the stderr
            // fingerprint carries the ArgumentException message.
            var (exitCode, _, stderr) = Run("--xmi");
            Assert.That(exitCode, Is.Not.Zero,
                "A flag with no trailing value is a runtime failure; exit must be non-zero.");
            Assert.That(stderr, Does.Contain("--xmi"),
                "stderr should name the offending flag.");
            Assert.That(stderr, Does.Contain("requires a value").Or.Contain("ArgumentException"),
                "stderr should carry the RequireValue-throw fingerprint.");
        }

        [Test]
        public void Missing_value_after_new_xmi_flag_exits_non_zero()
        {
            // Task #408 introduced --new-xmi as the preferred spelling. Its
            // RequireValue arm is a distinct switch case from --xmi; pin the
            // parallel failure surface so a later refactor can't silently
            // regress the preferred-flag arm while leaving the legacy alias
            // exercised.
            var (exitCode, _, stderr) = Run("--new-xmi");
            Assert.That(exitCode, Is.Not.Zero,
                "A --new-xmi with no trailing value is a runtime failure; exit non-zero.");
            Assert.That(stderr, Does.Contain("--new-xmi"),
                "stderr should name the offending flag verbatim.");
            Assert.That(stderr, Does.Contain("requires a value").Or.Contain("ArgumentException"),
                "stderr should carry the RequireValue-throw fingerprint.");
        }

        [Test]
        public void Missing_value_after_previous_xmi_flag_exits_non_zero()
        {
            var repoRoot = FindRepoRoot();
            var xmi = Path.Combine(repoRoot, XmiRelativePath);
            // --previous-xmi is the last token; RequireValue throws.
            var (exitCode, _, stderr) = Run("--xmi", xmi, "--previous-xmi");
            Assert.That(exitCode, Is.Not.Zero,
                "A --previous-xmi with no trailing value is a runtime failure; exit non-zero.");
            Assert.That(stderr, Does.Contain("--previous-xmi"),
                "stderr should name the offending flag.");
        }

        [Test]
        public void Missing_value_after_compat_version_label_exits_non_zero()
        {
            var repoRoot = FindRepoRoot();
            var xmi = Path.Combine(repoRoot, XmiRelativePath);
            var (exitCode, _, stderr) = Run("--xmi", xmi, "--compat-version-label");
            Assert.That(exitCode, Is.Not.Zero,
                "A --compat-version-label with no trailing value is a runtime failure; exit non-zero.");
            Assert.That(stderr, Does.Contain("--compat-version-label"),
                "stderr should name the offending flag.");
        }

        [Test]
        public void Missing_value_after_output_flag_exits_non_zero()
        {
            var repoRoot = FindRepoRoot();
            var xmi = Path.Combine(repoRoot, XmiRelativePath);
            var (exitCode, _, stderr) = Run("--xmi", xmi, "--output");
            Assert.That(exitCode, Is.Not.Zero,
                "A --output with no trailing value is a runtime failure; exit non-zero.");
            Assert.That(stderr, Does.Contain("--output"),
                "stderr should name the offending flag.");
        }

        [Test]
        public void Missing_value_after_json_dump_flag_exits_non_zero()
        {
            var repoRoot = FindRepoRoot();
            var xmi = Path.Combine(repoRoot, XmiRelativePath);
            var (exitCode, _, stderr) = Run("--xmi", xmi, "--json-dump");
            Assert.That(exitCode, Is.Not.Zero,
                "A --json-dump with no trailing value is a runtime failure; exit non-zero.");
            Assert.That(stderr, Does.Contain("--json-dump"),
                "stderr should name the offending flag.");
        }

        [Test]
        public void Missing_library_subdirectory_under_output_root_throws()
        {
            // Output root exists, but the required libraries/MTConnect.NET-Common
            // subdirectory is absent. Program's RenderCommonClasses fails
            // fast with a DirectoryNotFoundException. Pass --full-tree so the
            // zero-config auto-derive path doesn't intercept first with its
            // own "MTConnectVersions.cs not found" surface — this fixture is
            // pinning the RenderCommonClasses failure, not the auto-derive
            // failure (that path is covered by AutoDerivePreviousXmiTests).
            var repoRoot = FindRepoRoot();
            var xmi = Path.Combine(repoRoot, XmiRelativePath);
            var scratch = InitScratch("missing-lib-subdir");
            // Deliberately do NOT create the libraries/MTConnect.NET-Common subdir.
            var (exitCode, _, stderr) = Run("--xmi", xmi, "--output", scratch, "--full-tree");
            Assert.That(exitCode, Is.Not.Zero,
                "A missing library subdirectory must fail fast, not silently no-op.");
            Assert.That(stderr, Does.Contain("MTConnect.NET-Common").Or.Contain("DirectoryNotFoundException"),
                "stderr should identify which subdir is missing so the operator can create it.");
        }

        [Test]
        public void Malformed_xmi_exits_non_zero_and_surfaces_parse_failure()
        {
            var scratch = InitScratchWithLibraries("malformed-xmi");
            var badXmi = Path.Combine(scratch, "malformed.xml");
            // A well-formed XML that is not a SysML XMI. Two possible
            // surface behaviors:
            //   (a) MTConnectModel.Parse returns null → Program's full-tree
            //       branch prints "error: Failed to parse XMI" and returns 1.
            //   (b) MTConnectModel.Parse throws (missing UML root element,
            //       unhandled KeyNotFoundException, etc.) → the CLR host
            //       returns a non-zero abnormal-termination exit code
            //       (typically 134 on Linux, i.e. SIGABRT from an unhandled
            //       exception).
            // Both surfaces satisfy the coverage contract "malformed input is
            // a runtime failure". The (a) branch is the graceful, operator-
            // friendly one and would be a nice hardening target (a top-level
            // try/catch that mapped every parse exception to `return 1;`);
            // that hardening is tracked as a follow-up finding.
            File.WriteAllText(badXmi, "<?xml version=\"1.0\"?><notxmi/>");
            // --full-tree so the parse failure lands in the full-tree branch, not
            // in the zero-config auto-derive's PREV_VERSION resolver — this
            // fixture is pinning the parse-failure surface, not the auto-derive
            // one (that path is covered by AutoDerivePreviousXmiTests).
            var (exitCode, stdout, stderr) = Run("--xmi", badXmi, "--output", scratch, "--full-tree");
            Assert.That(exitCode, Is.Not.Zero,
                $"A malformed XMI must fail the invocation. exit={exitCode}\nstdout:\n{stdout}\nstderr:\n{stderr}");
            var combined = stdout + "\n" + stderr;
            Assert.That(combined,
                Does.Contain("Failed to parse XMI")
                .Or.Contain("parse")
                .Or.Contain("Exception")
                .Or.Contain("XmlException")
                .Or.Contain("NullReference"),
                "The output stream must surface a parse-failure fingerprint the operator can grep for.");
        }

        // Every hostile / malformed --compat-version-label value that
        // IsSafeCompatLabel is designed to reject. Each case exercises the
        // exit-2 guard branch (Program.cs:198-204). The regex accepts
        // 1..64 chars of [A-Za-z0-9_\-] followed by [A-Za-z0-9_\-.]*, no
        // leading dot; anything else must reject at argument-parse time.
        //
        // The `TestCaseSource` shape (versus inline `[TestCase]`) keeps the
        // 65-char oversize label programmatically constructed rather than
        // hard-coded, so a later ratchet of the length limit needs to
        // change only the source method + the guard.
        [TestCaseSource(nameof(HostileCompatLabelCases))]
        public void Hostile_compat_version_label_rejects_with_exit_2(
            string hostileLabel, string scenario)
        {
            var repoRoot = FindRepoRoot();
            var xmi = Path.Combine(repoRoot, XmiRelativePath);
            var scratch = InitScratchWithLibraries($"hostile-label-{scenario}");

            // --full-tree short-circuits the auto-derive resolver so the
            // safety check is exercised in isolation. Without --full-tree
            // the zero-config resolver would abort first (no
            // MTConnectVersions.cs under the scratch tree), masking the
            // IsSafeCompatLabel branch under a different early-return.
            var (exitCode, _, stderr) = Run(
                "--xmi", xmi,
                "--output", scratch,
                "--compat-version-label", hostileLabel,
                "--full-tree");

            Assert.That(exitCode, Is.EqualTo(2),
                $"Hostile --compat-version-label '{hostileLabel}' ({scenario}) must reject with usage-error exit 2. "
                + $"An exit 0/1 signals the IsSafeCompatLabel guard was bypassed. stderr:\n{stderr}");
            Assert.That(stderr, Does.Contain("--compat-version-label"),
                "stderr must name the offending flag so the operator sees which value the parser rejected.");
            Assert.That(stderr, Does.Contain("not a safe filename"),
                "stderr must carry the guard's rejection fingerprint (`not a safe filename`) "
                + "so the operator distinguishes label-shape rejection from other exit-2 causes.");
        }

        // Enumerates the hostile-label surface. Each entry is
        // (label, scenario-slug); the slug feeds the scratch-dir suffix so
        // parallel runs don't collide. Every entry must reject via
        // IsSafeCompatLabel returning false.
        private static object[] HostileCompatLabelCases()
        {
            return new object[]
            {
                new object[] { "../etc/passwd", "path-traversal-parent" },
                new object[] { "Compat/../secret", "path-traversal-inline" },
                new object[] { "sub/dir", "forward-slash" },
                new object[] { "sub\\dir", "backslash" },
                new object[] { ".hidden", "leading-dot" },
                new object[] { "..", "double-dot" },
                new object[] { "  ", "whitespace-only" },
                new object[] { "with space", "internal-space" },
                new object[] { "label;drop", "semicolon-injection" },
                new object[] { "label$var", "shell-metachar" },
                new object[] { "label|pipe", "pipe" },
                new object[] { "label\ttab", "control-tab" },
                new object[] { new string('A', 65), "over-length" },
            };
        }

        [Test]
        public void Empty_compat_version_label_rejects_with_exit_2()
        {
            // An empty string satisfies the flag-has-a-value check (RequireValue
            // returns "" rather than throwing) but must fail the safety guard
            // (IsNullOrWhiteSpace short-circuits IsSafeCompatLabel to false).
            // This is the boundary case for the length-lower-bound arm.
            var repoRoot = FindRepoRoot();
            var xmi = Path.Combine(repoRoot, XmiRelativePath);
            var scratch = InitScratchWithLibraries("hostile-label-empty");
            var (exitCode, _, stderr) = Run(
                "--xmi", xmi,
                "--output", scratch,
                "--compat-version-label", "",
                "--full-tree");
            Assert.That(exitCode, Is.EqualTo(2),
                $"An empty --compat-version-label must reject with usage-error exit 2. stderr:\n{stderr}");
            Assert.That(stderr, Does.Contain("not a safe filename"),
                "stderr must carry the guard's rejection fingerprint.");
        }

        // Safe-label positive cases — every documented default and every
        // pattern the auto-derive machinery emits must PASS the guard so a
        // ratchet of the regex (accidentally tightening it) can't silently
        // regress the happy path.
        [TestCase("Previous")]
        [TestCase("v2_7")]
        [TestCase("v10_15")]
        [TestCase("Release-2.6.0")]
        [TestCase("a")]
        // 64-char boundary case: 26 upper + 26 lower + 10 digits + `_-` = 64.
        // Exercises the length-upper-bound `label.Length > 64` arm at its
        // exact accept-side boundary.
        [TestCase("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789__")]
        public void Safe_compat_version_label_accepts_and_reaches_full_tree_branch(string safeLabel)
        {
            // Boundary: the last case is exactly 64 chars — the length-upper
            // bound. IsSafeCompatLabel rejects >64 but must accept ==64.
            Assert.That(safeLabel.Length, Is.LessThanOrEqualTo(64),
                "Test setup invariant — safe labels sit inside the length window.");

            var repoRoot = FindRepoRoot();
            var xmi = Path.Combine(repoRoot, XmiRelativePath);
            // Sanitise the label for the scratch-dir suffix (the label may
            // contain '.' which is legal in filenames but collides with the
            // dir-slug convention). Replace non-alnum with '_'.
            var scratchSuffix = "safe-label-" + System.Text.RegularExpressions.Regex.Replace(safeLabel, @"[^A-Za-z0-9]", "_");
            if (scratchSuffix.Length > 96) scratchSuffix = scratchSuffix[..96];
            var scratch = InitScratchWithLibraries(scratchSuffix);

            var (exitCode, _, stderr) = Run(
                "--xmi", xmi,
                "--output", scratch,
                "--compat-version-label", safeLabel,
                "--full-tree");

            Assert.That(exitCode, Is.Zero,
                $"Safe --compat-version-label '{safeLabel}' must not be rejected by the guard. stderr:\n{stderr}");
            Assert.That(stderr, Does.Not.Contain("not a safe filename"),
                "stderr must not carry the guard rejection message for a safe label.");
        }

        [Test]
        public void JsonDump_writes_the_dump_file_when_flag_supplied()
        {
            var repoRoot = FindRepoRoot();
            var xmi = Path.Combine(repoRoot, XmiRelativePath);
            var scratch = InitScratchWithLibraries("json-dump");
            var dumpPath = Path.Combine(scratch, "model.json");
            // --full-tree so the JSON-dump path is exercised without the
            // zero-config auto-derive stepping in (which would resolve to the
            // same-tree v2.7 XMI and successfully run delta mode, wasting time
            // on a delta the test doesn't assert against).
            var (exitCode, stdout, stderr) = Run("--xmi", xmi, "--output", scratch, "--json-dump", dumpPath, "--full-tree");
            Assert.That(exitCode, Is.Zero,
                $"--json-dump plus a valid XMI + output should succeed.\nstdout:\n{stdout}\nstderr:\n{stderr}");
            Assert.That(File.Exists(dumpPath), Is.True,
                "The dump file should exist at the requested path.");
            var dumpContent = File.ReadAllText(dumpPath);
            Assert.That(dumpContent.Length, Is.GreaterThan(1024),
                "The dump content must be a non-trivial JSON tree, not an empty file.");
            Assert.That(dumpContent.TrimStart(), Does.StartWith("{"),
                "The dump content must start as a JSON object.");
            Assert.That(stdout, Does.Contain("JSON dump: writing to"),
                "stdout should echo the resolved dump path so the operator can verify placement.");
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

        // Creates the scratch dir root without library subdirectories. Used
        // when the test needs a "valid output-root path that lacks the
        // library scaffolding" (exercises the throw path in
        // RenderCommonClasses / RenderJsonComponents / RenderXmlComponents).
        private static string InitScratch(string suffix)
        {
            var repoRoot = FindRepoRoot();
            var path = Path.Combine(repoRoot, ScratchRoot, suffix);
            if (Directory.Exists(path))
                Directory.Delete(path, recursive: true);
            Directory.CreateDirectory(path);
            return path;
        }

        // Creates the scratch dir root PLUS the three library subdirectories
        // the generator's full-tree branch guards against. Used for the
        // happy-path adjacent cases (malformed XMI, JSON-dump).
        private static string InitScratchWithLibraries(string suffix)
        {
            var path = InitScratch(suffix);
            Directory.CreateDirectory(Path.Combine(path, "libraries", "MTConnect.NET-Common"));
            Directory.CreateDirectory(Path.Combine(path, "libraries", "MTConnect.NET-JSON-cppagent"));
            Directory.CreateDirectory(Path.Combine(path, "libraries", "MTConnect.NET-XML"));
            return path;
        }

        private static (int ExitCode, string Stdout, string Stderr) Run(params string[] cliArgs)
        {
            var repoRoot = FindRepoRoot();
            var psi = new ProcessStartInfo("dotnet")
            {
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
    }
}
