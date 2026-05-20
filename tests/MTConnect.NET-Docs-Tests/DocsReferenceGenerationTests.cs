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

    [Test]
    public void Index_Page_Is_In_Sync()
    {
        var expected = IndexRenderer.Render();
        var path = Path.Combine(RepoRoot, "docs", "reference", "index.md");
        Assert.That(File.Exists(path), Is.True, $"missing {path}");
        Assert.That(File.ReadAllText(path), Is.EqualTo(expected));
    }
}
