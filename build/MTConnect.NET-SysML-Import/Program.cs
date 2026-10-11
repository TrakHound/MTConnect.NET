using MTConnect.SysML;
using MTConnect.SysML.CSharp;
using MTConnect.SysML.Json_cppagent;
using MTConnect.SysML.Xml;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

// SysML importer entry point. Runs on Linux / macOS / Windows / CI.
//
// Usage:
//     dotnet run --project build/MTConnect.NET-SysML-Import \
//         -- --new-xmi <path-to-MTConnectSysMLModel.xml> \
//            --output <repo-root> \
//            [--previous-xmi <path-to-prior-XMI>] \
//            [--compat-version-label <label>] \
//            [--full-tree] \
//            [--json-dump <path>]
//
// Flags:
//   --new-xmi <path>        SysML XMI file to consume. Required. Preferred
//                           spelling; --xmi remains as a legacy alias.
//   --xmi <path>            Legacy alias for --new-xmi. Kept for backwards
//                           compatibility with existing callers; new invocations
//                           should prefer --new-xmi.
//   --output <path>         Repository root. Each subgenerator writes into its
//                           own libraries/<LibraryName>/ subtree under this
//                           root. Required.
//   --previous-xmi <path>   Edge-case override for delta-driven mode. When
//                           supplied, uses this file as the previous-version
//                           XMI and skips the zero-config auto-derive step.
//                           Typical use cases: cross-version audit runs,
//                           regenerating against a historical XMI snapshot,
//                           and version-bumps that skip a version (where
//                           MTConnectVersions.Max does not match the intended
//                           PREV_VERSION).
//   --compat-version-label <label>
//                           Label used for the Compat/<label>.g.cs file name in
//                           delta mode. When --previous-xmi is supplied without
//                           an explicit label, defaults to "Previous" for
//                           backwards compatibility. When the previous-XMI is
//                           auto-derived from MTConnectVersions.Max, defaults
//                           to "v${PREV_XY_UNDERSCORE}" (e.g. "v2_7").
//   --full-tree             Explicit opt-in for the full-regeneration path.
//                           Disables both the zero-config auto-derive delta
//                           and the --previous-xmi override; every emitted
//                           .g.cs re-lands under its normal path.
//   --json-dump <path>      Optional. Writes the parsed MTConnectModel as JSON
//                           for debugging.
//
// Zero-config delta mode (default):
//   When neither --previous-xmi nor --full-tree is supplied, the importer
//   auto-derives PREV_VERSION from MTConnectVersions.Max (parsed out of
//   libraries/MTConnect.NET-Common/MTConnectVersions.cs under --output) and
//   resolves the prior-version XMI via one of:
//     Strategy B (primary): build/.cache/sysml-prev/MTConnectSysMLModel_v${PREV_VERSION}.xml
//     Strategy A (fallback): build/sysml-model/MTConnectSysMLModel.xml, gated
//                            on the submodule being checked out to tag
//                            v${PREV_VERSION} exactly.
//     Strategy C (fail-hard): neither resolves — throws with an actionable
//                             message naming both probed paths and pointing at
//                             the --previous-xmi override + the --full-tree
//                             escape hatch.
//
// See build/MTConnect.NET-SysML-Import/README.md for the full usage guide,
// the "Adding a new MTConnect Standard version" runbook, the determinism
// guarantee (regen against a pinned XMI tag must produce zero diff), and the
// delta-mode design notes (plan D4 — partial-class re-emit, not
// [TypeForwardedTo]).

string? newXmiPath = null;
string? previousXmiPath = null;
string? compatVersionLabel = null;
string? outputRoot = null;
string? jsonDumpPath = null;
bool fullTree = false;

for (int i = 0; i < args.Length; i++)
{
    switch (args[i])
    {
        case "--new-xmi":
            newXmiPath = RequireValue(args, ref i, "--new-xmi");
            break;
        case "--xmi":
            // Legacy alias for --new-xmi. Preserved for backwards compatibility.
            newXmiPath = RequireValue(args, ref i, "--xmi");
            break;
        case "--previous-xmi":
            previousXmiPath = RequireValue(args, ref i, "--previous-xmi");
            break;
        case "--compat-version-label":
            compatVersionLabel = RequireValue(args, ref i, "--compat-version-label");
            break;
        case "--full-tree":
            fullTree = true;
            break;
        case "--output":
            outputRoot = RequireValue(args, ref i, "--output");
            break;
        case "--json-dump":
            jsonDumpPath = RequireValue(args, ref i, "--json-dump");
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

if (string.IsNullOrEmpty(newXmiPath))
{
    // Legacy callers passed --xmi; the required-flag message keeps the same
    // spelling so grep-based operator scripts continue to match.
    Console.Error.WriteLine("error: --new-xmi <path> is required (legacy alias --xmi is still accepted).");
    PrintHelp();
    return 2;
}

if (!File.Exists(newXmiPath))
{
    Console.Error.WriteLine($"error: XMI file not found: {newXmiPath}");
    return 1;
}

if (previousXmiPath is not null && !File.Exists(previousXmiPath))
{
    Console.Error.WriteLine($"error: --previous-xmi file not found: {previousXmiPath}");
    return 1;
}

if (string.IsNullOrEmpty(outputRoot))
{
    Console.Error.WriteLine("error: --output <path> is required.");
    PrintHelp();
    return 2;
}

if (!Directory.Exists(outputRoot))
{
    Console.Error.WriteLine($"error: Output root not found: {outputRoot}");
    return 1;
}

// Delta-mode source selection: --full-tree overrides everything; else
// --previous-xmi if explicit; else zero-config auto-derive from
// MTConnectVersions.Max. The expected auto-derive failure modes
// (InvalidOperationException from a parse-shape miss or a Strategy-C fail-hard;
// FileNotFoundException from a missing MTConnectVersions.cs) are caught and
// mapped to a clean `error: ...` + exit 1 so the operator sees an actionable
// message on stderr rather than a stack trace on stdout. Any other exception
// class (e.g. IOException from a filesystem race, UnauthorizedAccessException)
// is intentionally NOT caught here — those escape to the runtime and surface
// as an unhandled stack trace, which is the right signal for an unexpected
// host-level failure that the operator-facing recovery text cannot address.
string? resolvedPreviousXmi = null;
string? autoDerivedCompatLabel = null;
if (!fullTree)
{
    if (previousXmiPath is not null)
    {
        resolvedPreviousXmi = previousXmiPath;
    }
    else
    {
        try
        {
            var (resolvedXmi, previousVersion) = ResolvePreviousXmi(outputRoot);
            resolvedPreviousXmi = resolvedXmi;
            autoDerivedCompatLabel = $"v{previousVersion.Major}_{previousVersion.Minor}";

            // PREV == NEW guard: when the new XMI's filename encodes the same
            // version as the auto-derived PREV_VERSION (from MTConnectVersions.Max),
            // the delta is empty by construction — MTConnectVersions.Max already
            // matches the version being generated. Log a warning and no-op
            // (skip delta generation, return success 0). The filename convention
            // is `MTConnectSysMLModel_v<major>.<minor>.xml`; the case-insensitive
            // `_[vV]` prefix admits both the lowercase and uppercase-V variants
            // seen historically.
            var newVersionMatch = Regex.Match(
                Path.GetFileName(newXmiPath),
                @"_[vV](?<major>\d+)\.(?<minor>\d+)\.xml$");
            if (newVersionMatch.Success)
            {
                var newMajor = int.Parse(newVersionMatch.Groups["major"].Value);
                var newMinor = int.Parse(newVersionMatch.Groups["minor"].Value);
                if (newMajor == previousVersion.Major && newMinor == previousVersion.Minor)
                {
                    Console.Error.WriteLine(
                        $"warning: latest MTConnect version (v{previousVersion.Major}.{previousVersion.Minor}) is already supported by MTConnectVersions.Max — no delta to derive; skipping emit.");
                    return 0;
                }
            }
        }
        catch (Exception ex) when (ex is InvalidOperationException or FileNotFoundException)
        {
            // Both throw sites carry actionable operator-facing messages
            // (probed cache path, expected submodule tag, override flag,
            // escape hatch). Surface the message on stderr with an `error:`
            // prefix and return the standard "runtime failure" exit code
            // (1) so the operator sees a clean CLI failure and not an
            // unhandled-exception stack trace.
            Console.Error.WriteLine($"error: {ex.Message}");
            return 1;
        }
    }
}

// Compat-label defaulting: explicit --compat-version-label wins; else the
// auto-derived "v${X}_${Y}" when we're in zero-config mode; else the legacy
// "Previous" default (preserved for callers passing an explicit
// --previous-xmi without a label).
bool compatLabelIsAutoDerived = compatVersionLabel is null && autoDerivedCompatLabel is not null;
compatVersionLabel ??= autoDerivedCompatLabel ?? "Previous";

// --compat-version-label flows through Path.Combine(compatDir, $"{label}.g.cs")
// and must produce a safe, contained filename on every host. Reject anything
// that would escape the Compat/ directory (path-separator sequences), select
// a legal but surprising target (drive letters, NUL bytes, ASCII controls),
// or produce a hidden dotfile. The auto-derived "v${X}_${Y}" shape and the
// legacy "Previous" default always pass; hostile operator input rejects here.
if (!IsSafeCompatLabel(compatVersionLabel))
{
    Console.Error.WriteLine(
        $"error: --compat-version-label value '{compatVersionLabel}' is not a safe filename. " +
        "Allowed shape: 1 to 64 characters, ASCII letters / digits / '_' / '-' / '.', no leading dot.");
    return 2;
}

Console.WriteLine($"XMI:    {newXmiPath}");
if (fullTree)
{
    Console.WriteLine("Mode:   full-tree (--full-tree; delta paths disabled)");
}
else if (previousXmiPath is not null)
{
    Console.WriteLine($"Prev:   {resolvedPreviousXmi}");
    Console.WriteLine($"Label:  {compatVersionLabel}");
    Console.WriteLine("Mode:   delta (--previous-xmi override)");
}
else
{
    Console.WriteLine($"Prev:   {resolvedPreviousXmi} (auto-derived from MTConnectVersions.Max)");
    // Annotate "(auto-derived)" only when the label WAS auto-derived. When
    // the operator passed an explicit --compat-version-label alongside the
    // zero-config prev-XMI path, that explicit label wins the ??= above,
    // and annotating it "(auto-derived)" would be a lie.
    Console.WriteLine(compatLabelIsAutoDerived
        ? $"Label:  {compatVersionLabel} (auto-derived)"
        : $"Label:  {compatVersionLabel}");
    Console.WriteLine("Mode:   delta (zero-config)");
}
Console.WriteLine($"Output: {outputRoot}");
if (jsonDumpPath is not null)
    Console.WriteLine($"JSON:   {jsonDumpPath}");

if (jsonDumpPath is not null)
{
    var mtconnectModelForDump = MTConnectModel.Parse(newXmiPath);
    if (mtconnectModelForDump == null)
    {
        Console.Error.WriteLine($"error: Failed to parse XMI: {newXmiPath}");
        return 1;
    }
    RenderJsonFile(mtconnectModelForDump, jsonDumpPath);
}

if (fullTree || resolvedPreviousXmi is null)
{
    // Full-tree mode (either explicit --full-tree or a caller that has
    // somehow reached here without a resolved previous XMI). Preserves the
    // pre-Phase-4 behavior bit-for-bit.
    var mtconnectModel = MTConnectModel.Parse(newXmiPath);
    if (mtconnectModel == null)
    {
        // Fail fast on a null model. The renderers below internally null-check
        // and silently no-op, producing zero output and exit 0. Surface the parse
        // failure here so the operator gets a proper non-zero exit + stderr.
        Console.Error.WriteLine($"error: Failed to parse XMI: {newXmiPath}");
        return 1;
    }
    Console.WriteLine($"Model parsed: type={mtconnectModel.GetType().Name}");

    Console.WriteLine("Rendering C# common classes...");
    RenderCommonClasses(mtconnectModel, outputRoot);
    Console.WriteLine("Rendering JSON-cppagent formatters...");
    RenderJsonComponents(mtconnectModel, outputRoot);
    Console.WriteLine("Rendering XML formatters...");
    RenderXmlComponents(mtconnectModel, outputRoot);
    Console.WriteLine("Done.");
    return 0;
}

// Delta mode. Render both XMIs into isolated scratch directories, then diff at
// the file level and emit only the changed/added files into outputRoot,
// concentrating unchanged files into Compat/<label>.g.cs per library.
return RenderDelta(newXmiPath, resolvedPreviousXmi, outputRoot, compatVersionLabel!);


static string RequireValue(string[] argv, ref int index, string flag)
{
    index++;
    if (index >= argv.Length)
        throw new ArgumentException($"Flag '{flag}' requires a value.");
    return argv[index];
}

// Validates that --compat-version-label produces a safe on-disk filename inside
// the per-library Compat/ directory. Rejects: null / empty / whitespace, any
// path separator or drive letter, ASCII control chars, leading dots (hidden
// files), and lengths outside 1..64 chars. The default "Previous" always
// passes; auto-derived "v${X}_${Y}" labels always pass; hostile inputs like
// "../../etc/passwd" or "Compat/../secret" reject at argument-parse time.
static bool IsSafeCompatLabel(string? label)
{
    if (string.IsNullOrWhiteSpace(label)) return false;
    if (label.Length > 64) return false;
    return Regex.IsMatch(label, @"^[A-Za-z0-9_\-][A-Za-z0-9_\-.]*$");
}

// Auto-derives the previous-version XMI path from MTConnectVersions.Max in the
// current tree state. Returns the resolved XMI path plus the parsed Version so
// the caller can derive the auto-derived Compat label. Throws with an
// actionable message when neither Strategy B (cache) nor Strategy A (submodule
// tag) resolves.
//
// Strategy A (fallback): build/sysml-model/MTConnectSysMLModel.xml, gated on
// the submodule tip being checked out exactly at tag v${PREV_VERSION}.
//
// Strategy B (primary): build/.cache/sysml-prev/MTConnectSysMLModel_v${PREV_VERSION}.xml.
//
// Strategy C (fail-hard): neither resolves — throw naming both probed paths so
// the operator can either populate the cache (Phase 3.2 of the version-bump
// runbook), re-check the submodule tag, pass --previous-xmi <path> explicitly,
// or pass --full-tree to disable delta mode.
static (string XmiPath, Version PreviousVersion) ResolvePreviousXmi(string outputRoot)
{
    var previousVersion = ReadMTConnectVersionsMax(outputRoot);

    // Strategy B (primary): cache path.
    var cachePath = Path.Combine(
        outputRoot, "build", ".cache", "sysml-prev",
        $"MTConnectSysMLModel_v{previousVersion.Major}.{previousVersion.Minor}.xml");
    if (File.Exists(cachePath))
    {
        return (cachePath, previousVersion);
    }

    // Strategy A (fallback): submodule tag check.
    var submoduleDir = Path.Combine(outputRoot, "build", "sysml-model");
    var submoduleXmi = Path.Combine(submoduleDir, "MTConnectSysMLModel.xml");
    var expectedTag = $"v{previousVersion.Major}.{previousVersion.Minor}";
    if (Directory.Exists(submoduleDir) && File.Exists(submoduleXmi))
    {
        var currentTag = TryGetSubmoduleTag(submoduleDir);
        if (currentTag is not null && string.Equals(currentTag, expectedTag, StringComparison.Ordinal))
        {
            return (submoduleXmi, previousVersion);
        }
    }

    // Strategy C: fail-hard with an actionable message.
    throw new InvalidOperationException(
        $"PREV_VERSION auto-derivation from MTConnectVersions.Max = {previousVersion.Major}.{previousVersion.Minor} failed. " +
        $"Neither cache path '{cachePath}' nor submodule tag '{expectedTag}' resolved. " +
        "Pass --previous-xmi <path> explicitly to override, or --full-tree to disable delta mode.");
}

// Parses MTConnectVersions.cs under `outputRoot` and returns the version that
// `Max` currently names. The parser locates the `public static Version Max =>
// VersionXY;` line, resolves `VersionXY` to its `new Version(X, Y)` literal
// below, and returns that Version. Text parsing keeps the importer free of a
// runtime dependency on MTConnect.NET-Common (which would create an awkward
// generator-emits-into-its-own-dependency ordering during clean rebuilds).
//
// Both regexes are line-anchored (`(?m)^[ \t]*public…`) so a stale
// `// Max => Version27;` decoy above the real declaration cannot match — the
// `//` sits between line start and `public`, breaking the anchor. A last-match
// preference (`.Matches().Last()`) is applied on both patterns so a
// hypothetical block-commented decoy of the shape
// `/* … public static Version Max => Version28; … */`
// still loses to the live line below it. This is the F-SIMP-501 shrink;
// no comment-stripper walker is required for the two-shape MTConnectVersions.cs
// surface.
static Version ReadMTConnectVersionsMax(string outputRoot)
{
    var versionsPath = Path.Combine(
        outputRoot, "libraries", "MTConnect.NET-Common", "MTConnectVersions.cs");
    if (!File.Exists(versionsPath))
    {
        throw new FileNotFoundException(
            $"MTConnectVersions.cs not found at {versionsPath}. " +
            "The auto-derive path needs this file to determine PREV_VERSION. " +
            "Pass --previous-xmi <path> explicitly to bypass the auto-derive, " +
            "or --full-tree to disable delta mode.",
            versionsPath);
    }

    // Read the source raw and rely on a line-anchored regex to skip
    // `// Max => Version27;` decoys — the `[ \t]*public` prefix at line start
    // (multiline mode) cannot match a `//`-commented line because the `//`
    // sits between line start and `public`. This is the F-SIMP-501 shrink:
    // no comment stripper, no shared-source link, no literal-aware walker;
    // just a targeted anchor plus a last-match preference so a hypothetical
    // block-commented decoy above the real declaration still loses to the
    // live line below it.
    var source = File.ReadAllText(versionsPath);

    // Match `public static Version Max => VersionXY;` at line start (multiline)
    // so a `// public static Version Max => Version27;` decoy is skipped by
    // the `[ \t]*` prefix which admits only whitespace before `public`. The
    // naming convention is enforced by the hand-authored constant table
    // above the Max property.
    var maxMatches = Regex.Matches(source, @"(?m)^[ \t]*public\s+static\s+Version\s+Max\s*=>\s*Version(?<xy>\d+)\s*;");
    if (maxMatches.Count == 0)
    {
        throw new InvalidOperationException(
            $"Could not locate `public static Version Max => VersionXY;` in {versionsPath}. " +
            "The auto-derive path relies on the documented naming convention. " +
            "Pass --previous-xmi <path> explicitly to bypass the auto-derive, " +
            "or --full-tree to disable delta mode.");
    }

    var xy = maxMatches[maxMatches.Count - 1].Groups["xy"].Value;

    // Match `public static readonly Version VersionXY = new Version(X, Y);`
    // so we can recover the major.minor pair. Accepts optional whitespace and
    // the `new(...)` target-typed form as well as the explicit `new Version(...)`.
    // Same line-anchor discipline as above.
    var constMatches = Regex.Matches(
        source,
        $@"(?m)^[ \t]*public\s+static\s+readonly\s+Version\s+Version{xy}\s*=\s*new(?:\s+Version)?\s*\(\s*(?<major>\d+)\s*,\s*(?<minor>\d+)\s*\)\s*;");
    if (constMatches.Count == 0)
    {
        throw new InvalidOperationException(
            $"Could not locate `public static readonly Version Version{xy} = new Version(X, Y);` in {versionsPath}. " +
            "The auto-derive path relies on the documented naming convention. " +
            "Pass --previous-xmi <path> explicitly to bypass the auto-derive, " +
            "or --full-tree to disable delta mode.");
    }

    var lastConst = constMatches[constMatches.Count - 1];
    var major = int.Parse(lastConst.Groups["major"].Value);
    var minor = int.Parse(lastConst.Groups["minor"].Value);
    return new Version(major, minor);
}

// Runs `git -C <submoduleDir> describe --exact-match --tags HEAD` and returns
// the tag name on success, null on any failure (non-git dir, no exact-match
// tag, git binary absent, non-zero exit). The tag-mismatch path is a normal
// zero-config outcome (Phase 3 checks the submodule out to the NEW-VERSION
// tag; the auto-derive expects PREV_VERSION), so failure here is a routine
// signal for "try the next strategy", not a fatal condition.
static string? TryGetSubmoduleTag(string submoduleDir)
{
    try
    {
        var psi = new ProcessStartInfo("git")
        {
            WorkingDirectory = submoduleDir,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        psi.ArgumentList.Add("-C");
        psi.ArgumentList.Add(submoduleDir);
        psi.ArgumentList.Add("describe");
        psi.ArgumentList.Add("--exact-match");
        psi.ArgumentList.Add("--tags");
        psi.ArgumentList.Add("HEAD");

        using var proc = Process.Start(psi);
        if (proc is null)
            return null;

        var stdoutTask = proc.StandardOutput.ReadToEndAsync();
        var stderrTask = proc.StandardError.ReadToEndAsync();
        System.Threading.Tasks.Task.WhenAll(stdoutTask, stderrTask).GetAwaiter().GetResult();
        proc.WaitForExit();

        if (proc.ExitCode != 0)
            return null;

        return stdoutTask.Result.Trim();
    }
    catch (Exception)
    {
        // Any exception path (git binary missing, permission denied, submodule
        // dir doesn't even hold a .git file) reduces to "try the next strategy".
        return null;
    }
}

static void PrintHelp()
{
    Console.WriteLine("""
        MTConnect.NET SysML Importer

        Usage:
          dotnet run --project build/MTConnect.NET-SysML-Import -- \
            --new-xmi <path-to-MTConnectSysMLModel.xml> \
            --output <repo-root> \
            [--previous-xmi <path-to-prior-XMI>] \
            [--compat-version-label <label>] \
            [--full-tree] \
            [--json-dump <path>]

        Zero-config delta mode is the default: PREV_VERSION is auto-derived
        from MTConnectVersions.Max in the current tree and the prior-version
        XMI is resolved from build/.cache/sysml-prev/ or the build/sysml-model
        submodule when it is checked out at the matching tag.

        Pass --full-tree to force full regeneration (both delta paths off).
        Pass --previous-xmi <path> to override the auto-derived prior XMI.
        The legacy --xmi flag is accepted as an alias for --new-xmi.

        See build/MTConnect.NET-SysML-Import/README.md for the full guide.
        """);
}

static void RenderJsonFile(MTConnectModel model, string path)
{
    var jsonOptions = new JsonSerializerOptions
    {
        WriteIndented = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    // --json-dump is operator-trusted (no path-traversal guard); echo the
    // resolved absolute path so the operator can verify exactly where the
    // dump landed when running with a relative path or a sibling-clone
    // launchSettings profile.
    var resolved = Path.GetFullPath(path);
    Console.WriteLine($"JSON dump: writing to {resolved}");

    var dir = Path.GetDirectoryName(resolved);
    if (!string.IsNullOrEmpty(dir))
        Directory.CreateDirectory(dir);

    var json = JsonSerializer.Serialize(model, options: jsonOptions);
    File.WriteAllText(resolved, json);
}

static void RenderCommonClasses(MTConnectModel model, string outputRoot)
{
    var outputPath = Path.Combine(outputRoot, "libraries", "MTConnect.NET-Common");
    if (!Directory.Exists(outputPath))
        throw new DirectoryNotFoundException($"MTConnect.NET-Common not found under output root: {outputPath}");

    CSharpTemplateRenderer.Render(model, outputPath);
}

static void RenderJsonComponents(MTConnectModel model, string outputRoot)
{
    var outputPath = Path.Combine(outputRoot, "libraries", "MTConnect.NET-JSON-cppagent");
    if (!Directory.Exists(outputPath))
        throw new DirectoryNotFoundException($"MTConnect.NET-JSON-cppagent not found under output root: {outputPath}");

    JsonCppAgentTemplateRenderer.Render(model, outputPath);
}

static void RenderXmlComponents(MTConnectModel model, string outputRoot)
{
    var outputPath = Path.Combine(outputRoot, "libraries", "MTConnect.NET-XML");
    if (!Directory.Exists(outputPath))
        throw new DirectoryNotFoundException($"MTConnect.NET-XML not found under output root: {outputPath}");

    XmlTemplateRenderer.Render(model, outputPath);
}

// Delta-driven regen. Renders both --new-xmi and --previous-xmi to isolated
// scratch trees, then emits the diff:
//
//   - Files present only in the NEW tree (ADDED) → written to outputRoot.
//   - Files present in both trees with different bytes (CHANGED) → written
//     to outputRoot (NEW tree's version).
//   - Files present only in the PREV tree (REMOVED) → DELETED from outputRoot
//     so the type stops shipping (the spec dropped it).
//   - Files present in both trees with identical bytes (UNCHANGED) →
//     concentrated into Compat/<label>.g.cs per library, per plan D4. The
//     individual .g.cs at outputRoot is DELETED so the Compat file is the
//     sole namespace host (avoids CS0101 duplicate-type errors when the
//     delta runs against a repo already carrying the committed .g.cs tree,
//     which is the realistic use case).
//
// The Compat file concatenates the unchanged files' bodies verbatim (each
// still carries its own `namespace X { ... }` block, so multi-namespace
// concentration is legal C#). Byte-identity of unchanged files is preserved,
// satisfying the Phase 4.1 invariant that Compat/<PrevVersion>.g.cs is
// `git diff`-clean against the deterministically-emitted full-tree output.
static int RenderDelta(string newXmiPath, string prevXmiPath, string outputRoot, string compatLabel)
{
    var scratchRoot = Path.Combine(Path.GetTempPath(), $"mtc-sysml-delta-{Guid.NewGuid():N}");
    var prevScratch = Path.Combine(scratchRoot, "prev");
    var newScratch = Path.Combine(scratchRoot, "new");

    try
    {
        Console.WriteLine($"Delta scratch: {scratchRoot}");
        Console.WriteLine("Rendering previous-XMI full tree to scratch...");
        RenderFullTreeToScratch(prevXmiPath, prevScratch);
        Console.WriteLine("Rendering new-XMI full tree to scratch...");
        RenderFullTreeToScratch(newXmiPath, newScratch);

        Console.WriteLine("Diffing scratch trees and emitting delta...");
        var stats = EmitDelta(prevScratch, newScratch, outputRoot, compatLabel);

        Console.WriteLine(
            $"Delta emission: added={stats.Added}, changed={stats.Changed}, " +
            $"unchanged-concentrated={stats.UnchangedConcentrated}, removed-skipped={stats.RemovedSkipped}, " +
            $"compat-files-written={stats.CompatFilesWritten}");
        Console.WriteLine("Done.");
        return 0;
    }
    finally
    {
        if (Directory.Exists(scratchRoot))
        {
            try { Directory.Delete(scratchRoot, recursive: true); }
            catch (IOException ex)
            {
                Console.Error.WriteLine($"warning: could not clean scratch {scratchRoot}: {ex.Message}");
            }
        }
    }
}

// Runs the full-tree renderer pipeline against `xmiPath`, writing every
// `.g.cs` artefact into <scratchRoot>/libraries/<library>/... Mirrors the
// full-tree branch above so the emitted bytes are byte-identical to what
// the operator would get from `--new-xmi <xmiPath> --output <scratchRoot>`
// with no `--previous-xmi`.
static void RenderFullTreeToScratch(string xmiPath, string scratchRoot)
{
    Directory.CreateDirectory(scratchRoot);
    Directory.CreateDirectory(Path.Combine(scratchRoot, "libraries", "MTConnect.NET-Common"));
    Directory.CreateDirectory(Path.Combine(scratchRoot, "libraries", "MTConnect.NET-JSON-cppagent"));
    Directory.CreateDirectory(Path.Combine(scratchRoot, "libraries", "MTConnect.NET-XML"));

    var model = MTConnectModel.Parse(xmiPath);
    if (model == null)
        throw new InvalidOperationException($"Failed to parse XMI: {xmiPath}");

    RenderCommonClasses(model, scratchRoot);
    RenderJsonComponents(model, scratchRoot);
    RenderXmlComponents(model, scratchRoot);
}

// File-level diff between prev and new scratch trees, emitting the delta into
// outputRoot. Returns per-category counts for the console summary.
static DeltaStats EmitDelta(string prevScratch, string newScratch, string outputRoot, string compatLabel)
{
    var stats = new DeltaStats();

    // Iterate per library so each library gets its own Compat/<label>.g.cs.
    string[] libraries = { "MTConnect.NET-Common", "MTConnect.NET-JSON-cppagent", "MTConnect.NET-XML" };
    foreach (var library in libraries)
    {
        var prevLibrary = Path.Combine(prevScratch, "libraries", library);
        var newLibrary = Path.Combine(newScratch, "libraries", library);
        var outputLibrary = Path.Combine(outputRoot, "libraries", library);

        if (!Directory.Exists(outputLibrary))
            throw new DirectoryNotFoundException($"{library} not found under output root: {outputLibrary}");

        var prevFiles = EnumerateGeneratedFiles(prevLibrary);
        var newFiles = EnumerateGeneratedFiles(newLibrary);

        var compatBody = new StringBuilder();
        var compatFileCount = 0;

        foreach (var relativePath in newFiles.Keys.OrderBy(k => k, StringComparer.Ordinal))
        {
            var newContent = newFiles[relativePath];
            if (prevFiles.TryGetValue(relativePath, out var prevContent))
            {
                if (ByteEquals(prevContent, newContent))
                {
                    // UNCHANGED — concentrate into Compat file.
                    if (compatFileCount == 0)
                    {
                        compatBody.AppendLine("// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.");
                        compatBody.AppendLine("// TrakHound Inc. licenses this file to you under the MIT license.");
                        compatBody.AppendLine();
                        compatBody.AppendLine("// Compat re-emit: concentrates every .g.cs file whose content did NOT change");
                        compatBody.AppendLine("// between --previous-xmi and --new-xmi. Each block carries its own namespace,");
                        compatBody.AppendLine("// so multi-namespace concentration is legal C#. Byte-identical to the");
                        compatBody.AppendLine("// full-tree emission for every included type (plan D4).");
                        compatBody.AppendLine();
                    }
                    compatBody.AppendLine($"// --- from {relativePath.Replace('\\', '/')} ---");
                    compatBody.Append(System.Text.Encoding.UTF8.GetString(newContent));
                    if (newContent.Length == 0 || newContent[^1] != (byte)'\n')
                        compatBody.AppendLine();
                    compatBody.AppendLine();
                    compatFileCount++;
                    stats.UnchangedConcentrated++;

                    // Concentration replaces the individual file. If the operator ran
                    // the delta against an outputRoot that already carried the
                    // committed .g.cs tree (the realistic use case — the repo root),
                    // leaving the individual file in place would produce CS0101
                    // duplicate-type errors when the Compat file re-emits the same
                    // namespaces. Delete pre-existing individual .g.cs so Compat is
                    // the single source for UNCHANGED types.
                    DeleteIfExists(Path.Combine(outputLibrary, relativePath));
                }
                else
                {
                    // CHANGED — emit new-tree version at its normal location.
                    WriteFile(Path.Combine(outputLibrary, relativePath), newContent);
                    stats.Changed++;
                }
            }
            else
            {
                // ADDED — emit new-tree version at its normal location.
                WriteFile(Path.Combine(outputLibrary, relativePath), newContent);
                stats.Added++;
            }
        }

        // REMOVED types (present in prev tree, absent from new tree). Delete the
        // stale individual file from outputRoot so it stops shipping in the
        // library; the type is intentionally gone from the new spec version.
        foreach (var relativePath in prevFiles.Keys)
        {
            if (!newFiles.ContainsKey(relativePath))
            {
                DeleteIfExists(Path.Combine(outputLibrary, relativePath));
                stats.RemovedSkipped++;
            }
        }

        if (compatFileCount > 0)
        {
            var compatDir = Path.Combine(outputLibrary, "Compat");
            Directory.CreateDirectory(compatDir);
            var compatPath = Path.Combine(compatDir, $"{compatLabel}.g.cs");
            File.WriteAllText(compatPath, compatBody.ToString());
            stats.CompatFilesWritten++;
        }
    }

    return stats;
}

// Enumerates every .g.cs file under `root` and returns a dictionary keyed by
// the forward-slash-normalised path relative to `root`, with the raw file
// bytes as value. Ordinal-key comparer keeps cross-platform behavior
// consistent (Linux CI vs. Windows local).
static Dictionary<string, byte[]> EnumerateGeneratedFiles(string root)
{
    var result = new Dictionary<string, byte[]>(StringComparer.Ordinal);
    if (!Directory.Exists(root))
        return result;

    foreach (var path in Directory.EnumerateFiles(root, "*.g.cs", SearchOption.AllDirectories))
    {
        var relative = Path.GetRelativePath(root, path).Replace('\\', '/');
        result[relative] = File.ReadAllBytes(path);
    }
    return result;
}

static bool ByteEquals(byte[] a, byte[] b)
    => ((ReadOnlySpan<byte>)a).SequenceEqual(b);

static void WriteFile(string path, byte[] contents)
{
    var directory = Path.GetDirectoryName(path);
    if (!string.IsNullOrEmpty(directory))
        Directory.CreateDirectory(directory);
    File.WriteAllBytes(path, contents);
}

// Idempotent delete. Missing files no-op; existing files delete without following
// symlinks (System.IO.File.Delete on .NET removes the link, not the target).
static void DeleteIfExists(string path)
{
    if (File.Exists(path))
        File.Delete(path);
}

// Per-run counters emitted at the tail of a delta invocation. Every category
// is reported so the operator can spot a regression at a glance (a spec bump
// that renames a type surfaces as {changed:1, removed-skipped:1, added:1};
// whereas a spec bump that only adds enum arms surfaces as {changed:1,
// unchanged-concentrated:N-1, removed-skipped:0, added:0}).
internal sealed class DeltaStats
{
    public int Added { get; set; }
    public int Changed { get; set; }
    public int UnchangedConcentrated { get; set; }
    public int RemovedSkipped { get; set; }
    public int CompatFilesWritten { get; set; }
}
