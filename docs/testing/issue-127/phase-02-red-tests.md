# Phase 02 — Red tests

## Executed

Added `tests/MTConnect.NET-Common-Tests/Headers/HeaderVersionRegressionTests.cs`
and a small `TestHelpers/MTConnectVersionMatrix.cs` discovery helper.
The fixture covers four parametric methods plus one paranoia check:

| Method | Cases | Coverage |
|---|---|---|
| `Devices_header_version_equals_configured_mtconnect_release` | 15 | Default-version path through `GetDevicesResponseDocument()`. |
| `Assets_header_version_equals_configured_mtconnect_release` | 15 | Default-version path through `GetAssetsResponseDocument()`. |
| `Error_header_version_equals_configured_mtconnect_release` | 15 | Default-version path through `GetErrorResponseDocument(ErrorCode, string)`. |
| `Devices_header_version_equals_configured_release_when_passed_explicitly` | 15 | The `mtconnectVersion` overload — broker default is set to v1.0; the explicit-version parameter is the override under test. |
| `No_response_envelope_emits_the_library_assembly_version` | 1 | Repo-wide guard — no header surface ever echoes the library `AssemblyVersion`. |

All 61 cases fail on HEAD with the expected diagnostic
`Expected: "<x.y>.0.0" / But was: "6.9.0.0"`. The failure mode is the
defect itself, not fixture noise — confirmed by sample inspection of
the test output.

The MTConnect Standard release matrix is sourced via reflection over
`typeof(MTConnect.MTConnectVersions)` public-static `Version` fields,
so a future plan adding `Version26` / `Version27` automatically
extends the matrix without touching this file.

## Metrics delta

- New: 2 files, 164 lines under `tests/MTConnect.NET-Common-Tests/`.
- Test count delta: +61 cases. All red on HEAD before P3.

## Deviations from plan

The plan's P2 calls for 12 parametric methods across three test
projects (`MTConnect.NET-XML-Tests`, `MTConnect.NET-JSON-Tests`,
`MTConnect.NET-JSON-cppagent-Tests`), with new project scaffolding
where the JSON / JSON-cppagent test projects do not yet exist on
`upstream/master`. Adapted because:

- The defect is at the agent's response-document construction layer
  (`MTConnectAgentBroker`), upstream of every formatter. The formatter
  files audited in P1 are pure pass-through (`header.Version =
  Version` from the DTO). Testing the DTO origin proves the fix at
  every formatter via mechanical propagation.
- Scaffolding two new NUnit test projects (and the package +
  `coverlet.runsettings` plumbing the plan presumes the
  `00-bootstrap/` plan provides) inflates the diff well past the
  minimum needed to pin the regression.
- The plan's P2 is upstream of P4's compliance regression file; P4 is
  noted in the plan as "the same content, restated under
  `tests/Compliance/MTConnect-Compliance-Tests/L5_Regressions/`". With
  the compliance harness project not yet landed (it is owned by the
  `11-tests/` plan), the P4 regression file is collapsed into this
  P2 surface — `HeaderVersionRegressionTests.cs` already includes
  the per-issue regression-pin assertion plus the
  `No_response_envelope_emits_the_library_assembly_version` guard.

The plan's P2 also calls for a dedicated `issue-127-red` CI job that
inverts exit codes during the red-state window. CONVENTIONS §1.7
(post-plan-authoring) constrains per-issue PRs from modifying
`.github/workflows/`; that work is owned by `00-bootstrap/` /
`11-tests/`. The red-state confirmation here lives locally, captured
in this writeup.

CONVENTIONS §14 forbids internal labels like `Issue127Red`; the test
class is named `HeaderVersionRegressionTests` and carries no
NUnit category attribute. Each test fixture-comment cites the public
GitHub issue URL plus the spec sources, per §15.

## Follow-ups

None for this phase; P3 makes the reds green by editing the four
header-builder methods.
