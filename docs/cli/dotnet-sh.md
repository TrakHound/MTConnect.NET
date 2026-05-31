# `tools/dotnet.sh`

`tools/dotnet.sh` wraps every `dotnet` invocation that contributor scripts make inside the repo. By default it forwards to the host `dotnet` on `PATH`. When passed `--docker` (or with `MTCONNECT_DOTNET_USE_DOCKER=1` exported), it instead runs `dotnet` inside an official `mcr.microsoft.com/dotnet/sdk` container with the repo bind-mounted at `/src`.

This is a **contributor** tool. End users running the shipped agent or adapter binaries do not need it.

A PowerShell sibling lives at `tools/dotnet.ps1` with the same semantics and the same flag spelling (`-Docker` instead of `--docker`).

## Synopsis

```text
tools/dotnet.sh [--docker | -d] <dotnet args...>
```

Or equivalently with the environment variable:

```bash
MTCONNECT_DOTNET_USE_DOCKER=1 tools/dotnet.sh <dotnet args...>
```

## Flags

| Flag | Description |
|---|---|
| `--docker`, `-d` | Run the `dotnet` invocation inside a `mcr.microsoft.com/dotnet/sdk` container instead of using the host `dotnet`. Equivalent to exporting `MTCONNECT_DOTNET_USE_DOCKER=1` for the call. Must be the first argument if passed; subsequent arguments are forwarded verbatim to `dotnet` inside the container. |

Every other argument is passed straight through to `dotnet`. The wrapper does no parsing of the inner command line.

## Environment variables

| Variable | Default | Description |
|---|---|---|
| `MTCONNECT_DOTNET_USE_DOCKER` | `0` | Set to `1` to force docker mode without supplying `--docker` per call. Useful in CI when every step should be containerized. |
| `MTCONNECT_DOTNET_SDK_TAG` | `8.0` | SDK image tag suffix. The wrapper resolves `mcr.microsoft.com/dotnet/sdk:${MTCONNECT_DOTNET_SDK_TAG}` when the full image is not overridden. Set to `9.0` to test against the .NET 9 SDK, `6.0` to test against .NET 6, etc. |
| `MTCONNECT_DOTNET_IMAGE` | `mcr.microsoft.com/dotnet/sdk:${MTCONNECT_DOTNET_SDK_TAG}` | Full container image to run. Override to pin to a digest, use a private registry, or swap in a debug-tagged SDK image. |
| `MTCONNECT_NUGET_VOLUME` | `mtconnect-net-nuget` | Named docker volume mounted at `/root/.nuget/packages` inside the container. Persists the NuGet cache across runs. Set to a different name to isolate per-branch caches. |
| `MTCONNECT_DOTNET_TOOLS_VOLUME` | `mtconnect-net-dotnet-tools` | Named docker volume mounted at `/root/.dotnet/tools` inside the container. Persists `dotnet tool` installations (reportgenerator, etc.) across runs. |
| `MTCONNECT_DOTNET_E2E_DIND` | `0` | Set to `1` to force the docker-in-docker pass-through (host network + bind-mounted `/var/run/docker.sock` + the host's `docker` CLI bind-mounted in) even when the invocation does not look like an E2E run. The wrapper enables this automatically when the dotnet args reference `tests/IntegrationTests`, `tests/E2E/`, an `IntegrationTests.csproj` file, or `tests/Compliance/`. |

When E2E pass-through is active, the wrapper also exports the following into the container:

- `MTCONNECT_E2E_DOCKER=true` — gates the `[Category("RequiresDocker")]` test filter.
- `TESTCONTAINERS_RYUK_DISABLED=true` — the Testcontainers Ryuk reaper does not work cleanly inside the SDK image, so the wrapper disables it. Containers spawned by the test run are cleaned up by the test fixtures themselves.
- `MTCONNECT_E2E_HOST_REPO_ROOT=<repo root on host>` — Testcontainers code that needs to bind-mount fixtures into spawned containers reads this to translate from in-container `/src` paths to the host path the docker daemon will see.

## Example invocations

Native (host `dotnet`):

```bash
tools/dotnet.sh build MTConnect.NET.sln
tools/dotnet.sh test tests/MTConnect.NET-Common-Tests
tools/dotnet.sh tool restore
```

Containerized SDK:

```bash
tools/dotnet.sh --docker build MTConnect.NET.sln
tools/dotnet.sh -d test tests/MTConnect.NET-Common-Tests --configuration Release
```

Containerized against a specific SDK version:

```bash
MTCONNECT_DOTNET_SDK_TAG=9.0 tools/dotnet.sh --docker build MTConnect.NET.sln
```

Containerized E2E run — the wrapper detects the test path and enables docker-in-docker automatically:

```bash
tools/dotnet.sh --docker test tests/E2E/MTConnect.NET-E2E-Agent
```

Forced docker-in-docker for a custom invocation that the auto-detection misses:

```bash
MTCONNECT_DOTNET_E2E_DIND=1 tools/dotnet.sh --docker test tests/MyCustomDockerTest
```

Reusing the wrapper from inside another script (this is how `tools/test.sh` invokes it):

```bash
DOTNET=(tools/dotnet.sh)
if [[ "${USE_DOCKER}" == "1" ]]; then
  export MTCONNECT_DOTNET_USE_DOCKER=1
fi
"${DOTNET[@]}" tool restore
"${DOTNET[@]}" build MTConnect.NET.sln --configuration Release
```

## Behavior

### Native path (default)

1. Resolve the repo root by walking up from `tools/dotnet.sh`.
2. `cd` into the repo root (so `dotnet` picks up `Directory.Build.props`, `Directory.Packages.props`, and the `MTConnect.NET.sln` solution file).
3. `exec dotnet "$@"`.

### Docker path

1. Resolve the SDK image (`MTCONNECT_DOTNET_IMAGE`, else `mcr.microsoft.com/dotnet/sdk:${MTCONNECT_DOTNET_SDK_TAG}`).
2. Resolve the NuGet and tools volumes by name.
3. If the invocation looks like an E2E run (or `MTCONNECT_DOTNET_E2E_DIND=1` is set), add `--network=host`, mount `/var/run/docker.sock`, bind-mount the host's `docker` CLI binary read-only, and export the three E2E environment variables listed above.
4. Run `docker run --rm` with:
   - `/src` bind-mounted to the repo root.
   - The named volumes for NuGet and dotnet tools mounted at the standard paths.
   - `HOME=/root`, `PATH` extended to include `/root/.dotnet/tools`.
   - `DOTNET_NOLOGO=1` and `DOTNET_CLI_TELEMETRY_OPTOUT=1`.
5. `dotnet "$@"` runs inside the container; the process is `exec`-ed so `tools/dotnet.sh`'s own PID is replaced by `docker run`.

## Platform notes

- **Linux**: the wrapper uses `cd -P` and `readlink` carefully to be portable; it does not depend on `readlink -f`. Works on Debian / Ubuntu / Fedora out of the box.
- **macOS**: tested under `bash` 3.2 (the system default) and bash 5.x from Homebrew. The wrapper avoids bash-4-only features for the macOS-default-bash path.
- **Windows**: use the PowerShell sibling at `tools/dotnet.ps1` (same flag spelling: `-Docker`). The shell wrapper works inside Git-Bash or WSL too.

## See also

- [`tools/test.sh`](./test-sh) — the test entry point that invokes this wrapper for every `dotnet test` run.
- [Configure & Use → Run](/configure/run) — running the shipped agent and adapter (where end users do not need this wrapper).
- [Compliance → Test harness](/compliance/test-harness) — running the per-spec-version conformance suite (uses `tools/test.sh --compliance`, which routes through this wrapper).
