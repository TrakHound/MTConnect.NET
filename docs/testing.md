# Testing — MTConnect.NET

This page is the entry point for everything test-related in MTConnect.NET. Per-version compliance matrices, the harness scripts, and the CI workflow are linked from here.

## Per-version compliance matrices

- [`docs/testing/v2-6.md`](testing/v2-6.md) — MTConnect Standard v2.6 compliance matrix.
- [`docs/testing/v2-7.md`](testing/v2-7.md) — MTConnect Standard v2.7 compliance matrix.
- [`docs/testing/workflows.md`](testing/workflows.md) — CI workflow + local harness catalog.

Each matrix lists every spec-defined element / attribute / enum value introduced or modified at that version with status (`Live` / `Pending`) and the test class that pins it.

## Test tiers

The repo organises tests into three tiers:

1. **Unit + integration** — `tests/<library>-Tests/`. Fast (< 30 s on a clean run), runs by default in CI and on `tools/test.sh` / `tools/test.ps1`. Filtered by `Category!=RequiresDocker&Category!=XsdLoadStrict` so Docker-gated suites and the strict XSD-load gate do not block the green path.
2. **Compliance** — `tests/Compliance/MTConnect-Compliance-Tests/`. Layered (`L1_XsdValidation`, `L2_XmiOclAssertions`, `L4_CrossImpl`, `L5_Regressions`); see [`tests/Compliance/MTConnect-Compliance-Tests/README.md`](../tests/Compliance/MTConnect-Compliance-Tests/README.md). Opt-in via `tools/test.sh --compliance` or `tools/test.ps1 -Compliance`.
3. **E2E** — `tests/IntegrationTests/` + `tests/E2E/**`. Docker-gated. Opt-in via `tools/test.sh --e2e` or `MTCONNECT_E2E_DOCKER=true`.

## Local entry points

- `tools/test.sh` (Linux / macOS) — `./tools/test.sh --help` lists every flag.
- `tools/test.ps1` (Windows / cross-platform PowerShell) — same surface as `test.sh`.
- `tools/dotnet.sh` / `tools/dotnet.ps1` — pinned `dotnet` SDK invocation; pass `--docker` to run inside the SDK container.

## CI

GitHub Actions workflow at [`.github/workflows/dotnet.yml`](../.github/workflows/dotnet.yml). Matrix builds against `ubuntu-latest` and `windows-latest`, .NET 8.0.x + 9.0.x, uploads TRX + Cobertura coverage as artifacts, surfaces a coverage summary in the job log. See [`docs/testing/workflows.md`](testing/workflows.md) for the workflow catalog.

## Coverage

`tests/coverlet.runsettings` is the shared Coverlet configuration. ReportGenerator (pinned via `.config/dotnet-tools.json`) turns the per-project Cobertura XML into HTML + text summaries under `coverage-report/`.
