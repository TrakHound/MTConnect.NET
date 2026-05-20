# `tools/test.sh`

`tools/test.sh` is the local test entry point for `MTConnect.NET`. It iterates every `tests/**/*.csproj` (so new test projects are picked up automatically without script edits), optionally adds the Compliance and Docker-gated E2E tiers, runs them through [`tools/dotnet.sh`](./dotnet-sh) so the same script works against the host `dotnet` or a containerized SDK, and produces a unified coverage report under `coverage-report/`.

This is a **contributor** tool. End users running the shipped agent or adapter binaries do not need it.

A PowerShell sibling lives at `tools/test.ps1` with the same semantics and the same flag names (`-Docker`, `-Compliance`, `-E2E`, `-Only` instead of `--docker`, `--compliance`, `--e2e`, `--only`).

## Synopsis

```text
tools/test.sh [--docker] [--compliance] [--e2e] [--only <pattern>] [--help]
```

## Flags

| Flag | Short | Description |
|---|---|---|
| `--docker` | `-d` | Run every `dotnet` invocation through `tools/dotnet.sh --docker`, i.e. inside the `mcr.microsoft.com/dotnet/sdk` container. Equivalent to exporting `MTCONNECT_DOTNET_USE_DOCKER=1` before the call. |
| `--compliance` | `-c` | Include the MTConnect compliance harness under `tests/Compliance/**` in addition to the default unit + integration tier. |
| `--e2e` | `-e` | Force the E2E / Docker-gated suites. Sets `MTCONNECT_E2E_DOCKER=true` and runs the Testcontainers-backed tests under `tests/IntegrationTests` and `tests/E2E/**`. |
| `--only <PATTERN>` | `-o` | Run only test projects whose path matches `PATTERN` (case-insensitive `grep -E` regex). Use to narrow the run to one library while iterating. |
| `--help` | `-h` | Print the usage block embedded at the top of the script and exit. |

## Environment variables

| Variable | Default | Description |
|---|---|---|
| `MTCONNECT_DOTNET_USE_DOCKER` | unset | When set to `1`, behaves as if `--docker` were passed. |
| `MTCONNECT_E2E_DOCKER` | unset (treated as `false`) | When set to `true` / `yes` / `on` / `1`, drops the default `Category!=RequiresDocker` filter so Docker-gated tests run. `--e2e` sets this for you. |

The script also forwards every variable that [`tools/dotnet.sh`](./dotnet-sh) honors (`MTCONNECT_DOTNET_SDK_TAG`, `MTCONNECT_DOTNET_IMAGE`, `MTCONNECT_NUGET_VOLUME`, `MTCONNECT_DOTNET_TOOLS_VOLUME`, `MTCONNECT_DOTNET_E2E_DIND`) to the underlying `dotnet` invocations.

## Tier semantics

The script splits tests into three tiers based on path:

1. **Unit + integration tier** — every `tests/**/*.csproj` excluding `tests/Compliance/**` and `tests/E2E/**`. Always runs. Filter `Category!=RequiresDocker` is applied unless `--e2e` is set.
2. **Compliance tier** — every `tests/Compliance/**/*.csproj`. Skipped unless `--compliance` is set. No category filter applied.
3. **E2E tier** — every `tests/IntegrationTests/**/*.csproj` plus `tests/E2E/**/*.csproj`. Skipped unless `MTCONNECT_E2E_DOCKER` evaluates truthy (which `--e2e` arranges).

When `--only <pattern>` is supplied, the unit + integration tier is filtered to projects whose path matches the regex. The Compliance and E2E tiers are not filtered by `--only` (they are infrastructure-gated, not pattern-gated).

## Example invocations

Default — unit + integration on the host `dotnet`:

```bash
tools/test.sh
```

Add the Compliance harness:

```bash
tools/test.sh --compliance
```

Run only the XML and SHDR test projects:

```bash
tools/test.sh --only 'XML|SHDR'
```

Containerized full run including E2E:

```bash
tools/test.sh --docker --e2e
```

Containerized full run including E2E and Compliance:

```bash
tools/test.sh --docker --compliance --e2e
```

Run a single test project quickly while iterating on a library:

```bash
tools/test.sh --only 'MTConnect.NET-Common-Tests'
```

## Output

The script writes the following under the repo root:

| Path | Contents |
|---|---|
| `TestResults/<project>/` | Per-project Cobertura coverage XMLs and TRX result files, one directory per test project. |
| `coverage-report/` | Merged coverage report from `reportgenerator`. Contains HTML browseable output (`index.html`), a Cobertura merged XML, a Markdown summary, and a plain-text summary. |
| `coverage-report/Summary.txt` | Plain-text coverage summary. Printed to stdout at the end of a successful run. |
| `coverage/` | (Reserved.) Cleared at the start of every run. |

The `TestResults/`, `coverage/`, and `coverage-report/` directories are deleted at the start of every run, so a partial previous run cannot pollute the next one.

## Behavior

1. Resolve the repo root by walking up from `tools/test.sh`.
2. Parse flags; honor `MTCONNECT_DOTNET_USE_DOCKER` and `MTCONNECT_E2E_DOCKER` env-vars.
3. Wipe `TestResults/`, `coverage/`, `coverage-report/` and recreate `TestResults/`.
4. `tools/dotnet.sh tool restore` — pull `reportgenerator` and any other local dotnet tools listed in `.config/dotnet-tools.json`.
5. Enumerate the unit + integration tier projects, apply `--only` if supplied, and run each with:
   ```bash
   tools/dotnet.sh test <proj> \
     --configuration Release \
     [--settings tests/coverlet.runsettings] \
     [--filter Category!=RequiresDocker] \
     --collect:"XPlat Code Coverage" \
     --results-directory TestResults/<project-name>
   ```
6. If `--compliance` was set, enumerate `tests/Compliance/**/*.csproj` and run each with the same options minus the category filter.
7. If E2E is enabled, enumerate `tests/IntegrationTests/**/*.csproj` and `tests/E2E/**/*.csproj` and run each with the same options minus the category filter.
8. Run `tools/dotnet.sh tool run reportgenerator` against every `TestResults/**/coverage.cobertura.xml`, writing the merged report to `coverage-report/` in HTML, text, Markdown, and Cobertura formats.
9. If `coverage-report/Summary.txt` exists, print it.

## Failure handling

The script runs under `set -euo pipefail`. Any failing `dotnet test` invocation aborts the run with a non-zero exit code; the coverage report is **not** produced in that case (so a partially-run suite cannot pretend to be a clean coverage measurement). To debug a specific failing project, use `--only` to narrow the run to that project plus its peers and re-run.

## Platform notes

- **Linux + macOS**: native run. The script uses `cd -P` and `readlink` carefully to be portable to bash 3.2 (the macOS default).
- **Windows**: use the PowerShell sibling at `tools/test.ps1` (same flag names: `-Docker`, `-Compliance`, `-E2E`, `-Only`). The shell script also works inside Git-Bash or WSL.

## See also

- [`tools/dotnet.sh`](./dotnet-sh) — the `dotnet` wrapper this script uses for every invocation.
- [Compliance → Test harness](/compliance/test-harness) — how to interpret the Compliance-tier output specifically.
- [SysML importer](./sysml-import) — runs `tools/test.sh` after every regeneration to verify the regenerated `*.g.cs` files compile and the suite stays green.
- [API reference](/api/) — the public surface that the tests under `tests/` exercise.
