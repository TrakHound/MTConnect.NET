// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace MTConnect.NET_Docs_Tests;

/// <summary>
/// Pure helpers shared with <see cref="RouteCheckTests"/>. Extracted so
/// the markdown-to-route mapping, the markdown enumeration, the vitepress
/// banner-port parser, the repo-root walk-up, and the free-port allocator
/// are reachable from a unit-tier fixture without spinning up Node,
/// Playwright, or the vitepress preview server. Every public method on
/// this class is pure or depends only on the local filesystem / OS sockets
/// and is therefore safe to exercise in the standard <c>dotnet test</c>
/// run that excludes <c>Category=E2E</c>.
/// </summary>
internal static class RouteCheckHelpers
{
    /// <summary>
    /// Maximum number of ancestor directories <see cref="LocateRepoRoot"/>
    /// will inspect before giving up. The bound is intentionally generous
    /// (32) so a deeper container path, a vendored / submoduled checkout,
    /// or a future test-output relayout can extend the chain without
    /// re-tripping this guard.
    /// </summary>
    public const int RepoRootMaxAncestorDepth = 32;

    /// <summary>
    /// Walks ancestor directories of <paramref name="startDirectory"/>
    /// looking for the first one that contains <paramref name="markerFile"/>.
    /// Returns the absolute path of the matching ancestor, or throws
    /// <see cref="InvalidOperationException"/> with a diagnostic message
    /// (starting directory + depth walked) when no ancestor matches inside
    /// <see cref="RepoRootMaxAncestorDepth"/> steps.
    /// </summary>
    /// <param name="startDirectory">Absolute path to walk up from.</param>
    /// <param name="markerFile">File name whose presence identifies the
    /// target directory (e.g. <c>MTConnect.NET.sln</c>).</param>
    public static string LocateRepoRoot(string startDirectory, string markerFile)
    {
        if (startDirectory is null) throw new ArgumentNullException(nameof(startDirectory));
        if (markerFile is null) throw new ArgumentNullException(nameof(markerFile));

        var dir = startDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        int depth;
        for (depth = 0; depth < RepoRootMaxAncestorDepth && !string.IsNullOrEmpty(dir); depth++)
        {
            if (File.Exists(Path.Combine(dir, markerFile))) return dir;
            dir = Path.GetDirectoryName(dir)!;
        }
        throw new InvalidOperationException(
            $"Could not locate repository root: walked {depth} ancestor(s) up from BaseDirectory '{startDirectory}' without finding {markerFile}.");
    }

    /// <summary>
    /// Convert an absolute markdown-file path to the VitePress route it maps
    /// to. Mirrors the original Node crawler's <c>mdFileToRoute</c>:
    /// <list type="bullet">
    ///   <item><description><c>&lt;docsRoot&gt;/index.md</c> → <c>/</c></description></item>
    ///   <item><description><c>&lt;docsRoot&gt;/getting-started.md</c> → <c>/getting-started</c></description></item>
    ///   <item><description><c>&lt;docsRoot&gt;/reference/index.md</c> → <c>/reference/</c></description></item>
    ///   <item><description><c>&lt;docsRoot&gt;/reference/cli.md</c> → <c>/reference/cli</c></description></item>
    /// </list>
    /// The mapping is case-sensitive and normalises path separators to
    /// forward slashes so the result is a URL-shaped string regardless of
    /// the host OS.
    /// </summary>
    public static string MdFileToRoute(string docsRoot, string absPath)
    {
        if (docsRoot is null) throw new ArgumentNullException(nameof(docsRoot));
        if (absPath is null) throw new ArgumentNullException(nameof(absPath));

        var rel = absPath.Substring(docsRoot.Length).Replace(Path.DirectorySeparatorChar, '/');
        if (!rel.StartsWith('/')) rel = "/" + rel;
        if (rel == "/index.md") return "/";
        if (rel.EndsWith("/index.md", StringComparison.Ordinal))
            return rel.Substring(0, rel.Length - "index.md".Length);
        return rel.Substring(0, rel.Length - ".md".Length);
    }

    /// <summary>
    /// Enumeration policy used by <see cref="CollectMarkdownFiles"/>. Skips
    /// reparse points (symlinks, junctions) so a loop-back symlink in
    /// <c>docs/</c> — <c>docs/loop -&gt; docs/</c> — cannot drive the
    /// recursion into a stack overflow. Recursion stays explicit
    /// (<see cref="EnumerationOptions.RecurseSubdirectories"/> is
    /// <see langword="false"/>) so the <c>node_modules</c> + dotfile skip
    /// rules apply at every level.
    /// </summary>
    public static readonly EnumerationOptions MarkdownEnumeration = new()
    {
        RecurseSubdirectories = false,
        AttributesToSkip = FileAttributes.ReparsePoint,
    };

    /// <summary>
    /// Recursively collect every <c>.md</c> file under <paramref name="dir"/>,
    /// skipping <c>node_modules</c> and any dot-prefixed directory (which
    /// includes <c>.vitepress/</c> — that holds the build cache and the
    /// <c>dist/</c> output, neither of which contains source markdown).
    /// Results are appended to <paramref name="results"/> in directory
    /// enumeration order.
    /// </summary>
    public static void CollectMarkdownFiles(string dir, List<string> results)
    {
        if (dir is null) throw new ArgumentNullException(nameof(dir));
        if (results is null) throw new ArgumentNullException(nameof(results));

        foreach (var entry in Directory.EnumerateFileSystemEntries(dir, "*", MarkdownEnumeration))
        {
            var name = Path.GetFileName(entry);
            if (Directory.Exists(entry))
            {
                if (name == "node_modules" || name.StartsWith('.')) continue;
                CollectMarkdownFiles(entry, results);
            }
            else if (File.Exists(entry) && entry.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
            {
                results.Add(entry);
            }
        }
    }

    /// <summary>
    /// Walks <paramref name="docsRoot"/>, converts every markdown file to
    /// its VitePress route, and returns the unique routes ordinally sorted.
    /// </summary>
    public static List<string> CollectRoutes(string docsRoot)
    {
        if (docsRoot is null) throw new ArgumentNullException(nameof(docsRoot));

        var files = new List<string>();
        CollectMarkdownFiles(docsRoot, files);
        return files
            .Select(f => MdFileToRoute(docsRoot, f))
            .Distinct(StringComparer.Ordinal)
            .OrderBy(r => r, StringComparer.Ordinal)
            .ToList();
    }

    /// <summary>
    /// Regex used to recover the bound port from the vitepress preview
    /// startup banner. The banner prints (e.g.)
    /// <c>➜  Local:   http://localhost:4173/</c>; matching
    /// <c>localhost</c> / <c>127.0.0.1</c> with an optional port group keeps
    /// the parse tolerant of variants between vitepress versions while
    /// still pinning the port digit run when present.
    /// </summary>
    public static readonly Regex PreviewBannerPortRegex = new(
        @"https?://(?:localhost|127\.0\.0\.1)(?::(?<port>\d+))?/?",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    /// <summary>
    /// Scan a drained vitepress preview log for a bound-port match and
    /// return the largest valid port observed, or <see langword="null"/>
    /// when no banner line has yet been seen. Locks
    /// <paramref name="log"/> while reading so a concurrent drainer cannot
    /// re-allocate the underlying buffer mid-snapshot.
    /// </summary>
    public static int? ExtractBannerPort(StringBuilder log)
    {
        if (log is null) throw new ArgumentNullException(nameof(log));

        string snapshot;
        lock (log) snapshot = log.ToString();
        int? best = null;
        foreach (Match m in PreviewBannerPortRegex.Matches(snapshot))
        {
            var g = m.Groups["port"];
            if (g.Success && int.TryParse(g.Value, out var p) && p > 0)
            {
                if (best is null || p > best.Value) best = p;
            }
        }
        return best;
    }

    /// <summary>
    /// Ask the OS for a currently-free TCP port on the loopback interface
    /// by binding ephemerally, reading the assigned port, and releasing.
    /// The caller is responsible for handling the unavoidable TOCTOU
    /// window between release and re-bind (vitepress itself reports the
    /// actually-bound port via <see cref="ExtractBannerPort"/>).
    /// </summary>
    public static int FindFreePort()
    {
        var listener = new TcpListener(System.Net.IPAddress.Loopback, 0);
        listener.Start();
        var port = ((System.Net.IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }
}
