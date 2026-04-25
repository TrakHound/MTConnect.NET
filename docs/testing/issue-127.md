# Issue #127 — Header.version reports library assembly version instead of configured MTConnect release

## 1. Defect + scope

The `Header.version` attribute on every MTConnect response document
(`MTConnectDevices`, `MTConnectStreams`, `MTConnectAssets`, `MTConnectError`)
must contain the MTConnect Standard release the agent is configured to serve
(for example `2.5.0.0` for an agent serving v2.5). The reference cppagent
implementation reports it correctly. MTConnect.NET regressed and emitted the
library assembly version (`6.9.0.0`) instead.

Source: <https://github.com/TrakHound/MTConnect.NET/issues/127>.

Spec citation:

- <https://docs.mtconnect.org/> Part 1.0 §3 (Header), Part 2.0 §7 (Streams
  envelope), Part 3.0 §5 (Devices envelope), Part 4.0 §5 (Assets envelope) —
  the `version` attribute on the `Header` element is the MTConnect Standard
  release the agent serves.
- XSD: `MTConnectDevices_<vN.M>.xsd`, `MTConnectStreams_<vN.M>.xsd`,
  `MTConnectAssets_<vN.M>.xsd`, `MTConnectError_<vN.M>.xsd` — the `Header`
  complex-type's `version` attribute is `xs:string`, formatted by cppagent
  as the four-segment release string (`2.7.0.0`).

In-scope:

- The four header builders in `MTConnectAgentBroker` (`GetDevicesHeader`,
  `GetStreamsHeader`, `GetAssetsHeader`, `GetErrorHeader`) and the six
  redundant overwrites in the response-document construction methods.
- Regression tests pinning the new behavior across every supported
  MTConnect Standard version constant.

Out of scope:

- The `MTConnectAgent.Version` property — retained as a legitimate library
  assembly version surface for diagnostics and logging.
- `Header.schemaVersion` (issue #128), `Header.testIndicator` (issue #131),
  v2.6 / v2.7 support (issue #133) — separate plans.

## 2. Investigation (P1)

- Root cause: four header-builder methods in
  `libraries/MTConnect.NET-Common/Agents/MTConnectAgentBroker.cs`
  (`GetDevicesHeader`, `GetStreamsHeader`, `GetAssetsHeader`,
  `GetErrorHeader`) write `Version = Version.ToString()` where the
  bare `Version` is the inherited library assembly version.
- Six redundant overwrites in the same file rewrite the same value
  after the builder runs. Removed by P3.
- Format target: four-segment string of the form `<major>.<minor>.0.0`,
  matching the cppagent reference. Achieved via
  `new Version(version.Major, version.Minor, 0, 0).ToString()`
  because `MTConnectVersions` constants only carry major + minor.
- No existing test asserts a literal `Header.version` value.
- See `docs/testing/issue-127/phase-01-defect-scoping.md`.

## 3. Red tests (P2)

- Test fixture:
  `tests/MTConnect.NET-Common-Tests/Headers/HeaderVersionRegressionTests.cs`.
- 61 NUnit cases (4 envelope paths × 15 versions + 1 guard) all red on
  HEAD with `Expected: "<x.y>.0.0" / But was: "6.9.0.0"`.
- Reflection-based version matrix auto-extends when new constants
  land in `MTConnectVersions`.
- See `docs/testing/issue-127/phase-02-red-tests.md`.

## 4. Library fix (P3)

- Single-file production change to
  `libraries/MTConnect.NET-Common/Agents/MTConnectAgentBroker.cs`.
- Four header builders route `MTConnectVersion` through a new
  private `FormatHeaderVersion` helper that produces the four-segment
  string (e.g. `2.5.0.0`).
- `GetErrorHeader` gains an optional `Version` parameter; the two
  `GetErrorResponseDocument` overloads now pass it through.
- Six redundant `header.Version = Version.ToString()` overwrites
  removed.
- All 61 red tests transitioned to green; pre-existing tests in the
  Common, XML, and SHDR test projects unaffected.
- See `docs/testing/issue-127/phase-03-library-fix.md`.

## 5. Regression pins (P4)

- The P2 fixture
  `tests/MTConnect.NET-Common-Tests/Headers/HeaderVersionRegressionTests.cs`
  already carries the per-issue regression assertions plus the
  repo-wide `No_response_envelope_emits_the_library_assembly_version`
  guard. No second file authored.
- The compliance harness project (`11-tests/` plan, P9) is not yet
  on `upstream/master`; on its arrival, the fixture moves under the
  L5 regression layout in that plan, not in this branch.
- See `docs/testing/issue-127/phase-04-regression-pins.md`.

## 6. E2E validation (P5)

- `tests/MTConnect.NET-XML-Tests/Headers/HeaderVersionXmlRoundTripTests.cs`
  drives a real broker through `XmlDevicesResponseDocument.ToXmlStream`
  and parses the emitted XML to assert the `Header[@version]`
  attribute equals the configured MTConnect release.
- 15 parametric cases (one per `MTConnectVersions` constant).
- The XML round-trip pins the formatter pass-through layer; the
  unit-tests in P2 cover the DTO origin layer. Together they
  span every defect surface for `Header.version` end-to-end.
- See `docs/testing/issue-127/phase-05-e2e-validation.md`.

## 7. Campaign summary

- Issue: <https://github.com/TrakHound/MTConnect.NET/issues/127> —
  `Header.version` reported the library assembly version
  (`6.9.0.0`) instead of the configured MTConnect Standard release.
- Root cause: four header builders in `MTConnectAgentBroker` wrote
  the bare `Version` identifier (the inherited
  `MTConnectAgent.Version` library-assembly value) into
  `header.Version`, plus six redundant overwrites in the
  response-document construction methods.
- Fix: route the configured `MTConnectVersion` through a new
  `FormatHeaderVersion` helper that emits the four-segment shape
  (e.g. `2.5.0.0`) the cppagent reference uses. Drop the six
  redundant overwrites. `MTConnectAgent.Version` retained as a
  diagnostic surface.
- Coverage: 100 % by inspection on the touched method bodies in
  `MTConnectAgentBroker.cs`. The plan's coverlet runsettings
  infrastructure is owned by `00-bootstrap/`; the gate falls back
  to the manual inspection captured in `phase-03-library-fix.md`
  until that plan lands.
- Tests: 76 NUnit cases pinned across the unit and round-trip
  layers (61 broker-DTO + 15 XML round-trip).
- Public API change: none. `GetErrorHeader` gained an optional
  `Version` parameter, which is non-breaking for external callers
  (the method is `private`).
- Out-of-scope follow-ups surfaced during the audit:
  - `Header.schemaVersion` hardcoded — issue #128.
  - `Header.testIndicator` always emitted as `false` — issue #131.
  - v2.6 / v2.7 standard release support absent — issue #133.
- See `docs/testing/issue-127/phase-06-docs-and-finalisation.md`
  for the per-phase audit and PR coordination notes.
