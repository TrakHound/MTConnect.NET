# MTConnect.NET-SysML-Import

Code generator that consumes the **MTConnect SysML model XMI** and emits the partial-class C# definitions under `libraries/MTConnect.NET-Common/`, `libraries/MTConnect.NET-JSON-cppagent/`, and `libraries/MTConnect.NET-XML/`. Every `.g.cs` file under those library trees is the output of this tool.

## When to run it

You need to run this tool when:

1. **A new MTConnect Standard version is released** — extend the `MTConnectVersions` constants (see §3 below), then regenerate from the new version's XMI tag.
2. **An XMI tag is updated mid-version** — re-run with the same version's XMI to pick up corrected attribute names, descriptions, etc.
3. **The Scriban templates under `CSharp/Templates/`, `Json-cppagent/Templates/`, or `Xml/Templates/` are edited** — re-run against the current XMI to refresh every `.g.cs`.

## Prerequisites

- .NET 8.0 SDK or newer.
- Local clone of [`mtconnect/mtconnect_sysml_model`](https://github.com/mtconnect/mtconnect_sysml_model) checked out to the version tag you want to import.
- (Optional) `dotnet tool restore` executed in this repo if you want to use the pinned tooling (ReportGenerator, etc.).

## Quick start

### 1. Sync the SysML model

The MTConnect SysML XMI is tracked as a submodule under `build/sysml-model/`. Initialize once after cloning the repo, then check out the version tag you want to regen against.

```bash
# Run once after the initial clone (or after a `git pull` that updates the submodule pointer):
git submodule update --init build/sysml-model

# Switch to a different version tag for a per-version regen:
git -C build/sysml-model checkout v2.7   # or v2.5, v2.6, v2.0, ... — see `git -C build/sysml-model tag`
git -C build/sysml-model rev-parse HEAD  # capture the SHA for the regen-provenance doc
```

The submodule's default tip is the latest published spec tag. The same gitdir backs every worktree the contributor creates with `git worktree add`, so a per-worktree `git submodule update --init build/sysml-model` is enough to populate the path inside the worktree.

For parallel multi-version regens (e.g. re-running the generator across v2.5, v2.6, v2.7 after a template change), create worktrees inside the submodule itself:

```bash
git -C build/sysml-model worktree add /tmp/sysml-v2.5 v2.5
git -C build/sysml-model worktree add /tmp/sysml-v2.6 v2.6
# build/sysml-model itself stays on v2.7
```

Each `/tmp/sysml-vX.Y/MTConnectSysMLModel.xml` can then be passed to a separate importer invocation in parallel.

### 2. Run the importer

```bash
# From the repo root, after the submodule is checked out. Zero-config: the
# importer auto-derives PREV_VERSION from MTConnectVersions.Max and resolves
# the prior-version XMI automatically, so the common case is a single-flag
# invocation.
dotnet run --project build/MTConnect.NET-SysML-Import \
    -- --new-xmi build/sysml-model/MTConnectSysMLModel.xml \
       --output "$(pwd)"

# Force full regeneration (skip both delta paths):
dotnet run --project build/MTConnect.NET-SysML-Import \
    -- --new-xmi build/sysml-model/MTConnectSysMLModel.xml \
       --output "$(pwd)" \
       --full-tree
```

If running against a side worktree (for multi-version regens):

```bash
dotnet run --project build/MTConnect.NET-SysML-Import \
    -- --new-xmi /tmp/sysml-v2.5/MTConnectSysMLModel.xml \
       --output "$(pwd)" \
       --full-tree
```

The `--xmi` flag from pre-#408 invocations is still accepted as a legacy alias
for `--new-xmi`; new call sites should prefer `--new-xmi`.

### 3. Inspect + commit

```bash
git status                      # see which .g.cs files changed
git diff libraries/             # review the diff before committing
git add libraries/MTConnect.NET-Common
git commit -m 'feat(common): regenerate from vX.Y XMI'
git add libraries/MTConnect.NET-JSON-cppagent
git commit -m 'feat(json-cppagent): regenerate formatters from vX.Y XMI'
git add libraries/MTConnect.NET-XML
git commit -m 'feat(xml): regenerate formatters from vX.Y XMI'
```

Split the regen into per-target commits so reviewers can audit each layer independently.

## CLI

| Flag | Required | Default | Purpose |
|---|---|---|---|
| `--new-xmi <path>` | Yes | — | Path to the new-version SysML XMI to consume. Preferred spelling from task #408 onwards. |
| `--xmi <path>` | — | — | Legacy alias for `--new-xmi`. Kept for backwards compatibility with pre-#408 callers; new invocations should prefer `--new-xmi`. |
| `--output <path>` | Yes | — | Repository root. Each renderer writes into its own `libraries/<LibraryName>/` subtree under this root. |
| `--previous-xmi <path>` | No | auto-derived | Explicit override for the prior-version XMI in delta mode. When supplied, skips the zero-config auto-derive step and uses this file as the previous-version XMI. Typical use cases: cross-version audit runs, regenerating against a historical XMI snapshot, and version-bumps that skip a version (where `MTConnectVersions.Max` does not match the intended `PREV_VERSION`). Files present in `--previous-xmi`'s tree but absent from `--new-xmi`'s tree (REMOVED types) are **deleted** from the output tree. Files concentrated into `Compat/<label>.g.cs` (UNCHANGED types) are also deleted from their individual `.g.cs` paths — the Compat file becomes the sole namespace host to avoid CS0101 duplicate-type collisions when `--output` points at a repo already carrying a full committed `.g.cs` tree. |
| `--compat-version-label <label>` | No | `v${PREV_XY_UNDERSCORE}` (auto-derived) or `Previous` (explicit-override fallback) | Label used for the `Compat/<label>.g.cs` file name in delta mode. In zero-config mode the label auto-derives from `MTConnectVersions.Max` as `v${X}_${Y}` (e.g. `v2_7`); with an explicit `--previous-xmi` the legacy `Previous` default applies. Must match `^[A-Za-z0-9_\-][A-Za-z0-9_\-.]*$`, ≤ 64 chars, no leading dot — hostile inputs like `../../etc/passwd` reject at exit 2. Ignored under `--full-tree`. |
| `--full-tree` | No | delta by default | Explicit opt-in for the full-regeneration path. Disables both the zero-config auto-derive delta and the `--previous-xmi` override; every emitted `.g.cs` re-lands under its normal path. |
| `--json-dump <path>` | No | not written | If set, dumps the parsed `MTConnectModel` as JSON. Useful for debugging. |
| `--help`, `-h` | — | — | Print usage and exit. |

`--new-xmi` (or `--xmi`) and `--output` are mandatory. Running with no arguments exits with `error: --new-xmi <path> is required (legacy alias --xmi is still accepted).` (exit code 2) and prints help.

### Zero-config delta mode (default from task #408)

When neither `--previous-xmi` nor `--full-tree` is supplied, the importer parses `MTConnectVersions.Max` from `libraries/MTConnect.NET-Common/MTConnectVersions.cs` under `--output` and resolves the prior-version XMI internally, in this order:

1. **Strategy B (primary)** — `build/.cache/sysml-prev/MTConnectSysMLModel_v${PREV_VERSION}.xml`, populated per Phase 3.2 of the version-bump runbook (`docs/testing/vX-Y.md`).
2. **Strategy A (fallback)** — `build/sysml-model/MTConnectSysMLModel.xml`, gated on `git -C build/sysml-model describe --exact-match --tags HEAD` returning `v${PREV_VERSION}` exactly. Covers the dev-loop case where the operator has not yet promoted the submodule tip past the prior-version tag.
3. **Strategy C (fail-hard)** — neither resolves. Exits with `error: PREV_VERSION auto-derivation from MTConnectVersions.Max = ${PREV_VERSION} failed. …` on stderr + exit code 1, naming both probed paths and directing the operator at `--previous-xmi` (explicit override) or `--full-tree` (delta-disable escape hatch).

The zero-config default keeps a Phase 3 version-bump invocation single-flag:

```bash
dotnet run --project build/MTConnect.NET-SysML-Import \
    -- --new-xmi  build/sysml-model/MTConnectSysMLModel.xml \
       --output   "$(pwd)"
```

### Delta mode with explicit `--previous-xmi`

The explicit-override path is for the exceptional cases where `MTConnectVersions.Max` does not name the intended `PREV_VERSION` (cross-version audit, regen against a historical XMI snapshot, or a spec bump that skips a version).

```bash
dotnet run --project build/MTConnect.NET-SysML-Import \
    -- --new-xmi        /tmp/mtconnect-sysml/v2.8/MTConnectSysMLModel.xml \
       --previous-xmi   /tmp/mtconnect-sysml/v2.5/MTConnectSysMLModel.xml \
       --compat-version-label  v2_5 \
       --output         "$(pwd)"
```

Emission partitions per file:

- **ADDED** (in new only) → written to normal `libraries/<lib>/...` path.
- **CHANGED** (in both, different bytes) → written to normal path (new tree's version).
- **REMOVED** (in prev only) → deleted from `--output` (the type stops shipping — the spec dropped it).
- **UNCHANGED** (in both, identical bytes) → concentrated into `libraries/<lib>/Compat/<label>.g.cs`; the individual `.g.cs` file is deleted from the output tree so the Compat file is the sole namespace host.

Byte-identity of unchanged types is preserved (plan D4 invariant); dropping `--previous-xmi` returns to full-tree mode bit-for-bit. Regression is covered by `tests/MTConnect.NET-Generator-Tests/DeltaRegenTests` and `DeltaCompatAndStatsTests`.

## Visual Studio F5 workflow

`Properties/launchSettings.json` ships three launch profiles so F5 / Run from VS / Rider works out of the box without re-typing CLI args:

| Profile | When to use it |
|---|---|
| `Import (env vars)` | You set `MTCONNECT_XMI_PATH` and `MTCONNECT_NET_REPO` as system / user env vars before launching VS / Rider (or as profile-scoped variables you add yourself in the launch-profile dropdown). The profile passes whatever the env vars resolve to. Best for a "set once, never edit" setup. The launch profile does not pre-populate these variables — set them in your shell / system env first, otherwise the importer crashes with `error: XMI file not found`. |
| `Import (sibling clone of mtconnect_sysml_model)` | You've cloned `mtconnect/mtconnect_sysml_model` as a sibling directory of this repo (so the path `../../../../mtconnect_sysml_model/MTConnectSysMLModel.xml` resolves from the importer project). Switch standard version with `git -C ../mtconnect_sysml_model checkout v2.7` (or any other tag) before pressing F5. |
| `Import (json-dump enabled, sibling clone)` | Same as the previous profile but also writes the parsed `MTConnectModel` JSON dump to `.cache/mtconnect-model.json` in the repo root. Useful when debugging the parser. |

Pick the profile from the run-target dropdown in Visual Studio (or `Run / Debug Configurations` in Rider). If you need a one-off variant, copy a profile and edit its `commandLineArgs`.

## What it generates

The renderer emits three layers, all into pre-existing library directories:

| Renderer | Output root | What lands |
|---|---|---|
| `CSharpTemplateRenderer` | `libraries/MTConnect.NET-Common/` | DataItem subclasses, Component subclasses, Composition types, enum definitions, Configuration sub-elements, Asset hierarchy, Observation events. ~850 `.g.cs` files at v2.7. |
| `JsonCppAgentTemplateRenderer` | `libraries/MTConnect.NET-JSON-cppagent/` | `JsonComponents.g.cs`, `JsonEvents.g.cs`, `JsonSamples.g.cs`, `JsonMeasurements.g.cs` — flat catalog files that the cppagent JSON formatter reflects over. |
| `XmlTemplateRenderer` | `libraries/MTConnect.NET-XML/` | `XmlMeasurements.g.cs`, `XmlCuttingItem.g.cs`, `XmlCuttingToolLifeCycle.g.cs` — XML formatter helpers. |

## Adding a new MTConnect Standard version

When a new MTConnect version is released, the steps are:

### 1. Update `MTConnectVersions.cs`

```csharp
// libraries/MTConnect.NET-Common/MTConnectVersions.cs
public static Version Max => Version28;   // bump the ceiling

public static readonly Version Version28 = new Version(2, 8);   // add the constant
```

### 2. Regenerate against the new XMI tag

```bash
git -C /tmp/mtconnect-sysml fetch --tags
git -C /tmp/mtconnect-sysml checkout v2.8
dotnet run --project build/MTConnect.NET-SysML-Import \
    -- --new-xmi /tmp/mtconnect-sysml/MTConnectSysMLModel.xml \
       --output "$(pwd)"
```

### 3. Build + verify

```bash
dotnet build MTConnect.NET.sln -c Debug
```

Build must be `0 Error(s)`. The universal cross-package parent resolver in `MTConnectClassModel.ResolveDanglingParents` automatically grafts any missing parent class that the new version places outside the per-package parser's reach — so a brand-new `*DataSet` / `*Result` / `Abstract*` style of class added in a future version compiles without a generator code change. If a new class introduces a field whose declared datatype lives in a foreign package, the resolver intentionally prunes that field on the grafted base; expect a few stripped-property follow-ups visible in the diff.

### 4. Download the XSDs

```bash
mkdir -p tests/Compliance/MTConnect-Compliance-Tests/Schemas/v2_8
cd tests/Compliance/MTConnect-Compliance-Tests/Schemas/v2_8
for kind in Devices Streams Assets Error; do
  curl -sf -O "https://schemas.mtconnect.org/schemas/MTConnect${kind}_2.8.xsd"
  curl -sf -O "https://schemas.mtconnect.org/schemas/MTConnect${kind}_2.8_1.0.xsd"
done
```

### 5. Update the README + per-library NuGet descriptions

```bash
sed -i 's|Supports MTConnect Versions up to v2\.7|Supports MTConnect Versions up to v2.8|g' \
  README.md $(grep -rl 'Supports MTConnect Versions up to v2\.7' libraries agent adapter)
```

### 6. Per-version compliance doc

Author `docs/testing/v2-8.md` modelled on `docs/testing/v2-6.md` and `docs/testing/v2-7.md`. List every (DataItem / Component / enum value / Configuration) delta from the previous version with a pinned-test column.

## Generator architecture

```
build/MTConnect.NET-SysML-Import/
├── Program.cs                          # CLI entry point
├── TemplateLoader.cs                   # Helper: file-not-found → throws clearly
├── CSharp/
│   ├── TemplateRenderer.cs             # Drives MTConnect.NET-Common output
│   ├── ClassModel.cs                   # Per-class Scriban model
│   ├── EnumModel.cs                    # Per-enum Scriban model
│   ├── ComponentType.cs / DataItemType.cs / CompositionType.cs / …
│   └── Templates/*.scriban             # ~15 Scriban template files
├── Json-cppagent/
│   ├── TemplateRenderer.cs             # Drives JSON-cppagent output
│   └── Templates/{Components,Events,Samples,Measurements}.scriban
└── Xml/
    ├── TemplateRenderer.cs             # Drives XML output
    └── Templates/{XmlCuttingItem,XmlCuttingToolLifeCycle,XmlMeasurements}.scriban
```

`MTConnect.NET-SysML` (the library — separate from this tool) does the XMI parsing and exposes `MTConnectModel.Parse(xmiPath)`. The importer here holds the templates and the orchestration logic.

### Cross-package parent resolver

A common XMI pattern: a class in package A declares a generalization (parent) that lives in package B. The per-package parsers in `MTConnect.NET-SysML/Models/*` only walk their own sub-tree, so the parent stays invisible and any C# subclass referencing it fails to compile. The importer runs `MTConnectClassModel.ResolveDanglingParents` automatically (called from `MTConnectModel.Parse`) which:

1. Scans every parsed `Classes` list for class entries whose `ParentName` isn't in the local set.
2. Looks each missing parent up in the global XMI by `xmi:id` (the authoritative reference — multiple UML classes can share a name across packages).
3. Grafts a freshly-parsed `MTConnectClassModel` instance into the same list under the same `idPrefix`.
4. Single-pass: the grafted parent has its `ParentName` / `ParentUmlId` stripped, so each pass either converges or there's nothing more to do.

The grafted parent has its own `ParentName`, `ParentUmlId`, and `Properties` cleared — see the inline rationale in `MTConnectClassModel.cs:ResolveDanglingParents`. This makes the importer version-agnostic: any future MTConnect version that introduces a cross-package parent picks up the resolver automatically.

## Determinism guarantee

Running the importer against the **exact same XMI tag** as the last regen produces **byte-identical** output (`git diff libraries/` empty). This is a critical correctness gate: a non-empty diff against a reproduced regen indicates either (a) a Scriban version change, (b) a template edit, or (c) a non-deterministic enumeration order somewhere in the parser.

When upgrading Scriban or editing templates, **always** run a v2.5 / v2.6 / v2.7 dry-run regen first and resolve any drift in a dedicated commit before bumping to a new version.

## Common pitfalls

| Symptom | Likely cause |
|---|---|
| Importer prints "Done." but no `.g.cs` files change | Scriban template tree missing or case-mismatched. Build output should contain `CSharp/Templates/`, `Json-cppagent/Templates/`, `Xml/Templates/` — case-correct. The `EnsureTemplateTreesExist` startup check now catches this before XMI parse. |
| `CS0246: type 'X' could not be found` after regen | A new XMI version introduced a cross-package parent that the resolver couldn't graft — typically because the parent lives in a sub-model whose `Classes` list isn't yet enumerated by `MTConnectModel.CollectClassLists`. Add it to that helper. |
| `InvalidCastException` in `CSharpTemplateRenderer.Render` | A property's `Id` matches a suffix-based class selector. The `Result` selector now type-guards; new selectors should follow the same pattern (`typeof(MTConnectClassModel).IsAssignableFrom(type) && Id.EndsWith(...)`)|
| Older NuGet vulnerability warnings on Scriban | Scriban now pinned at 7.2.0 — the 5.x advisories no longer apply. If a warning resurfaces on a fresh dependency, audit the resolved version with `dotnet list package --vulnerable --include-transitive`. |

## Reproducibility

Every regen commit on the upstream repo records:
- `mtconnect/mtconnect_sysml_model` SHA in the commit body.
- The version tag (`v2.X`).
- (Optional) a `docs/testing/v2-X/regen-provenance.md` block documenting the SHA + the importer commit at the time of the run.

A reviewer can re-run the importer against the same SHA and confirm zero diff against the PR's `.g.cs` changes.
