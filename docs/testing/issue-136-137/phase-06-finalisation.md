# Phase 06 — Finalisation

## Campaign summary

This campaign closes the merged-cluster pair `#136` + `#137`:

- **#136** — `new Device()` auto-generated a fresh random `Uuid` per
  construction, silently violating the MTConnect XSD `uuid` identity
  contract (each reconstruction produced a new identity, so a
  connector restart produced a new historian / dashboard row under
  the same `Device.Name`).
- **#137** — adjacent default-value defects in two POCO families:
  - `Device` default ctor stamped placeholder identity (`Id` from
    `StringFunctions.RandomString(10)`; `Name = "dev"`).
  - Every concrete `*Component` subclass (115 generated `*.g.cs`)
    plus the SysML-import Scriban template back-filled `Name = NameId`
    (e.g. `"axes"`, `"linear"`, `"systems"`).

Fix landed:

- Three identity / naming lines removed from
  `libraries/MTConnect.NET-Common/Devices/Device.cs` default ctor.
- `Name = NameId;` removed from 115 generated
  `libraries/MTConnect.NET-Common/Devices/Components/*.g.cs` files.
- Same line removed from
  `build/MTConnect.NET-SysML-Import/CSharp/Templates/Devices.ComponentType.scriban`
  so future regenerations produce the new shape.

## Breaking change

`new Device().Id`, `new Device().Name`, `new Device().Uuid`, and
`new <T>Component().Name` all return `null` after this PR, instead of
placeholder values. Callers must set those fields explicitly; failure
surfaces at wire-emission XSD validation (or downstream MTConnect
validators) instead of silently shipping placeholder identity.

`Agent : Device` inherits the cleaned-up base ctor; `new Agent()`
likewise leaves identity fields `null`. `Agent(MTConnectAgent)` is
unchanged — it still derives `Id`, `Name`, and `Uuid` from the agent
parameter.

## Tests landed

- 16 NUnit tests under
  `[Category("DeviceComponentDefaultsRemoved")]` in
  `tests/MTConnect.NET-Common-Tests/Devices/` (P2 reds, now green
  after P3).
- 7 NUnit regression-pin tests at
  `tests/MTConnect.NET-Common-Tests/Regressions/DeviceComponentDefaultsRegressionTests.cs`
  including reflection guards for every concrete `Device`-derived
  type and every concrete `Component` subclass (P4).
- 3 wire-shape NUnit tests at
  `tests/MTConnect.NET-XML-Tests/Devices/DeviceCtorDefaultsWireShapeTests.cs`
  pinning the empty-attribute-value emission and the explicit-setter
  round-trip (P5).

## Validation summary

- `dotnet build MTConnect.NET.sln -c Debug` — 0 errors, 563
  warnings (all pre-existing on `upstream/master`).
- `dotnet test tests/MTConnect.NET-Common-Tests/` — 24 passed.
- `dotnet test tests/MTConnect.NET-XML-Tests/` — 7 passed.
- `dotnet test tests/MTConnect.NET-SHDR-Tests/` — 27 passed.
- `dotnet test tests/IntegrationTests/` — 2 passed.
- `tests/MTConnect.NET-HTTP-Tests/` and
  `tests/MTConnect.NET-Tests-Agents/` cannot run on this base —
  pre-existing `net6.0` target framework incompatibility on
  `upstream/master`. Not in scope for this campaign.

## Deviations summary

- Plan-file commit subjects revised in flight to remove forbidden
  scopes (`docs(issue-136-137)` → `docs(testing)`) and forbidden
  category labels (`Issue136Red` → `DeviceComponentDefaultsRemoved`,
  matched at the test-fixture level by the 2026-04-25 plan-hardening
  pass). Detail in each per-phase writeup's "Deviations from plan"
  section.
- The plan's `01-foundation.md` cited the upstream defect as
  `Id = "4A1GF40513"`; the upstream HEAD has moved to
  `Id = StringFunctions.RandomString(10)`. The fix removes the line
  regardless. Documented in the P1 phase writeup.
- The plan's `03-red-tests.md` and `04-library-fix.md` called for an
  `issue-136-137-red` CI workflow; per CONVENTIONS §1.7 per-issue
  PRs do not modify `.github/workflows/` and that commit was
  dropped.
- The plan's `06-e2e-validation.md` HTTP+MQTT Docker E2E was
  downgraded to a wire-shape smoke at the XML serialisation layer;
  the higher-fidelity E2E plumbing depends on bootstrap / tests-plan
  deliverables that have not yet landed on `upstream/master`. Once
  those land, the agent-restart scenarios can be added in a
  follow-up commit. Documented in the P5 phase writeup.
- The plan's `05-regression-pins.md` cited the `compliance` test
  project for the regression fixture; that project does not exist on
  `upstream/master`, so the fallback location
  `tests/MTConnect.NET-Common-Tests/Regressions/` was used per the
  plan's own fallback note. The filename is hardened from
  `Issue136_*` to `DeviceComponentDefaultsRegressionTests` per
  CONVENTIONS §14.

## Close-out

Per the dispatch envelope, this campaign stops at the end of phase
07. The §1.5 close-out (`git rebase -i`, `gh pr ready`,
`gh pr edit --add-reviewer`) is left to the user.

## Documentation

This file. Campaign index `docs/testing/issue-136-137.md` §7 links
to it.

## Follow-ups

- Reactivate `tests/MTConnect.NET-HTTP-Tests/` and
  `tests/MTConnect.NET-Tests-Agents/` by retargeting them from
  `net6.0` to `net8.0`. Track in `todo.md §F`.
- Add HTTP+MQTT-over-Docker E2E scenarios for this contract once the
  bootstrap test infrastructure lands. Track in `todo.md §F`.
- Move the regression fixture from `common-tests` to the L5
  compliance harness once `tests/Compliance/MTConnect-Compliance-Tests/`
  lands. Track in `todo.md §F`.
