// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace MTConnect.NET_Generator_Tests
{
    /// <summary>
    /// Guards against the JSON-cppagent generator emitting references to
    /// <c>Component</c> / <c>DataItem</c> types the SysML model marks
    /// deprecated.
    ///
    /// PR #233 taught <c>CSharpTemplateRenderer</c> to stamp
    /// <c>[System.Obsolete("Deprecated in vX.Y")]</c> onto every
    /// <c>MTConnect.NET-Common</c> class whose SysML model entry carries a
    /// non-empty <c>Deprecated</c> literal (see
    /// <c>Devices.ComponentType.scriban</c> / <c>Devices.DataItemType.scriban</c>,
    /// guarded by <c>{{- if (deprecated) }}</c>). The JSON-cppagent
    /// generator (<c>JsonCppAgentTemplateRenderer.WriteComponents/
    /// WriteEvents/WriteSamples</c>) was never taught the same rule — it
    /// emits one property + one <c>{Type}.TypeId</c> reference per type
    /// unconditionally, so every deprecated type re-surfaces as a CS0618
    /// reference to an <c>[Obsolete]</c> member. Under this repository's
    /// <c>TreatWarningsAsErrors=true</c> baseline (PR #219) that is 176
    /// build errors across <c>Devices/JsonComponents.g.cs</c>,
    /// <c>Streams/JsonEvents.g.cs</c>, and <c>Streams/JsonSamples.g.cs</c>.
    ///
    /// This test regenerates both trees from the same XMI into an isolated
    /// scratch directory, collects every Common class name the generator
    /// stamped <c>[System.Obsolete]</c> onto, and asserts none of those
    /// names appear as a type reference (<c>{Name}.TypeId</c> or a bare
    /// <c>{Name}</c> token) anywhere in the regenerated JSON-cppagent
    /// <c>.g.cs</c> files. It is deliberately independent of whether the
    /// committed tree happens to match the regenerated one — that is
    /// <see cref="ByteIdenticalRegenTests.Current_XMI_regen_matches_committed_g_cs_tree"/>'s
    /// job — so it stays a direct pin on the "no obsolete references"
    /// contract even if a future template change alters unrelated emission
    /// details.
    /// </summary>
    [TestFixture]
    public class JsonCppagentObsoleteReferenceGuardTests
    {
        private const string SlnFileName = "MTConnect.NET.sln";
        private const string GeneratorProject = "build/MTConnect.NET-SysML-Import";
        private const string XmiRelativePath = "build/sysml-model/MTConnectSysMLModel.xml";
        private const string GenScratchDir = ".claude/gen-test-out/obsolete-reference-guard";

        // Matches a Common `.g.cs` class declaration stamped with
        // [System.Obsolete(...)] by the CSharp generator's
        // `{{- if (deprecated) }}` block. The attribute always renders on
        // the line immediately above the `public [abstract] class Name`
        // declaration (Devices.ComponentType.scriban /
        // Devices.DataItemType.scriban); Singleline lets '.' cross the
        // intervening newline while the [^\r\n]* attribute-argument capture
        // stays confined to its own line.
        private static readonly Regex ObsoleteClassPattern = new(
            @"\[System\.Obsolete\([^\r\n]*\)\]\s*\r?\n\s*public\s+(?:abstract\s+)?class\s+(?<name>\w+)",
            RegexOptions.Compiled | RegexOptions.Singleline);

        [Test]
        public void JsonCppagent_regen_emits_no_references_to_obsolete_types()
        {
            var repoRoot = FindRepoRoot();
            var xmiPath = Path.Combine(repoRoot, XmiRelativePath);
            Assert.That(File.Exists(xmiPath), Is.True,
                $"XMI snapshot missing at {xmiPath}. Is the build/sysml-model submodule initialized?");

            var scratchRoot = Path.Combine(repoRoot, GenScratchDir);
            InitScratch(scratchRoot);
            RunGenerator(repoRoot, xmiPath, scratchRoot);

            var commonRoot = Path.Combine(scratchRoot, "libraries", "MTConnect.NET-Common");
            var jsonCppagentRoot = Path.Combine(scratchRoot, "libraries", "MTConnect.NET-JSON-cppagent");

            var obsoleteTypeNames = CollectObsoleteTypeNames(commonRoot);
            Assert.That(obsoleteTypeNames, Is.Not.Empty,
                "Expected at least one [System.Obsolete] Common class from the current XMI " +
                "(e.g. PowerComponent, AmperageDataItem) — found none. Either the XMI changed " +
                "or ObsoleteClassPattern no longer matches the generator's emission shape.");

            var violations = FindObsoleteReferences(jsonCppagentRoot, obsoleteTypeNames);

            Assert.That(violations, Is.Empty,
                $"JSON-cppagent regen references {violations.Count} obsolete Common type(s), " +
                "which becomes a CS0618 build error under TreatWarningsAsErrors=true:\n\n" +
                string.Join("\n", violations.Take(30)) +
                (violations.Count > 30 ? $"\n... and {violations.Count - 30} more" : ""));
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
            // --full-tree: same rationale as ByteIdenticalRegenTests — the
            // scratch dir lacks MTConnectVersions.cs, so zero-config delta
            // mode would abort before any templates render.
            psi.ArgumentList.Add("--full-tree");

            using var proc = Process.Start(psi)
                ?? throw new InvalidOperationException("Failed to start dotnet run for the generator.");

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

        // Scans every Common .g.cs file for [System.Obsolete] class
        // declarations and returns the set of emitted class names
        // (PowerComponent, AmperageDataItem, ...).
        private static HashSet<string> CollectObsoleteTypeNames(string commonRoot)
        {
            var names = new HashSet<string>(StringComparer.Ordinal);
            if (!Directory.Exists(commonRoot))
                return names;

            foreach (var file in Directory.EnumerateFiles(commonRoot, "*.g.cs", SearchOption.AllDirectories))
            {
                var content = File.ReadAllText(file);
                foreach (Match match in ObsoleteClassPattern.Matches(content))
                {
                    names.Add(match.Groups["name"].Value);
                }
            }
            return names;
        }

        // Scans every JSON-cppagent .g.cs file for a whole-word reference
        // to any name in `obsoleteTypeNames`. Word-boundary matching avoids
        // false positives from names that are a substring of another
        // identifier (e.g. `Power` inside `PowerStatusDataItem` would not
        // spuriously match a bare `Power` search were one ever added).
        private static List<string> FindObsoleteReferences(string jsonCppagentRoot, HashSet<string> obsoleteTypeNames)
        {
            var violations = new List<string>();
            if (!Directory.Exists(jsonCppagentRoot))
                return violations;

            foreach (var file in Directory.EnumerateFiles(jsonCppagentRoot, "*.g.cs", SearchOption.AllDirectories))
            {
                var lines = File.ReadAllLines(file);
                for (var lineNumber = 0; lineNumber < lines.Length; lineNumber++)
                {
                    var line = lines[lineNumber];
                    foreach (var obsoleteName in obsoleteTypeNames)
                    {
                        if (Regex.IsMatch(line, $@"\b{Regex.Escape(obsoleteName)}\b"))
                        {
                            violations.Add($"{Path.GetFileName(file)}:{lineNumber + 1}: references '{obsoleteName}' -> {line.Trim()}");
                        }
                    }
                }
            }
            return violations;
        }
    }
}
