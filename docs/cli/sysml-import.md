# SysML importer CLI

`MTConnect.NET-SysML-Import` is the in-repo code generator that turns an XMI export of the MTConnect SysML model into the `*.g.cs` source files under `libraries/MTConnect.NET-Common/`, `libraries/MTConnect.NET-XML/`, and `libraries/MTConnect.NET-JSON-cppagent/`. It is the bridge between the standard's normative model (`mtconnect/mtconnect_sysml_model`) and the .NET library's typed surface.

The importer is **not shipped** as a binary. It lives in `build/MTConnect.NET-SysML-Import/` and is run by maintainers during a spec-version bump, a generator-template change, or the addition of a new wire-format codec. End users consume the regenerated `*.g.cs` files transparently through the shipped NuGet packages.

The CLI surface is defined by `Program.cs` in `build/MTConnect.NET-SysML-Import/`. See also the auto-generated [CLI reference entry](../reference/cli#mtconnect-net-sysml-import), which is emitted from the same source at docs-build time.

## Synopsis

```text
dotnet run --project build/MTConnect.NET-SysML-Import -- \
    --new-xmi <path-to-MTConnectSysMLModel.xml> \
    --output <repo-root> \
    [--previous-xmi <path-to-prior-XMI>] \
    [--compat-version-label <label>] \
    [--full-tree] \
    [--json-dump <path>]
```

`--new-xmi` and `--output` are required. Every other flag is optional. The default mode is **zero-config delta**: the importer auto-derives PREV_VERSION from `MTConnectVersions.Max` in the current tree and resolves the prior-version XMI from a well-known cache path or a tag-gated submodule checkout. No hand-editing of `Program.cs` is needed.

## Flags

| Flag | Argument | Description |
|---|---|---|
| `--new-xmi` | `<path>` | SysML XMI file to consume. Required. Preferred spelling; `--xmi` remains as a legacy alias. |
| `--xmi` | `<path>` | Legacy alias for `--new-xmi`. Kept for backwards compatibility with existing callers; new invocations should prefer `--new-xmi`. Both flags land on the same `newXmiPath` slot; passing both is legal (the second wins). |
| `--output` | `<path>` | Repository root. Each subgenerator writes into its own `libraries/<LibraryName>/` subtree under this root. Required. |
| `--previous-xmi` | `<path>` | Edge-case override for delta-driven mode. When supplied, the importer uses this file as the previous-version XMI and skips the zero-config auto-derive step. Typical use cases: cross-version audit runs, regenerating against a historical XMI snapshot, and version bumps that skip a version (where `MTConnectVersions.Max` does not match the intended PREV_VERSION). |
| `--compat-version-label` | `<label>` | Label used for the `Compat/<label>.g.cs` file name in delta mode. When `--previous-xmi` is supplied without an explicit label, defaults to `Previous` for backwards compatibility. When the previous-XMI is auto-derived from `MTConnectVersions.Max`, defaults to `v${PREV_XY_UNDERSCORE}` (e.g. `v2_7`). Rejected at argument parse time if the value would produce an unsafe filename (path separators, drive letters, ASCII control chars, leading dots, or length outside 1–64 chars). |
| `--full-tree` | — | Explicit opt-in for the full-regeneration path. Disables both the zero-config auto-derive delta and the `--previous-xmi` override; every emitted `*.g.cs` re-lands under its normal path. Use this when the delta path is impossible (no prior XMI available, no cache populated, submodule tag unknown) or when reviewing the whole generated tree in a single diff. |
| `--json-dump` | `<path>` | Optional. Writes the parsed `MTConnectModel` as JSON to `<path>` for debugging. Runs before the renderers so the dump reflects the exact input to the delta step. |
| `--help`, `-h` | — | Print usage information and exit. |

## Modes

The importer picks one of three modes based on the flag combination:

### Delta (zero-config, default)

Fires when neither `--previous-xmi` nor `--full-tree` is supplied. The importer parses `libraries/MTConnect.NET-Common/MTConnectVersions.cs` under `--output`, extracts `PREV_VERSION` from the `Max` property (e.g. `Max => Version27` resolves to `2.7`), and resolves the prior-version XMI using one of these strategies in order:

1. **Strategy B (primary)** — `build/.cache/sysml-prev/MTConnectSysMLModel_v${PREV_VERSION}.xml` under `--output`. Populated by the maintainer as part of the version-bump runbook (Phase 3.2).
2. **Strategy A (fallback)** — `build/sysml-model/MTConnectSysMLModel.xml`, gated on the submodule tip being checked out at tag `v${PREV_VERSION}` exactly. `git -C build/sysml-model describe --exact-match --tags HEAD` must return the expected tag; anything else falls through.
3. **Strategy C (fail-hard)** — neither resolves. The importer throws with an actionable message naming the probed cache path, the expected submodule tag, the `--previous-xmi` override, and the `--full-tree` escape hatch.

The auto-derived Compat label is `v${X}_${Y}` (e.g. `v2_7`), matching the resolved PREV_VERSION. An explicit `--compat-version-label` overrides that default.

### Delta (`--previous-xmi` override)

Fires when `--previous-xmi <path>` is supplied without `--full-tree`. The importer uses the supplied file directly as the prior-version XMI and skips the auto-derive resolver entirely. The Compat label defaults to `Previous` in this mode (legacy behavior) unless `--compat-version-label` is passed.

Use this mode for cross-version audit runs, historical XMI snapshots, or version bumps that skip a version.

### Full-tree (`--full-tree`)

Fires when `--full-tree` is supplied. Disables both delta paths and re-emits every generated file under its normal `libraries/<LibraryName>/…/*.g.cs` path. Preserves the pre-Phase-4 behavior bit for bit.

Use this mode when the delta path is impossible (no cache, wrong submodule tag) or when the maintainer wants to review the full generated tree in a single diff.

## Delta emission

In delta mode the importer renders both XMIs into isolated scratch directories, diffs them at the file level, and writes only the difference back to `--output`:

- **ADDED** (in NEW only) — written normally under the target library.
- **CHANGED** (in both, different bytes) — written normally (the NEW tree's version).
- **REMOVED** (in PREV only) — deleted from `--output` so the type stops shipping.
- **UNCHANGED** (in both, identical bytes) — concentrated into `Compat/<label>.g.cs` per library; the individual per-type `*.g.cs` is deleted so the Compat file is the sole namespace host (avoids `CS0101` duplicate-type errors when the delta runs against a repo already carrying the committed `*.g.cs` tree).

The stats line printed to stdout (`Delta emission: added=A changed=C removed=R unchanged-concentrated=U`) is the operator's summary of what landed.

## Example invocations

Zero-config delta against the checked-in submodule XMI:

```bash
dotnet run --project build/MTConnect.NET-SysML-Import -- \
    --new-xmi build/sysml-model/MTConnectSysMLModel.xml \
    --output .
```

The importer parses `MTConnectVersions.Max`, probes the cache and submodule tag, and emits the delta.

Version bump that skips a version — pass the historical XMI explicitly:

```bash
dotnet run --project build/MTConnect.NET-SysML-Import -- \
    --new-xmi ~/xmi/MTConnectSysMLModel-v2.9.xml \
    --previous-xmi ~/xmi/MTConnectSysMLModel-v2.5.xml \
    --compat-version-label v2_5 \
    --output .
```

Full regeneration (delta paths disabled):

```bash
dotnet run --project build/MTConnect.NET-SysML-Import -- \
    --new-xmi build/sysml-model/MTConnectSysMLModel.xml \
    --output . \
    --full-tree
```

Dump the parsed model to JSON alongside a delta regen:

```bash
dotnet run --project build/MTConnect.NET-SysML-Import -- \
    --new-xmi build/sysml-model/MTConnectSysMLModel.xml \
    --output . \
    --json-dump /tmp/mtconnect-model.json
```

Windows / PowerShell uses the same command shape; only the paths change.

## Exit codes

| Code | Meaning |
|---|---|
| `0` | Success. Delta / full-tree emission completed and every renderer returned cleanly. |
| `1` | Runtime failure — XMI file not found, output root not found, parse error, or the auto-derive fail-hard message. Details on stderr with an `error:` prefix. |
| `2` | Usage error — missing required flag (`--new-xmi`, `--output`), unknown flag, or an unsafe `--compat-version-label` value. Help text is reprinted on stderr. |

## Maintainer workflow

The full workflow to advance the library onto a new spec version:

```bash
# 1. Fetch the new XMI from mtconnect/mtconnect_sysml_model into the submodule
git submodule update --remote build/sysml-model

# 2. Populate the prior-version cache so the auto-derive resolver has a hit
mkdir -p build/.cache/sysml-prev
cp build/sysml-model/MTConnectSysMLModel.xml \
   build/.cache/sysml-prev/MTConnectSysMLModel_v2.7.xml
git -C build/sysml-model checkout v2.8

# 3. Run the importer (zero-config delta)
dotnet run --project build/MTConnect.NET-SysML-Import -- \
    --new-xmi build/sysml-model/MTConnectSysMLModel.xml \
    --output .

# 4. Verify the regenerated *.g.cs files compile + tests pass
tools/test.sh

# 5. Diff the delta output to confirm only spec-driven changes landed
git diff --stat libraries/**/*.g.cs

# 6. Commit the regeneration in a single commit per spec version
git add libraries/**/*.g.cs
git commit -m "build(sysml): regenerate against v2.8 XMI"
```

See `build/MTConnect.NET-SysML-Import/README.md` for the full "Adding a new MTConnect Standard version" runbook, the determinism guarantee (regen against a pinned XMI tag must produce zero diff), and the delta-mode design notes (plan D4 — partial-class re-emit, not `[TypeForwardedTo]`).

## Configuration

The importer has no configuration file. Every input and output is supplied on the command line. The renderer templates themselves are checked in under `build/MTConnect.NET-SysML-Import/CSharp/`, `build/MTConnect.NET-SysML-Import/Xml/`, and `build/MTConnect.NET-SysML-Import/Json-cppagent/`; editing a template changes what the next regeneration emits.

## Output discipline

- The regenerator overwrites every `*.g.cs` file it produces. In delta mode, REMOVED files (types the spec dropped) are actively deleted from `--output`. In full-tree mode, orphan files (regenerator output that no longer maps to a live SysML element) are **not** deleted — the maintainer reviews the diff and removes orphans manually.
- Hand-written files alongside the generated ones use the convention `<Type>.cs` (hand-written) versus `<Type>.g.cs` (generated). The hand-written file typically adds members the generator does not produce (e.g. helper methods, secondary constructors); both files live in the same `partial class`.
- `git diff libraries/**/*.g.cs` after a regeneration is the authoritative review surface for spec-version advancement.

## Verification

After a regeneration:

```bash
# Full unit + integration suite
tools/test.sh

# Compliance harness — the per-spec-version conformance tests
tools/test.sh --compliance

# Lint + formatter pass to keep the regenerator's output style consistent
dotnet format MTConnect.NET.sln --verify-no-changes
```

A regeneration is considered clean when (a) the test suite is green at every previously-green configuration, (b) the formatter reports no changes needed, and (c) the diff against the previous state explains every changed line in terms of a specific SysML model element.

## See also

- [CLI reference → `MTConnect.NET-SysML-Import`](../reference/cli#mtconnect-net-sysml-import) — the auto-generated flag table emitted from `Program.cs` at docs-build time.
- [Configure & Use → Run](/configure/run) — running the agent against the regenerated library to verify end-to-end behavior.
- [Compliance](/compliance/) — the per-version compliance matrix that the regenerator advances.
- [API reference → `MTConnect.SysML.MTConnectModel`](/api/MTConnect.SysML.MTConnectModel) — the in-memory model the XMI parser produces and the renderers walk.
- [API reference → `MTConnect.SysML.ModelHelper`](/api/MTConnect.SysML.ModelHelper) — the helper surface the per-language renderers call into.
- [API reference → `MTConnect.SysML` namespace](/api/MTConnect.SysML) — the SysML model plus per-renderer entry points.
- [`tools/test.sh`](./test-sh) — runs after a regeneration to verify the suite stays green.
- [`tools/dotnet.sh`](./dotnet-sh) — wraps the `dotnet run` invocation if the regeneration is being done inside a containerized SDK.
