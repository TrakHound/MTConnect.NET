// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.NET_DocsGen;

// DocsGen entry point. Walks the repository source tree (Roslyn-only,
// no compiled assemblies) and emits the auto-generated VitePress
// reference pages:
//
//   docs/reference/http-api.md
//   docs/reference/environment-variables.md
//   docs/reference/configuration.md
//   docs/reference/cli.md
//
// Usage:
//   dotnet run --project build/MTConnect.NET-DocsGen \
//       -- --repo <repo-root> [--out <docs-root>]
//
// Flags:
//   --repo <path>  Repository root. Required.
//   --out  <path>  Docs root. Defaults to "<repo>/docs".
//   --check        Run the inventory but do NOT write files; exits 0
//                  iff the inventory matches what is already on disk.
//
// The same inventory routines are exposed as public static methods on
// `RouteInventory`, `EnvVarInventory`, `ConfigInventory`, and
// `CliInventory` so the validation test in `tests/` can re-run them
// independently and assert the on-disk markdown is in lock-step with
// the source tree.

string? repoRoot = null;
string? docsRoot = null;
bool checkOnly = false;

for (int i = 0; i < args.Length; i++)
{
    switch (args[i])
    {
        case "--repo":
            repoRoot = RequireValue(args, ref i, "--repo");
            break;
        case "--out":
            docsRoot = RequireValue(args, ref i, "--out");
            break;
        case "--check":
            checkOnly = true;
            break;
        case "-h":
        case "--help":
            PrintHelp();
            return 0;
        default:
            Console.Error.WriteLine($"Unknown argument: {args[i]}");
            PrintHelp();
            return 2;
    }
}

if (string.IsNullOrEmpty(repoRoot))
{
    Console.Error.WriteLine("--repo is required.");
    PrintHelp();
    return 2;
}

repoRoot = Path.GetFullPath(repoRoot);
docsRoot ??= Path.Combine(repoRoot, "docs");
docsRoot = Path.GetFullPath(docsRoot);

if (!Directory.Exists(repoRoot))
{
    Console.Error.WriteLine($"Repository root does not exist: {repoRoot}");
    return 2;
}

var referenceDir = Path.Combine(docsRoot, "reference");
Directory.CreateDirectory(referenceDir);

Console.WriteLine($"==> repo : {repoRoot}");
Console.WriteLine($"==> docs : {docsRoot}");

var routes = RouteInventory.Collect(repoRoot);
var envVars = EnvVarInventory.Collect(repoRoot);
var configs = ConfigInventory.Collect(repoRoot);
var clis = CliInventory.Collect(repoRoot);

Console.WriteLine($"    routes      : {routes.Count}");
Console.WriteLine($"    env-vars    : {envVars.Count}");
Console.WriteLine($"    config keys : {configs.Sum(c => c.Properties.Count)} across {configs.Count} types");
Console.WriteLine($"    CLIs        : {clis.Count} ({clis.Count(c => c.Category == "shipped")} shipped, {clis.Count(c => c.Category == "contributor")} contributor)");

var httpMd = HttpApiRenderer.Render(routes);
var envMd = EnvVarRenderer.Render(envVars);
var configMd = ConfigRenderer.Render(configs);
var cliMd = CliRenderer.Render(clis);

var paths = new (string path, string content)[]
{
    (Path.Combine(referenceDir, "http-api.md"), httpMd),
    (Path.Combine(referenceDir, "environment-variables.md"), envMd),
    (Path.Combine(referenceDir, "configuration.md"), configMd),
    (Path.Combine(referenceDir, "cli.md"), cliMd),
    (Path.Combine(referenceDir, "index.md"), IndexRenderer.Render()),
};

if (checkOnly)
{
    bool drift = false;
    foreach (var (path, expected) in paths)
    {
        if (!File.Exists(path))
        {
            Console.Error.WriteLine($"MISSING: {path}");
            drift = true;
            continue;
        }
        var actual = File.ReadAllText(path);
        if (!string.Equals(actual, expected, StringComparison.Ordinal))
        {
            Console.Error.WriteLine($"DRIFT  : {path}");
            drift = true;
        }
    }
    if (drift)
    {
        Console.Error.WriteLine("docs reference is out of date; re-run without --check to regenerate.");
        return 1;
    }
    Console.WriteLine("==> reference pages match source inventory.");
    return 0;
}

foreach (var (path, content) in paths)
{
    File.WriteAllText(path, content);
    Console.WriteLine($"    wrote {Path.GetRelativePath(repoRoot, path)} ({content.Length:N0} chars)");
}

Console.WriteLine("==> done.");
return 0;

static string RequireValue(string[] args, ref int i, string flag)
{
    if (i + 1 >= args.Length)
    {
        throw new InvalidOperationException($"{flag} requires a value");
    }
    return args[++i];
}

static void PrintHelp()
{
    Console.WriteLine("dotnet run --project build/MTConnect.NET-DocsGen --");
    Console.WriteLine("    --repo  <repo-root>   Repository root (required)");
    Console.WriteLine("    --out   <docs-root>   Docs root (defaults to <repo>/docs)");
    Console.WriteLine("    --check               Verify pages are in sync; exit 1 on drift");
}
