// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MTConnect.NET_DocsGen;

/// <summary>
/// A single environment-variable reference discovered by either the
/// Roslyn (C#) or the shell-script (Bash / PowerShell) collector.
/// </summary>
public sealed record EnvVarInfo(
    string Name,
    string FallbackLiteral,
    string FileRelativePath,
    int Line,
    string Context,
    string Kind);              // "csharp-read" | "shell-read" | "shell-write" | "powershell-read" | "powershell-write"

/// <summary>
/// Walks every `*.cs`, `tools/*.sh`, and `tools/*.ps1` file under the
/// repository (excluding build outputs and the docs/ node_modules
/// tree) and records every environment-variable reference. The
/// recognised patterns are:
///
///   - C# (`*.cs`):
///       <c>Environment.GetEnvironmentVariable("NAME")</c>
///       <c>Environment.GetEnvironmentVariable("NAME", EnvironmentVariableTarget.X)</c>
///   - Bash (`*.sh`):
///       <c>${MTCONNECT_NAME:-default}</c> (read with default)
///       <c>${MTCONNECT_NAME}</c>            (bare read)
///       <c>export MTCONNECT_NAME=value</c>  (write)
///   - PowerShell (`*.ps1`):
///       <c>$env:MTCONNECT_NAME</c>          (read)
///       <c>$env:MTCONNECT_NAME = 'value'</c> (write)
///
/// Each call captures the variable name, the optional default literal
/// that appears alongside the read, the file:line location, and a
/// short snippet of context.
/// </summary>
public static class EnvVarInventory
{
    private static readonly string[] ExcludedDirectoryNames =
    {
        "bin", "obj", "node_modules", ".git", ".vitepress",
    };

    // Match `MTCONNECT_FOO`, `DOTNET_BAR`, plus any other ALL-CAPS env-var
    // shape used by the repo. Lower-case shell vars are deliberately
    // ignored — they are local variables, not OS env vars.
    private static readonly System.Text.RegularExpressions.Regex EnvNameRe =
        new(@"\b([A-Z][A-Z0-9_]{2,})\b", System.Text.RegularExpressions.RegexOptions.Compiled);

    /// <summary>
    /// Walks every <c>*.cs</c> file under <paramref name="repoRoot"/>
    /// plus the <c>tools/*.sh</c> and <c>tools/*.ps1</c> trees and
    /// returns every env-var read / write / docker-pass discovered.
    /// Output is deterministically sorted by name, kind, file, line.
    /// </summary>
    /// <param name="repoRoot">Repository root.</param>
    /// <returns>Ordered list of env-var references.</returns>
    public static IReadOnlyList<EnvVarInfo> Collect(string repoRoot)
    {
        var results = new List<EnvVarInfo>();

        // ---- C# read sites -------------------------------------------------
        foreach (var file in EnumerateCsFiles(repoRoot))
        {
            string text;
            try { text = File.ReadAllText(file); }
            catch { continue; }

            // Cheap text gate before parsing.
            if (!text.Contains("GetEnvironmentVariable", StringComparison.Ordinal)) continue;

            var tree = CSharpSyntaxTree.ParseText(text, path: file);
            var rel = Path.GetRelativePath(repoRoot, file).Replace('\\', '/');

            // Local resolution of `const string X = "..."` so call sites
            // like `GetEnvironmentVariable(FixtureDirEnv)` still resolve
            // without a semantic model.
            var localConsts = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (var field in tree.GetRoot().DescendantNodes().OfType<FieldDeclarationSyntax>())
            {
                bool isConst = field.Modifiers.Any(t => t.IsKind(SyntaxKind.ConstKeyword));
                if (!isConst) continue;
                foreach (var v in field.Declaration.Variables)
                {
                    if (v.Initializer?.Value is LiteralExpressionSyntax l
                        && l.IsKind(SyntaxKind.StringLiteralExpression))
                    {
                        localConsts[v.Identifier.ValueText] = l.Token.ValueText;
                    }
                }
            }

            foreach (var inv in tree.GetRoot().DescendantNodes().OfType<InvocationExpressionSyntax>())
            {
                if (inv.Expression is not MemberAccessExpressionSyntax m) continue;
                if (m.Name.Identifier.ValueText != "GetEnvironmentVariable") continue;

                var args = inv.ArgumentList.Arguments;
                if (args.Count == 0) continue;
                string? name = null;
                if (args[0].Expression is LiteralExpressionSyntax lit && lit.IsKind(SyntaxKind.StringLiteralExpression))
                {
                    name = lit.Token.ValueText;
                }
                else if (args[0].Expression is IdentifierNameSyntax id
                         && localConsts.TryGetValue(id.Identifier.ValueText, out var resolved))
                {
                    name = resolved;
                }
                if (name is null) continue;

                var line = inv.GetLocation().GetLineSpan().StartLinePosition.Line + 1;

                // Look at the enclosing statement / expression for a
                // ?? "default" or `?? Environment.GetEnvironmentVariable(...)`
                // fallback.
                var fallback = string.Empty;
                var parent = inv.Parent;
                while (parent is not null && parent is not StatementSyntax && parent is not MemberDeclarationSyntax)
                {
                    if (parent is BinaryExpressionSyntax bin && bin.IsKind(SyntaxKind.CoalesceExpression))
                    {
                        fallback = bin.Right.ToString();
                        break;
                    }
                    parent = parent.Parent;
                }

                var context = inv.Parent?.ToString() ?? inv.ToString();
                if (context.Length > 160) context = context.Substring(0, 157) + "...";
                context = context.Replace('\n', ' ').Replace('\r', ' ').Replace('|', '/');

                results.Add(new EnvVarInfo(name, fallback, rel, line, context, "csharp-read"));
            }
        }

        // ---- Shell / PowerShell sites --------------------------------------
        var toolsDir = Path.Combine(repoRoot, "tools");
        if (Directory.Exists(toolsDir))
        {
            foreach (var sh in Directory.EnumerateFiles(toolsDir, "*.sh", SearchOption.AllDirectories))
            {
                CollectShell(sh, repoRoot, results);
            }
            foreach (var ps in Directory.EnumerateFiles(toolsDir, "*.ps1", SearchOption.AllDirectories))
            {
                CollectPowerShell(ps, repoRoot, results);
            }
        }

        return results
            .OrderBy(e => e.Name, StringComparer.Ordinal)
            .ThenBy(e => e.Kind, StringComparer.Ordinal)
            .ThenBy(e => e.FileRelativePath, StringComparer.Ordinal)
            .ThenBy(e => e.Line)
            .ToList();
    }

    // ----- Shell collectors -------------------------------------------------

    // Matches `${VAR:-default}` (read with default literal) and `${VAR}` /
    // `$VAR` (bare read). The default literal can be either bare text or a
    // quoted string. The default capture stops at the closing `}` of the
    // outer expansion and explicitly allows a nested `${…}` inside (for
    // patterns like `${MTCONNECT_DOTNET_IMAGE:-${IMAGE_DEFAULT}}`).
    private static readonly System.Text.RegularExpressions.Regex ShellReadRe = new(
        @"\$\{(?<name>[A-Z][A-Z0-9_]{2,})(?:\:-(?<default>(?:\$\{[^}]*\}|[^}])*))?\}",
        System.Text.RegularExpressions.RegexOptions.Compiled);

    // Matches `export VAR=value` and `VAR=value` at the start of a line.
    private static readonly System.Text.RegularExpressions.Regex ShellWriteRe = new(
        @"^\s*(?:export\s+)?(?<name>[A-Z][A-Z0-9_]{2,})=(?<value>[^\s#]*)",
        System.Text.RegularExpressions.RegexOptions.Compiled);

    // Matches `docker run -e VAR=value` or `-e "VAR=value"` — the variable
    // is passed into a child container's environment. From a documentation
    // standpoint these are still env-var references the codebase honors.
    private static readonly System.Text.RegularExpressions.Regex DockerEnvRe = new(
        @"-e\s+""?(?<name>[A-Z][A-Z0-9_]{2,})=(?<value>[^""\s]*)",
        System.Text.RegularExpressions.RegexOptions.Compiled);

    // Matches the start of a heredoc and captures the delimiter, with or
    // without single-quote / double-quote wrapping (which only affects
    // interpolation, not the body-bracket).
    private static readonly System.Text.RegularExpressions.Regex HeredocStartRe = new(
        @"<<-?\s*['""]?(?<delim>[A-Za-z_][A-Za-z0-9_]*)['""]?",
        System.Text.RegularExpressions.RegexOptions.Compiled);

    private static void CollectShell(string file, string repoRoot, List<EnvVarInfo> sink)
    {
        string[] lines;
        try { lines = File.ReadAllLines(file); }
        catch { return; }
        var rel = Path.GetRelativePath(repoRoot, file).Replace('\\', '/');

        // De-dupe identical hits on the same (kind, line) tuple — a single
        // PS expression that mentions `$env:X` twice should yield one row.
        var seen = new HashSet<(string Name, string Kind, int Line)>();
        string? heredocDelim = null;

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];

            // Track heredoc bodies. A heredoc opens on `<<EOF`, `<<-EOF`,
            // `<<'EOF'`, etc., and closes on a line whose trim equals the
            // delimiter. Help-text heredocs use the variable name in
            // documentation prose, so skipping the body avoids false
            // positives like `MTCONNECT_X=1)` inside indented prose.
            if (heredocDelim is not null)
            {
                if (line.Trim() == heredocDelim)
                {
                    heredocDelim = null;
                }
                continue;
            }

            var trimmed = line.TrimStart();
            // Skip comment lines for both reads and writes.
            if (trimmed.StartsWith('#')) continue;

            var hdStart = HeredocStartRe.Match(line);
            if (hdStart.Success)
            {
                heredocDelim = hdStart.Groups["delim"].Value;
                // Fall through and still scan the *opener* line below in
                // case it has a leading `command` with an env-var read on
                // the same line before the `<<DELIM`.
            }

            // Reads: `${NAME:-default}` or `${NAME}`.
            foreach (System.Text.RegularExpressions.Match m in ShellReadRe.Matches(line))
            {
                var name = m.Groups["name"].Value;
                if (!IsRelevantVar(name)) continue;
                if (!seen.Add((name, "shell-read", i + 1))) continue;
                var def = m.Groups["default"].Success ? m.Groups["default"].Value : string.Empty;
                sink.Add(new EnvVarInfo(name, def, rel, i + 1, Snip(line), "shell-read"));
            }

            // Writes: `export NAME=value`.
            var w = ShellWriteRe.Match(line);
            if (w.Success)
            {
                var name = w.Groups["name"].Value;
                if (IsRelevantVar(name) && seen.Add((name, "shell-write", i + 1)))
                {
                    sink.Add(new EnvVarInfo(name, w.Groups["value"].Value, rel, i + 1, Snip(line), "shell-write"));
                }
            }

            // `docker run -e VAR=value` style — pass the var into a child
            // container's environment.
            foreach (System.Text.RegularExpressions.Match m in DockerEnvRe.Matches(line))
            {
                var name = m.Groups["name"].Value;
                if (!IsRelevantVar(name)) continue;
                if (!seen.Add((name, "docker-env", i + 1))) continue;
                sink.Add(new EnvVarInfo(name, m.Groups["value"].Value, rel, i + 1, Snip(line), "docker-env"));
            }
        }
    }

    // Matches `$env:NAME` reads inside expressions and conditions.
    private static readonly System.Text.RegularExpressions.Regex PsReadRe = new(
        @"\$env:(?<name>[A-Z][A-Z0-9_]{2,})",
        System.Text.RegularExpressions.RegexOptions.Compiled);

    // Matches `$env:NAME = 'value'` writes.
    private static readonly System.Text.RegularExpressions.Regex PsWriteRe = new(
        @"\$env:(?<name>[A-Z][A-Z0-9_]{2,})\s*=\s*(?<value>'[^']*'|""[^""]*""|\S+)",
        System.Text.RegularExpressions.RegexOptions.Compiled);

    private static void CollectPowerShell(string file, string repoRoot, List<EnvVarInfo> sink)
    {
        string[] lines;
        try { lines = File.ReadAllLines(file); }
        catch { return; }
        var rel = Path.GetRelativePath(repoRoot, file).Replace('\\', '/');

        // De-dupe identical hits on the same (kind, line) tuple.
        var seen = new HashSet<(string Name, string Kind, int Line)>();

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            var trimmed = line.TrimStart();
            // PowerShell comments start with `#` too.
            if (trimmed.StartsWith('#')) continue;

            // Writes first so a single line like `$env:X = '1'` is only
            // counted once (a write line also matches the read pattern).
            var w = PsWriteRe.Match(line);
            if (w.Success)
            {
                var name = w.Groups["name"].Value;
                if (IsRelevantVar(name) && seen.Add((name, "powershell-write", i + 1)))
                {
                    sink.Add(new EnvVarInfo(name, w.Groups["value"].Value, rel, i + 1, Snip(line), "powershell-write"));
                    continue;
                }
            }

            foreach (System.Text.RegularExpressions.Match m in PsReadRe.Matches(line))
            {
                var name = m.Groups["name"].Value;
                if (!IsRelevantVar(name)) continue;
                if (!seen.Add((name, "powershell-read", i + 1))) continue;
                sink.Add(new EnvVarInfo(name, string.Empty, rel, i + 1, Snip(line), "powershell-read"));
            }
        }
    }

    // Keep the inventory focused on MTConnect-namespaced environment
    // variables plus the small set of well-known cross-cutting ones the
    // repo actually honors. Without this filter every uppercase token in
    // every line of every script (PATH, HOME, NUGET_PACKAGES, ...) would
    // flood the inventory.
    private static bool IsRelevantVar(string name) =>
        name.StartsWith("MTCONNECT_", StringComparison.Ordinal);

    private static string Snip(string s)
    {
        var t = s.Trim();
        return t.Length > 160 ? t.Substring(0, 157) + "..." : t;
    }

    private static IEnumerable<string> EnumerateCsFiles(string root)
    {
        var stack = new Stack<string>();
        stack.Push(root);
        while (stack.Count > 0)
        {
            var dir = stack.Pop();
            IEnumerable<string> subs;
            try { subs = Directory.EnumerateDirectories(dir); }
            catch { continue; }
            foreach (var sub in subs)
            {
                var name = Path.GetFileName(sub);
                if (ExcludedDirectoryNames.Contains(name, StringComparer.Ordinal)) continue;
                stack.Push(sub);
            }
            IEnumerable<string> files;
            try { files = Directory.EnumerateFiles(dir, "*.cs"); }
            catch { continue; }
            foreach (var f in files) yield return f;
        }
    }
}
