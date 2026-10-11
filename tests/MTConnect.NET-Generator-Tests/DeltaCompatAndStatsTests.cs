// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace MTConnect.NET_Generator_Tests
{
    /// <summary>
    /// Delta-mode invariants that <see cref="DeltaRegenTests"/> does not
    /// cover: the Compat file's header + multi-namespace concentration, the
    /// <c>--compat-version-label</c> default (<c>"Previous"</c>), and the
    /// stdout stats line's per-category counter reporting.
    ///
    /// <para>
    /// The plan-D4 contract for the concentrated Compat file is:
    /// <list type="bullet">
    ///   <item>Prefixed with the TrakHound copyright + MIT licence header.</item>
    ///   <item>Each concentrated block introduced by a
    ///     <c>// --- from &lt;relative-path&gt; ---</c> divider and prefixed by
    ///     the source file's original body verbatim (including its
    ///     <c>namespace X { ... }</c> block, since multi-namespace
    ///     concentration is legal C#).</item>
    ///   <item>Byte-identical to the source file's body for every UNCHANGED
    ///     entry (so <c>git diff</c> shows zero drift after a rebuild).</item>
    /// </list>
    /// </para>
    ///
    /// <para>
    /// The stdout stats line is the operator's telemetry surface: every
    /// invocation prints <c>Delta emission: added=N, changed=N,
    /// unchanged-concentrated=N, removed-skipped=N, compat-files-written=N</c>
    /// so a spec bump's shape is grep-able. Pinning the format keeps the
    /// operator-facing contract explicit; a silent rename of any counter
    /// key would flip these tests RED.
    /// </para>
    /// </summary>
    [TestFixture]
    public class DeltaCompatAndStatsTests
    {
        private const string SlnFileName = "MTConnect.NET.sln";
        private const string GeneratorProject = "build/MTConnect.NET-SysML-Import";
        private const string XmiRelativePath = "build/sysml-model/MTConnectSysMLModel.xml";
        private const string ScratchRoot = ".claude/gen-test-out/delta-compat";

        [Test]
        public void Same_XMI_stats_line_reports_zero_added_changed_removed_and_positive_unchanged()
        {
            var repoRoot = FindRepoRoot();
            var xmiPath = Path.Combine(repoRoot, XmiRelativePath);
            var scratch = InitScratch("same-stats");

            var (exitCode, stdout, stderr) = RunDelta(xmiPath, previousXmiPath: xmiPath,
                compatLabel: "Baseline", output: scratch);
            Assert.That(exitCode, Is.Zero, $"Generator exited non-zero.\nstdout:\n{stdout}\nstderr:\n{stderr}");

            var stats = ParseStatsLine(stdout);
            Assert.That(stats.Added, Is.Zero, "Same XMI on both sides: no ADDED files.");
            Assert.That(stats.Changed, Is.Zero, "Same XMI on both sides: no CHANGED files.");
            Assert.That(stats.RemovedSkipped, Is.Zero, "Same XMI on both sides: no REMOVED files.");
            Assert.That(stats.UnchangedConcentrated, Is.GreaterThan(0),
                "Same XMI on both sides: every emitted file goes into the UNCHANGED-concentrated partition.");
            Assert.That(stats.CompatFilesWritten, Is.EqualTo(3),
                "Same XMI on both sides: one Compat/<label>.g.cs per library (three libraries).");
        }

        [Test]
        public void Mutated_XMI_stats_line_reports_changed_gt_zero_and_zero_added_removed()
        {
            var repoRoot = FindRepoRoot();
            var xmiPath = Path.Combine(repoRoot, XmiRelativePath);
            var scratch = InitScratch("mutated-stats");

            var originalXmi = File.ReadAllText(xmiPath);
            const string original =
                "unchangeable coordinate system that has machine zero as its origin.";
            const string mutated =
                "STATS_MUTATION_MARKER coordinate system that has machine zero as its origin.";
            Assert.That(originalXmi, Does.Contain(original),
                "XMI fixture must retain the stable mutation target; update the constant if the source moved.");

            var mutatedXmi = originalXmi.Replace(original, mutated);
            var mutatedXmiPath = Path.Combine(scratch, "MutatedSysML.xml");
            File.WriteAllText(mutatedXmiPath, mutatedXmi);

            var (exitCode, stdout, stderr) = RunDelta(mutatedXmiPath, previousXmiPath: xmiPath,
                compatLabel: "PriorSpec", output: scratch);
            Assert.That(exitCode, Is.Zero, $"Generator exited non-zero.\nstdout:\n{stdout}\nstderr:\n{stderr}");

            var stats = ParseStatsLine(stdout);
            Assert.That(stats.Changed, Is.GreaterThan(0),
                "Description-only mutation must surface at least one CHANGED file.");
            Assert.That(stats.Added, Is.Zero,
                "Description-only mutation must not surface any ADDED files (no new type names).");
            Assert.That(stats.RemovedSkipped, Is.Zero,
                "Description-only mutation must not surface any REMOVED files (no dropped type names).");
            Assert.That(stats.UnchangedConcentrated, Is.GreaterThan(0),
                "Description-only mutation must leave the majority of files UNCHANGED-concentrated.");
        }

        [Test]
        public void Default_compat_version_label_is_Previous_when_flag_omitted()
        {
            // The default value is documented in Program.cs as "Previous".
            // Verify the emitted Compat/<label>.g.cs file uses that label
            // when --compat-version-label is not passed.
            var repoRoot = FindRepoRoot();
            var xmiPath = Path.Combine(repoRoot, XmiRelativePath);
            var scratch = InitScratch("default-label");

            var (exitCode, stdout, stderr) = RunDeltaWithoutLabel(xmiPath, previousXmiPath: xmiPath,
                output: scratch);
            Assert.That(exitCode, Is.Zero, $"Generator exited non-zero.\nstdout:\n{stdout}\nstderr:\n{stderr}");

            var compatFiles = Directory
                .EnumerateFiles(scratch, "*.g.cs", SearchOption.AllDirectories)
                .Select(p => p.Replace('\\', '/'))
                .Where(p => p.Contains("/Compat/"))
                .ToList();

            Assert.That(compatFiles.Count, Is.EqualTo(3),
                "One Compat file per library (three libraries).");
            foreach (var compatFile in compatFiles)
                Assert.That(compatFile, Does.EndWith("/Compat/Previous.g.cs"),
                    "When --compat-version-label is omitted, the file name must default to 'Previous.g.cs'.");
        }

        [Test]
        public void Compat_file_header_carries_copyright_and_licence_and_plan_D4_summary()
        {
            var repoRoot = FindRepoRoot();
            var xmiPath = Path.Combine(repoRoot, XmiRelativePath);
            var scratch = InitScratch("compat-header");

            var (exitCode, stdout, stderr) = RunDelta(xmiPath, previousXmiPath: xmiPath,
                compatLabel: "HeaderCheck", output: scratch);
            Assert.That(exitCode, Is.Zero, $"Generator exited non-zero.\nstdout:\n{stdout}\nstderr:\n{stderr}");

            var compatFile = Directory
                .EnumerateFiles(scratch, "HeaderCheck.g.cs", SearchOption.AllDirectories)
                .FirstOrDefault();
            Assert.That(compatFile, Is.Not.Null,
                "At least one Compat/HeaderCheck.g.cs should be present after the delta emission.");

            var body = File.ReadAllText(compatFile!);
            Assert.That(body, Does.Contain("// Copyright (c)"),
                "Compat file must open with the TrakHound copyright header.");
            Assert.That(body, Does.Contain("TrakHound Inc. licenses this file to you under the MIT license."),
                "Compat file must carry the MIT licence banner.");
            Assert.That(body, Does.Contain("plan D4"),
                "Compat file must reference plan D4 in its provenance comment.");
            Assert.That(body, Does.Contain("Byte-identical to the"),
                "Compat file must promise byte-identical re-emission for concentrated types.");
        }

        [Test]
        public void Compat_file_body_carries_from_divider_per_concentrated_entry()
        {
            var repoRoot = FindRepoRoot();
            var xmiPath = Path.Combine(repoRoot, XmiRelativePath);
            var scratch = InitScratch("compat-dividers");

            var (exitCode, stdout, stderr) = RunDelta(xmiPath, previousXmiPath: xmiPath,
                compatLabel: "Divider", output: scratch);
            Assert.That(exitCode, Is.Zero, $"Generator exited non-zero.\nstdout:\n{stdout}\nstderr:\n{stderr}");

            var compatFiles = Directory
                .EnumerateFiles(scratch, "Divider.g.cs", SearchOption.AllDirectories)
                .ToList();
            Assert.That(compatFiles, Is.Not.Empty, "At least one Compat/Divider.g.cs must exist.");

            foreach (var compatFile in compatFiles)
            {
                var body = File.ReadAllText(compatFile);
                var dividers = Regex.Matches(body, @"^// --- from .+ ---$", RegexOptions.Multiline).Count;
                Assert.That(dividers, Is.GreaterThan(0),
                    $"Compat file {compatFile} must have at least one `// --- from <relative-path> ---` divider.");
            }
        }

        [Test]
        public void Compat_file_body_preserves_multiple_namespace_blocks()
        {
            var repoRoot = FindRepoRoot();
            var xmiPath = Path.Combine(repoRoot, XmiRelativePath);
            var scratch = InitScratch("compat-namespaces");

            var (exitCode, stdout, stderr) = RunDelta(xmiPath, previousXmiPath: xmiPath,
                compatLabel: "Namespaces", output: scratch);
            Assert.That(exitCode, Is.Zero, $"Generator exited non-zero.\nstdout:\n{stdout}\nstderr:\n{stderr}");

            // MTConnect.NET-Common carries the highest namespace diversity;
            // its Compat file must retain multiple `namespace X` blocks.
            var commonCompat = Directory
                .EnumerateFiles(Path.Combine(scratch, "libraries", "MTConnect.NET-Common"),
                    "Namespaces.g.cs", SearchOption.AllDirectories)
                .FirstOrDefault();
            Assert.That(commonCompat, Is.Not.Null,
                "MTConnect.NET-Common/Compat/Namespaces.g.cs must exist.");

            var body = File.ReadAllText(commonCompat!);
            var namespaceMatches = Regex.Matches(body, @"^\s*namespace\s+MTConnect", RegexOptions.Multiline).Count;
            Assert.That(namespaceMatches, Is.GreaterThan(1),
                "Multi-namespace concentration is the whole point of plan D4's Compat design; "
                + "a single-namespace Compat body signals the concatenation has collapsed the "
                + "source-file boundaries.");
        }

        [Test]
        public void Stats_line_uses_the_exact_documented_key_ordering_and_syntax()
        {
            var repoRoot = FindRepoRoot();
            var xmiPath = Path.Combine(repoRoot, XmiRelativePath);
            var scratch = InitScratch("stats-syntax");

            var (exitCode, stdout, _) = RunDelta(xmiPath, previousXmiPath: xmiPath,
                compatLabel: "Syntax", output: scratch);
            Assert.That(exitCode, Is.Zero, "Same XMI on both sides is a valid delta invocation.");

            // Exact regex so a rename of a counter key trips this test loudly.
            var pattern = new Regex(
                @"Delta emission: added=\d+, changed=\d+, unchanged-concentrated=\d+, "
                + @"removed-skipped=\d+, compat-files-written=\d+");
            Assert.That(pattern.IsMatch(stdout), Is.True,
                "stdout must carry the operator-facing stats line in its documented shape. "
                + "Renaming any counter key breaks the operator's grep contract.");
        }

        // --- helpers -----------------------------------------------------

        private sealed class StatsLine
        {
            public int Added;
            public int Changed;
            public int UnchangedConcentrated;
            public int RemovedSkipped;
            public int CompatFilesWritten;
        }

        private static StatsLine ParseStatsLine(string stdout)
        {
            var match = Regex.Match(stdout,
                @"Delta emission: added=(?<a>\d+), changed=(?<c>\d+), "
                + @"unchanged-concentrated=(?<u>\d+), removed-skipped=(?<r>\d+), "
                + @"compat-files-written=(?<w>\d+)");
            if (!match.Success)
                throw new AssertionException(
                    "stdout does not carry the expected 'Delta emission: ...' stats line.\n" + stdout);
            return new StatsLine
            {
                Added = int.Parse(match.Groups["a"].Value),
                Changed = int.Parse(match.Groups["c"].Value),
                UnchangedConcentrated = int.Parse(match.Groups["u"].Value),
                RemovedSkipped = int.Parse(match.Groups["r"].Value),
                CompatFilesWritten = int.Parse(match.Groups["w"].Value)
            };
        }

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

        private static string InitScratch(string suffix)
        {
            var repoRoot = FindRepoRoot();
            var path = Path.Combine(repoRoot, ScratchRoot, suffix);
            if (Directory.Exists(path))
                Directory.Delete(path, recursive: true);
            Directory.CreateDirectory(path);
            Directory.CreateDirectory(Path.Combine(path, "libraries", "MTConnect.NET-Common"));
            Directory.CreateDirectory(Path.Combine(path, "libraries", "MTConnect.NET-JSON-cppagent"));
            Directory.CreateDirectory(Path.Combine(path, "libraries", "MTConnect.NET-XML"));
            return path;
        }

        private static (int ExitCode, string Stdout, string Stderr) RunDelta(
            string xmiPath, string previousXmiPath, string compatLabel, string output)
        {
            var repoRoot = FindRepoRoot();
            var psi = BuildStartInfo(repoRoot);
            psi.ArgumentList.Add("run");
            psi.ArgumentList.Add("--no-build");
            psi.ArgumentList.Add("--project");
            psi.ArgumentList.Add(GeneratorProject);
            psi.ArgumentList.Add("--");
            psi.ArgumentList.Add("--xmi");
            psi.ArgumentList.Add(xmiPath);
            psi.ArgumentList.Add("--previous-xmi");
            psi.ArgumentList.Add(previousXmiPath);
            psi.ArgumentList.Add("--compat-version-label");
            psi.ArgumentList.Add(compatLabel);
            psi.ArgumentList.Add("--output");
            psi.ArgumentList.Add(output);
            return Execute(psi);
        }

        private static (int ExitCode, string Stdout, string Stderr) RunDeltaWithoutLabel(
            string xmiPath, string previousXmiPath, string output)
        {
            var repoRoot = FindRepoRoot();
            var psi = BuildStartInfo(repoRoot);
            psi.ArgumentList.Add("run");
            psi.ArgumentList.Add("--no-build");
            psi.ArgumentList.Add("--project");
            psi.ArgumentList.Add(GeneratorProject);
            psi.ArgumentList.Add("--");
            psi.ArgumentList.Add("--xmi");
            psi.ArgumentList.Add(xmiPath);
            psi.ArgumentList.Add("--previous-xmi");
            psi.ArgumentList.Add(previousXmiPath);
            psi.ArgumentList.Add("--output");
            psi.ArgumentList.Add(output);
            return Execute(psi);
        }

        private static ProcessStartInfo BuildStartInfo(string repoRoot) => new("dotnet")
        {
            WorkingDirectory = repoRoot,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        private static (int ExitCode, string Stdout, string Stderr) Execute(ProcessStartInfo psi)
        {
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
