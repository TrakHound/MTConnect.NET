// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Linq;
using MTConnect.NET_DocsGen;
using NUnit.Framework;

namespace MTConnect.NET_Docs_Tests;

/// <summary>
/// Validates that the auto-generated reference pages under
/// `docs/reference/` are in lock-step with what the same Roslyn
/// inventories produce when re-run inside the test process. Adding
/// a new HTTP endpoint, environment-variable read, or configuration
/// property without regenerating the reference fails this fixture
/// and therefore CI.
///
/// Run locally:
///
///   dotnet test tests/MTConnect.NET-Docs-Tests
///
/// Regenerate the reference pages:
///
///   dotnet run --project build/MTConnect.NET-DocsGen -- --repo .
/// </summary>
[TestFixture]
public class DocsReferenceGenerationTests
{
    private static string RepoRoot
    {
        get
        {
            // bin/Debug/net8.0/ -> three levels up to test project,
            // then one more to `tests/`, then one more to repo root.
            // Strip the trailing path separator first so
            // GetDirectoryName actually ascends each call.
            var dir = AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            for (int i = 0; i < 10 && !string.IsNullOrEmpty(dir); i++)
            {
                if (File.Exists(Path.Combine(dir, "MTConnect.NET.sln"))) return dir;
                dir = Path.GetDirectoryName(dir);
            }
            throw new InvalidOperationException("Could not locate repository root from " + AppContext.BaseDirectory);
        }
    }

    /// <summary>Pins the behaviour expressed by the test name: http api page is in sync with source.</summary>
    [Test]
    public void HttpApi_Page_Is_In_Sync_With_Source()
    {
        var endpoints = RouteInventory.Collect(RepoRoot);
        Assert.That(endpoints.Count, Is.GreaterThan(0), "expected at least one HTTP route");

        var expected = HttpApiRenderer.Render(endpoints);
        var path = Path.Combine(RepoRoot, "docs", "reference", "http-api.md");
        Assert.That(File.Exists(path), Is.True, $"missing {path}");
        var actual = File.ReadAllText(path);

        if (!string.Equals(actual, expected, StringComparison.Ordinal))
        {
            Assert.Fail($"docs/reference/http-api.md is out of sync with the source. Regenerate with:\n  dotnet run --project build/MTConnect.NET-DocsGen -- --repo .");
        }
    }

    /// <summary>Pins the behaviour expressed by the test name: environment variables page is in sync with source.</summary>
    [Test]
    public void EnvironmentVariables_Page_Is_In_Sync_With_Source()
    {
        var vars = EnvVarInventory.Collect(RepoRoot);

        var expected = EnvVarRenderer.Render(vars);
        var path = Path.Combine(RepoRoot, "docs", "reference", "environment-variables.md");
        Assert.That(File.Exists(path), Is.True, $"missing {path}");
        var actual = File.ReadAllText(path);

        if (!string.Equals(actual, expected, StringComparison.Ordinal))
        {
            Assert.Fail("docs/reference/environment-variables.md is out of sync with the source. Regenerate with:\n  dotnet run --project build/MTConnect.NET-DocsGen -- --repo .");
        }
    }

    /// <summary>Pins the behaviour expressed by the test name: configuration page is in sync with source.</summary>
    [Test]
    public void Configuration_Page_Is_In_Sync_With_Source()
    {
        var classes = ConfigInventory.Collect(RepoRoot);
        Assert.That(classes.Count, Is.GreaterThan(0), "expected at least one configuration option class");

        var expected = ConfigRenderer.Render(classes);
        var path = Path.Combine(RepoRoot, "docs", "reference", "configuration.md");
        Assert.That(File.Exists(path), Is.True, $"missing {path}");
        var actual = File.ReadAllText(path);

        if (!string.Equals(actual, expected, StringComparison.Ordinal))
        {
            Assert.Fail("docs/reference/configuration.md is out of sync with the source. Regenerate with:\n  dotnet run --project build/MTConnect.NET-DocsGen -- --repo .");
        }
    }

    /// <summary>Pins the behaviour expressed by the test name: cli page is in sync with source.</summary>
    [Test]
    public void Cli_Page_Is_In_Sync_With_Source()
    {
        var clis = CliInventory.Collect(RepoRoot);
        Assert.That(clis.Count, Is.GreaterThan(0), "expected at least one CLI tool");
        // Shipped agent + adapter must both be present; if either goes
        // missing the inventory has regressed and the page would lose
        // critical surface coverage.
        Assert.That(clis.Any(c => c.Name == "mtconnect.net-agent"), Is.True,
            "mtconnect.net-agent should be discovered as a shipped CLI");
        Assert.That(clis.Any(c => c.Name == "mtconnect.net-adapter"), Is.True,
            "mtconnect.net-adapter should be discovered as a shipped CLI");

        var expected = CliRenderer.Render(clis);
        var path = Path.Combine(RepoRoot, "docs", "reference", "cli.md");
        Assert.That(File.Exists(path), Is.True, $"missing {path}");
        var actual = File.ReadAllText(path);

        if (!string.Equals(actual, expected, StringComparison.Ordinal))
        {
            Assert.Fail("docs/reference/cli.md is out of sync with the source. Regenerate with:\n  dotnet run --project build/MTConnect.NET-DocsGen -- --repo .");
        }
    }

    /// <summary>
    /// Direct pin for the cycle-4 DocsGen bounded-scan fix
    /// (<c>CliInventory.CollectDotNetTool</c>): the <c>takesValue</c>
    /// regex must be bounded to the CURRENT <c>case</c> block, otherwise
    /// a boolean switch flag sitting above a value-taking neighbor
    /// (e.g. <c>--full-tree</c> above <c>case "--output": … RequireValue</c>)
    /// would falsely inherit the neighbor's <c>&lt;value&gt;</c> shape.
    ///
    /// <para>
    /// The golden-file <c>Cli_Page_Is_In_Sync_With_Source</c> test would
    /// also catch this via the rendered <c>cli.md</c>, but a targeted
    /// unit-style pin here surfaces the regression with a branch-scoped
    /// failure message before the golden-file diff is even computed.
    /// </para>
    /// </summary>
    [Test]
    public void SysMLImport_FullTree_Flag_Is_Detected_As_Switch_Not_Value_Flag()
    {
        var clis = CliInventory.Collect(RepoRoot);
        var sysml = clis.FirstOrDefault(c => c.Name == "MTConnect.NET-SysML-Import");
        Assert.That(sysml, Is.Not.Null,
            "MTConnect.NET-SysML-Import must be discovered in the inventory.");

        var fullTree = sysml!.Flags.FirstOrDefault(f => f.Name == "--full-tree");
        Assert.That(fullTree, Is.Not.Null,
            "--full-tree flag must appear in the sysml-import inventory.");
        Assert.That(fullTree!.ArgShape, Is.Null,
            "--full-tree is a boolean switch (case body: `fullTree = true; break;`). "
            + "The bounded RequireValue scan must NOT leak in the value shape from the "
            + "neighboring --output / --json-dump cases. An ArgShape of `<value>` here "
            + "means the bounded-scan regex regressed to an unbounded lookahead.");
    }

    /// <summary>Pins the behaviour expressed by the test name: endpoint code has no stale entries in markdown.</summary>
    [Test]
    public void Endpoint_Code_Has_No_Stale_Entries_In_Markdown()
    {
        // Inverse check: every fenced `GET /path` heading in the
        // markdown must be backed by a row in the freshly collected
        // inventory. Catches the case where a route is removed in
        // source but the markdown still references it.
        var endpoints = RouteInventory.Collect(RepoRoot);
        var liveKeys = endpoints
            .Select(e => $"{e.Method} {e.PathTemplate} — {e.Source}")
            .ToHashSet();

        var path = Path.Combine(RepoRoot, "docs", "reference", "http-api.md");
        var lines = File.ReadAllLines(path);
        foreach (var line in lines)
        {
            // Look for "### `GET /probe` &mdash; Ceen"
            if (!line.StartsWith("### `", StringComparison.Ordinal)) continue;
            var endTick = line.IndexOf('`', 5);
            if (endTick < 0) continue;
            var verbPath = line.Substring(5, endTick - 5);
            var mdashIdx = line.IndexOf("&mdash;", StringComparison.Ordinal);
            if (mdashIdx < 0) continue;
            var source = line.Substring(mdashIdx + "&mdash;".Length).Trim();
            var docKey = $"{verbPath} — {source}";
            Assert.That(liveKeys, Does.Contain(docKey),
                $"docs lists endpoint that no longer exists in source: {docKey}");
        }
    }

    /// <summary>Pins the behaviour expressed by the test name: index page is in sync.</summary>
    [Test]
    public void Index_Page_Is_In_Sync()
    {
        var expected = IndexRenderer.Render();
        var path = Path.Combine(RepoRoot, "docs", "reference", "index.md");
        Assert.That(File.Exists(path), Is.True, $"missing {path}");
        Assert.That(File.ReadAllText(path), Is.EqualTo(expected));
    }
}
