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
///   - Node.js. In producer mode (local + unsharded CI) the setup
///     runs `npm ci` when `node_modules/` is absent and then always
///     invokes `vitepress build` directly (bypassing the docfx
///     `prebuild` hook that would clobber `obj/project.assets.json`).
///     In consumer mode (`ROUTE_SHARD_TOTAL > 1`) the shard consumes
///     a `dist/` produced by the `docs-prepare` CI job and skips the
///     rebuild when `docs/.vitepress/dist/index.html` is present.
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

    /// <summary>Hard deadline for the preview-server bind. `npm ci` and
    /// `vitepress build` are earlier, sequential <c>OneTimeSetUp</c>
    /// stages that already ran to completion before this countdown
    /// starts (see <see cref="RunVitepressBuild"/>), so 60 s covers only
    /// vitepress preview's own startup — anything past that is a real
    /// failure (dist/ missing, port collision, vitepress CLI usage
    /// error) worth surfacing as a TimeoutException with the drained
    /// startup log.</summary>
    private const int ServerReadyTimeoutMs = 60_000;

    /// <summary>Hard deadline for the vitepress build spawned by
    /// <see cref="RunVitepressBuild"/>. 20 minutes bounds the worst
    /// documented cold path (cold node_modules cache + full SSR pass
    /// on a slow runner completes in ~5 min); anything past that
    /// implies a hang (deadlocked worker, HMR loop, wedged fetch)
    /// worth surfacing as an InvalidOperationException with the
    /// drained output rather than a wall-clock CI timeout that
    /// discards the diagnostic.</summary>
    private const int VitepressBuildTimeoutMs = 20 * 60 * 1000;

    /// <summary>Hard deadline for `npm ci`, spawned by
    /// <see cref="RunNpm"/> when <c>docs/node_modules</c> is absent. 10
    /// minutes bounds a cold registry fetch on a slow runner; anything
    /// past that implies a hang (registry outage, interactive prompt,
    /// corrupt lockfile) worth surfacing as an InvalidOperationException
    /// with the drained output rather than hanging OneTimeSetUp
    /// indefinitely with no diagnostic.</summary>
    private const int NpmTimeoutMs = 10 * 60 * 1000;

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
    private static DateTime _fixtureStartTime;
    private static string _distDir = string.Empty;

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
        _fixtureStartTime = DateTime.UtcNow;

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
            _distDir = distDir;

            // Install node_modules if the cache is missing (first run on a
            // clean checkout or after `rm -rf node_modules`). Separated from
            // the build step so a repeated local run skips the network-bound
            // install while still always rebuilding the dist artefact.
            if (!Directory.Exists(Path.Combine(_docsRoot, "node_modules")))
            {
                stage = "npm ci";
                RunNpm("ci", _docsRoot);
            }

            // Producer vs. consumer mode.
            //
            // Producer mode (non-shard local + non-shard CI leg): the
            // fixture is the sole authority on dist/, so it always
            // rebuilds — a warm-cache local run must still walk a tree
            // generated from the CURRENT source markdown, current
            // config.ts, current sidebar, and so on. Honoring a
            // pre-existing dist/index.html sentinel was the earlier
            // policy and it silently walked a stale tree whenever a
            // developer re-ran `dotnet test` after editing source: the
            // walked routes, meta tags, and rendered HTML lagged the
            // source by an arbitrary distance (a stale dist from Jun 2
            // failed the landing-page og:image assertion on Aug 17 for
            // exactly this reason, because the config-side fix that
            // added the https:// og:image URL had landed since the last
            // build).
            //
            // Consumer mode (sharded CI matrix, ROUTE_SHARD_TOTAL > 1):
            // CI's sharded route-check jobs download the dist/ tree
            // from the `docs-prepare` workflow artifact and skip the
            // rebuild. The docs-prepare job is the single docfx-owning
            // producer.
            //
            // Why call vitepress directly instead of `npm run build`:
            // the `prebuild` hook wired into `package.json` runs
            // `docs/scripts/generate-api-ref.sh`, which does a
            // `dotnet build -c Debug --no-incremental` sweep of every
            // library, agent, adapter and module project. Under a full
            // `dotnet test MTConnect.NET.sln -c Release` invocation the
            // solution build is still in flight (multi-TFM Release
            // outputs for net47, net461, net472, net9.0, net10.0, …
            // build in parallel with the net8.0 test hosts), so
            // clobbering each project's `obj/project.assets.json` back
            // to a Debug-only net8.0 view races the Release build and
            // trips NETSDK1005 on every non-net8.0 target that MSBuild
            // has not yet linked. Invoking vitepress directly walks the
            // same source markdown, produces the same dist/, keeps the
            // producer-mode rebuild guarantee, and leaves the obj/
            // tree untouched. The api reference sub-tree under
            // docs/api/ stays as whatever the last regen produced —
            // this fixture does not own that regen (the docs-prepare
            // workflow and `docs/scripts/generate-api-ref.sh` do).
            var distIndex = Path.Combine(distDir, "index.html");
            var (_, shardTotal) = RouteCheckHelpers.ReadShardEnv();
            var isConsumerShard = shardTotal > 1;
            if (!isConsumerShard || !File.Exists(distIndex))
            {
                stage = "vitepress build";
                RunVitepressBuild(_docsRoot);
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
                navTitleText: document.querySelector('.VPNavBarTitle')?.textContent?.trim() ?? null,
                heroImageSrc: (document.querySelector('.VPHero .VPImage')
                    ?? document.querySelector('.VPHero img[src*=""logo""]'))?.getAttribute('src') ?? null,
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
            // og:image must be an absolute https:// URL; LinkedIn, Slack unfurl,
            // and the classic X crawler reject root-relative paths for social cards.
            Assert.That(probes.OgTitle, Is.EqualTo("MTConnect.NET"),
                "og:title meta does not match the expected site title");
            Assert.That(probes.OgImage, Is.Not.Null.And.Not.Empty, "og:image meta missing");
            Assert.That(probes.OgImage, Does.StartWith("https://"),
                $"og:image must be an absolute https:// URL for social-card crawlers — got '{probes.OgImage}'");
            Assert.That(probes.OgImage, Does.EndWith("/logo.png"),
                $"og:image does not point at /logo.png — got '{probes.OgImage}'");

            // Twitter Card — large summary card with the same image.
            Assert.That(probes.TwitterCard, Is.EqualTo("summary_large_image"),
                "twitter:card meta is not 'summary_large_image'");
            Assert.That(probes.TwitterImage, Is.Not.Null.And.Not.Empty, "twitter:image meta missing");
            Assert.That(probes.TwitterImage, Does.StartWith("https://"),
                $"twitter:image must be an absolute https:// URL for social-card crawlers — got '{probes.TwitterImage}'");
            Assert.That(probes.TwitterImage, Does.EndWith("/logo.png"),
                $"twitter:image does not point at /logo.png — got '{probes.TwitterImage}'");

            // Nav title — text-only (per maintainer decision in commit
            // c2041cef "Updated Docs Site" 2026-06-26: the logo was
            // deemed too small to read in the nav bar, so the title
            // reverted to the plain text "MTConnect.NET"). The nav-bar
            // no longer contains an <img>; the wordmark rendering lives
            // in the hero block below.
            Assert.That(probes.NavTitleText, Is.EqualTo("MTConnect.NET"),
                $"nav-bar title text does not match 'MTConnect.NET' — got '{probes.NavTitleText}'");

            // Hero image — pinned per §1.0d-trigies-semel (the maintainer-supplied
            // logo must render in the landing hero block, not just the nav bar).
            // VitePress's default home layout renders the hero image as
            // .VPHero .VPImage when `hero.image.src` is set in index.md;
            // the fallback `.VPHero img[src*='logo']` covers theme-overridden
            // cases where a custom hero component is in play. Per maintainer
            // commit c2041cef ("Updated Docs Site" 2026-06-26), the hero
            // image points at `logo-large.png` — a bigger asset than the
            // 32-px favicon `logo.png` — so the assertion accepts any
            // `/logo*.png` variant to survive future asset swaps in the
            // same shape.
            Assert.That(probes.HeroImageSrc, Is.Not.Null.And.Not.Empty,
                "no image rendered inside .VPHero — hero.image did not take effect");
            Assert.That(probes.HeroImageSrc, Does.Match(@"/logo[^/]*\.png$"),
                $"hero image src does not match /logo*.png — got '{probes.HeroImageSrc}'");

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

        [JsonPropertyName("navTitleText")]
        public string? NavTitleText { get; set; }

        [JsonPropertyName("heroImageSrc")]
        public string? HeroImageSrc { get; set; }

        [JsonPropertyName("downloadCtaHref")]
        public string? DownloadCtaHref { get; set; }
    }

    /// <summary>
    /// Negative companion to <see cref="Landing_Page_Carries_The_House_Style_Surfaces"/>
    /// per §10a: the positive test only proves the hero image
    /// <c>src</c> attribute is wired through to <c>/logo.png</c>, not
    /// that the asset is actually reachable. A future asset-pipeline
    /// regression (renamed file, missed copy step, accidental
    /// <c>.gitignore</c> entry) would pass the meta-/DOM-attribute
    /// check above but break the hero image for end users. Fetching
    /// the asset over HTTP and asserting <c>200 + image/png</c> closes
    /// that gap.
    /// </summary>
    [Test]
    public async Task Landing_Hero_Image_Asset_Resolves()
    {
        Assert.That(_browser, Is.Not.Null, "browser was not initialised");

        var context = await _browser!.NewContextAsync();
        try
        {
            // Reuse the Playwright APIRequest plumbing the favicon check
            // in the positive test already exercises — keeps the
            // negative test on the same HTTP transport as everything
            // else in this fixture (no parallel HttpClient surface to
            // keep in sync with the preview-server lifecycle).
            var assetUrl = _baseUrl + "/logo.png";
            var response = await context.APIRequest.GetAsync(assetUrl);
            Assert.That(response.Status, Is.EqualTo(200),
                $"the hero image asset at {assetUrl} returned HTTP {response.Status}");
            var contentType = response.Headers.TryGetValue("content-type", out var ct) ? ct : null;
            Assert.That(contentType, Is.Not.Null.And.StartsWith("image/png"),
                $"the hero image asset content-type is '{contentType}', expected 'image/png'");
        }
        finally
        {
            await context.CloseAsync();
        }
    }

    /// <summary>
    /// Negative regression for the rebuild-when-needed invariant:
    /// asserts that <c>dist/index.html</c> was written during this
    /// fixture invocation when the fixture is the sole producer of the
    /// dist tree. A regression that silently dropped the rebuild — for
    /// example, swapping the presence sentinel without an alternative
    /// producer — would leave the mtime behind, making the stale-dist
    /// defect observable in CI.
    /// </summary>
    /// <remarks>
    /// Sharded CI runs (matrix env var <c>ROUTE_SHARD_TOTAL</c> &gt; 1)
    /// download the dist artifact from the upstream <c>docs-prepare</c>
    /// job and intentionally bypass the in-fixture build — the
    /// <c>docs-prepare</c> job is the single docfx-owning producer
    /// and each shard is a pure consumer, so the mtime invariant does
    /// not apply and the test is inconclusive. Local invocations and
    /// the unsharded CI leg still enforce it (the producer path
    /// invokes <c>vitepress build</c> directly, bypassing the
    /// <c>package.json</c> <c>prebuild</c> hook so the shard runners'
    /// missing docfx binary is a non-issue for the fixture itself).
    /// </remarks>
    [Test]
    [Category("E2E")]
    public void OneTimeSetUp_Rebuilds_Dist_When_Fixture_Is_Producer()
    {
        var (_, shardTotal) = RouteCheckHelpers.ReadShardEnv();
        if (shardTotal > 1)
        {
            Assert.Inconclusive(
                $"Skipping rebuild-mtime invariant under shard mode (ROUTE_SHARD_TOTAL={shardTotal}); " +
                "the docs-prepare CI job is the dist producer and this shard is a consumer.");
        }

        // Pin the contract for the producer-mode path: the fixture's
        // OneTimeSetUp produces a fresh dist whose mtime is no older
        // than the test fixture invocation start time. A regression
        // that silently dropped the in-fixture build would let mtime
        // fall behind.
        var distIndex = Path.Combine(_distDir, "index.html");
        var distMtime = File.GetLastWriteTimeUtc(distIndex);
        Assert.That(distMtime, Is.GreaterThanOrEqualTo(_fixtureStartTime),
            $"dist/index.html mtime {distMtime:O} is older than fixture start " +
            $"{_fixtureStartTime:O} — OneTimeSetUp skipped the rebuild");
    }

    /// <summary>
    /// Walks every markdown-backed route the docs/ tree implies and
    /// asserts none rendered a 404. Failures are collected across all
    /// workers and reported in a single ordered summary so the diff
    /// is reviewable in one assertion message.
    /// </summary>
    /// <remarks>
    /// Honours the <c>ROUTE_SHARD_INDEX</c> / <c>ROUTE_SHARD_TOTAL</c>
    /// environment variables so a CI matrix can parallelise the walk
    /// across N legs. Unset env vars collapse to the no-sharding
    /// identity path (<c>1 of 1</c> — every route on a single shard);
    /// local <c>dotnet test</c> runs therefore exercise the full route
    /// set without any extra ceremony.
    /// </remarks>
    [Test]
    public async Task Every_Markdown_Backed_Route_Resolves_Without_A_404()
    {
        Assert.That(_browser, Is.Not.Null, "browser was not initialised");

        var allRoutes = RouteCheckHelpers.CollectRoutes(_docsRoot);
        Assert.That(allRoutes.Count, Is.GreaterThan(0), "expected at least one markdown-backed route");

        // CI matrix dimension `shard: [1, 2, 3, 4]` injects the two env
        // vars below; local runs leave them unset and walk every route.
        // The helper returns (1, 1) on unset/unparseable input, so the
        // shard slice collapses to the input on the no-sharding path.
        var (shardIndex, shardTotal) = RouteCheckHelpers.ReadShardEnv();
        var routes = RouteCheckHelpers.ShardRoutes(allRoutes, shardIndex, shardTotal);
        TestContext.Out.WriteLine(
            $"Route shard {shardIndex} of {shardTotal}: walking {routes.Count} of {allRoutes.Count} route(s)");
        // Enumerate the actual route paths the shard owns so a CI
        // failure log is self-describing — a reviewer can see at a
        // glance which subset of the markdown tree this shard walked
        // without re-deriving the modulus mapping by hand.
        foreach (var route in routes)
        {
            TestContext.Out.WriteLine($"  shard {shardIndex}/{shardTotal} route: {route}");
        }

        // A surplus shard (more shards than routes) legitimately walks
        // zero routes; that is success, not failure.
        if (routes.Count == 0)
        {
            TestContext.Out.WriteLine(
                $"shard {shardIndex}/{shardTotal} is empty (more shards than routes); nothing to walk");
            return;
        }

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

    /// <summary>
    /// Invoke the local vitepress binary directly against the docs root,
    /// bypassing the <c>package.json</c> <c>prebuild</c> hook that
    /// <c>npm run build</c> would trigger. The prebuild step runs
    /// <c>docs/scripts/generate-api-ref.sh</c>, which does a
    /// <c>dotnet build -c Debug --no-incremental</c> sweep across the
    /// entire library, agent, adapter and module surface; that sweep
    /// rewrites every touched project's <c>obj/project.assets.json</c>
    /// to a Debug-only <c>net8.0</c> view and races any in-flight
    /// multi-TFM Release build (NETSDK1005 on <c>net47</c>,
    /// <c>net9.0</c>, <c>net10.0</c>, …). This helper resolves
    /// <c>node_modules/vitepress/bin/vitepress.js</c> relative to the
    /// docs root, drains stdout+stderr concurrently to avoid the
    /// classic pipe-deadlock pattern, bounds the child by
    /// <see cref="VitepressBuildTimeoutMs"/> so a wedged worker
    /// surfaces as an actionable exception rather than a job-level
    /// timeout that discards the diagnostic, and rethrows with the
    /// captured output when the child exits non-zero. The
    /// <see cref="Process"/> handle is disposed on every path so a
    /// warm test-runner does not leak file descriptors across reruns.
    /// </summary>
    /// <param name="docsRoot">
    /// Absolute path to the docs site (<c>docs/</c> under the repo
    /// root); becomes the child process's working directory and the
    /// anchor for the <c>node_modules</c> lookup.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the vitepress binary cannot be located,
    /// <see cref="Process.Start(ProcessStartInfo)"/> returns
    /// <see langword="null"/>, the child fails to exit within
    /// <see cref="VitepressBuildTimeoutMs"/> milliseconds (the child
    /// tree is killed before the exception is thrown), or the child
    /// process exits with a non-zero code. The captured stdout and
    /// stderr are appended to the exception message in every failure
    /// mode for diagnosis.
    /// </exception>
    private static void RunVitepressBuild(string docsRoot)
    {
        var vitepressEntry = Path.Combine(docsRoot, "node_modules", "vitepress", "bin", "vitepress.js");
        if (!File.Exists(vitepressEntry))
        {
            throw new InvalidOperationException(
                $"Cannot invoke vitepress build directly — expected entry point at '{vitepressEntry}' does not exist. Run `npm ci` under {docsRoot} first (the OneTimeSetUp does this when node_modules is missing).");
        }

        var psi = new ProcessStartInfo
        {
            FileName = "node",
            WorkingDirectory = docsRoot,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
        };
        // Mirror package.json's `build` script memory budget — vitepress
        // build's Vue SSR pass can exceed V8's default 2 GB old-space
        // when the source tree is thousands of pages.
        psi.ArgumentList.Add("--max-old-space-size=8192");
        psi.ArgumentList.Add(vitepressEntry);
        psi.ArgumentList.Add("build");

        RunProcess(psi, VitepressBuildTimeoutMs, "`node … vitepress.js build`");
    }

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

        RunProcess(psi, NpmTimeoutMs, $"`npm {arguments}`");
    }

    /// <summary>
    /// Spawn <paramref name="psi"/>, drain stdout/stderr concurrently to
    /// avoid the classic pipe-deadlock pattern, bound the wait by
    /// <paramref name="timeoutMs"/> so a wedged child surfaces as an
    /// actionable exception rather than a wall-clock CI timeout that
    /// discards the diagnostic, and rethrow with the captured output on
    /// either a timeout (after killing the child tree) or a non-zero
    /// exit code. Shared by <see cref="RunVitepressBuild"/> and
    /// <see cref="RunNpm"/> so both child-process launches get the same
    /// dispose/timeout/kill hardening rather than diverging over time.
    /// </summary>
    /// <param name="psi">Fully configured start info; stdout/stderr must
    /// already be redirected.</param>
    /// <param name="timeoutMs">Hard wait deadline in milliseconds.</param>
    /// <param name="label">Human-readable command label used in
    /// exception messages (e.g. <c>`npm ci`</c>).</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <see cref="Process.Start(ProcessStartInfo)"/> returns
    /// <see langword="null"/>, the child fails to exit within
    /// <paramref name="timeoutMs"/> milliseconds (the child tree is
    /// killed before the exception is thrown), or the child process
    /// exits with a non-zero code. The captured stdout and stderr are
    /// appended to the exception message in every failure mode for
    /// diagnosis.
    /// </exception>
    private static void RunProcess(ProcessStartInfo psi, int timeoutMs, string label)
    {
        // `using` guarantees the OS handle + redirected pipes are
        // released even when the drain/wait/exit-code path throws — a
        // warm test-runner otherwise accumulates handles and can starve
        // pipes across reruns.
        using var proc = Process.Start(psi)
            ?? throw new InvalidOperationException($"Failed to start {label}");

        var stdoutTask = proc.StandardOutput.ReadToEndAsync();
        var stderrTask = proc.StandardError.ReadToEndAsync();

        // Bound the child wait so a wedged process (deadlocked worker,
        // hung fetch, interactive prompt, infinite loop) surfaces as an
        // actionable exception with the drained partial output rather
        // than a wall-clock CI timeout that discards it.
        if (!proc.WaitForExit(timeoutMs))
        {
            try
            {
                proc.Kill(entireProcessTree: true);
            }
            catch
            {
                // Best-effort — the child may already be exiting; a
                // failure to signal is not itself the diagnostic.
            }
            // Give the drains one last chance to complete after the
            // kill; ignore any fault so the timeout message is what
            // the caller sees.
            try { Task.WaitAll(new[] { stdoutTask, stderrTask }, millisecondsTimeout: 2_000); } catch { }
            var partialStdout = stdoutTask.IsCompletedSuccessfully ? SanitizeForException(stdoutTask.Result) : "<drain incomplete>";
            var partialStderr = stderrTask.IsCompletedSuccessfully ? SanitizeForException(stderrTask.Result) : "<drain incomplete>";
            throw new InvalidOperationException(
                $"{label} did not exit within {timeoutMs} ms — killed the child tree and captured what stdout/stderr had been drained.{Environment.NewLine}stdout:{Environment.NewLine}{partialStdout}{Environment.NewLine}stderr:{Environment.NewLine}{partialStderr}");
        }

        Task.WaitAll(stdoutTask, stderrTask);
        var stdout = SanitizeForException(stdoutTask.Result);
        var stderr = SanitizeForException(stderrTask.Result);

        if (proc.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"{label} exited {proc.ExitCode}{Environment.NewLine}stdout:{Environment.NewLine}{stdout}{Environment.NewLine}stderr:{Environment.NewLine}{stderr}");
        }
    }

    /// <summary>
    /// Strip ASCII control characters (other than newline/carriage
    /// return/tab) from captured child-process output before it is
    /// interpolated into an exception message. A compromised or
    /// misbehaving npm dependency could otherwise emit control
    /// sequences that spoof adjacent lines in aggregated CI log
    /// viewers; this keeps the diagnostic text inert.
    /// </summary>
    /// <param name="text">Raw captured stdout or stderr.</param>
    /// <returns><paramref name="text"/> with disallowed control
    /// characters removed.</returns>
    private static string SanitizeForException(string text)
    {
        return new string(text.Where(c => !char.IsControl(c) || c is '\n' or '\r' or '\t').ToArray());
    }
}
