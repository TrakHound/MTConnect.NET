// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;

namespace MTConnect.NET_Docs_Tests;

/// <summary>
/// End-to-end route walk against the built VitePress site. Spawns
/// `vitepress preview` over the docs/.vitepress/dist/ artifact, then
/// navigates every route the markdown source tree implies in a headless
/// Chromium browser and fails on any client-side 404. Catches the failure
/// modes a filesystem link checker cannot see: VitePress router
/// misregistration (e.g. the trailing-slash-vs-cleanUrls bug that hid in
/// the original sidebar config), missing static assets, JS errors in
/// custom theme components.
///
/// Run locally (from the repo root):
///
///   dotnet test tests/MTConnect.NET-Docs-Tests --filter Category=E2E
///
/// Prerequisites:
///   - Node.js (the setup invokes `npm ci` + `npm run build` if the
///     docs/.vitepress/dist/ artifact is missing).
///   - The Microsoft.Playwright package's chromium browser binary
///     (installed automatically by the fixture's one-time setup).
/// </summary>
[TestFixture]
[Category("E2E")]
public class RouteCheckTests
{
    /// <summary>Worker count for the parallel route walk. 8 is the
    /// empirical sweet spot on a high-core developer laptop — beyond
    /// that, vitepress preview's single Node event loop becomes the
    /// bottleneck and per-route wall time degrades. Capping at the
    /// visible CPU count keeps a 2-vCPU GitHub-hosted runner from
    /// over-subscribing (the cap collapses to ProcessorCount there).</summary>
    private static readonly int Concurrency = Math.Min(8, Environment.ProcessorCount);

    /// <summary>Backoff between TCP-probe attempts while waiting for
    /// vitepress preview to bind its port. 200 ms keeps the busy-loop
    /// cost trivial while still ringing the door bell ~5x per second.</summary>
    private const int ServerReadyPollMs = 200;

    /// <summary>Hard deadline for the preview-server bind. 60 s
    /// accommodates a cold CI runner where `npm ci` + `npm run build`
    /// + vitepress startup land before the first port probe — anything
    /// past that is a real failure (dist/ missing, port collision,
    /// vitepress CLI usage error) worth surfacing as a TimeoutException
    /// with the drained startup log.</summary>
    private const int ServerReadyTimeoutMs = 60_000;

    /// <summary>Per-page navigation timeout. 30 s covers a slow runner
    /// with a cold network cache; anything past that is a real failure
    /// (vitepress hang, JS exception that prevents Load) worth failing
    /// the route on rather than waiting indefinitely.</summary>
    private const int PageNavigationTimeoutMs = 30_000;

    private Process? _previewServer;
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private string _baseUrl = string.Empty;
    private string _docsRoot = string.Empty;
    private readonly System.Text.StringBuilder _previewLog = new();
    private Task? _previewStdoutDrain;
    private Task? _previewStderrDrain;

    // Walk-up bound is intentionally large (32) rather than fitting the
    // current `bin/Debug/netN.N/` suffix exactly. A deeper container path,
    // a vendored / submoduled checkout, or a future test-output relayout
    // can extend the chain without re-tripping this guard. The exception
    // message names both the starting BaseDirectory and the depth walked
    // so a future failure is diagnosable from the assertion alone. The
    // walk itself lives in RouteCheckHelpers so it can be unit-tested
    // without bootstrapping the Node + Playwright fixture.
    private static string RepoRoot =>
        RouteCheckHelpers.LocateRepoRoot(AppContext.BaseDirectory, "MTConnect.NET.sln");

    /// <summary>
    /// Build the docs site if needed, install the Playwright chromium
    /// binary, spawn `vitepress preview` against the built dist/ tree,
    /// and launch a headless browser ready for the route walk. Wraps
    /// every stage in a try/catch that rethrows with stage context +
    /// drained preview log so a partial failure is diagnosable from
    /// the assertion message alone.
    /// </summary>
    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        // Wrap the bootstrap so a partial failure (npm bootstrap throw,
        // chromium-install non-zero exit, preview-server bind timeout)
        // rethrows with whatever state was captured up to that point —
        // the partial `_previewLog`, the bootstrap stage that failed —
        // rather than a bare exception with no context. OneTimeTearDown
        // runs unconditionally and cleans up whatever was allocated.
        var stage = "init";
        try
        {
            _docsRoot = Path.Combine(RepoRoot, "docs");
            var distDir = Path.Combine(_docsRoot, ".vitepress", "dist");

            // The test owns its build prerequisite. If the dist/ tree
            // doesn't already exist, run `npm ci && npm run build` from
            // docs/ to produce it. `npm run build` invokes the `prebuild`
            // hook (regenerate api + reference), so the rendered site
            // matches what CI builds on every push. The presence check
            // looks for index.html specifically because a partial /
            // stale dist tree (e.g. just an assets/ subdirectory left
            // from a prior aborted run) is not a usable preview target.
            if (!File.Exists(Path.Combine(distDir, "index.html")))
            {
                stage = "npm ci";
                RunNpm("ci", _docsRoot);
                stage = "npm run build";
                RunNpm("run build", _docsRoot);
            }

            // Install the chromium binary the Playwright .NET binding drives.
            // Idempotent: re-installs cleanly if the cache is already warm.
            stage = "playwright install chromium";
            var installExit = Microsoft.Playwright.Program.Main(new[] { "install", "chromium" });
            if (installExit != 0)
            {
                throw new InvalidOperationException($"`playwright install chromium` exited {installExit}");
            }

            // Hand the port allocation off to vitepress so there is no TOCTOU
            // window between picking a free port and binding it. `--port 0` is
            // a hint, not a guarantee — vitepress can ignore it and pick its
            // own default (5173) — so the actual bound port is parsed off the
            // drained startup banner (`Local: http://localhost:NNNN/`).
            stage = "start preview server";
            var port = RouteCheckHelpers.FindFreePort();
            (_previewServer, _previewStdoutDrain, _previewStderrDrain) =
                StartPreviewServer(port, distDir, _docsRoot, _previewLog);
            var boundPort = await WaitForServerAsync(port, _previewServer, _previewLog);
            _baseUrl = $"http://127.0.0.1:{boundPort}";

            stage = "launch chromium";
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
        }
        catch (Exception ex)
        {
            string snapshot;
            lock (_previewLog) snapshot = _previewLog.ToString();
            var context = string.IsNullOrEmpty(snapshot)
                ? $"OneTimeSetUp failed at stage '{stage}' with no preview output captured."
                : $"OneTimeSetUp failed at stage '{stage}'. Preview server output captured so far:{Environment.NewLine}{snapshot}";
            throw new InvalidOperationException(context, ex);
        }
    }

    /// <summary>
    /// Close the browser, dispose the Playwright runtime, kill the
    /// preview-server process tree, and await the drain tasks with a
    /// bounded timeout. Runs unconditionally — including after a
    /// OneTimeSetUp failure — so a partially-allocated state still
    /// gets torn down.
    /// </summary>
    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        if (_browser is not null) await _browser.CloseAsync();
        _playwright?.Dispose();
        StopPreviewServer(_previewServer);
        // Drain tasks complete when ReadLineAsync returns null (the pipes
        // close once Kill takes the child down). A small bounded wait stops
        // a stuck reader from holding teardown indefinitely.
        await AwaitDrainAsync(_previewStdoutDrain);
        await AwaitDrainAsync(_previewStderrDrain);
    }

    private static async Task AwaitDrainAsync(Task? drain)
    {
        if (drain is null) return;
        var completed = await Task.WhenAny(drain, Task.Delay(2_000));
        if (completed == drain)
        {
            try { await drain; }
            catch (IOException) { /* pipe closed mid-read on Kill */ }
            catch (ObjectDisposedException) { /* process disposed */ }
        }
    }

    /// <summary>
    /// Negative regression for the 404 detector: visits an unmapped
    /// route and asserts at least one of the two signals (.NotFound
    /// element, document.title starting with '404') fires. Without
    /// this, a future Playwright / VitePress upgrade that broke a
    /// signal would silently pass-through every real 404.
    /// </summary>
    [Test]
    public async Task A_Synthetic_Unmapped_Route_Surfaces_As_A_404()
    {
        // Pins the detector itself per §10a. If a future Playwright / VitePress
        // upgrade silently breaks one of the two signals (.NotFound element,
        // document.title startsWith '404'), the positive test would still pass
        // — every real markdown-backed route would continue to render fine —
        // but a real 404 would go undetected. This test makes sure the detector
        // fires on a URL that has no markdown source behind it.
        Assert.That(_browser, Is.Not.Null, "browser was not initialised");
        var context = await _browser!.NewContextAsync();
        try
        {
            var failure = await CheckRouteAsync(context, _baseUrl, "/this-route-does-not-exist");
            Assert.That(failure, Is.Not.Null,
                "expected /this-route-does-not-exist to surface as a 404, but the detector returned no indicator");
        }
        finally
        {
            await context.CloseAsync();
        }
    }

    /// <summary>
    /// Pins the docs-site house-style surfaces the maintainer signed off
    /// on in Discussion #184 (see <c>docs/development/house-style.md</c>):
    /// the brand logo wired through <c>themeConfig.logo</c>, the favicon
    /// link, the Open Graph + Twitter Card meta block, the
    /// <c>theme-color</c> meta, and the 'Download latest release' hero
    /// CTA pointing at the GitHub releases page.
    /// </summary>
    /// <remarks>
    /// Each check is a focused property assertion on the rendered HTML
    /// so the failure message names the missing surface directly
    /// ('og:image meta tag missing', 'theme-color meta is #ffffff,
    /// expected #0073e6'). A single bundled assertion would surface
    /// 'page does not match expected snapshot' and force the reader
    /// into a diff — the structured form is the §10a positive-+-negative
    /// pin per surface, all in one test.
    ///
    /// The favicon asset is also fetched over HTTP to guarantee the
    /// <c>href</c> resolves — a stale path that 404s would still
    /// satisfy the meta-tag presence check, so the fetch closes the
    /// gap between 'tag exists' and 'tag works'.
    /// </remarks>
    [Test]
    public async Task Landing_Page_Carries_The_House_Style_Surfaces()
    {
        Assert.That(_browser, Is.Not.Null, "browser was not initialised");

        var context = await _browser!.NewContextAsync();
        try
        {
            var page = await context.NewPageAsync();
            var response = await page.GotoAsync(_baseUrl + "/", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.Load,
                Timeout = PageNavigationTimeoutMs,
            });
            Assert.That(response, Is.Not.Null, "homepage navigation returned no response");
            Assert.That(response!.Ok, Is.True, $"homepage returned HTTP {response.Status}");

            var probes = await page.EvaluateAsync<HouseStyleProbes>(@"() => ({
                themeColor: document.querySelector('meta[name=""theme-color""]')?.getAttribute('content') ?? null,
                faviconHref: document.querySelector('link[rel=""icon""]')?.getAttribute('href') ?? null,
                faviconType: document.querySelector('link[rel=""icon""]')?.getAttribute('type') ?? null,
                ogTitle: document.querySelector('meta[property=""og:title""]')?.getAttribute('content') ?? null,
                ogImage: document.querySelector('meta[property=""og:image""]')?.getAttribute('content') ?? null,
                twitterCard: document.querySelector('meta[name=""twitter:card""]')?.getAttribute('content') ?? null,
                twitterImage: document.querySelector('meta[name=""twitter:image""]')?.getAttribute('content') ?? null,
                heroLogoSrc: document.querySelector('.VPNavBarTitle img.logo')?.getAttribute('src') ?? null,
                downloadCtaHref: (() => {
                    const link = Array.from(document.querySelectorAll('.VPHero a, a'))
                        .find(a => /Download latest release/i.test(a.textContent ?? ''));
                    return link ? link.getAttribute('href') : null;
                })()
            })");

            // theme-color — brand accent per Discussion #184.
            Assert.That(probes.ThemeColor, Is.EqualTo("#0073e6"),
                "theme-color meta does not match the maintainer-confirmed brand accent");

            // Favicon — present, PNG, points at /logo.png (base-prefixed
            // when DOCS_BASE is set, so endsWith() is the stable match).
            Assert.That(probes.FaviconHref, Is.Not.Null.And.Not.Empty, "favicon <link rel=icon> missing");
            Assert.That(probes.FaviconHref, Does.EndWith("/logo.png"),
                $"favicon href does not point at /logo.png — got '{probes.FaviconHref}'");
            Assert.That(probes.FaviconType, Is.EqualTo("image/png"),
                "favicon type attribute is not 'image/png'");

            // Open Graph — title + image surface so social previews render.
            Assert.That(probes.OgTitle, Is.EqualTo("MTConnect.NET"),
                "og:title meta does not match the expected site title");
            Assert.That(probes.OgImage, Is.Not.Null.And.Not.Empty, "og:image meta missing");
            Assert.That(probes.OgImage, Does.EndWith("/logo.png"),
                $"og:image does not point at /logo.png — got '{probes.OgImage}'");

            // Twitter Card — large summary card with the same image.
            Assert.That(probes.TwitterCard, Is.EqualTo("summary_large_image"),
                "twitter:card meta is not 'summary_large_image'");
            Assert.That(probes.TwitterImage, Is.Not.Null.And.Not.Empty, "twitter:image meta missing");
            Assert.That(probes.TwitterImage, Does.EndWith("/logo.png"),
                $"twitter:image does not point at /logo.png — got '{probes.TwitterImage}'");

            // themeConfig.logo — VitePress renders the logo as an <img>
            // inside .VPNavBarTitle when themeConfig.logo is set. The
            // text site title is hidden (themeConfig.siteTitle: false)
            // because the logo PNG already carries the wordmark.
            Assert.That(probes.HeroLogoSrc, Is.Not.Null.And.Not.Empty,
                "no <img> rendered inside .VPNavBarTitle — themeConfig.logo did not take effect");
            Assert.That(probes.HeroLogoSrc, Does.EndWith("/logo.png"),
                $"nav logo src does not point at /logo.png — got '{probes.HeroLogoSrc}'");

            // Hero 'Download latest release' CTA — text + canonical link.
            Assert.That(probes.DownloadCtaHref, Is.EqualTo(
                    "https://github.com/TrakHound/MTConnect.NET/releases/latest"),
                "'Download latest release' hero CTA missing or points elsewhere");

            // Closing the gap between 'tag present' and 'tag works':
            // fetch the favicon over HTTP and assert it returns 200.
            // A stale logo path (e.g. an old `/favicon.ico` reference
            // after a rename) would satisfy the meta-tag check above
            // but break the favicon for end users.
            var faviconUrl = probes.FaviconHref!.StartsWith("http", StringComparison.Ordinal)
                ? probes.FaviconHref
                : _baseUrl + probes.FaviconHref;
            var faviconResponse = await page.Context.APIRequest.GetAsync(faviconUrl);
            Assert.That(faviconResponse.Status, Is.EqualTo(200),
                $"favicon at {faviconUrl} returned HTTP {faviconResponse.Status}");

            await page.CloseAsync();
        }
        finally
        {
            await context.CloseAsync();
        }
    }

    // The JS payload returned by the house-style probe uses camelCase
    // keys; pin them explicitly so a future Playwright upgrade that
    // tightens case-insensitive deserialisation cannot silently turn
    // every probe into a null pass-through (which would hide a
    // regression in the rendered meta surface).
    private sealed class HouseStyleProbes
    {
        [JsonPropertyName("themeColor")]
        public string? ThemeColor { get; set; }

        [JsonPropertyName("faviconHref")]
        public string? FaviconHref { get; set; }

        [JsonPropertyName("faviconType")]
        public string? FaviconType { get; set; }

        [JsonPropertyName("ogTitle")]
        public string? OgTitle { get; set; }

        [JsonPropertyName("ogImage")]
        public string? OgImage { get; set; }

        [JsonPropertyName("twitterCard")]
        public string? TwitterCard { get; set; }

        [JsonPropertyName("twitterImage")]
        public string? TwitterImage { get; set; }

        [JsonPropertyName("heroLogoSrc")]
        public string? HeroLogoSrc { get; set; }

        [JsonPropertyName("downloadCtaHref")]
        public string? DownloadCtaHref { get; set; }
    }

    /// <summary>
    /// Walks every markdown-backed route the docs/ tree implies and
    /// asserts none rendered a 404. Failures are collected across all
    /// workers and reported in a single ordered summary so the diff
    /// is reviewable in one assertion message.
    /// </summary>
    [Test]
    public async Task Every_Markdown_Backed_Route_Resolves_Without_A_404()
    {
        Assert.That(_browser, Is.Not.Null, "browser was not initialised");

        var routes = RouteCheckHelpers.CollectRoutes(_docsRoot);
        Assert.That(routes.Count, Is.GreaterThan(0), "expected at least one markdown-backed route");

        var failures = await WalkRoutesAsync(_browser!, _baseUrl, routes, Concurrency);

        if (failures.Count > 0)
        {
            var ordered = failures.OrderBy(f => f.Route, StringComparer.Ordinal).ToList();
            var lines = string.Join(Environment.NewLine, ordered.Select(f => $"  {f.Route} — {f.Indicator}"));
            Assert.Fail($"{ordered.Count} route(s) returned a 404:{Environment.NewLine}{lines}");
        }
    }

    // Walk every route with one BrowserContext per worker. Allocating a
    // context once per worker (vs once per route via Browser.NewPageAsync)
    // amortises the ~0.5–1 s context-creation cost over the full share
    // and exercises VitePress's warm-router path on the second visit
    // onwards. Expected 5–10× wall-time reduction over per-route contexts.
    private static async Task<List<(string Route, string Indicator)>> WalkRoutesAsync(
        IBrowser browser, string baseUrl, List<string> routes, int workerCount)
    {
        var failures = new List<(string Route, string Indicator)>();
        var failuresLock = new object();

        // FIFO queue of routes; workers pull from the same queue so
        // a slow page on one worker doesn't strand its pre-allocated
        // share — work-stealing falls out for free.
        var queue = new System.Collections.Concurrent.ConcurrentQueue<string>(routes);

        var workers = Enumerable.Range(0, Math.Min(workerCount, routes.Count))
            .Select(async _ =>
            {
                var context = await browser.NewContextAsync();
                try
                {
                    while (queue.TryDequeue(out var route))
                    {
                        var failure = await CheckRouteAsync(context, baseUrl, route);
                        if (failure is not null)
                        {
                            lock (failuresLock) failures.Add(failure.Value);
                        }
                    }
                }
                finally
                {
                    await context.CloseAsync();
                }
            });

        await Task.WhenAll(workers);
        return failures;
    }

    // ─── Route check ─────────────────────────────────────────────────────────

    // Detects VitePress's client-side 404 page. Two signals are used:
    // the `.NotFound` element rendered by the default theme's NotFound
    // component, and the `<title>` element — which the static 404.html
    // emits as `404 | <site title>` (e.g. `404 | MTConnect.NET`), so a
    // prefix match on `404` catches the title regardless of the trailing
    // site-title suffix.
    //
    // The original detector also checked for body text containing
    // `PAGE NOT FOUND`, but that signal is too loose — a real
    // markdown-backed route may quote the phrase in prose (e.g. the
    // docs-site page that documents this very detector). Dropping it
    // avoids false positives without sacrificing coverage: every real
    // VitePress 404 still renders the `.NotFound` element and the
    // `404 | ...` title.
    //
    // WaitUntilState.Load (not NetworkIdle) is used here deliberately.
    // VitePress's SPA keeps background work running indefinitely —
    // analytics pings, web-vitals beacons, hot-reload polling — so
    // NetworkIdle never settles within any reasonable timeout. The
    // `<title>` signal is server-rendered into the static 404.html so
    // it is available at Load; the `.NotFound` signal appears once Vue
    // hydrates, which on a built site happens during the Load event's
    // sub-resource phase.
    private static async Task<(string Route, string Indicator)?> CheckRouteAsync(IBrowserContext context, string baseUrl, string route)
    {
        var url = baseUrl + route;
        var page = await context.NewPageAsync();
        try
        {
            await page.GotoAsync(url, new PageGotoOptions
            {
                WaitUntil = WaitUntilState.Load,
                Timeout = PageNavigationTimeoutMs,
            });

            var detection = await page.EvaluateAsync<NotFoundDetection>(@"() => ({
                hasClass: !!document.querySelector('.NotFound'),
                title404: (document.title ?? '').startsWith('404')
            })");

            if (detection.HasClass) return (route, ".NotFound element present");
            if (detection.Title404) return (route, "document.title starts with '404'");
            return null;
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    // The JS payload returned by EvaluateAsync uses camelCase keys
    // (`hasClass`, `title404`); pin them explicitly so a future
    // Playwright upgrade that tightens case-insensitive deserialisation
    // cannot silently turn every route into a no-detection pass-through
    // (which would hide a real 404).
    private sealed class NotFoundDetection
    {
        [JsonPropertyName("hasClass")]
        public bool HasClass { get; set; }

        [JsonPropertyName("title404")]
        public bool Title404 { get; set; }
    }

    // ─── Preview server lifecycle ────────────────────────────────────────────

    private static (Process Process, Task StdoutDrain, Task StderrDrain) StartPreviewServer(int port, string distDir, string docsRoot, System.Text.StringBuilder log)
    {
        // Use `npx vitepress preview` so the local node_modules copy
        // is invoked without needing a global install. On Windows the
        // `npx` shim is npx.cmd; ProcessStartInfo doesn't auto-resolve
        // the extension, so name it explicitly.
        var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        var fileName = isWindows ? "npx.cmd" : "npx";

        var psi = new ProcessStartInfo
        {
            FileName = fileName,
            WorkingDirectory = docsRoot,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
        };
        psi.ArgumentList.Add("vitepress");
        psi.ArgumentList.Add("preview");
        psi.ArgumentList.Add("--port");
        psi.ArgumentList.Add(port.ToString(System.Globalization.CultureInfo.InvariantCulture));
        psi.ArgumentList.Add("--outDir");
        psi.ArgumentList.Add(distDir);

        var proc = Process.Start(psi)
            ?? throw new InvalidOperationException("Failed to start `vitepress preview` process");

        // Drain stdout/stderr into the shared log buffer so the
        // child doesn't block on a full pipe and so the readiness
        // wait can surface the actual error if the preview fails
        // before binding (e.g. dist/ missing, port collision the
        // free-port finder lost the race to, vitepress CLI usage
        // error). The drain tasks are tracked so OneTimeTearDown can
        // await them after Kill; an exception narrower than `catch`
        // keeps real failures visible.
        var stdoutDrain = DrainPipeAsync(proc.StandardOutput, log, "[stdout] ");
        var stderrDrain = DrainPipeAsync(proc.StandardError, log, "[stderr] ");

        return (proc, stdoutDrain, stderrDrain);
    }

    private static Task DrainPipeAsync(System.IO.StreamReader reader, System.Text.StringBuilder log, string prefix)
    {
        return Task.Run(async () =>
        {
            try
            {
                string? line;
                while ((line = await reader.ReadLineAsync()) is not null)
                {
                    lock (log) log.AppendLine(prefix + line);
                }
            }
            catch (IOException) { /* pipe closed mid-read on Kill */ }
            catch (ObjectDisposedException) { /* process disposed */ }
        });
    }

    private static async Task<int> WaitForServerAsync(int requestedPort, Process proc, System.Text.StringBuilder log)
    {
        var deadline = DateTime.UtcNow.AddMilliseconds(ServerReadyTimeoutMs);
        while (DateTime.UtcNow < deadline)
        {
            if (proc.HasExited)
            {
                string snapshot;
                lock (log) snapshot = log.ToString();
                throw new InvalidOperationException(
                    $"vitepress preview exited prematurely with code {proc.ExitCode} before binding to 127.0.0.1:{requestedPort}.{Environment.NewLine}{snapshot}");
            }

            // Prefer the port reported in the startup banner — it is the
            // authoritative answer to "which port did the child actually
            // bind?". Falls back to the requested port if the banner has
            // not yet been drained. ExtractBannerPort lives in
            // RouteCheckHelpers so the parse is unit-tested separately.
            var observedPort = RouteCheckHelpers.ExtractBannerPort(log) ?? requestedPort;
            try
            {
                using var client = new TcpClient();
                await client.ConnectAsync(System.Net.IPAddress.Loopback, observedPort);
                return observedPort;
            }
            catch (SocketException)
            {
                await Task.Delay(ServerReadyPollMs);
            }
        }
        string finalSnapshot;
        lock (log) finalSnapshot = log.ToString();
        throw new TimeoutException(
            $"vitepress preview did not bind to 127.0.0.1:{requestedPort} within {ServerReadyTimeoutMs / 1000}s.{Environment.NewLine}{finalSnapshot}");
    }

    private static void StopPreviewServer(Process? proc)
    {
        if (proc is null) return;
        try
        {
            if (!proc.HasExited)
            {
                // Kill the whole process tree — `npx` spawns
                // `vitepress`, which spawns the node preview server;
                // killing only npx leaves the actual server orphaned.
                proc.Kill(entireProcessTree: true);
                proc.WaitForExit(5_000);
            }
        }
        catch
        {
            /* best-effort cleanup */
        }
        finally
        {
            proc.Dispose();
        }
    }

    // ─── npm bootstrap ───────────────────────────────────────────────────────

    private static void RunNpm(string arguments, string workingDirectory)
    {
        var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        var fileName = isWindows ? "npm.cmd" : "npm";

        var psi = new ProcessStartInfo
        {
            FileName = fileName,
            WorkingDirectory = workingDirectory,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
        };
        foreach (var token in arguments.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            psi.ArgumentList.Add(token);
        }

        var proc = Process.Start(psi)
            ?? throw new InvalidOperationException($"Failed to start `npm {arguments}`");

        // Drain both pipes concurrently so the child doesn't block on a
        // full stderr buffer while the parent waits on stdout — the
        // classic pipe-deadlock pattern. `npm ci` emits stderr volume in
        // the form of deprecation warnings and peer-dep notices that on a
        // verbose run can exceed the OS pipe buffer (typically 64 KB).
        var stdoutTask = proc.StandardOutput.ReadToEndAsync();
        var stderrTask = proc.StandardError.ReadToEndAsync();
        Task.WaitAll(stdoutTask, stderrTask);
        var stdout = stdoutTask.Result;
        var stderr = stderrTask.Result;
        proc.WaitForExit();

        if (proc.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"`npm {arguments}` exited {proc.ExitCode}{Environment.NewLine}stdout:{Environment.NewLine}{stdout}{Environment.NewLine}stderr:{Environment.NewLine}{stderr}");
        }
    }
}
