// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace MTConnect.NET_Generator_Tests
{
    /// <summary>
    /// Cross-verification of the delta-driven regen mode (Phase 4.3).
    ///
    /// <para>
    /// The delta mode is opt-in via <c>--previous-xmi &lt;path&gt;</c>. When
    /// supplied, the generator renders both XMIs to isolated scratch
    /// directories, diffs the emitted trees at the file level, and emits only
    /// the changed / added files to <c>--output</c>, concentrating unchanged
    /// files into a single <c>Compat/&lt;label&gt;.g.cs</c> per library
    /// (plan D4).
    /// </para>
    ///
    /// <para>
    /// Historical-XMI iteration is out of scope (per ottobolyos 2026-08-20 —
    /// the <c>build/sysml-model</c> submodule ships a single snapshot per
    /// spec bump). This fixture instead verifies the delta mechanism against
    /// a DELIBERATELY-MUTATED copy of the current XMI: the mutation flips
    /// one description string and asserts the resulting delta captures the
    /// flip surgically (one changed file, the rest of the tree concentrated
    /// into Compat).
    /// </para>
    /// </summary>
    [TestFixture]
    public class DeltaRegenTests
    {
        private const string SlnFileName = "MTConnect.NET.sln";
        private const string GeneratorProject = "build/MTConnect.NET-SysML-Import";
        private const string XmiRelativePath = "build/sysml-model/MTConnectSysMLModel.xml";
        private const string GenScratchDir = ".claude/gen-test-out/delta-mutated";

        // The mutation target is the MACHINE literal's ownedComment body inside
        // the CoordinateSystemEnum enumeration (XMI element id
        // _19_0_3_68e0225_1597921579016_122540_182). The comment body flows
        // into TWO emitted artefacts: the enum's own doc-comment
        // (libraries/MTConnect.NET-Common/Devices/DataItemCoordinateSystem.g.cs)
        // and the sibling Descriptions const table
        // (libraries/MTConnect.NET-Common/Devices/DataItemCoordinateSystemDescriptions.g.cs).
        // Both should surface as CHANGED; every other file in the tree stays
        // UNCHANGED and gets concentrated into the Compat file.
        private const string OriginalDescriptionFragment =
            "unchangeable coordinate system that has machine zero as its origin.";

        private const string MutatedDescriptionFragment =
            "MUTATED_DELTA_MARKER coordinate system that has machine zero as its origin.";

        // The emitted files the mutation should surface as CHANGED.
        private static readonly string[] ExpectedChangedFiles = new[]
        {
            "libraries/MTConnect.NET-Common/Devices/DataItemCoordinateSystem.g.cs",
            "libraries/MTConnect.NET-Common/Devices/DataItemCoordinateSystemDescriptions.g.cs",
        };

        [Test]
        public void Delta_mode_against_same_XMI_concentrates_every_file_into_Compat()
        {
            var repoRoot = FindRepoRoot();
            var xmiPath = Path.Combine(repoRoot, XmiRelativePath);
            Assert.That(File.Exists(xmiPath), Is.True,
                $"XMI snapshot missing at {xmiPath}. Is the build/sysml-model submodule initialized?");

            var scratchRoot = Path.Combine(repoRoot, GenScratchDir, "same");
            InitScratch(scratchRoot);

            // Same XMI on both sides — every emitted file is UNCHANGED, so the
            // whole tree ends up concentrated into one Compat/<label>.g.cs per
            // library. Zero individually-emitted .g.cs files.
            var (exitCode, stdout, _) = RunGenerator(repoRoot, xmiPath, previousXmiPath: xmiPath,
                compatLabel: "Baseline", output: scratchRoot);

            Assert.That(exitCode, Is.Zero, $"Generator exited non-zero. stdout:\n{stdout}");

            var individualFiles = EnumerateGeneratedFiles(scratchRoot)
                .Where(f => !f.Contains("/Compat/"))
                .ToList();
            var compatFiles = EnumerateGeneratedFiles(scratchRoot)
                .Where(f => f.Contains("/Compat/"))
                .ToList();

            Assert.That(individualFiles, Is.Empty,
                "Same XMI on both sides should produce zero individual .g.cs files (all types unchanged).\n" +
                "Unexpected individual files:\n  " + string.Join("\n  ", individualFiles.Take(20)));
            Assert.That(compatFiles.Count, Is.EqualTo(3),
                "Same XMI on both sides should produce exactly three Compat files (one per library).\n" +
                "Actual files: " + string.Join(", ", compatFiles));
            foreach (var compatFile in compatFiles)
                Assert.That(compatFile, Does.EndWith("/Compat/Baseline.g.cs"),
                    "Compat file name should honor --compat-version-label.");
        }

        [Test]
        public void Delta_mode_against_mutated_XMI_emits_only_the_changed_file()
        {
            var repoRoot = FindRepoRoot();
            var xmiPath = Path.Combine(repoRoot, XmiRelativePath);
            Assert.That(File.Exists(xmiPath), Is.True,
                $"XMI snapshot missing at {xmiPath}. Is the build/sysml-model submodule initialized?");

            var scratchRoot = Path.Combine(repoRoot, GenScratchDir, "mutated");
            InitScratch(scratchRoot);

            // Prepare the mutated XMI as a sibling of the original in the scratch
            // dir. Reading + string-replace keeps the mutation atomic and easy to
            // audit — the marker is grep-visible in the emitted output on flip.
            var originalXmi = File.ReadAllText(xmiPath);
            Assert.That(originalXmi, Does.Contain(OriginalDescriptionFragment),
                $"XMI fixture lost the mutation target `{OriginalDescriptionFragment}`. " +
                "The mutated-XMI cross-verify needs a stable target line; update the " +
                "OriginalDescriptionFragment constant if the source moved.");

            var mutatedXmi = originalXmi.Replace(OriginalDescriptionFragment, MutatedDescriptionFragment);
            Assert.That(mutatedXmi, Is.Not.EqualTo(originalXmi),
                "String replace produced identical content; mutation would be a no-op.");

            var mutatedXmiPath = Path.Combine(scratchRoot, "MutatedSysML.xml");
            File.WriteAllText(mutatedXmiPath, mutatedXmi);

            // Delta emission: --previous-xmi is the original, --xmi is the mutated
            // copy. Expected: one changed file (the CoordinateSystem descriptions),
            // every other file concentrated into Compat.
            var (exitCode, stdout, stderr) = RunGenerator(repoRoot, mutatedXmiPath, previousXmiPath: xmiPath,
                compatLabel: "PriorSpec", output: scratchRoot);

            Assert.That(exitCode, Is.Zero,
                $"Generator exited non-zero.\nstdout:\n{stdout}\nstderr:\n{stderr}");

            var allFiles = EnumerateGeneratedFiles(scratchRoot).ToList();
            var individualFiles = allFiles.Where(f => !f.Contains("/Compat/")).ToList();
            var compatFiles = allFiles.Where(f => f.Contains("/Compat/")).ToList();

            Assert.That(compatFiles.Count, Is.EqualTo(3),
                "One Compat file per library is expected regardless of the mutation shape.\n" +
                "Actual Compat files: " + string.Join(", ", compatFiles));

            // The mutation should surface EXACTLY the expected changed files
            // (the CoordinateSystem enum's main .g.cs + its Descriptions .g.cs).
            // Any other individual emission implies the mutation propagated
            // further than expected (a signal for the fidelity audit, not a
            // test failure per se, but the tight assertion is what makes this
            // test surgical).
            var normalisedIndividuals = individualFiles.Select(f => f.Replace('\\', '/')).OrderBy(f => f, StringComparer.Ordinal).ToArray();
            Assert.That(normalisedIndividuals, Is.EqualTo(ExpectedChangedFiles),
                "The mutation is expected to surface exactly two CHANGED files " +
                "(the enum + its Descriptions). A different set means the delta " +
                "classifier drifted, or the mutation propagated further than the " +
                "test contract expects.");

            // Cross-check: each expected changed file's body contains the mutation
            // marker, and the Compat files do not.
            foreach (var relative in normalisedIndividuals)
            {
                var changedContent = File.ReadAllText(Path.Combine(scratchRoot, relative));
                Assert.That(changedContent, Does.Contain("MUTATED_DELTA_MARKER"),
                    $"The emitted changed file {relative} must carry the mutation marker.");
            }

            foreach (var compatFile in compatFiles)
            {
                var compatContent = File.ReadAllText(Path.Combine(scratchRoot, compatFile));
                Assert.That(compatContent, Does.Not.Contain("MUTATED_DELTA_MARKER"),
                    $"Compat file {compatFile} must not carry the mutation marker (it belongs " +
                    "to the CHANGED partition, not the UNCHANGED-concentrated partition).");
            }
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
                $"Could not locate {SlnFileName} in any ancestor of {AppContext.BaseDirectory}. " +
                "The test must run from within the MTConnect.NET repository.");
        }

        private static void InitScratch(string path)
        {
            if (Directory.Exists(path))
                Directory.Delete(path, recursive: true);
            Directory.CreateDirectory(path);
            Directory.CreateDirectory(Path.Combine(path, "libraries", "MTConnect.NET-Common"));
            Directory.CreateDirectory(Path.Combine(path, "libraries", "MTConnect.NET-JSON-cppagent"));
            Directory.CreateDirectory(Path.Combine(path, "libraries", "MTConnect.NET-XML"));
        }

        private static (int ExitCode, string Stdout, string Stderr) RunGenerator(
            string repoRoot, string xmiPath, string previousXmiPath, string compatLabel, string output)
        {
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
            psi.ArgumentList.Add("--xmi");
            psi.ArgumentList.Add(xmiPath);
            psi.ArgumentList.Add("--previous-xmi");
            psi.ArgumentList.Add(previousXmiPath);
            psi.ArgumentList.Add("--compat-version-label");
            psi.ArgumentList.Add(compatLabel);
            psi.ArgumentList.Add("--output");
            psi.ArgumentList.Add(output);

            using var proc = Process.Start(psi)
                ?? throw new InvalidOperationException("Failed to start dotnet run for the generator.");

            // Drain stdout AND stderr concurrently — blocking on one pipe while
            // the child writes >4 KB to the other deadlocks (Linux pipe buffer
            // fills, child blocks on write, parent blocks on read of the empty
            // pipe). See ByteIdenticalRegenTests for the same defense.
            var stdoutTask = proc.StandardOutput.ReadToEndAsync();
            var stderrTask = proc.StandardError.ReadToEndAsync();
            System.Threading.Tasks.Task.WhenAll(stdoutTask, stderrTask).GetAwaiter().GetResult();
            proc.WaitForExit();
            return (proc.ExitCode, stdoutTask.Result, stderrTask.Result);
        }

        // Enumerates every .g.cs under `root` and returns forward-slash-normalised
        // paths relative to `root`. Ordering is stable (Ordinal sort) so
        // failure messages are reproducible.
        private static IEnumerable<string> EnumerateGeneratedFiles(string root)
        {
            if (!Directory.Exists(root))
                return Enumerable.Empty<string>();

            return Directory.EnumerateFiles(root, "*.g.cs", SearchOption.AllDirectories)
                .Select(p => Path.GetRelativePath(root, p).Replace('\\', '/'))
                .Where(p => !p.Contains("/bin/") && !p.Contains("/obj/") &&
                            !p.StartsWith("bin/") && !p.StartsWith("obj/"))
                .OrderBy(p => p, StringComparer.Ordinal)
                .ToList();
        }
    }
}
