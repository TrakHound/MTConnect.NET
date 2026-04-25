# Issue #128 — JSON-cppagent schemaVersion hardcoded

## 1. Defect + scope

`MTConnectStreams.schemaVersion` and `MTConnectDevices.schemaVersion` were
hardcoded to the literal `"2.0"` in both ctors of
`JsonMTConnectStreams` and `JsonMTConnectDevices`, regardless of the
agent's configured `DefaultVersion`. The cppagent JSON-MQTT format
contract requires the configured release to flow through to the wire.

Surface (two production files):

- `libraries/MTConnect.NET-JSON-cppagent/Streams/JsonMTConnectStreams.cs`
- `libraries/MTConnect.NET-JSON-cppagent/Devices/JsonMTConnectDevices.cs`

`JsonMTConnectAssets.cs` does not expose `SchemaVersion` and is unaffected.

## 2. Investigation (P1)

- Both hardcode sites confirmed at HEAD (Streams ctors lines 24-40; Devices ctors lines 26-45).
- `IStreamsResponseOutputDocument.Version` / `IDevicesResponseDocument.Version` already carry the configured release; no pipeline plumbing required.
- Two-segment format (`Version.ToString()`) matches cppagent reference output.

See `docs/testing/issue-128/phase-01-defect-scoping.md` for the full
inventory + decision record.

## 3. Red tests (P2)

- 30 NUnit cases (Streams + Devices × 14 library versions).
- 28 fail with `Expected "<configured>" / But was: "2.0"`; 2 cases (v2.0) pass coincidentally.
- Category: `SchemaVersionFromConfiguration` (descriptive label per CONVENTIONS §14).

See `docs/testing/issue-128/phase-02-red-tests.md` for the matrix +
sample failure output + CI-gate notes.

## 4. Library fix (P3)

- Replace `SchemaVersion = "2.0"` in both ctors of `JsonMTConnectStreams` + `JsonMTConnectDevices` with `document.Version?.ToString()`.
- Default ctor no longer stamps `"2.0"`; the value is null when no document is supplied (previously a defect-masking literal).
- 28/28 previously-red cases turn green; 31/31 total in the test project pass.

See `docs/testing/issue-128/phase-03-library-fix.md` for the diff +
behaviour notes.

## 5. Regression pins (P4)

- `Issue128_SchemaVersionConfiguredTests` — green-on-arrival regression for both envelopes × 14 versions.
- `Issue128_HardcodedLiteralGuardTests` — regex-grep guard refusing re-introduction of any `SchemaVersion = "<literal>";` assignment in the two touched files.

See `docs/testing/issue-128/phase-04-regression-pins.md`.

## 6. E2E validation (P5)

Deferred — Docker-gated MQTT round-trip requires `Testcontainers` /
`MQTTnet` infrastructure that lives on `feat/issue-133`. Authoring it
here would silently duplicate that branch's deliverables. The
unit-level matrix in P2/P3/P4 covers the entire formatter contract;
E2E is a follow-up commit on this PR after #133 merges and the rebase
makes the infrastructure available.

See `docs/testing/issue-128/phase-05-e2e-validation.md`.

## 7. Campaign summary (P6)

- Issue: TrakHound/MTConnect.NET#128 — JSON-cppagent `schemaVersion`
  hardcoded to `"2.0"` regardless of `AgentConfiguration.DefaultVersion`.
- Root cause: literal assignment in both ctors of
  `JsonMTConnectStreams.cs` + `JsonMTConnectDevices.cs`; the configured
  version was already on the response document but never read.
- Fix: replace the literal with `document.Version?.ToString()` inside
  the existing null guard. Default ctor no longer stamps `"2.0"`.
- Format: two-segment via `System.Version.ToString()` (e.g. `"2.5"`)
  matching cppagent's wire output.
- Tests: 30 parametric cases assert correct behaviour; 28 regression
  pins keep the fix safe; 2 guard cases refuse re-introduction of any
  hardcoded `SchemaVersion = "<literal>";` assignment.
- Coverage: both touched files exercised by the matrix.
- E2E: deferred pending bootstrap merge — see phase 05 writeup.
- No public API change.
