# Testing — workflow catalog

User-observable end-to-end paths through MTConnect.NET, plus the CI / local
test entry points that exercise them. Pairs with [`docs/testing.md`](../testing.md)
(top-level testing topic) and the per-version matrices under
[`docs/testing/`](.).

## End-to-end workflow catalog

Each row is a user-observable path from input to output. The owning
test class is the canonical fixture for the workflow.

Category convention for the integration project
(`tests/MTConnect.NET-Integration-Tests/`):

- **No category** — light, deterministic integration tests
  (`ClientAgentCommunicationTests`, `GenerateDevicesXmlTests`,
  `HttpServerLoopbackBindingTests`). These run in the dedicated CI
  integration step and in the default local sweep.
- **`[Category("E2E")]`** — heavy in-process-server workflow fixtures
  that boot a real `MTConnectAgentBroker` + HTTP server per class
  (`HttpProbeWorkflowTests`, `HttpAssetWorkflowTests`,
  `ConfigurationPolymorphicHttpProbeWorkflowTests`). Excluded from the
  default path so they cannot re-introduce timing fragility on a shared
  runner; run only when `MTCONNECT_E2E_DOCKER=true` (`tools/test.sh --e2e`
  / `tools/test.ps1 -E2E`).
- **`[Category("RequiresDocker")]`** — fixtures that spin Testcontainers
  (`MqttRelayWorkflowTests`, and the cppagent parity tests under
  `tests/Compliance/`). Run only when `MTCONNECT_E2E_DOCKER=true`.

| ID | Workflow | Input fixture | Expected output | Owning test class | Category |
|---|---|---|---|---|---|
| W01 | HTTP Probe — devices envelope | in-process `MTConnectAgentBroker` + `devices-tpl.xml` | `MTConnectDevices` envelope with the seeded device | `tests/MTConnect.NET-Integration-Tests/Workflows/HttpProbeWorkflowTests.cs` | E2E |
| W02 | HTTP Current — observation snapshot | in-process broker + an SHDR-fed dataitem | `MTConnectStreams` envelope with the observation | `tests/MTConnect.NET-Integration-Tests/ClientAgentCommunicationTests.cs::GetCurrentFieldShouldReturnUpdatedValue` | (default) |
| W03 | HTTP Sample — observation stream | in-process broker + an SHDR-fed dataitem with from + count | `MTConnectStreams` envelope containing the observation history | `tests/MTConnect.NET-Integration-Tests/ClientAgentCommunicationTests.cs::WaitForSampleShouldSucceedAfterFirstItemIsSent` | (default) |
| W04 | HTTP Asset — asset retrieval | in-process broker seeded with a `CuttingToolAsset` | `MTConnectAssets` envelope containing the asset | `tests/MTConnect.NET-Integration-Tests/Workflows/HttpAssetWorkflowTests.cs` | E2E |
| W05 | SHDR adapter -> agent -> HTTP client | `ShdrAdapter` + `MTConnectHttpClient` | client receives observation through the agent | `tests/MTConnect.NET-Integration-Tests/ClientAgentCommunicationTests.cs::WaitForSampleShouldSucceedAfterFirstItemIsSent` | (default) |
| W06 | MQTT relay — agent publishes, consumer receives | in-process broker + MqttRelay agent module + `eclipse-mosquitto:2.0.22` (Testcontainers) | downstream MQTTnet subscriber receives a `/Current/<uuid>` payload carrying the injected observation | `tests/MTConnect.NET-Integration-Tests/Workflows/MqttRelayWorkflowTests.cs` | RequiresDocker |
| W07 | cppagent JSON v2 parity | shared `Fixtures/cppagent-parity-device.xml` against `mtconnect/agent:latest` (Testcontainers) and in-process MT.NET | normalized `/probe`, `/current`, `/sample` shapes byte-equal modulo `Fixtures/cross-impl-whitelist.json` | `tests/Compliance/MTConnect-Compliance-Tests/L2_CrossImpl/CppAgentParityWorkflowTests.cs` | RequiresDocker, E2E |
| W08 | HTTP probe round-trips Motion.Axis as `IAxisDataSet` | in-process broker + Device with `Motion.Axis = AxisDataSet` | `/probe` envelope narrows back to `IAxisDataSet` | `tests/MTConnect.NET-Integration-Tests/Workflows/ConfigurationPolymorphicHttpProbeWorkflowTests.cs` | E2E |
| W09 | HTTP probe round-trips CoordinateSystem.Origin as `IOriginDataSet` | in-process broker + Device with `CoordinateSystem.Origin = OriginDataSet` | `/probe` envelope narrows back to `IOriginDataSet` | `tests/MTConnect.NET-Integration-Tests/Workflows/ConfigurationPolymorphicHttpProbeWorkflowTests.cs` | E2E |
| W10 | HTTP probe round-trips Transformation.Rotation as `IRotationDataSet` | in-process broker + Device with `Transformation.Rotation = RotationDataSet` | `/probe` envelope narrows back to `IRotationDataSet` | `tests/MTConnect.NET-Integration-Tests/Workflows/ConfigurationPolymorphicHttpProbeWorkflowTests.cs` | E2E |
| W11 | XML <-> JSON round-trip | golden XML fixture | JSON serialization -> XML deserialization -> structural equality | `tests/MTConnect.NET-XML-Tests/Streams/Current.cs` (existing) | (default) |

## CI workflow — `.github/workflows/dotnet.yml`

`build-test-coverage` runs on every push to `master` and on every
non-draft PR against `master` (drafts skip; flipping ready fires the
run on `ready_for_review`).

**Matrix:** `ubuntu-latest` × `windows-latest`, .NET SDK `8.0.x` + `9.0.x`.

**Steps:**

1. Checkout (`actions/checkout`).
2. Setup .NET (`actions/setup-dotnet`) — installs both 8.0.x and 9.0.x.
3. `dotnet tool restore` — pins ReportGenerator via `.config/dotnet-tools.json`.
4. `dotnet restore MTConnect.NET.sln`.
5. `dotnet build MTConnect.NET.sln --configuration Debug --no-restore`.
6. `dotnet test MTConnect.NET.sln` with `tests/coverlet.runsettings` and
   `--filter "Category!=RequiresDocker&Category!=XsdLoadStrict"`. This is
   the unit + light-integration run. Followed by a guard step that fails
   the job if the TRX `<Counters total=...>` is 0.
7. `dotnet build` the integration project with `-p:IntegrationCoverage=true`
   (the solution build compiled it as a non-test assembly because its
   `IsTestProject` is false without the flag), then
   `dotnet test tests/MTConnect.NET-Integration-Tests/...` `--no-build`
   with `tests/coverlet.integration.runsettings` and
   `--filter "Category!=RequiresDocker&Category!=XsdLoadStrict&Category!=E2E"`.
   Followed by the same zero-test guard so a future discovery regression
   hard-fails instead of false-greening.
8. `reportgenerator` → HTML + Markdown + Text summary at `coverage-report/`.
9. Upload TRX + Cobertura + HTML report as artifact `test-results-<os>`
   (retention 14 days).
10. Surface the text summary in the job log via `$GITHUB_STEP_SUMMARY`.

**Permissions:** `contents: read` only — no commit / release /
package-write privileges.

**Filter rationale:**

- `Category!=RequiresDocker` skips the Testcontainers-gated cppagent
  parity + MQTT-relay classes; those run only when
  `MTCONNECT_E2E_DOCKER=true` is exported.
- `Category!=E2E` (integration step only) skips the heavy
  in-process-server workflow fixtures (`HttpProbeWorkflowTests`,
  `HttpAssetWorkflowTests`,
  `ConfigurationPolymorphicHttpProbeWorkflowTests`). Those boot a real
  broker + HTTP server per class and re-introduce the timing fragility
  the coverage-isolation split exists to remove; they run under
  `MTCONNECT_E2E_DOCKER=true`. `ClientAgentCommunicationTests`,
  `GenerateDevicesXmlTests` and `HttpServerLoopbackBindingTests` carry
  no category and still run in the integration step.
- `Category!=XsdLoadStrict` skips the strict-XSD-load sweep that
  surfaces known failures (XSD-1.1 features + missing xlink imports).
  The sweep is opt-in via
  `dotnet test tests/Compliance/MTConnect-Compliance-Tests/MTConnect-Compliance-Tests.csproj --filter "Category=XsdLoadStrict"`.

## Why the integration project is a separate CI step

The integration project drives the in-process Agent + embedded HTTP
server + SHDR adapter as one timing-critical sample-delivery hot path.
Under coverlet IL instrumentation of `MTConnect.NET-Common` /
`MTConnect.NET-HTTP` on a slow shared runner a Sample observation can
arrive after the test's assertion wait. Those two assemblies are
covered, faster, by `MTConnect.NET-Common-Tests` /
`MTConnect.NET-HTTP-Tests`, so excluding their instrumentation here
loses no net (merged) coverage. `MTConnect.NET-SHDR` stays instrumented
(this suite is its only runtime coverage). The project therefore runs
with `tests/coverlet.integration.runsettings` (the shared config plus
those two assemblies excluded, plus `MaxCpuCount=1` — scoped to this
step only; the shared file no longer pins it). VSTest run-settings
precedence is the CLI settings flag over `RunSettingsFilePath`, so a
per-project settings file cannot override a solution-wide `--settings`;
running the project as its own step is the only way to give it a
different coverage scope, hence the `IsTestProject` / `IntegrationCoverage`
gating in the `.csproj`.

## Local — `tools/test.sh` (Linux / macOS / Git Bash)

```bash
./tools/test.sh                # default sweep — unit + light integration
./tools/test.sh --compliance   # also runs tests/Compliance/**
./tools/test.sh --e2e          # forces MTCONNECT_E2E_DOCKER=true; widens
                               #   the integration filter to also run the
                               #   Category=E2E / RequiresDocker fixtures
./tools/test.sh --docker       # routes every dotnet call through tools/dotnet.sh --docker
./tools/test.sh --only XML     # regex-filter to projects matching XML
./tools/test.sh --help         # full flag listing
```

The integration project is owned by exactly one tier — the default
loop — which builds it with `-p:IntegrationCoverage=true`, applies
`tests/coverlet.integration.runsettings`, and excludes
`Category=E2E`/`RequiresDocker` by default. `--e2e` widens that filter
in place rather than re-running the project in a separate tier.

## Local — `tools/test.ps1` (PowerShell, all platforms)

```powershell
./tools/test.ps1                # default sweep
./tools/test.ps1 -Compliance    # also runs tests/Compliance/**
./tools/test.ps1 -E2E           # forces MTCONNECT_E2E_DOCKER=true; widens
                                #   the integration filter as above
./tools/test.ps1 -Docker        # routes every dotnet call through tools/dotnet.ps1 -Docker
./tools/test.ps1 -Only XML      # regex-filter to projects matching XML
```

`test.ps1` is at parity with `test.sh`: the new project path
(`tests/MTConnect.NET-Integration-Tests`), the
`-p:IntegrationCoverage=true` build flag, the dedicated integration
runsettings, and the single-owner tier ownership.

## SDK pinning — `tools/dotnet.{sh,ps1}`

Wraps `dotnet` with a pinned SDK version (`8.0` by default). Pass
`--docker` / `-Docker` to run inside `mcr.microsoft.com/dotnet/sdk:<tag>`.
Override the tag via `MTCONNECT_DOTNET_SDK_TAG`; override the image via
`MTCONNECT_DOTNET_IMAGE`.

## Coverage configuration

`tests/coverlet.runsettings` is the shared config for every test
project except the integration project. Format: `cobertura`. Excludes
test-only assemblies + bak files. `tests/coverlet.integration.runsettings`
is the shared config plus `MTConnect.NET-Common` / `MTConnect.NET-HTTP`
excluded from instrumentation and `MaxCpuCount=1`, applied only to the
integration project's dedicated step. ReportGenerator (pinned via
`.config/dotnet-tools.json`) merges every Cobertura XML into an
HTML + Summary at `coverage-report/`.
