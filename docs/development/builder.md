# Release builder (`MTConnect.NET.Builder`)

- **Source path** — `build/MTConnect.NET.Builder/`
- **Project type** — .NET console application (TFM: `net8.0`)
- **Audience** — repository maintainers and release engineers; not consumed by library users.

## Purpose

`MTConnect.NET.Builder` is the in-tree tool that assembles release artefacts from the repository: NuGet packages, the Windows agent installer (Inno Setup), the agent Docker images, and the GitHub release that carries the installer. It captures the cross-cutting metadata (publisher, version, repository URL, package tags, icon URLs) in one configuration file rather than scattered across thirty-odd `.csproj` files.

The tool is not part of the public library surface — it does not ship as a NuGet package, the docs site does not link it from the user-facing nav, and it is excluded from the auto-generated API reference. It is documented here so contributors who need to cut a release can find it.

## When to run it

Run the builder when cutting a release. Day-to-day contributors do not invoke it; `dotnet build` against `MTConnect.NET.sln` is enough for normal development.

## Configuration

Each build server keeps its own `config.[ID].yaml` in `build/MTConnect.NET.Builder/`, copied from the checked-in `config.template.yaml`. The `config.*.yaml` files (other than the template) are git-ignored, so local paths, NuGet API keys and GitHub tokens stay on the build server. Commands take `--configurationId [ID]` and use `config.default.yaml` when it is not given.

Relative paths in the configuration are resolved against the builder directory, so the template's defaults (`input: ..\..`) build the repository the builder is in, from any working directory.

The top-level keys are:

| Key | Type | Purpose |
| --- | --- | --- |
| `publisher` | string | The publisher string baked into NuGet metadata and installer descriptors. |
| `input` | path | The repository root to read source from. |
| `output` | path | The directory the build artefacts land in. |
| `versionInfoPath` | path | The file the release version is read from (defaults to `[input]/Directory.Build.props`). |
| `innoSetupPath` | path | The Inno Setup compiler executable (`ISCC.exe`). |
| `removeVersionSuffix` | bool | Build without the `VersionSuffix` (ex. `-prerelease`). |
| `signing` | map | Optional code signing with Azure Artifact Signing (see below). |
| `gitHubRepo` / `gitHubUser` / `gitHubToken` | string | The repository the GitHub release is created in. |
| `agent` | map | The agent installer and Docker image manifest. |
| `libraries` | list of maps | NuGet package groups to pack and publish. |

The `agent.installer` block carries the application name, output filename, target runtimes (`win-x64`, `win-x86`) and the framework matrix. The `agent.docker` block lists one image per platform (base image, tag, runtime and framework); Alpine base images need a `linux-musl-*` runtime.

## Versions

Versions come from `Directory.Build.props` and git:

- **Version** — `VersionPrefix` (ex. `7.2.0`). Used for the installer filename, the Docker tags, the GitHub release and the output directory.
- **Revision** — the number of commits since the `VersionPrefix` line last changed ("git height"), so every machine produces the same version for the same commit.
- **Package version** — `[VersionPrefix].[Revision]` plus the suffix (ex. `7.2.0.71-prerelease`). Used for every NuGet package and as the `FileVersion` of the agent.
- **Informational version** — the package version plus the commit (ex. `7.2.0.71-prerelease+e62d93d`, with `.dirty` when built with uncommitted changes).

`global version` prints all of them. Publishing refuses to run when the repository has uncommitted changes, because the version would not identify the commit.

## Build and run

`build.ps1` (or `build.bat`) in the builder directory runs a complete release: it builds the builder in Release, builds the libraries, installer and Docker images, then publishes them and tags the built commit `v[VersionPrefix]` locally.

```
.\build.ps1                   # build and publish
.\build.ps1 -SkipPublish      # build only
.\build.ps1 -SkipBuild        # publish the most recent build
.\build.ps1 -SkipDocker       # everything except the Docker images
.\build.ps1 -Quiet:$false     # show the dotnet / Inno Setup / docker output
```

The individual commands can also be run directly:

| Command | What it does |
| --- | --- |
| `global build` / `global publish` | Every build or publish step below. |
| `global version` | Prints the version, package version and commit. |
| `libraries nuget build` / `publish` | Packs each library group with one `dotnet pack`; pushes every package in parallel (`--skip-duplicate`). |
| `agent installer build` | Publishes the agent for every runtime and framework in parallel, then compiles one installer with a setup type per combination. |
| `agent docker build` / `publish` | Builds one image per platform; publishes `[version]-[tag]` images and the multi-platform `[version]` and `latest` tags. |
| `github release publish` | Creates a draft release on the built commit and uploads the installer. |

Add `--verbose` to show the tool output as it runs; otherwise it is shown only when a step fails. In a `DEBUG` build the tool drops into an interactive prompt that accepts the same commands. The command parser is in `CommandParser/`; the per-task parts are in `Parts/`.

## Output

The tool writes to `[output]/[version]/`:

- `libraries/` — the NuGet packages and symbol packages, one per entry under `libraries[].packages[]`.
- `mtconnect.net-agent-[version]-[suffix]-install.exe` — the Windows installer.
- `build.json` — the version and commit the agent was built from, used to check a release before it is published.

Docker images are built into the local Docker engine and pushed to the registry referenced by `imageName` on publish.

## Code signing

When `signing.dlibPath` is set, the builder signs the published `MTConnect*.dll` and agent files, the installer and its uninstaller with `signtool` and Azure Artifact Signing. It needs the Windows SDK `signtool`, `Microsoft.Azure.ArtifactSigningClientTools`, an Azure CLI login (`az login`) and a `metadata.json` (git-ignored) naming the signing account and certificate profile.

## Updating the manifest

When a new library project lands in the repository, add a corresponding entry under `libraries[].packages[]` in `config.template.yaml` (and the build servers' configurations). When a new runtime or framework lands in the support matrix, extend the relevant `runtimes` / `frameworks` lists; the installer's setup types are generated from them.

## See also

- [tools/dotnet.sh](/cli/dotnet-sh) and [tools/test.sh](/cli/test-sh) — the day-to-day contributor CLIs (these do not invoke the builder).
- [SysML importer](/cli/sysml-import) — the other in-tree code-generation tool; runs at codebase-edit time, not at release time.
