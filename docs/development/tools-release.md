# `tools/release/` scripts

Every script under `tools/release/` is a TypeScript file executed via
`tsx`. The release workflow (`.github/workflows/release.yml`) is the
only production consumer; scripts also run standalone under
`--dry-run` for local verification.

Every top-level script exposes a `main(argv)` export and a
run-when-invoked-directly shim, so it doubles as a library and a
CLI. `shell.ts` is a shared helper — it has no `main(argv)`, only
the reusable spawn-and-log surface every other script imports.

## `pack.ts`

Runs `dotnet pack MTConnect.NET.sln -c Release` with the version
provided on `--version`. Every project with `IsPackable=true` in its
`.csproj` produces a `.nupkg` + a `.snupkg` under `build/output/nupkg/`.

```
tsx tools/release/pack.ts --version 7.0.0-dev.42
```

## `nuget-push.ts`

Pushes every `.nupkg` in a directory to a NuGet feed. Reads the API
key from `NUGET_API_KEY` (or from `--api-key`). Symbol packages are
pushed automatically by `dotnet nuget push` when they sit alongside
their parent `.nupkg`; the script does not iterate them separately.

```
NUGET_API_KEY=... tsx tools/release/nuget-push.ts --input build/output/nupkg
```

## `docker-build.ts`

Builds one native-arch image via `docker buildx build --load` and
tags it `<image>:<version>-<arch>`. The workflow calls it once on
`ubuntu-latest` (`linux/amd64`) and once on `ubuntu-24.04-arm`
(`linux/arm64`); a follow-up `docker-manifest` step merges the two.

```
tsx tools/release/docker-build.ts --version 7.0.0-dev.42 --platform linux/amd64
```

## `docker-push.ts`

Two modes:

- Per-arch push — `--platform linux/amd64` or `--platform linux/arm64`
  pushes the matching per-arch tag.
- Manifest merge — `--manifest` (mutually exclusive with `--platform`)
  merges both per-arch tags into a single multi-arch tag
  `<image>:<version>` via `docker buildx imagetools create`.

```
tsx tools/release/docker-push.ts --version 7.0.0-dev.42 --platform linux/amd64
tsx tools/release/docker-push.ts --version 7.0.0-dev.42 --manifest
```

## `sbom.ts`

Generates an SPDX SBOM for either the `.nupkg` set (via
`Microsoft.Sbom.DotNetTool`) or a specific Docker image (via
`syft`, the SBOM engine `anchore/sbom-action` wraps in CI). Writes
outputs to `build/output/sbom/`.

```
tsx tools/release/sbom.ts --nuget --input build/output/nupkg
tsx tools/release/sbom.ts --docker trakhound/mtconnect-agent:7.0.0-dev.42
```

## `gh-release-create.ts`

Cuts a GitHub pre-release with the `.nupkg`s + SBOMs attached and
the Docker image reference in the release notes. Always uses
`--prerelease`; stable releases are out of scope for phase 1.

```
gh auth login  # once, if not already authenticated
tsx tools/release/gh-release-create.ts --version 7.0.0-dev.42 \
    --docker-image trakhound/mtconnect-agent:7.0.0-dev.42
```

## `shell.ts` — shared helper

Not a CLI. Exposes:

- `run(cmd, args, opts?)` — inherit-stdio `spawn` wrapper that
  throws on non-zero exit and echoes each command it runs. Under
  `opts.dryRun`, prints `[dry-run] <cmd>` and skips the spawn.
- `renderCmd(cmd, args)` — human-readable rendering of the command
  as it would appear on stdout. Redacts the value that follows any
  `SECRET_ARG_NAMES` arg (`--api-key`, `--password`, `-p`,
  `--token`) so a CI-log echo cannot leak a credential.
- `parseDryRun(argv)` — pulls the `--dry-run` flag out of an argv
  list; every CLI above uses this to preserve a uniform flag
  surface without pulling in a heavier CLI library.
- `requireEnv(name)` / `optionalEnv(name)` — throwing / undefined
  variants for environment-variable lookup.

## `--dry-run`

Every CLI accepts `--dry-run`. Under that flag every subprocess
invocation is logged instead of executed — the shape of the pipeline
can be verified end-to-end on a workstation without publishing.
