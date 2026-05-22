// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text.RegularExpressions;

namespace MTConnect.NET_DocsGen;

/// <summary>
/// Describes a single command-line tool the repository exposes. The
/// taxonomy is intentionally narrow:
///
///   - "shipped" CLIs ship as released binaries — the standalone
///     <c>MTConnect-Agent</c> + <c>MTConnect-Adapter</c> hosts.
///   - "contributor" CLIs live under <c>tools/</c> and the
///     <c>build/</c> tree — every contributor uses them but they are
///     never released.
/// </summary>
public sealed record CliInfo(
    string Name,                              // e.g. "mtconnect.net-agent"
    string Category,                          // "shipped" | "contributor"
    string Summary,                           // single-paragraph description
    string SourceRelativePath,                // canonical source file
    IReadOnlyList<CliCommand> Commands,
    IReadOnlyList<CliFlag> Flags,
    IReadOnlyList<CliArg> Arguments);

/// <summary>
/// A single sub-command keyword the CLI dispatches on, e.g.
/// <c>run</c> / <c>debug</c> / <c>install</c>.
/// </summary>
public sealed record CliCommand(string Name, string Description);

/// <summary>
/// A single CLI flag, with its short alias if any, accepted argument
/// shape, and human-facing description. The description is normally
/// pulled out of the script's docstring or the C# program's printed
/// help text.
/// </summary>
public sealed record CliFlag(
    string Name,                              // e.g. "--docker"
    string? Short,                            // e.g. "-d", null if none
    string? ArgShape,                         // e.g. "PATTERN", null if a switch
    string Description);

/// <summary>
/// A single positional argument.
/// </summary>
public sealed record CliArg(string Name, string Description);

/// <summary>
/// Builds the full CLI inventory. Each tool has a dedicated collector
/// because the help-text shape differs between the C# entry points
/// and the bash / PowerShell wrappers; the collectors agree only on
/// the output <see cref="CliInfo"/> record.
/// </summary>
public static class CliInventory
{
    public static IReadOnlyList<CliInfo> Collect(string repoRoot)
    {
        var results = new List<CliInfo>();

        // ---- Shipped: agent + adapter -------------------------------------
        var agentApp = Path.Combine(repoRoot, "agent", "MTConnect.NET-Applications-Agents", "MTConnectAgentApplication.cs");
        if (File.Exists(agentApp))
        {
            results.Add(CollectAgentLike(
                name: "mtconnect.net-agent",
                file: agentApp,
                repoRoot: repoRoot,
                summary: "Standalone HTTP agent host. Loads `agent.config.yaml` (or `agent.config.json`), builds an `MTConnectAgentBroker`, instantiates every configured agent module, and runs them inside a single process. The CLI surface is defined by `MTConnectAgentApplication.Run(args)`.",
                argument: new CliArg("configuration_file", "Path to the agent configuration file. Absolute or relative to the executable's directory. If omitted, the agent looks for `agent.config.json`, then `agent.config.yaml`, and copies `agent.config.default.yaml` into place if neither exists.")));
        }

        var adapterApp = Path.Combine(repoRoot, "adapter", "MTConnect.NET-Applications-Adapter", "MTConnectAdapterApplication.cs");
        if (File.Exists(adapterApp))
        {
            results.Add(CollectAgentLike(
                name: "mtconnect.net-adapter",
                file: adapterApp,
                repoRoot: repoRoot,
                summary: "Standalone adapter host. Pumps observations from a data source (SHDR socket, MQTT topic, or a custom `DataSource` implementation) into a downstream agent. Reads `adapter.config.yaml` at startup. The CLI surface is defined by `MTConnectAdapterApplication.Run(args)`.",
                argument: new CliArg("configuration_file", "Path to the adapter configuration file. Defaults to `adapter.config.yaml` next to the executable when omitted.")));
        }

        // ---- Contributor: tools/*.sh + tools/*.ps1 (paired) ---------------
        var toolsDir = Path.Combine(repoRoot, "tools");
        if (Directory.Exists(toolsDir))
        {
            foreach (var sh in Directory.EnumerateFiles(toolsDir, "*.sh", SearchOption.TopDirectoryOnly)
                         .OrderBy(p => p, StringComparer.Ordinal))
            {
                var info = CollectShellCli(sh, repoRoot);
                if (info is not null) results.Add(info);
            }
        }

        // ---- Contributor: SysML-Import + DocsGen --------------------------
        var sysmlMain = Path.Combine(repoRoot, "build", "MTConnect.NET-SysML-Import", "Program.cs");
        if (File.Exists(sysmlMain))
        {
            results.Add(CollectDotNetTool(
                name: "MTConnect.NET-SysML-Import",
                file: sysmlMain,
                repoRoot: repoRoot,
                summary: "Parses an `MTConnectSysMLModel.xml` (the XMI export of the standard's SysML model) and regenerates the `*.g.cs` source files under `libraries/MTConnect.NET-Common/`, `libraries/MTConnect.NET-XML/`, and `libraries/MTConnect.NET-JSON-cppagent/`. Run by maintainers when a new spec version lands."));
        }

        var docsGenMain = Path.Combine(repoRoot, "build", "MTConnect.NET-DocsGen", "Program.cs");
        if (File.Exists(docsGenMain))
        {
            results.Add(CollectDotNetTool(
                name: "MTConnect.NET-DocsGen",
                file: docsGenMain,
                repoRoot: repoRoot,
                summary: "Walks the repository source tree with Roslyn and emits the auto-generated VitePress reference pages under `docs/reference/`. The same inventory routines are exposed as public static methods so the validation test can re-run them independently."));
        }

        return results
            .OrderBy(c => c.Category == "shipped" ? 0 : 1)
            .ThenBy(c => c.Name, StringComparer.Ordinal)
            .ToList();
    }

    // ----- agent + adapter --------------------------------------------------

    // Matches the `Console.WriteLine(string.Format("{0,...}  |  {1,...}", "name", "desc"))`
    // pattern in the C# PrintHelp methods.
    private static readonly Regex AgentHelpRowRe = new(
        @"Console\.WriteLine\([^""]*""\{0,\d+\}\s*\|\s*\{1,\d+\}""\s*,\s*""(?<name>[^""]+)""\s*,\s*""(?<desc>[^""]+)""",
        RegexOptions.Compiled);

    private static CliInfo CollectAgentLike(string name, string file, string repoRoot, string summary, CliArg argument)
    {
        var text = File.ReadAllText(file);
        var commands = new List<CliCommand>();
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (Match m in AgentHelpRowRe.Matches(text))
        {
            var cmd = m.Groups["name"].Value;
            var desc = m.Groups["desc"].Value;
            // Defensive guards:
            //   - PrintHelp blocks include a single header row
            //     ("Command  |  Description"). Skip it.
            //   - The same PrintHelp emits the positional-argument row
            //     ("configuration_file") in the same table format. That
            //     belongs in Arguments, not Commands, and the caller already
            //     supplies a richer description for it.
            //   - Some entry points are invoked twice (Windows vs. Linux
            //     branches). Dedupe by command name.
            if (cmd.Equals("Command", StringComparison.OrdinalIgnoreCase)) continue;
            if (cmd.Equals(argument.Name, StringComparison.OrdinalIgnoreCase)) continue;
            if (!seen.Add(cmd)) continue;
            commands.Add(new CliCommand(cmd, desc));
        }

        return new CliInfo(
            Name: name,
            Category: "shipped",
            Summary: summary,
            SourceRelativePath: Path.GetRelativePath(repoRoot, file).Replace('\\', '/'),
            Commands: commands,
            Flags: Array.Empty<CliFlag>(),
            Arguments: new[] { argument });
    }

    // ----- tools/*.sh -------------------------------------------------------

    // Matches a typical bash help block "Flags:" entry of the form:
    //   -d, --docker        Description that may wrap across
    //                       multiple lines until the next flag or
    //                       a blank line.
    // The two-flag form (`-d, --docker`) and the single-flag form
    // (`--docker`) are both accepted; an optional argument shape
    // (`PATTERN`, `<path>`, `[file]`) is captured. The leading-whitespace
    // bound is capped at 6 characters so deeply-indented continuation
    // lines like `                       --docker (also honored …)` are
    // not parsed as new flag entries — they fall through to the
    // description-continuation branch below.
    private static readonly Regex ShellHelpFlagRe = new(
        @"^(?<lead>\s{1,6})(?:(?<shrt>-[a-zA-Z]),\s+)?(?<long>--[a-zA-Z][a-zA-Z0-9-]+)(?:\s+(?<arg>[A-Z][A-Z0-9_<>\[\]]*))?\s+(?<desc>\S.*)$",
        RegexOptions.Compiled);

    private static CliInfo? CollectShellCli(string file, string repoRoot)
    {
        var lines = File.ReadAllLines(file);
        var rel = Path.GetRelativePath(repoRoot, file).Replace('\\', '/');
        var name = Path.GetFileName(file);

        // Pull the leading docstring (the first block of `#` lines after
        // the shebang) as the summary.
        var summary = ExtractShellSummary(lines);

        // Walk the file for the help-block "Flags:" section. Only the
        // FIRST "Flags:" block is consumed — typical scripts repeat the
        // same content in both their docstring (leading `#` block) and
        // their print_help heredoc, and we only want one row per flag.
        var flags = new List<CliFlag>();
        bool inFlags = false;
        bool flagsBlockDone = false;
        CliFlag? pending = null;
        var descBuf = new System.Text.StringBuilder();

        void Flush()
        {
            if (pending is null) return;
            var desc = descBuf.ToString().Trim();
            flags.Add(pending with { Description = desc });
            pending = null;
            descBuf.Clear();
        }

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            var trimmed = line.TrimEnd();

            // Enter the "Flags:" block — but only once. After the first
            // block closes, the second occurrence (in print_help) is
            // ignored so the row set isn't duplicated.
            if (Regex.IsMatch(trimmed, @"^\s*#?\s*Flags:\s*$"))
            {
                if (flagsBlockDone) continue;
                inFlags = true;
                continue;
            }
            if (!inFlags) continue;

            // Exit when we hit a blank line, a `set -` line, or the
            // closing heredoc delimiter.
            if (trimmed.Length == 0
                || Regex.IsMatch(trimmed, @"^\s*[A-Z]+$")          // EOF / HEREDOC delim
                || Regex.IsMatch(trimmed, @"^\s*set\s+-")
                || Regex.IsMatch(trimmed, @"^\s*#?\s*[A-Z][A-Za-z\s]+:\s*$"))  // next "Usage:" / etc.
            {
                Flush();
                inFlags = false;
                flagsBlockDone = true;
                continue;
            }

            // Strip a leading `#` (for files where the help block lives
            // in the doc-comment block, not the heredoc).
            var content = line.StartsWith('#') ? line.Substring(1) : line;

            var m = ShellHelpFlagRe.Match(content);
            if (m.Success)
            {
                Flush();
                pending = new CliFlag(
                    Name: m.Groups["long"].Value,
                    Short: m.Groups["shrt"].Success ? m.Groups["shrt"].Value : null,
                    ArgShape: m.Groups["arg"].Success ? m.Groups["arg"].Value : null,
                    Description: string.Empty);
                descBuf.Clear();
                descBuf.Append(m.Groups["desc"].Value);
            }
            else if (pending is not null)
            {
                // Continuation line: append after a space.
                descBuf.Append(' ').Append(content.Trim());
            }
        }
        Flush();

        // Skip scripts with no flags at all — they are tiny shell wrappers
        // that the cli/ reference doesn't need to enumerate.
        if (flags.Count == 0) return null;

        return new CliInfo(
            Name: name,
            Category: "contributor",
            Summary: summary,
            SourceRelativePath: rel,
            Commands: Array.Empty<CliCommand>(),
            Flags: flags,
            Arguments: Array.Empty<CliArg>());
    }

    private static string ExtractShellSummary(string[] lines)
    {
        // Skip the shebang, then take consecutive `#` lines as prose
        // until either a blank line or a non-`#` line. The result is a
        // single space-joined paragraph trimmed to 320 chars max.
        var sb = new System.Text.StringBuilder();
        int start = 0;
        if (lines.Length > 0 && lines[0].StartsWith("#!", StringComparison.Ordinal)) start = 1;
        for (int i = start; i < lines.Length; i++)
        {
            var l = lines[i];
            if (l.Length == 0) break;
            if (!l.StartsWith('#')) break;
            // Stop at the first "Usage:" or "Flags:" header — that marks
            // the end of the prose block.
            var stripped = l.TrimStart('#').TrimStart();
            if (Regex.IsMatch(stripped, @"^[A-Z][A-Za-z\s]+:\s*$")) break;
            if (sb.Length > 0) sb.Append(' ');
            sb.Append(stripped);
        }
        var text = sb.ToString().Trim();
        return text.Length > 320 ? text.Substring(0, 317) + "..." : text;
    }

    // ----- DocsGen + SysML-Import -------------------------------------------

    // Matches `case "--flag":` and the immediately-following
    // `RequireValue` / break statements in the typical
    // `for (int i = 0; ...)` argument parser pattern.
    private static readonly Regex DotnetFlagRe = new(
        @"case\s+""(?<flag>--[a-zA-Z][a-zA-Z0-9-]*)""\s*:",
        RegexOptions.Compiled);

    private static CliInfo CollectDotNetTool(string name, string file, string repoRoot, string summary)
    {
        var text = File.ReadAllText(file);
        var lines = text.Split('\n');

        // First pass: try to read structured flag descriptions out of the
        // leading `//` Flags: block (same shape as the bash help blocks).
        var headerDescs = ExtractDotnetHeaderFlagDescriptions(lines);

        var flags = new List<CliFlag>();
        foreach (Match m in DotnetFlagRe.Matches(text))
        {
            var flagName = m.Groups["flag"].Value;
            if (flags.Any(f => f.Name == flagName)) continue;

            // Description-source order: leading `//` header block (preferred,
            // longer prose with newlines), then Console.WriteLine printed help.
            string? desc = null;
            if (headerDescs.TryGetValue(flagName, out var headerDesc)) desc = headerDesc;
            desc ??= ExtractDotnetFlagDescription(text, flagName);

            // Detect whether the case body calls `RequireValue` — if it
            // does, the flag takes a value.
            bool takesValue = Regex.IsMatch(text,
                $@"case\s+""{Regex.Escape(flagName)}""\s*:[\s\S]{{0,200}}?RequireValue");
            flags.Add(new CliFlag(
                Name: flagName,
                Short: null,
                ArgShape: takesValue ? "<value>" : null,
                Description: desc ?? string.Empty));
        }

        return new CliInfo(
            Name: name,
            Category: "contributor",
            Summary: summary,
            SourceRelativePath: Path.GetRelativePath(repoRoot, file).Replace('\\', '/'),
            Commands: Array.Empty<CliCommand>(),
            Flags: flags
                .OrderBy(f => f.Name, StringComparer.Ordinal)
                .ToList(),
            Arguments: Array.Empty<CliArg>());
    }

    private static Dictionary<string, string> ExtractDotnetHeaderFlagDescriptions(string[] lines)
    {
        // Scan the leading `//` block for a "Flags:" header, then collect
        // the indented flag rows that follow until a blank `//` line or a
        // non-`//` line.
        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        bool inFlags = false;
        string? pendingFlag = null;
        var descBuf = new System.Text.StringBuilder();

        void Flush()
        {
            if (pendingFlag is null) return;
            result[pendingFlag] = descBuf.ToString().Trim();
            pendingFlag = null;
            descBuf.Clear();
        }

        // Matches `//   --flag <arg>    Description...` and
        // `//   --flag    Description...` shapes.
        var headerFlagRe = new Regex(
            @"^//(?<lead>\s{1,6})(?<long>--[a-zA-Z][a-zA-Z0-9-]+)(?:\s+<[^>]+>)?\s+(?<desc>\S.*)$",
            RegexOptions.Compiled);

        bool seenAnyComment = false;
        foreach (var rawLine in lines)
        {
            var line = rawLine.TrimEnd('\r');
            // A non-comment line either skips us forward to the first
            // `//` block (the file may start with `using …;` and friends)
            // or, once we've started consuming a comment block, ends it.
            if (!line.StartsWith("//", StringComparison.Ordinal))
            {
                if (!seenAnyComment) continue;
                Flush();
                break;
            }
            seenAnyComment = true;
            var trimmed = line.Substring(2).TrimStart();
            if (Regex.IsMatch(trimmed, @"^Flags:\s*$"))
            {
                inFlags = true;
                continue;
            }
            if (!inFlags) continue;
            // A blank `//` line ends the block.
            if (trimmed.Length == 0)
            {
                Flush();
                inFlags = false;
                continue;
            }
            var m = headerFlagRe.Match(line);
            if (m.Success)
            {
                Flush();
                pendingFlag = m.Groups["long"].Value;
                descBuf.Clear();
                descBuf.Append(m.Groups["desc"].Value);
            }
            else if (pendingFlag is not null)
            {
                descBuf.Append(' ').Append(trimmed);
            }
        }
        Flush();
        return result;
    }

    private static string? ExtractDotnetFlagDescription(string text, string flagName)
    {
        // The help-print block emits lines like:
        //   Console.WriteLine("    --repo  <repo-root>   Repository root (required)");
        // Walk the WriteLine literals looking for the flag name.
        var helpRe = new Regex(
            $@"Console\.WriteLine\(""(?<line>\s*{Regex.Escape(flagName)}\s+[^""]+)""",
            RegexOptions.Compiled);
        var m = helpRe.Match(text);
        if (!m.Success) return null;
        var raw = m.Groups["line"].Value.Trim();
        // Drop the leading flag name + any value-shape so only the
        // description remains.
        var stripped = Regex.Replace(raw, $@"^{Regex.Escape(flagName)}\s+(?:<[^>]+>)?\s*", "");
        return stripped.Trim();
    }
}
