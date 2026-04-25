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

See `docs/testing/issue-127/phase-02-red-tests.md`.

## 4. Library fix (P3)

See `docs/testing/issue-127/phase-03-library-fix.md`.

## 5. Regression pins (P4)

See `docs/testing/issue-127/phase-04-regression-pins.md`.

## 6. E2E validation (P5)

See `docs/testing/issue-127/phase-05-e2e-validation.md`.

## 7. Campaign summary

See `docs/testing/issue-127/phase-06-docs-and-finalisation.md`.
