# Phase 04 — Regression pins

## Executed

Added a single permanent regression-pin fixture at
`tests/MTConnect.NET-Common-Tests/Regressions/DeviceComponentDefaultsRegressionTests.cs`.
Naming follows CONVENTIONS §14: the fixture name describes the
contract it pins (`DeviceComponentDefaultsRegressionTests`), not the
issue number. The plan's draft name (`Issue136_*`) is forbidden; the
hardened name lives here instead.

The fixture pins:

- `Device_default_constructor_leaves_identity_fields_null` —
  multi-assertion that `Device.Id`, `Device.Name`, `Device.Uuid` are
  all `null` after default construction.
- `Agent_default_constructor_leaves_identity_fields_null` — same for
  the `Agent : Device` subclass, since Agent inherits the cleaned-up
  base ctor.
- `Sequential_default_Devices_share_null_Uuid` — the original #136
  symptom: GUID drift across constructions. Pinned because the bug is
  silent (no exception; just per-construction identity).
- `Object_initializer_continues_to_set_Device_identity` — no-regression
  guard: callers using object-initialiser syntax must still be able to
  set identity.
- `No_Device_subclass_default_constructor_back_fills_identity` —
  reflection guard walking every concrete `Device`-derived type with a
  public default ctor and asserting it leaves `Id`, `Name`, and
  `Uuid` `null`.
- `No_Component_subclass_default_constructor_back_fills_Name` —
  reflection guard walking every concrete `Component`-derived type with
  a public default ctor and asserting it leaves `Name` `null`. The
  base `MTConnect.Devices.Component` is excluded from the assertion
  (it has no `NameId` to back-fill from); concrete subclasses are
  enforced.
- `Reflection_guard_walks_a_meaningful_set_of_Component_subclasses` —
  defence against silent drift: if a future refactor moves the
  generated subclasses out of the assembly, the prior guard would
  pass vacuously; this test fails loudly in that case.

The fixture is **not** tagged `[Category("DeviceComponentDefaultsRemoved")]`.
That category remains on the P2 red fixtures so they continue to
behave as the documented red-state filter; the regression fixture
runs under the default category and is enforced by every CI run.

## No tests-plan migration

Per the plan overview §6 and CONVENTIONS §17.8 row about plan
hardening: neither #136 nor #137 was in `plans/11-tests/`'s
upstream-issues starter list (which covered #128-#135). No edits to
the tests plan are needed; the regression fixture lives entirely
inside this campaign's PR.

## Validation

- `dotnet test tests/MTConnect.NET-Common-Tests/` — 24 passed, 0
  failed (1 pre-existing `Test1`, 16 `DeviceComponentDefaultsRemoved`
  reds-now-green, 7 regression fixtures).

## Deviations from plan

- The plan's `05-regression-pins.md` cites the test file location as
  `tests/Compliance/MTConnect-Compliance-Tests/L5_Regressions/Issue136_DeviceCtorUuidStabilityTests.cs`
  with a fallback to `tests/MTConnect.NET-Common-Tests/Regressions/Issue136_*`.
  The compliance project does not exist on `upstream/master`; the
  fallback location is used. The filename is hardened from
  `Issue136_*` to `DeviceComponentDefaultsRegressionTests` per
  CONVENTIONS §14.
- The plan's two-commit split (`test(compliance)` + `test(compliance)`
  for guard) collapses into one `test(common-tests)` commit because
  (a) the file lives in `common-tests`, not `compliance`, per §5.3
  scope alignment, and (b) the fixture is a single coherent
  regression-pin — splitting it would violate the §5.4 "production
  code wins" + atomic-commit principle.

## Documentation

This file. Campaign index `docs/testing/issue-136-137.md` §5 links
to it.

## Follow-ups

- Once the compliance project (`tests/Compliance/MTConnect-Compliance-Tests/`)
  lands per `plans/11-tests/`, this fixture can move there with a
  follow-up rename / relocation. Track in `todo.md §F`.
