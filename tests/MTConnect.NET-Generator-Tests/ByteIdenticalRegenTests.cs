// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using NUnit.Framework;

namespace MTConnect.NET_Generator_Tests
{
    /// <summary>
    /// Byte-identical regeneration guards for the SysML importer.
    ///
    /// Two guards land side by side:
    ///
    /// <list type="bullet">
    ///   <item><see cref="Regen_is_deterministic_across_two_invocations"/> —
    ///     the everyday guard. Regenerates the tree twice against the same
    ///     XMI and asserts the two emitted trees are byte-identical. This
    ///     locks in the determinism guarantee the template consolidations
    ///     in Phase 3 rely on: any consolidation that alters emission
    ///     behavior flips this test RED regardless of whether the
    ///     committed <c>libraries/**/*.g.cs</c> tree is currently in sync
    ///     with the generator.</item>
    ///   <item><see cref="Current_XMI_regen_matches_committed_g_cs_tree"/> —
    ///     the strict baseline guard. Diffs a fresh regen against the
    ///     committed tree and fails on any drift. The Phase 3.1 dry-run
    ///     on 2026-08-20 surfaced a 78-file drift (15 committed
    ///     <c>.g.cs</c> files the generator no longer emits + 63 files
    ///     with content drift); Phase 4.1 resolved every case in the
    ///     preceding commit train (10 missing Pallet measurement
    ///     interfaces routed through a new template + <c>MeasurementModel
    ///     .RenderInterface()</c> wire-up, 5 orphaned <c>.g.cs</c> files
    ///     deleted after a codebase-wide grep confirmed zero consumers,
    ///     63 whitespace-drift files refreshed to current-generator
    ///     output). The guard now runs on every CI test sweep.</item>
    /// </list>
    ///
    /// Scope decision (ottobolyos 2026-08-20): current-XMI only. The
    /// <c>build/sysml-model</c> submodule ships one snapshot per MTConnect
    /// Standard version bump; iterating over historical XMI tags is not part
    /// of the Phase 3 scope. When a new spec version lands, a sibling
    /// byte-identical guard commit adds coverage for that version's XMI.
    /// </summary>
    [TestFixture]
    public class ByteIdenticalRegenTests
    {
        // Well-known repo-relative paths. Discovery walks up from the test
        // assembly's base directory to the repo root (the first ancestor
        // that contains MTConnect.NET.sln).
        private const string SlnFileName = "MTConnect.NET.sln";
        private const string GeneratorProject = "build/MTConnect.NET-SysML-Import";
        private const string XmiRelativePath = "build/sysml-model/MTConnectSysMLModel.xml";
        private const string GenScratchDirPrimary = ".claude/gen-test-out/byte-identical";
        private const string GenScratchDirSecondary = ".claude/gen-test-out/byte-identical-2";

        [Test]
        public void Regen_is_deterministic_across_two_invocations()
        {
            var repoRoot = FindRepoRoot();
            var xmiPath = Path.Combine(repoRoot, XmiRelativePath);
            Assert.That(File.Exists(xmiPath), Is.True,
                $"XMI snapshot missing at {xmiPath}. Is the build/sysml-model submodule initialized?");

            var scratchA = Path.Combine(repoRoot, GenScratchDirPrimary);
            var scratchB = Path.Combine(repoRoot, GenScratchDirSecondary);
            InitScratch(scratchA);
            InitScratch(scratchB);

            RunGenerator(repoRoot, xmiPath, scratchA);
            RunGenerator(repoRoot, xmiPath, scratchB);

            var hashesA = HashGeneratedTree(Path.Combine(scratchA, "libraries"));
            var hashesB = HashGeneratedTree(Path.Combine(scratchB, "libraries"));

            var diff = CompareTrees(hashesA, hashesB);
            Assert.That(diff.Length, Is.Zero,
                "Regenerator is NOT deterministic: two back-to-back invocations against " +
                "the same XMI produced different .g.cs trees. Any Phase 3 template " +
                "consolidation that changes the emission surface would flip this test RED.\n\n" +
                diff);
        }

        [Test]
        public void Current_XMI_regen_matches_committed_g_cs_tree()
        {
            var repoRoot = FindRepoRoot();
            var xmiPath = Path.Combine(repoRoot, XmiRelativePath);
            Assert.That(File.Exists(xmiPath), Is.True,
                $"XMI snapshot missing at {xmiPath}. Is the build/sysml-model submodule initialized?");

            var scratchRoot = Path.Combine(repoRoot, GenScratchDirPrimary);
            InitScratch(scratchRoot);
            RunGenerator(repoRoot, xmiPath, scratchRoot);

            var emitted = HashGeneratedTree(Path.Combine(scratchRoot, "libraries"));
            var committed = HashGeneratedTree(Path.Combine(repoRoot, "libraries"));

            var diff = CompareTrees(committed, emitted, leftLabel: "committed", rightLabel: "regenerated");
            Assert.That(diff.Length, Is.Zero,
                "Regeneration is not byte-identical to the committed .g.cs tree. Either " +
                "the templates changed emission behavior, the parser drifted, or the " +
                "committed generated files were hand-edited.\n\n" + diff);
        }

        // --- helpers -----------------------------------------------------

        // Locates the repo root by walking up from the test assembly's base
        // directory until a directory containing MTConnect.NET.sln is found.
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

        // Wipes the target directory, then scaffolds the three library
        // subdirectories the generator's Program.cs guards its renderer
        // entry points on (fail-fast against pointing --output at the
        // wrong tree). The generator populates only .g.cs files inside
        // these subtrees; hand-authored .cs files live alongside but are
        // never emitted, so the scaffolding stays empty.
        private static void InitScratch(string path)
        {
            if (Directory.Exists(path))
                Directory.Delete(path, recursive: true);
            Directory.CreateDirectory(path);
            Directory.CreateDirectory(Path.Combine(path, "libraries", "MTConnect.NET-Common"));
            Directory.CreateDirectory(Path.Combine(path, "libraries", "MTConnect.NET-JSON-cppagent"));
            Directory.CreateDirectory(Path.Combine(path, "libraries", "MTConnect.NET-XML"));
        }

        // Invokes the generator via `dotnet run --no-build --project <path>
        // -- --xmi <xmi> --output <scratch>`. The generator project is
        // wired as a ProjectReference on this test csproj so MSBuild
        // builds it ahead of the test run; --no-build keeps the invocation
        // cheap. Non-zero exit fires the caller with the full stdout /
        // stderr in the exception.
        private static void RunGenerator(string repoRoot, string xmiPath, string scratchRoot)
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
            psi.ArgumentList.Add("--output");
            psi.ArgumentList.Add(scratchRoot);
            // --full-tree pins the byte-identical guard to the full-regeneration
            // path. Without it the zero-config auto-derive (task #408) would
            // kick in against the scratch dir, which lacks
            // libraries/MTConnect.NET-Common/MTConnectVersions.cs, and abort
            // with a PREV_VERSION resolver error before any templates render.
            psi.ArgumentList.Add("--full-tree");

            using var proc = Process.Start(psi)
                ?? throw new InvalidOperationException("Failed to start dotnet run for the generator.");

            // Drain stdout AND stderr concurrently. Blocking on ReadToEnd() for
            // one pipe while the child writes >4 KB to the other deadlocks
            // (Linux pipe buffer fills, child blocks on write, parent blocks on
            // read of the empty pipe). Task.WhenAll on the two async reads and
            // WaitForExitAsync side-steps the deadlock entirely.
            var stdoutTask = proc.StandardOutput.ReadToEndAsync();
            var stderrTask = proc.StandardError.ReadToEndAsync();
            System.Threading.Tasks.Task.WhenAll(stdoutTask, stderrTask).GetAwaiter().GetResult();
            proc.WaitForExit();
            var stdout = stdoutTask.Result;
            var stderr = stderrTask.Result;

            if (proc.ExitCode != 0)
            {
                throw new InvalidOperationException(
                    $"Generator exited with code {proc.ExitCode}.\n" +
                    $"stdout:\n{stdout}\n" +
                    $"stderr:\n{stderr}");
            }
        }

        // Walks the tree, hashing every .g.cs file. Returns a dictionary
        // keyed by the path relative to <root> (forward-slash normalised)
        // with the SHA-256 hash of the file's byte content as value.
        //
        // MSBuild-generated intermediates under bin/ and obj/ (a library's
        // GlobalUsings.g.cs from Microsoft.NET.Sdk.CSharp.CoreCompile.targets,
        // ImplicitNamespaceImports.g.cs, etc.) are skipped — the generator
        // never touches them, and their presence would spuriously flip this
        // test RED on any host that has already built the solution.
        private static Dictionary<string, byte[]> HashGeneratedTree(string root)
        {
            var result = new Dictionary<string, byte[]>(StringComparer.Ordinal);
            if (!Directory.Exists(root))
                return result;

            using var sha = SHA256.Create();
            foreach (var file in Directory.EnumerateFiles(root, "*.g.cs", SearchOption.AllDirectories))
            {
                var relative = Path.GetRelativePath(root, file).Replace('\\', '/');
                if (relative.Contains("/bin/") || relative.Contains("/obj/") ||
                    relative.StartsWith("bin/") || relative.StartsWith("obj/"))
                    continue;
                using var stream = File.OpenRead(file);
                result[relative] = sha.ComputeHash(stream);
            }
            return result;
        }

        // Emits a human-readable diff report between two path -> hash
        // dictionaries. Returns an empty string when the two are identical.
        private static string CompareTrees(
            Dictionary<string, byte[]> left,
            Dictionary<string, byte[]> right,
            string leftLabel = "expected",
            string rightLabel = "actual")
        {
            var onlyLeft = left.Keys.Except(right.Keys).OrderBy(k => k).ToList();
            var onlyRight = right.Keys.Except(left.Keys).OrderBy(k => k).ToList();
            var mismatched = left.Keys.Intersect(right.Keys)
                .Where(k => !left[k].SequenceEqual(right[k]))
                .OrderBy(k => k)
                .ToList();

            var report = new StringBuilder();
            AppendListing(report, $"Missing in {rightLabel} (present in {leftLabel})", onlyLeft);
            AppendListing(report, $"Extra in {rightLabel} (absent from {leftLabel})", onlyRight);
            AppendListing(report, "Content mismatch", mismatched);
            return report.ToString();
        }

        private static void AppendListing(StringBuilder sink, string heading, List<string> entries)
        {
            if (entries.Count == 0)
                return;

            sink.AppendLine($"{heading} ({entries.Count} files):");
            foreach (var path in entries.Take(20))
                sink.AppendLine($"  {path}");
            if (entries.Count > 20)
                sink.AppendLine($"  ... and {entries.Count - 20} more");
        }
    }
}
