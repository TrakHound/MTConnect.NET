// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
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
/// Run locally:
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
    private const int Concurrency = 8;
    private const int ServerReadyPollMs = 200;
    private const int ServerReadyTimeoutMs = 60_000;
    private const int PageNavigationTimeoutMs = 15_000;

    private Process? _previewServer;
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private string _baseUrl = string.Empty;
    private string _docsRoot = string.Empty;
    private readonly System.Text.StringBuilder _previewLog = new();

    private static string RepoRoot
    {
        get
        {
            var dir = AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            for (int i = 0; i < 10 && !string.IsNullOrEmpty(dir); i++)
            {
                if (File.Exists(Path.Combine(dir, "MTConnect.NET.sln"))) return dir;
                dir = Path.GetDirectoryName(dir)!;
            }
            throw new InvalidOperationException("Could not locate repository root from " + AppContext.BaseDirectory);
        }
    }

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
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
            RunNpm("ci", _docsRoot);
            RunNpm("run build", _docsRoot);
        }

        // Install the chromium binary the Playwright .NET binding drives.
        // Idempotent: re-installs cleanly if the cache is already warm.
        var installExit = Microsoft.Playwright.Program.Main(new[] { "install", "chromium" });
        if (installExit != 0)
        {
            throw new InvalidOperationException($"`playwright install chromium` exited {installExit}");
        }

        var port = FindFreePort();
        _baseUrl = $"http://127.0.0.1:{port}";
        _previewServer = StartPreviewServer(port, distDir, _docsRoot, _previewLog);
        await WaitForServerAsync(port, _previewServer, _previewLog);

        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        if (_browser is not null) await _browser.CloseAsync();
        _playwright?.Dispose();
        StopPreviewServer(_previewServer);
    }

    [Test]
    public async Task Every_Markdown_Backed_Route_Resolves_Without_A_404()
    {
        Assert.That(_browser, Is.Not.Null, "browser was not initialised");

        var routes = CollectRoutes(_docsRoot);
        Assert.That(routes.Count, Is.GreaterThan(0), "expected at least one markdown-backed route");

        var failures = new List<(string Route, string Indicator)>();
        var failuresLock = new object();
        using var semaphore = new SemaphoreSlim(Concurrency);

        var tasks = routes.Select(async route =>
        {
            await semaphore.WaitAsync();
            try
            {
                var failure = await CheckRouteAsync(_browser!, _baseUrl, route);
                if (failure is not null)
                {
                    lock (failuresLock) failures.Add(failure.Value);
                }
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);

        if (failures.Count > 0)
        {
            var ordered = failures.OrderBy(f => f.Route, StringComparer.Ordinal).ToList();
            var lines = string.Join(Environment.NewLine, ordered.Select(f => $"  {f.Route} — {f.Indicator}"));
            Assert.Fail($"{ordered.Count} route(s) returned a 404:{Environment.NewLine}{lines}");
        }
    }

    // ─── Route derivation ────────────────────────────────────────────────────

    // Convert an absolute .md file path to the VitePress route it maps to.
    // Mirrors the original Node crawler's mdFileToRoute:
    //   docs/index.md           -> /
    //   docs/getting-started.md -> /getting-started
    //   docs/reference/index.md -> /reference/
    //   docs/reference/cli.md   -> /reference/cli
    private static string MdFileToRoute(string docsRoot, string absPath)
    {
        var rel = absPath.Substring(docsRoot.Length).Replace(Path.DirectorySeparatorChar, '/');
        if (!rel.StartsWith('/')) rel = "/" + rel;
        if (rel == "/index.md") return "/";
        if (rel.EndsWith("/index.md", StringComparison.Ordinal))
            return rel.Substring(0, rel.Length - "index.md".Length);
        return rel.Substring(0, rel.Length - ".md".Length);
    }

    private static List<string> CollectRoutes(string docsRoot)
    {
        var files = new List<string>();
        CollectMarkdownFiles(docsRoot, files);
        return files
            .Select(f => MdFileToRoute(docsRoot, f))
            .Distinct(StringComparer.Ordinal)
            .OrderBy(r => r, StringComparer.Ordinal)
            .ToList();
    }

    private static void CollectMarkdownFiles(string dir, List<string> results)
    {
        foreach (var entry in Directory.EnumerateFileSystemEntries(dir))
        {
            var name = Path.GetFileName(entry);
            if (Directory.Exists(entry))
            {
                // Skip node_modules and any dot-prefixed directory
                // (.vitepress carries the build cache + dist).
                if (name == "node_modules" || name.StartsWith('.')) continue;
                CollectMarkdownFiles(entry, results);
            }
            else if (File.Exists(entry) && entry.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
            {
                results.Add(entry);
            }
        }
    }

    // ─── Route check ─────────────────────────────────────────────────────────

    // Detects VitePress's client-side 404 page. The original Node
    // crawler used three signals; mirror them here exactly so the
    // test's failure surface matches what the script caught.
    private static async Task<(string Route, string Indicator)?> CheckRouteAsync(IBrowser browser, string baseUrl, string route)
    {
        var url = baseUrl + route;
        var page = await browser.NewPageAsync();
        try
        {
            await page.GotoAsync(url, new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle,
                Timeout = PageNavigationTimeoutMs,
            });

            var detection = await page.EvaluateAsync<NotFoundDetection>(@"() => ({
                hasClass: !!document.querySelector('.NotFound'),
                title404: document.title === '404',
                bodyMatches: (document.body?.innerText ?? '').toUpperCase().includes('PAGE NOT FOUND')
            })");

            if (detection.HasClass) return (route, ".NotFound element present");
            if (detection.Title404) return (route, "document.title == '404'");
            if (detection.BodyMatches) return (route, "body text contains 'PAGE NOT FOUND'");
            return null;
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    private sealed class NotFoundDetection
    {
        public bool HasClass { get; set; }
        public bool Title404 { get; set; }
        public bool BodyMatches { get; set; }
    }

    // ─── Preview server lifecycle ────────────────────────────────────────────

    private static int FindFreePort()
    {
        var listener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, 0);
        listener.Start();
        var port = ((System.Net.IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    private static Process StartPreviewServer(int port, string distDir, string docsRoot, System.Text.StringBuilder log)
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
        // error).
        _ = Task.Run(async () =>
        {
            try
            {
                string? line;
                while ((line = await proc.StandardOutput.ReadLineAsync()) is not null)
                {
                    lock (log) log.AppendLine("[stdout] " + line);
                }
            }
            catch { /* process exited */ }
        });
        _ = Task.Run(async () =>
        {
            try
            {
                string? line;
                while ((line = await proc.StandardError.ReadLineAsync()) is not null)
                {
                    lock (log) log.AppendLine("[stderr] " + line);
                }
            }
            catch { /* process exited */ }
        });

        return proc;
    }

    private static async Task WaitForServerAsync(int port, Process proc, System.Text.StringBuilder log)
    {
        var deadline = DateTime.UtcNow.AddMilliseconds(ServerReadyTimeoutMs);
        while (DateTime.UtcNow < deadline)
        {
            if (proc.HasExited)
            {
                string snapshot;
                lock (log) snapshot = log.ToString();
                throw new InvalidOperationException(
                    $"vitepress preview exited prematurely with code {proc.ExitCode} before binding to 127.0.0.1:{port}.{Environment.NewLine}{snapshot}");
            }
            try
            {
                using var client = new TcpClient();
                await client.ConnectAsync(System.Net.IPAddress.Loopback, port);
                return;
            }
            catch (SocketException)
            {
                await Task.Delay(ServerReadyPollMs);
            }
        }
        string finalSnapshot;
        lock (log) finalSnapshot = log.ToString();
        throw new TimeoutException(
            $"vitepress preview did not bind to 127.0.0.1:{port} within {ServerReadyTimeoutMs / 1000}s.{Environment.NewLine}{finalSnapshot}");
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

        // Drain both pipes so the child doesn't block; surface output
        // on failure for diagnostics.
        var stdout = proc.StandardOutput.ReadToEnd();
        var stderr = proc.StandardError.ReadToEnd();
        proc.WaitForExit();

        if (proc.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"`npm {arguments}` exited {proc.ExitCode}{Environment.NewLine}stdout:{Environment.NewLine}{stdout}{Environment.NewLine}stderr:{Environment.NewLine}{stderr}");
        }
    }
}
