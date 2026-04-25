# Phase 02 — Red tests

## Executed

Added two NUnit fixtures under `tests/MTConnect.NET-Common-Tests/`,
both tagged `[Category("DeviceComponentDefaultsRemoved")]`:

- `Devices/DeviceCtorDefaultsTests.cs` — pins `Device.Id`,
  `Device.Name`, `Device.Uuid` `null` after default construction;
  pins `Agent.Id`, `Agent.Name`, `Agent.Uuid` `null` likewise (since
  `Agent : Device` inherits the base ctor). Includes
  object-initialiser sanity tests confirming explicit setters still
  work, plus tests confirming the campaign does not strip the
  infrastructure assignments (`Type = TypeId`, the collection
  initialisations).
- `Devices/Components/ComponentCtorDefaultsTests.cs` — reflection-walk
  fixture asserting every concrete `Component` subclass in
  `MTConnect.Devices.Components` (115 generated `*.g.cs` files) leaves
  `Name` `null` after default construction. A sibling test verifies
  every same subclass keeps its `Type` assignment. Two additional
  tests confirm the explicit `Name` setter and object-initialiser
  paths still work.

Test namespace is `MTConnect.Tests.Common.DeviceCtorDefaults` /
`MTConnect.Tests.Common.Devices.Components` — the Device fixture lives
under a slightly off-pattern namespace deliberately, because the
existing `tests/MTConnect.NET-Common-Tests/Devices/Device.cs` declares
a class literally named `Device` in `MTConnect.Tests.Common.Devices`,
which shadows `MTConnect.Devices.Device` lookup inside that namespace.
The off-pattern namespace dodges the shadowing without renaming the
existing test class.

## Red-test exit state

`dotnet test tests/MTConnect.NET-Common-Tests/ --filter
'TestCategory=DeviceComponentDefaultsRemoved'` returns exit code 1
with 8 failures and 8 passes:

- Failures (expected — the production code today still stamps the
  placeholders): every assertion that the identity / name field is
  `null` fails on the unfixed library. Examples:
  - `Default_constructor_leaves_Id_null` — got
    `"E3EW2P9B8A"` (a fresh `RandomString(10)`).
  - `Default_constructor_leaves_Name_null` — got `"dev"`.
  - `Default_constructor_leaves_Uuid_null` — got a fresh GUID.
  - `Sequential_default_constructors_produce_identical_null_identity` —
    each `new Device()` produces a different random Id and a
    different fresh GUID.
  - `Agent_default_constructor_leaves_*_null` — Agent inherits the
    base ctor leak.
  - `Every_concrete_Component_subclass_default_ctor_leaves_Name_null`
    — fails on the first subclass walked, with `Name = "actuator"`.
- Passes (no-regression sanity): `Object_initializer_*` tests, the
  `Type`-still-set tests, and the collections-still-initialised test.

## Deviations from plan

- The plan's `03-red-tests.md` §"CI job" calls for an
  `issue-136-137-red` CI workflow with inverted exit code. Per
  CONVENTIONS §1.7, per-issue PRs do not modify
  `.github/workflows/`; red-state confirmation runs locally via
  `dotnet test --filter`. The `ci(repo): add issue-136-137-red
  confirmation job` commit is dropped.
- The plan's red category was originally `Issue136Red`. Per the
  2026-04-25 plan-hardening pass it became
  `DeviceComponentDefaultsRemoved` to satisfy CONVENTIONS §14 (no
  internal-reference category names). The fixtures in this commit
  use the hardened category name.

## Documentation

This file. Campaign index `docs/testing/issue-136-137.md` §3 links
to it.

## Follow-ups

None for this phase.
