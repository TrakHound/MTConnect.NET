# Release builder (`MTConnect.NET.Builder`)

- **Source path** — `build/MTConnect.NET.Builder/`
- **Project type** — .NET console application (TFM: `net8.0`)
- **Audience** — repository maintainers and release engineers; not consumed by library users.

## Purpose

`MTConnect.NET.Builder` is the in-tree tool that assembles release artefacts from the repository: NuGet packages, Windows installers (Inno Setup), Docker images, and the GitHub release entries that point at them. It exists because the release set spans seven layers (libraries, agent applications, adapter applications, agent modules, adapter modules, container images, installers) and each layer has its own framework matrix and packaging quirks; a single script captures the cross-cutting metadata (publisher, version, repository URL, package tags, icon URLs) in one place rather than scattered across thirty-odd `.csproj` files.

The tool is not part of the public library surface — it does not ship as a NuGet package, the docs site does not link it from the user-facing nav, and it is excluded from the auto-generated API reference. It is documented here so contributors who need to cut a release can find it.

## When to run it

Run the builder when cutting a release. Day-to-day contributors do not invoke it; `dotnet build` against `MTConnect.NET.sln` is enough for normal development.

## Configuration

The tool reads two YAML files at start-up:

- `config.default.yaml` — checked in; the canonical template with the layered manifest (libraries, agent, adapter, packages, installers, docker images).
- `config.production.yaml` — local override (not checked in); points the `input` / `output` / `versionInfoPath` / `innoSetupPath` keys at the build machine's paths.

The top-level keys are:

| Key | Type | Purpose |
| --- | --- | --- |
| `publisher` | string | The publisher string baked into NuGet metadata and installer descriptors. |
| `input` | path | The repository root to read source from. |
| `output` | path | The directory the build artefacts land in. |
| `versionInfoPath` | path | The file the tool reads the release version from (`Directory.Build.props`). |
| `innoSetupPath` | path | The Inno Setup compiler executable (`iscc.exe`). |
| `agent` | map | Per-installer + per-image manifest for the agent. |
| `adapter` | map | Per-installer + per-image manifest for the adapter. |
| `libraries` | list of maps | NuGet package families to pack and publish. |

Each `installer` block carries an application name, output filename, target runtimes (e.g. `win-x86`, `win-x64`), and the .NET framework matrix to compile for. Each `docker` block carries the base image, runtime, and framework per architecture.

## Build and run

```
dotnet run --project build/MTConnect.NET.Builder
```

In a `DEBUG` build the tool drops into an interactive prompt that accepts the same commands as the command-line interface; in a release build it reads its arguments and runs to completion. The command parser is in `CommandParser/`; the per-task parts are in `Parts/` (one folder per build phase — `agent`, `adapter`, `libraries`, `installer`, `docker`).

## Output

The tool produces:

- NuGet packages, one per library project listed under `libraries[].packages[]`, written to `output/nuget/`.
- Inno Setup installers, one per `runtime` × `framework` combination listed under `agent.installer` and `adapter.installer`, written to `output/installers/`.
- Docker images, one per `images[]` entry under `agent.docker` and `adapter.docker`, pushed to the registry referenced by `imageName`.
- GitHub release metadata via Octokit, posted against the configured repository.

## Updating the manifest

When a new library project lands in the repository, add a corresponding entry under `libraries[].packages[]` in `config.default.yaml`. When a new runtime or framework lands in the support matrix, extend the relevant `runtimes` / `frameworks` lists. The tool reads the manifest dynamically; there is no code change to make alongside.

## See also

- [tools/dotnet.sh](/cli/dotnet-sh) and [tools/test.sh](/cli/test-sh) — the day-to-day contributor CLIs (these do not invoke the builder).
- [SysML importer](/cli/sysml-import) — the other in-tree code-generation tool; runs at codebase-edit time, not at release time.
