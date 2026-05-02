# SysML importer

`MTConnect.NET-SysML-Import` is the in-repo code-generator that turns an XMI export of the MTConnect SysML model into the `*.g.cs` source files under `libraries/MTConnect.NET-Common/`, `libraries/MTConnect.NET-XML/`, and `libraries/MTConnect.NET-JSON-cppagent/`. It is the bridge between the standard's normative model (`mtconnect/mtconnect_sysml_model`) and the .NET library's typed surface.

This tool is **not shipped** as a binary. It lives in `build/MTConnect.NET-SysML-Import/` and is run by maintainers when:

- A new spec version is tagged in `mtconnect/mtconnect_sysml_model` and the library is being moved onto it.
- A code-side change to the generator templates lands and the `*.g.cs` files need to be regenerated.
- A new generator-output target is added (e.g. a new wire-format codec).

End users do not run this tool. End users consume the regenerated `*.g.cs` files transparently through the shipped NuGet packages.

## Source

- Project: `build/MTConnect.NET-SysML-Import/MTConnect.NET-SysML-Import.csproj`
- Entry point: `build/MTConnect.NET-SysML-Import/Program.cs`
- Renderer templates: `build/MTConnect.NET-SysML-Import/CSharp/`, `build/MTConnect.NET-SysML-Import/Xml/`, `build/MTConnect.NET-SysML-Import/Json-cppagent/`
- Parser library: `libraries/MTConnect.NET-SysML/` (the `MTConnect.SysML` namespace — XMI loader plus the in-memory `MTConnectModel`).

## Current CLI surface

`Program.cs` is a top-level statement file with **hardcoded paths**, not a parameterized CLI. Running it executes four steps in sequence against the input file and the output directories listed at the top of `Program.cs`:

1. **Parse XMI** — `MTConnectModel.Parse(xmlPath)` reads the XMI export.
2. **Render JSON dump** — serializes the parsed model to a JSON file (currently at `C:\temp\mtconnect-model.json`). Useful for inspecting what the parser saw.
3. **Render `MTConnect.NET-Common` C# classes** — `CSharpTemplateRenderer.Render(model, outputPath)` writes `*.g.cs` files into `libraries/MTConnect.NET-Common/`.
4. **Render `MTConnect.NET-JSON-cppagent` codec classes** — `JsonCppAgentTemplateRenderer.Render(model, outputPath)` writes `*.g.cs` files into `libraries/MTConnect.NET-JSON-cppagent/`.
5. **Render `MTConnect.NET-XML` codec classes** — `XmlTemplateRenderer.Render(model, outputPath)` writes `*.g.cs` files into `libraries/MTConnect.NET-XML/`.

The three paths a maintainer typically needs to edit before running the tool are at the top of `Program.cs`:

| Variable | Purpose | Current default |
|---|---|---|
| `xmlPath` | XMI input file path. The `MTConnectSysMLModel.xml` file produced by exporting the standard's SysML model. | `D:\TrakHound\MTConnect\Standard\v2.5\MTConnectSysMLModel.xml` |
| The JSON dump path inside `RenderJsonFile()` | Where to write the in-memory model as JSON (for inspection / diffing). | `C:\temp\mtconnect-model.json` |
| The `outputPath` literals inside `RenderCommonClasses()` / `RenderJsonComponents()` / `RenderXmlComponents()` | Relative paths from the executable to each target library, computed against `AppDomain.CurrentDomain.BaseDirectory`. | `../../../../../libraries/MTConnect.NET-Common`, `../../../../../libraries/MTConnect.NET-JSON-cppagent`, `../../../../../libraries/MTConnect.NET-XML` (resolved from `bin/{Configuration}/{TFM}/` so they land in the source tree). |

A future change may refactor `Program.cs` to read `--xmi <path>`, `--output <dir>`, and `--json-dump <path>` from `args[]`; this page will be updated when that lands. Until then, the workflow is: edit the three constants, save, run, commit the regenerated `*.g.cs`.

## Example workflow

The full maintainer workflow to advance the library onto a new spec version:

```bash
# 1. Fetch the new XMI from mtconnect/mtconnect_sysml_model
git -C ~/git/mtconnect/mtconnect_sysml_model pull
cp ~/git/mtconnect/mtconnect_sysml_model/MTConnectSysMLModel.xml \
   ~/MTConnect/Standard/v2.6/MTConnectSysMLModel.xml

# 2. Edit build/MTConnect.NET-SysML-Import/Program.cs:
#    point xmlPath at the new v2.6 file.

# 3. Run the importer
dotnet run --project build/MTConnect.NET-SysML-Import

# 4. Verify the regenerated *.g.cs files compile + tests pass
tools/test.sh

# 5. Diff the regenerated output to confirm only spec-driven changes landed
git diff --stat libraries/MTConnect.NET-Common/**/*.g.cs

# 6. Commit the regeneration in a single commit per spec version
git add libraries/**/*.g.cs
git commit -m "build(sysml): regenerate against v2.6 XMI"
```

On Windows the same workflow works under PowerShell; the only path that needs editing is the `xmlPath` constant in `Program.cs`, which is currently spelled as a Windows-style path.

## Configuration

The tool has no configuration file. All inputs and outputs are literal strings in `Program.cs`. The renderer templates themselves are checked in under `build/MTConnect.NET-SysML-Import/CSharp/`, `build/MTConnect.NET-SysML-Import/Xml/`, and `build/MTConnect.NET-SysML-Import/Json-cppagent/`; editing a template changes what the next regeneration emits.

## Output discipline

- The regenerator overwrites every `*.g.cs` file it produces. Files that are not regenerated (because the corresponding SysML element disappeared in the new spec version) are **not** deleted — the maintainer reviews the diff and removes orphans manually.
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

- [Configure & Use → Run](/configure/run) — running the agent against the regenerated library to verify end-to-end behavior.
- [Compliance](/compliance/) — the per-version compliance matrix that the regenerator advances.
- [API reference → `MTConnect.SysML.MTConnectModel`](/api/) — the in-memory model the XMI parser produces and the renderers walk.
- [API reference → `MTConnect.SysML.CSharp.CSharpTemplateRenderer`](/api/) — the C# renderer that produces `MTConnect.NET-Common`.
- [API reference → `MTConnect.SysML.Xml.XmlTemplateRenderer`](/api/) — the XML renderer that produces `MTConnect.NET-XML`.
- [API reference → `MTConnect.SysML.Json_cppagent.JsonCppAgentTemplateRenderer`](/api/) — the JSON v2 renderer that produces `MTConnect.NET-JSON-cppagent`.
- [`tools/test.sh`](./test-sh) — runs after a regeneration to verify the suite stays green.
- [`tools/dotnet.sh`](./dotnet-sh) — wraps the `dotnet run` invocation if the regeneration is being done inside a containerized SDK.
