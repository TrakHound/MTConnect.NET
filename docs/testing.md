# Testing — MTConnect.NET

This page is the entry point for everything test-related in MTConnect.NET. Per-version compliance matrices, the harness scripts, and the CI workflow are linked from here.

## Per-version compliance matrices

- [`docs/testing/v2-6.md`](testing/v2-6.md) — MTConnect Standard v2.6 compliance matrix.
- [`docs/testing/v2-7.md`](testing/v2-7.md) — MTConnect Standard v2.7 compliance matrix.
- [`docs/testing/version-matrix-convention.md`](testing/version-matrix-convention.md) — topic-first single-file-per-topic fixture convention (how to add tests for a new spec version).
- [`docs/testing/workflows.md`](testing/workflows.md) — CI workflow + local harness catalog.

Each matrix lists every spec-defined element / attribute / enum value introduced or modified at that version with status (`Live` / `Pending`) and the test class that pins it.

## Test tiers

The repo organizes tests into four tiers:

1. **Unit + integration** — `tests/<library>-Tests/`. Fast (< 30 s on a clean run), runs by default in CI and on `tools/test.sh` / `tools/test.ps1`. Filtered by `Category!=XsdLoadStrict` so the strict XSD-load gate does not block the green path.
2. **Compliance** — `tests/Compliance/MTConnect-Compliance-Tests/`. Layered (`L1_XsdValidation`, `L2_CrossImpl`); see [`tests/Compliance/MTConnect-Compliance-Tests/README.md`](https://github.com/TrakHound/MTConnect.NET/blob/master/tests/Compliance/MTConnect-Compliance-Tests/README.md). Opt-in via `tools/test.sh --compliance` or `tools/test.ps1 -Compliance`.
3. **E2E** — `tests/MTConnect.NET-Integration-Tests/` + `tests/E2E/**`. Docker-gated. Opt-in via `tools/test.sh --e2e` or `MTCONNECT_E2E_DOCKER=true`.
4. **Generator regen guards** — `tests/MTConnect.NET-Generator-Tests/`. Dispatches the `build/MTConnect.NET-SysML-Import` CLI via `dotnet run --no-build` and asserts byte-identical regeneration against the current XMI (`Regen_is_deterministic_across_two_invocations` + `Current_XMI_regen_matches_committed_g_cs_tree`) plus surgical delta capture on a mutated-XMI cross-verify (`Delta_mode_against_same_XMI_concentrates_every_file_into_Compat` + `Delta_mode_against_mutated_XMI_emits_only_the_changed_file`). Complementary CLI failure-path + Compat body + stats-line invariants pinned by `CliInvocationFailureTests` and `DeltaCompatAndStatsTests`. Runs by default in the standard `dotnet test` sweep; see [`docs/testing/mutation-testing.md`](testing/mutation-testing.md) for the paired Stryker.NET mutation-score gate.

## Local entry points

- `tools/test.sh` (Linux / macOS) — `./tools/test.sh --help` lists every flag.
- `tools/test.ps1` (Windows / cross-platform PowerShell) — same surface as `test.sh`.
- `tools/dotnet.sh` / `tools/dotnet.ps1` — pinned `dotnet` SDK invocation; pass `--docker` to run inside the SDK container.

## CI

GitHub Actions workflow at [`.github/workflows/dotnet.yml`](https://github.com/TrakHound/MTConnect.NET/blob/master/.github/workflows/dotnet.yml). Matrix builds against `ubuntu-latest` and `windows-latest`, .NET 8.0.x + 9.0.x, uploads TRX + Cobertura coverage as artifacts, surfaces a coverage summary in the job log. See [`docs/testing/workflows.md`](testing/workflows.md) for the workflow catalog.

## Coverage

`tests/coverlet.runsettings` is the shared Coverlet configuration. ReportGenerator (pinned via `.config/dotnet-tools.json`) turns the per-project Cobertura XML into HTML + text summaries under `coverage-report/`.
