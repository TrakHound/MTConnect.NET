# Testing — workflow catalogue

CI workflow + local test entry points. Pairs with [`docs/testing.md`](../testing.md) (top-level testing topic) and the per-version matrices under [`docs/testing/`](.).

## CI workflow — `.github/workflows/dotnet.yml`

`build-test-coverage` runs on every push to `master` and on every non-draft PR against `master` (drafts skip; flipping ready fires the run on `ready_for_review`).

**Matrix:** `ubuntu-latest` × `windows-latest`, .NET SDK `8.0.x` + `9.0.x`.

**Steps:**

1. Checkout (`actions/checkout`).
2. Setup .NET (`actions/setup-dotnet`) — installs both 8.0.x and 9.0.x.
3. `dotnet tool restore` — pins ReportGenerator via `.config/dotnet-tools.json`.
4. `dotnet restore MTConnect.NET.sln`.
5. `dotnet build MTConnect.NET.sln --configuration Debug --no-restore`.
6. `dotnet test MTConnect.NET.sln --filter "Category!=RequiresDocker&Category!=XsdLoadStrict"` with `tests/coverlet.runsettings`. Produces TRX + Cobertura coverage XML.
7. `reportgenerator` → HTML + Markdown + Text summary at `coverage-report/`.
8. Upload TRX + Cobertura + HTML report as artifact `test-results-<os>` (retention 14 days).
9. Surface the text summary in the job log via `$GITHUB_STEP_SUMMARY`.

**Permissions:** `contents: read` only — no commit / release / package-write privileges.

**Filter rationale:**

- `Category!=RequiresDocker` skips the Testcontainers-gated cppagent parity + integration-test classes; those run only when `MTCONNECT_E2E_DOCKER=true` is exported.
- `Category!=XsdLoadStrict` skips the 122-XSD strict-load sweep that surfaces 54 known failures (XSD-1.1 features + missing xlink imports). The sweep is opt-in via `dotnet test tests/Compliance/MTConnect-Compliance-Tests/MTConnect-Compliance-Tests.csproj --filter "Category=XsdLoadStrict"`. A follow-up PR adds an XSD 1.1 validator + the W3C xlink XSD pre-load so the category goes green.

## Local — `tools/test.sh` (Linux / macOS / Git Bash)

```bash
./tools/test.sh                # default sweep — unit + integration tiers
./tools/test.sh --compliance   # also runs tests/Compliance/**
./tools/test.sh --e2e          # forces MTCONNECT_E2E_DOCKER=true
./tools/test.sh --docker       # routes every dotnet call through tools/dotnet.sh --docker
./tools/test.sh --only XML     # regex-filter to projects matching XML
./tools/test.sh --help         # full flag listing
```

## Local — `tools/test.ps1` (PowerShell, all platforms)

```powershell
./tools/test.ps1                # default sweep
./tools/test.ps1 -Compliance    # also runs tests/Compliance/**
./tools/test.ps1 -E2E           # forces MTCONNECT_E2E_DOCKER=true
./tools/test.ps1 -Docker        # routes every dotnet call through tools/dotnet.ps1 -Docker
./tools/test.ps1 -Only XML      # regex-filter to projects matching XML
```

## SDK pinning — `tools/dotnet.{sh,ps1}`

Wraps `dotnet` with a pinned SDK version (`8.0` by default). Pass `--docker` / `-Docker` to run inside `mcr.microsoft.com/dotnet/sdk:<tag>`. Override the tag via `MTCONNECT_DOTNET_SDK_TAG`; override the image via `MTCONNECT_DOTNET_IMAGE`.

## Coverage configuration — `tests/coverlet.runsettings`

Shared across every test project. Format: `cobertura,opencover`. Excludes test-only assemblies + bak files. ReportGenerator (pinned via `.config/dotnet-tools.json`) consumes the Cobertura XML.
