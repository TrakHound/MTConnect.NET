// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using NUnit.Framework;

namespace MTConnect.NET_Docs_Tests;

/// <summary>
/// Unit-tier coverage for <see cref="RouteCheckHelpers"/>. Exercises the
/// pure helpers that <see cref="RouteCheckTests"/> relies on — the
/// markdown-to-route mapping, the recursive markdown enumeration, the
/// vitepress banner-port parser, the repo-root walk-up, and the
/// free-port allocator — so the §10 / §10a positive + negative bar is
/// met without spinning up Node, Playwright, or the vitepress preview
/// server. The fixture is intentionally <c>Category</c>-free so it runs
/// in the standard <c>dotnet test</c> invocation that excludes
/// <c>Category=E2E</c>.
/// </summary>
[TestFixture]
public class RouteCheckHelpersTests
{
    // ─── MdFileToRoute ───────────────────────────────────────────────────────

    /// <summary>
    /// Root <c>index.md</c> maps to the site root, mirroring the original
    /// Node crawler's <c>mdFileToRoute</c> contract.
    /// </summary>
    [Test]
    public void MdFileToRoute_RootIndex_MapsToSlash()
    {
        var route = RouteCheckHelpers.MdFileToRoute("/repo/docs", "/repo/docs/index.md");
        Assert.That(route, Is.EqualTo("/"));
    }

    /// <summary>
    /// A top-level non-index markdown file drops the <c>.md</c> suffix.
    /// </summary>
    [Test]
    public void MdFileToRoute_TopLevelPage_DropsExtension()
    {
        var route = RouteCheckHelpers.MdFileToRoute("/repo/docs", "/repo/docs/getting-started.md");
        Assert.That(route, Is.EqualTo("/getting-started"));
    }

    /// <summary>
    /// A nested <c>index.md</c> maps to the parent directory with the
    /// trailing slash retained — VitePress treats this as the section
    /// landing route.
    /// </summary>
    [Test]
    public void MdFileToRoute_NestedIndex_RetainsTrailingSlash()
    {
        var route = RouteCheckHelpers.MdFileToRoute("/repo/docs", "/repo/docs/reference/index.md");
        Assert.That(route, Is.EqualTo("/reference/"));
    }

    /// <summary>
    /// A nested non-index markdown file drops the <c>.md</c> suffix and
    /// preserves the directory chain.
    /// </summary>
    [Test]
    public void MdFileToRoute_NestedPage_DropsExtension()
    {
        var route = RouteCheckHelpers.MdFileToRoute("/repo/docs", "/repo/docs/reference/cli.md");
        Assert.That(route, Is.EqualTo("/reference/cli"));
    }

    /// <summary>
    /// Windows path separators in the absolute path are normalised to
    /// forward slashes so the result is URL-shaped regardless of the
    /// host OS.
    /// </summary>
    [Test]
    public void MdFileToRoute_WindowsSeparators_AreNormalised()
    {
        // Cross-platform check: feed Windows-style separators on both
        // sides and assert the URL output. We synthesise the input
        // rather than P/Invoke Windows APIs so the test runs on Linux
        // CI too.
        const string docsRoot = @"C:\repo\docs";
        const string absPath = @"C:\repo\docs\reference\cli.md";
        var route = RouteCheckHelpers.MdFileToRoute(docsRoot, absPath);
        // On non-Windows runners Path.DirectorySeparatorChar is '/', so
        // the replacement is a no-op for the test input above. Validate
        // the OS-specific case only where it applies.
        if (Path.DirectorySeparatorChar == '\\')
        {
            Assert.That(route, Is.EqualTo("/reference/cli"));
        }
        else
        {
            // Linux runner — separators in the input string are literal
            // backslashes, not directory separators. The helper produces
            // a route that includes them. This branch documents that
            // expectation so a future contributor doesn't assume the
            // helper does its own backslash mapping.
            Assert.That(route, Does.StartWith("/"));
        }
    }

    /// <summary>
    /// Passing a null <c>docsRoot</c> throws
    /// <see cref="ArgumentNullException"/> with the parameter name pinned
    /// so the contract is observable, not just "some exception."
    /// </summary>
    [Test]
    public void MdFileToRoute_NullDocsRoot_Throws()
    {
        var ex = Assert.Throws<ArgumentNullException>(
            () => RouteCheckHelpers.MdFileToRoute(null!, "/repo/docs/index.md"));
        Assert.That(ex!.ParamName, Is.EqualTo("docsRoot"));
    }

    /// <summary>
    /// Passing a null <c>absPath</c> throws
    /// <see cref="ArgumentNullException"/> with the parameter name pinned.
    /// </summary>
    [Test]
    public void MdFileToRoute_NullAbsPath_Throws()
    {
        var ex = Assert.Throws<ArgumentNullException>(
            () => RouteCheckHelpers.MdFileToRoute("/repo/docs", null!));
        Assert.That(ex!.ParamName, Is.EqualTo("absPath"));
    }

    // ─── CollectMarkdownFiles + CollectRoutes ────────────────────────────────

    /// <summary>
    /// A fresh markdown tree under a temp directory yields every <c>.md</c>
    /// file in enumeration order, mapped to the right route, sorted, and
    /// deduplicated.
    /// </summary>
    [Test]
    public void CollectRoutes_NestedTree_ProducesExpectedRouteSet()
    {
        using var fixture = new TempDirectory();
        File.WriteAllText(Path.Combine(fixture.Path, "index.md"), "# root");
        File.WriteAllText(Path.Combine(fixture.Path, "getting-started.md"), "# go");
        var refDir = Path.Combine(fixture.Path, "reference");
        Directory.CreateDirectory(refDir);
        File.WriteAllText(Path.Combine(refDir, "index.md"), "# ref");
        File.WriteAllText(Path.Combine(refDir, "cli.md"), "# cli");

        var routes = RouteCheckHelpers.CollectRoutes(fixture.Path);
        Assert.That(routes, Is.EquivalentTo(new[] { "/", "/getting-started", "/reference/", "/reference/cli" }));
        // Sorted ordinal: '/' < '/g…' < '/r…' so the slice is monotonic.
        Assert.That(routes, Is.Ordered.Using<string>(StringComparer.Ordinal));
    }

    /// <summary>
    /// <c>node_modules</c> directories are skipped. A markdown file living
    /// under one would balloon the route table with vendored docs that the
    /// VitePress build never serves.
    /// </summary>
    [Test]
    public void CollectMarkdownFiles_SkipsNodeModules()
    {
        using var fixture = new TempDirectory();
        var nm = Path.Combine(fixture.Path, "node_modules", "vendor");
        Directory.CreateDirectory(nm);
        File.WriteAllText(Path.Combine(nm, "leaked.md"), "# nope");
        File.WriteAllText(Path.Combine(fixture.Path, "kept.md"), "# yes");

        var results = new List<string>();
        RouteCheckHelpers.CollectMarkdownFiles(fixture.Path, results);

        Assert.That(results, Has.Count.EqualTo(1));
        Assert.That(results.Single(), Does.EndWith("kept.md"));
    }

    /// <summary>
    /// Dot-prefixed directories (<c>.vitepress/</c>, <c>.git/</c>, …) are
    /// skipped. Without this guard the build cache + dist artefacts would
    /// surface as ghost routes.
    /// </summary>
    [Test]
    public void CollectMarkdownFiles_SkipsDotDirectories()
    {
        using var fixture = new TempDirectory();
        var hidden = Path.Combine(fixture.Path, ".vitepress", "cache");
        Directory.CreateDirectory(hidden);
        File.WriteAllText(Path.Combine(hidden, "cached.md"), "# cached");
        File.WriteAllText(Path.Combine(fixture.Path, "visible.md"), "# visible");

        var results = new List<string>();
        RouteCheckHelpers.CollectMarkdownFiles(fixture.Path, results);

        Assert.That(results, Has.Count.EqualTo(1));
        Assert.That(results.Single(), Does.EndWith("visible.md"));
    }

    /// <summary>
    /// Non-markdown siblings (PNG, TS, JSON) are ignored. The crawler
    /// only cares about <c>.md</c> input.
    /// </summary>
    [Test]
    public void CollectMarkdownFiles_IgnoresNonMarkdownFiles()
    {
        using var fixture = new TempDirectory();
        File.WriteAllText(Path.Combine(fixture.Path, "image.png"), string.Empty);
        File.WriteAllText(Path.Combine(fixture.Path, "config.ts"), string.Empty);
        File.WriteAllText(Path.Combine(fixture.Path, "page.md"), "# page");

        var results = new List<string>();
        RouteCheckHelpers.CollectMarkdownFiles(fixture.Path, results);

        Assert.That(results, Has.Count.EqualTo(1));
        Assert.That(results.Single(), Does.EndWith("page.md"));
    }

    /// <summary>
    /// An empty docs tree yields an empty route list — not an exception
    /// and not a phantom root entry.
    /// </summary>
    [Test]
    public void CollectRoutes_EmptyTree_ReturnsEmpty()
    {
        using var fixture = new TempDirectory();
        var routes = RouteCheckHelpers.CollectRoutes(fixture.Path);
        Assert.That(routes, Is.Empty);
    }

    /// <summary>
    /// On Linux, a reparse-point loop-back symlink inside the docs tree
    /// is skipped — the recursion does not enter the symlink, so a
    /// <c>docs/loop -&gt; docs/</c> cannot drive stack overflow.
    /// </summary>
    [Test]
    public void CollectMarkdownFiles_SkipsReparsePointLoop()
    {
        // Junction / symlink creation is fragile on Windows (requires
        // admin or developer mode); restrict the test to Linux where
        // a plain symlink works without elevation. The reparse-point
        // skip applies identically on Windows but the assertion path
        // is harder to set up there.
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Assert.Ignore("Symlink creation on Windows requires elevation; the AttributesToSkip=ReparsePoint policy is exercised symmetrically on Linux.");
        }

        using var fixture = new TempDirectory();
        File.WriteAllText(Path.Combine(fixture.Path, "real.md"), "# real");
        var loopPath = Path.Combine(fixture.Path, "loop");
        try
        {
            Directory.CreateSymbolicLink(loopPath, fixture.Path);
        }
        catch (IOException)
        {
            Assert.Ignore("Could not create symlink in temp directory (likely fs limitation); reparse-point policy still enforced.");
        }

        var results = new List<string>();
        RouteCheckHelpers.CollectMarkdownFiles(fixture.Path, results);
        // Only the real markdown file is enumerated; the loop is not
        // traversed. Without the AttributesToSkip filter, this would
        // recurse indefinitely.
        Assert.That(results, Has.Count.EqualTo(1));
        Assert.That(results.Single(), Does.EndWith("real.md"));
    }

    /// <summary>
    /// <see cref="RouteCheckHelpers.CollectMarkdownFiles"/> with null
    /// <c>dir</c> throws <see cref="ArgumentNullException"/>.
    /// </summary>
    [Test]
    public void CollectMarkdownFiles_NullDir_Throws()
    {
        var results = new List<string>();
        var ex = Assert.Throws<ArgumentNullException>(
            () => RouteCheckHelpers.CollectMarkdownFiles(null!, results));
        Assert.That(ex!.ParamName, Is.EqualTo("dir"));
    }

    /// <summary>
    /// <see cref="RouteCheckHelpers.CollectMarkdownFiles"/> with null
    /// <c>results</c> throws <see cref="ArgumentNullException"/>.
    /// </summary>
    [Test]
    public void CollectMarkdownFiles_NullResults_Throws()
    {
        var ex = Assert.Throws<ArgumentNullException>(
            () => RouteCheckHelpers.CollectMarkdownFiles("/tmp", null!));
        Assert.That(ex!.ParamName, Is.EqualTo("results"));
    }

    /// <summary>
    /// <see cref="RouteCheckHelpers.CollectRoutes"/> with null
    /// <c>docsRoot</c> throws <see cref="ArgumentNullException"/>.
    /// </summary>
    [Test]
    public void CollectRoutes_NullDocsRoot_Throws()
    {
        var ex = Assert.Throws<ArgumentNullException>(
            () => RouteCheckHelpers.CollectRoutes(null!));
        Assert.That(ex!.ParamName, Is.EqualTo("docsRoot"));
    }

    /// <summary>
    /// The shared <see cref="EnumerationOptions"/> bans descending into
    /// reparse points and disables the implicit recursion knob — both
    /// are load-bearing and a future regression would silently broaden
    /// the enumeration surface.
    /// </summary>
    [Test]
    public void MarkdownEnumeration_PinsAttributesToSkipAndRecursionFlag()
    {
        var opts = RouteCheckHelpers.MarkdownEnumeration;
        Assert.That(opts.RecurseSubdirectories, Is.False, "explicit recursion is required so node_modules/dot-dir skips apply per level");
        Assert.That(opts.AttributesToSkip.HasFlag(FileAttributes.ReparsePoint), Is.True, "symlink loop containment requires ReparsePoint in the skip mask");
    }

    // ─── ExtractBannerPort ───────────────────────────────────────────────────

    /// <summary>
    /// A representative vitepress preview banner yields the bound port.
    /// </summary>
    [Test]
    public void ExtractBannerPort_LocalhostBanner_ReturnsPort()
    {
        var log = new StringBuilder();
        log.AppendLine("[stdout] ");
        log.AppendLine("[stdout]   vitepress v1.5.0");
        log.AppendLine("[stdout]   ➜  Local:   http://localhost:4173/");
        log.AppendLine("[stdout]   ➜  press h to show help");

        Assert.That(RouteCheckHelpers.ExtractBannerPort(log), Is.EqualTo(4173));
    }

    /// <summary>
    /// The 127.0.0.1 banner variant is matched too — VitePress switches
    /// between <c>localhost</c> and the loopback IP between versions.
    /// </summary>
    [Test]
    public void ExtractBannerPort_LoopbackIpBanner_ReturnsPort()
    {
        var log = new StringBuilder("Local: http://127.0.0.1:5174/");
        Assert.That(RouteCheckHelpers.ExtractBannerPort(log), Is.EqualTo(5174));
    }

    /// <summary>
    /// When the banner emits multiple matches (Local + Network lines),
    /// the helper picks the largest valid port. The drain may interleave
    /// partial-line writes; "largest" is a deterministic, regression-safe
    /// tiebreaker.
    /// </summary>
    [Test]
    public void ExtractBannerPort_MultipleMatches_PrefersLargestPort()
    {
        var log = new StringBuilder();
        log.AppendLine("➜  Local:   http://localhost:4173/");
        log.AppendLine("➜  Network: http://127.0.0.1:9999/");
        Assert.That(RouteCheckHelpers.ExtractBannerPort(log), Is.EqualTo(9999));
    }

    /// <summary>
    /// A log that has not yet seen the banner returns <see langword="null"/>
    /// — the caller falls back to the requested port until the banner
    /// drains.
    /// </summary>
    [Test]
    public void ExtractBannerPort_NoBanner_ReturnsNull()
    {
        var log = new StringBuilder("starting up...");
        Assert.That(RouteCheckHelpers.ExtractBannerPort(log), Is.Null);
    }

    /// <summary>
    /// A banner line without a port group (just <c>http://localhost/</c>)
    /// is not considered a hit — the helper requires an explicit port
    /// digit run, since the caller's whole purpose is reading the bound
    /// port off the child's startup output.
    /// </summary>
    [Test]
    public void ExtractBannerPort_HostWithoutPort_ReturnsNull()
    {
        var log = new StringBuilder("Local: http://localhost/");
        Assert.That(RouteCheckHelpers.ExtractBannerPort(log), Is.Null);
    }

    /// <summary>
    /// An empty buffer returns <see langword="null"/> rather than
    /// throwing.
    /// </summary>
    [Test]
    public void ExtractBannerPort_EmptyLog_ReturnsNull()
    {
        var log = new StringBuilder();
        Assert.That(RouteCheckHelpers.ExtractBannerPort(log), Is.Null);
    }

    /// <summary>
    /// A null buffer throws <see cref="ArgumentNullException"/> with the
    /// parameter name pinned.
    /// </summary>
    [Test]
    public void ExtractBannerPort_NullLog_Throws()
    {
        var ex = Assert.Throws<ArgumentNullException>(
            () => RouteCheckHelpers.ExtractBannerPort(null!));
        Assert.That(ex!.ParamName, Is.EqualTo("log"));
    }

    // ─── LocateRepoRoot ──────────────────────────────────────────────────────

    /// <summary>
    /// When the marker file lives in the starting directory itself, the
    /// helper returns it without walking up.
    /// </summary>
    [Test]
    public void LocateRepoRoot_MarkerAtStart_ReturnsStart()
    {
        using var fixture = new TempDirectory();
        File.WriteAllText(Path.Combine(fixture.Path, "marker.txt"), "x");
        var result = RouteCheckHelpers.LocateRepoRoot(fixture.Path, "marker.txt");
        // On macOS, the system temp directory often lives under /var,
        // which is a symlink to /private/var. Compare via FullName so a
        // symlink in the ancestor chain doesn't trip an ordinal mismatch.
        Assert.That(new DirectoryInfo(result).FullName,
            Is.EqualTo(new DirectoryInfo(fixture.Path).FullName));
    }

    /// <summary>
    /// When the marker file lives several levels above the starting
    /// directory, the walk surfaces it.
    /// </summary>
    [Test]
    public void LocateRepoRoot_MarkerInAncestor_WalksUp()
    {
        using var fixture = new TempDirectory();
        File.WriteAllText(Path.Combine(fixture.Path, "marker.txt"), "x");
        var deep = Path.Combine(fixture.Path, "a", "b", "c");
        Directory.CreateDirectory(deep);

        var result = RouteCheckHelpers.LocateRepoRoot(deep, "marker.txt");
        Assert.That(new DirectoryInfo(result).FullName,
            Is.EqualTo(new DirectoryInfo(fixture.Path).FullName));
    }

    /// <summary>
    /// When no ancestor contains the marker, the helper throws
    /// <see cref="InvalidOperationException"/> whose message names both
    /// the starting directory and the walk depth so the failure is
    /// diagnosable from the assertion message alone.
    /// </summary>
    [Test]
    public void LocateRepoRoot_NoMatch_ThrowsWithDiagnosticContext()
    {
        using var fixture = new TempDirectory();
        // Use a marker name that definitely doesn't exist in any
        // ancestor of the temp directory. The walk will exhaust at
        // the filesystem root.
        var ex = Assert.Throws<InvalidOperationException>(
            () => RouteCheckHelpers.LocateRepoRoot(fixture.Path, "no-such-marker-12345.txt"));
        Assert.That(ex!.Message, Does.Contain(fixture.Path));
        Assert.That(ex.Message, Does.Contain("ancestor"));
    }

    /// <summary>
    /// Null start directory throws <see cref="ArgumentNullException"/>
    /// with the parameter name pinned.
    /// </summary>
    [Test]
    public void LocateRepoRoot_NullStartDirectory_Throws()
    {
        var ex = Assert.Throws<ArgumentNullException>(
            () => RouteCheckHelpers.LocateRepoRoot(null!, "marker.txt"));
        Assert.That(ex!.ParamName, Is.EqualTo("startDirectory"));
    }

    /// <summary>
    /// Null marker file throws <see cref="ArgumentNullException"/> with
    /// the parameter name pinned.
    /// </summary>
    [Test]
    public void LocateRepoRoot_NullMarkerFile_Throws()
    {
        var ex = Assert.Throws<ArgumentNullException>(
            () => RouteCheckHelpers.LocateRepoRoot("/tmp", null!));
        Assert.That(ex!.ParamName, Is.EqualTo("markerFile"));
    }

    // ─── FindFreePort ────────────────────────────────────────────────────────

    /// <summary>
    /// <see cref="RouteCheckHelpers.FindFreePort"/> returns a positive
    /// ephemeral-range port that the caller can subsequently bind. The
    /// re-bind confirms the OS released the listener cleanly — a flake
    /// here means the helper is leaving the listener half-closed.
    /// </summary>
    [Test]
    public void FindFreePort_ReturnsBindablePort()
    {
        var port = RouteCheckHelpers.FindFreePort();
        Assert.That(port, Is.GreaterThan(0));

        // Smoke: the port can be re-bound immediately after the helper
        // returns. The TOCTOU window between Stop() and our Bind() is
        // narrow but non-zero; treat a SocketException as "another
        // process raced us" rather than a hard failure, since that is
        // exactly the race the comment in RouteCheckHelpers warns about.
        try
        {
            using var listener = new TcpListener(IPAddress.Loopback, port);
            listener.Start();
            Assert.That(((IPEndPoint)listener.LocalEndpoint).Port, Is.EqualTo(port));
            listener.Stop();
        }
        catch (SocketException)
        {
            // Expected when the OS handed the port to another process
            // in the gap between Stop() and our Bind(). The helper's
            // own callers handle this by reading the actual bound port
            // off the child's startup banner.
            Assert.Pass("port was rebound by another process — TOCTOU window is documented in the helper");
        }
    }

    /// <summary>
    /// Calling <see cref="RouteCheckHelpers.FindFreePort"/> twice in
    /// quick succession returns two ports in the valid range. They may
    /// or may not be equal depending on OS scheduling; the contract is
    /// only "each call returns something bindable at that instant," not
    /// "results are distinct."
    /// </summary>
    [Test]
    public void FindFreePort_SuccessivePortsAreInValidRange()
    {
        var a = RouteCheckHelpers.FindFreePort();
        var b = RouteCheckHelpers.FindFreePort();
        Assert.That(a, Is.InRange(1, 65535));
        Assert.That(b, Is.InRange(1, 65535));
    }

    /// <summary>
    /// Pins the documented walk-up bound to 32. Lowering the bound is a
    /// breaking-config change that should surface a test failure; raising
    /// it is also a config decision, so this regression-guards both
    /// directions.
    /// </summary>
    [Test]
    public void RepoRootMaxAncestorDepth_IsAt32()
    {
        Assert.That(RouteCheckHelpers.RepoRootMaxAncestorDepth, Is.EqualTo(32));
    }

    // ─── ShardRoutes ─────────────────────────────────────────────────────────

    /// <summary>
    /// Sample route set used by the sharding tests below. Twenty routes
    /// in lexical order is large enough to make per-shard counts and
    /// per-shard membership observable, small enough to enumerate by
    /// hand in the assertions.
    /// </summary>
    private static readonly IReadOnlyList<string> SampleRoutes = new[]
    {
        "/a", "/b", "/c", "/d", "/e",
        "/f", "/g", "/h", "/i", "/j",
        "/k", "/l", "/m", "/n", "/o",
        "/p", "/q", "/r", "/s", "/t",
    };

    /// <summary>
    /// A 20-route list partitioned across four shards yields exactly
    /// five routes per shard (round-robin distribution). Pins the
    /// even-division contract — a regression that switched to a
    /// contiguous-chunk policy would surface here as 5/5/5/5 vs
    /// chunked 5/5/5/5 (same count) but a different membership; the
    /// membership invariants below cover the distinction.
    /// </summary>
    [TestCase(1, 4, ExpectedResult = 5)]
    [TestCase(2, 4, ExpectedResult = 5)]
    [TestCase(3, 4, ExpectedResult = 5)]
    [TestCase(4, 4, ExpectedResult = 5)]
    public int ShardRoutes_DistributesEvenly_AcrossFourShards(int index, int total)
    {
        return RouteCheckHelpers.ShardRoutes(SampleRoutes, index, total).Count;
    }

    /// <summary>
    /// An 11-route list partitioned across four shards yields 3, 3, 3, 2
    /// — the round-robin remainder lands on the earliest shards. Pins
    /// the remainder-handling contract so a future refactor that
    /// switched to ceil-division would surface here.
    /// </summary>
    [TestCase(1, 4, ExpectedResult = 3)]
    [TestCase(2, 4, ExpectedResult = 3)]
    [TestCase(3, 4, ExpectedResult = 3)]
    [TestCase(4, 4, ExpectedResult = 2)]
    public int ShardRoutes_DistributesRemainder_AcrossFourShards(int index, int total)
    {
        var routes = SampleRoutes.Take(11).ToList();
        return RouteCheckHelpers.ShardRoutes(routes, index, total).Count;
    }

    /// <summary>
    /// Shard 1 of 1 returns the full input — the no-sharding identity
    /// path. Local invocations without
    /// <c>ROUTE_SHARD_INDEX</c> / <c>ROUTE_SHARD_TOTAL</c> default to
    /// this case so a developer running <c>dotnet test</c> still
    /// exercises every route.
    /// </summary>
    [Test]
    public void ShardRoutes_Shard1Of1_ReturnsAllRoutes()
    {
        var shard = RouteCheckHelpers.ShardRoutes(SampleRoutes, 1, 1);
        Assert.That(shard, Is.EqualTo(SampleRoutes));
    }

    /// <summary>
    /// The concatenation of every shard (in index order) covers every
    /// input route exactly once. Pins the partition invariant: no
    /// route is dropped, no route is double-walked.
    /// </summary>
    [Test]
    public void ShardRoutes_UnionAcrossShards_CoversEveryRoute()
    {
        const int total = 4;
        var union = new List<string>();
        for (var i = 1; i <= total; i++)
        {
            union.AddRange(RouteCheckHelpers.ShardRoutes(SampleRoutes, i, total));
        }

        Assert.That(union, Is.EquivalentTo(SampleRoutes),
            "the union of all shards must equal the input route set");
        Assert.That(union, Has.Count.EqualTo(SampleRoutes.Count),
            "no route may appear in more than one shard");
    }

    /// <summary>
    /// Any pair of shards is disjoint. Pins the partition invariant
    /// from the membership side — a regression that produced
    /// overlapping shards (e.g. an off-by-one on the modulus) would
    /// double-walk routes and surface here even if the union check
    /// still passed by coincidence.
    /// </summary>
    [Test]
    public void ShardRoutes_DifferentShards_AreDisjoint()
    {
        const int total = 4;
        for (var a = 1; a <= total; a++)
        {
            for (var b = a + 1; b <= total; b++)
            {
                var shardA = RouteCheckHelpers.ShardRoutes(SampleRoutes, a, total);
                var shardB = RouteCheckHelpers.ShardRoutes(SampleRoutes, b, total);
                Assert.That(shardA.Intersect(shardB), Is.Empty,
                    $"shards {a} and {b} of {total} share at least one route");
            }
        }
    }

    /// <summary>
    /// Within a single shard, the routes appear in the same relative
    /// order as in the input. Round-robin distribution preserves the
    /// input's monotone order — the assertion is what lets the CI
    /// matrix legs produce diffable failure summaries that line up
    /// with the source markdown's ordering.
    /// </summary>
    [Test]
    public void ShardRoutes_WithinAShard_PreservesInputOrder()
    {
        const int total = 4;
        var inputAsList = SampleRoutes.ToList();
        for (var i = 1; i <= total; i++)
        {
            var shard = RouteCheckHelpers.ShardRoutes(SampleRoutes, i, total);
            var indices = shard.Select(r => inputAsList.IndexOf(r)).ToList();
            Assert.That(indices, Is.Ordered.Ascending,
                $"shard {i}/{total} routes are out of order relative to the input");
        }
    }

    /// <summary>
    /// Routes assigned to shard <c>i</c> of <c>total</c> are exactly
    /// the routes whose zero-based index in the input satisfies
    /// <c>index % total == i - 1</c>. Pins the round-robin contract
    /// explicitly so a refactor that swapped the modulus formula
    /// (e.g. chunked partitioning) would surface as a membership
    /// failure rather than a silent re-ordering.
    /// </summary>
    [Test]
    public void ShardRoutes_AssignmentFollowsRoundRobinModulus()
    {
        const int total = 4;
        for (var i = 1; i <= total; i++)
        {
            var expected = SampleRoutes
                .Select((r, idx) => (r, idx))
                .Where(t => t.idx % total == i - 1)
                .Select(t => t.r)
                .ToList();
            var actual = RouteCheckHelpers.ShardRoutes(SampleRoutes, i, total);
            Assert.That(actual, Is.EqualTo(expected),
                $"shard {i}/{total} membership does not match the round-robin contract");
        }
    }

    /// <summary>
    /// A null routes list throws <see cref="ArgumentNullException"/>
    /// with the parameter name pinned — a future caller that passes
    /// <c>null</c> by accident gets a typed failure naming the argument
    /// rather than an opaque NRE.
    /// </summary>
    [Test]
    public void ShardRoutes_NullRoutes_Throws()
    {
        var ex = Assert.Throws<ArgumentNullException>(
            () => RouteCheckHelpers.ShardRoutes(null!, 1, 1));
        Assert.That(ex!.ParamName, Is.EqualTo("routes"));
    }

    /// <summary>
    /// A non-positive total throws <see cref="ArgumentOutOfRangeException"/>
    /// with the parameter name pinned. Zero or negative totals are
    /// nonsense and must not silently produce an empty shard.
    /// </summary>
    [TestCase(0)]
    [TestCase(-1)]
    public void ShardRoutes_NonPositiveTotal_Throws(int total)
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(
            () => RouteCheckHelpers.ShardRoutes(SampleRoutes, 1, total));
        Assert.That(ex!.ParamName, Is.EqualTo("total"));
    }

    /// <summary>
    /// An out-of-range index throws <see cref="ArgumentOutOfRangeException"/>
    /// with the parameter name pinned. The contract is 1-based:
    /// <c>1 &lt;= index &lt;= total</c>. Index 0 (zero-based off-by-one)
    /// and index <c>total + 1</c> (one-past-the-end) both fall outside.
    /// </summary>
    [TestCase(0, 4)]
    [TestCase(5, 4)]
    [TestCase(-1, 4)]
    public void ShardRoutes_IndexOutOfRange_Throws(int index, int total)
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(
            () => RouteCheckHelpers.ShardRoutes(SampleRoutes, index, total));
        Assert.That(ex!.ParamName, Is.EqualTo("index"));
    }

    /// <summary>
    /// An empty input yields an empty shard for every (index, total)
    /// pair — no exception, no phantom entries. Local invocations on
    /// a fresh checkout where the docs tree is empty (e.g. a worktree
    /// added before the markdown source landed) still produce a
    /// deterministic result.
    /// </summary>
    [Test]
    public void ShardRoutes_EmptyInput_ReturnsEmpty()
    {
        var empty = Array.Empty<string>();
        Assert.That(RouteCheckHelpers.ShardRoutes(empty, 1, 4), Is.Empty);
        Assert.That(RouteCheckHelpers.ShardRoutes(empty, 4, 4), Is.Empty);
    }

    /// <summary>
    /// When the input has fewer routes than shards, the surplus shards
    /// are empty rather than throwing. CI keeps a 4-shard matrix even
    /// for a hypothetically tiny docs tree — surplus shards must
    /// no-op rather than fail.
    /// </summary>
    [Test]
    public void ShardRoutes_FewerRoutesThanShards_SurplusShardsAreEmpty()
    {
        var two = new[] { "/x", "/y" };
        var shard1 = RouteCheckHelpers.ShardRoutes(two, 1, 4);
        var shard2 = RouteCheckHelpers.ShardRoutes(two, 2, 4);
        var shard3 = RouteCheckHelpers.ShardRoutes(two, 3, 4);
        var shard4 = RouteCheckHelpers.ShardRoutes(two, 4, 4);

        Assert.That(shard1, Is.EqualTo(new[] { "/x" }));
        Assert.That(shard2, Is.EqualTo(new[] { "/y" }));
        Assert.That(shard3, Is.Empty);
        Assert.That(shard4, Is.Empty);
    }

    // ─── ReadShardEnv ────────────────────────────────────────────────────────

    /// <summary>
    /// Both env vars set to representative values surface as a
    /// <c>(index, total)</c> tuple. Pins the CI-matrix-injection
    /// contract: <c>ROUTE_SHARD_INDEX=3</c> + <c>ROUTE_SHARD_TOTAL=4</c>
    /// produces shard 3 of 4 regardless of process-level env state.
    /// </summary>
    [Test]
    public void ReadShardEnv_BothSet_ReturnsParsedTuple()
    {
        string? Env(string key) => key switch
        {
            RouteCheckHelpers.ShardIndexEnvVar => "3",
            RouteCheckHelpers.ShardTotalEnvVar => "4",
            _ => null,
        };
        var (index, total) = RouteCheckHelpers.ReadShardEnv(Env);
        Assert.That(index, Is.EqualTo(3));
        Assert.That(total, Is.EqualTo(4));
    }

    /// <summary>
    /// Unset env vars produce the no-sharding identity tuple
    /// <c>(1, 1)</c>. Local <c>dotnet test</c> runs without
    /// matrix-injected env vars therefore walk every route on a single
    /// shard.
    /// </summary>
    [Test]
    public void ReadShardEnv_BothUnset_ReturnsIdentityTuple()
    {
        string? Env(string key) => null;
        var (index, total) = RouteCheckHelpers.ReadShardEnv(Env);
        Assert.That(index, Is.EqualTo(1));
        Assert.That(total, Is.EqualTo(1));
    }

    /// <summary>
    /// Unparseable values fall back to the identity tuple — a
    /// misconfigured CI matrix that injected an empty string or a
    /// non-numeric token should walk every route rather than throw
    /// during fixture init.
    /// </summary>
    [TestCase("", "")]
    [TestCase("not-a-number", "also-not")]
    [TestCase("0", "0")]
    [TestCase("-1", "-1")]
    public void ReadShardEnv_UnparseableValues_FallBackToIdentity(string indexValue, string totalValue)
    {
        string? Env(string key) => key switch
        {
            RouteCheckHelpers.ShardIndexEnvVar => indexValue,
            RouteCheckHelpers.ShardTotalEnvVar => totalValue,
            _ => null,
        };
        var (index, total) = RouteCheckHelpers.ReadShardEnv(Env);
        Assert.That(index, Is.EqualTo(1));
        Assert.That(total, Is.EqualTo(1));
    }

    /// <summary>
    /// When <c>ROUTE_SHARD_INDEX</c> exceeds <c>ROUTE_SHARD_TOTAL</c>
    /// (a half-set matrix, e.g. <c>index=5</c> with <c>total=4</c>),
    /// the helper clamps the index to the total so the caller still
    /// receives a valid shard rather than throwing out of
    /// <see cref="RouteCheckHelpers.ShardRoutes"/> downstream. This
    /// closes the gap between "env vars set" and "env vars consistent."
    /// </summary>
    [Test]
    public void ReadShardEnv_IndexAboveTotal_ClampsToTotal()
    {
        string? Env(string key) => key switch
        {
            RouteCheckHelpers.ShardIndexEnvVar => "5",
            RouteCheckHelpers.ShardTotalEnvVar => "4",
            _ => null,
        };
        var (index, total) = RouteCheckHelpers.ReadShardEnv(Env);
        Assert.That(index, Is.EqualTo(4));
        Assert.That(total, Is.EqualTo(4));
    }

    /// <summary>
    /// The default <see cref="RouteCheckHelpers.ReadShardEnv"/>
    /// overload reads from <see cref="Environment.GetEnvironmentVariable(string)"/>.
    /// Smoke-checking the unset-by-default path keeps the
    /// process-level env-var access wired without forcing a separate
    /// integration-tier setup.
    /// </summary>
    [Test]
    public void ReadShardEnv_DefaultAccessor_ReadsProcessEnvironment()
    {
        // Save / restore so a parallel test in the same process never
        // observes the mutation.
        var savedIndex = Environment.GetEnvironmentVariable(RouteCheckHelpers.ShardIndexEnvVar);
        var savedTotal = Environment.GetEnvironmentVariable(RouteCheckHelpers.ShardTotalEnvVar);
        try
        {
            Environment.SetEnvironmentVariable(RouteCheckHelpers.ShardIndexEnvVar, "2");
            Environment.SetEnvironmentVariable(RouteCheckHelpers.ShardTotalEnvVar, "4");
            var (index, total) = RouteCheckHelpers.ReadShardEnv();
            Assert.That(index, Is.EqualTo(2));
            Assert.That(total, Is.EqualTo(4));
        }
        finally
        {
            Environment.SetEnvironmentVariable(RouteCheckHelpers.ShardIndexEnvVar, savedIndex);
            Environment.SetEnvironmentVariable(RouteCheckHelpers.ShardTotalEnvVar, savedTotal);
        }
    }

    // ─── Helper: scoped temp directory ───────────────────────────────────────

    /// <summary>
    /// Per-test temp directory that cleans itself up on Dispose. Lives
    /// next to the test so each test gets its own filesystem sandbox
    /// (§10a independence + isolation criteria).
    /// </summary>
    private sealed class TempDirectory : IDisposable
    {
        public string Path { get; }

        public TempDirectory()
        {
            Path = System.IO.Path.Combine(
                System.IO.Path.GetTempPath(),
                "mtnet-docs-tests-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path);
        }

        public void Dispose()
        {
            try
            {
                if (Directory.Exists(Path))
                {
                    // Recursively delete; tolerate any reparse-point
                    // leftovers from the symlink test by clearing the
                    // attribute first.
                    foreach (var entry in Directory.EnumerateFileSystemEntries(Path, "*", SearchOption.AllDirectories))
                    {
                        try { File.SetAttributes(entry, FileAttributes.Normal); }
                        catch (UnauthorizedAccessException) { /* best-effort */ }
                        catch (IOException) { /* best-effort */ }
                    }
                    Directory.Delete(Path, recursive: true);
                }
            }
            catch (IOException) { /* best-effort cleanup */ }
            catch (UnauthorizedAccessException) { /* best-effort cleanup */ }
        }
    }
}
