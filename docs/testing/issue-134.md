# Issue #134 — Organizers.Systems list stale relative to System substitution-group

## 1. Defect + scope

`Organizers.Systems` (the hand-maintained list at `libraries/MTConnect.NET-Common/Devices/Organizers.cs`)
is consumed by `Device.AddComponent()` to decide whether a programmatically-added
component is auto-wrapped under a `<Systems>` organizer. The list is stale relative
to the System substitution-group declared by the MTConnect SysML model and XSD —
notably it omits `Heating`, `Cooling`, `Pressure`, `Vacuum`, and `AirHandler`. As a
result, two peer System components added via `Device.AddComponent()` land at
asymmetric tree depths (one auto-wrapped under `Systems`, the other left at the
device root).

Authoritative source for substitution-group membership: the MTConnect SysML model
(`mtconnect/mtconnect_sysml_model`) tagged at the library's currently regenerated
revision. Each `*Component.g.cs` whose summary description begins with "System
that ..." or "System composed of ..." is a System substitution-group member.

## 2. Investigation (P1)

See [`issue-134/phase-01-defect-scoping.md`](issue-134/phase-01-defect-scoping.md).

Summary: 5 members missing from `Organizers.Systems` relative to the SysML
substitution-group enumeration (`AirHandler`, `Cooling`, `Heating`, `Pressure`,
`Vacuum`); 0 members to remove. Strategy A (hand-edit the list to the union)
selected.

## 3. Red tests (P2)

See [`issue-134/phase-02-red-tests.md`](issue-134/phase-02-red-tests.md).

`tests/MTConnect.NET-Common-Tests/Devices/OrganizersSystemsTests.cs` adds 58
parametric NUnit cases under `[Category("OrganizersSystemsSubstitutionGroup")]`;
18 fail on HEAD for the right reason (missing members + asymmetric peer depth).

## 4. Library fix (P3)

See [`issue-134/phase-03-library-fix.md`](issue-134/phase-03-library-fix.md).

`_systems` initializer extended with `AirHandler`, `Cooling`, `Heating`,
`Pressure`, `Vacuum` and sorted alphabetically; SysML source cited inline.
`Organizers.cs` reaches 100% / 100% / 100% with the addition of
`OrganizersAccessorsTests`.

## 5. Regression pins (P4)

See [`issue-134/phase-04-regression-pins.md`](issue-134/phase-04-regression-pins.md).

Two guards in
`tests/MTConnect.NET-Common-Tests/Devices/OrganizersSystemsRegressionTests.cs`:
literal pin against the System substitution-group TypeId list, plus a
reflection-based detector that walks the assembly and asserts every
`*Component` whose `DescriptionText` matches the SysML `System` descendant
phrasing appears in `Organizers.Systems`.

## 6. E2E validation (P5)

See [`issue-134/phase-05-e2e-validation.md`](issue-134/phase-05-e2e-validation.md).

`OrganizersSystemsEndToEndTests` exercises `Device.AddComponent()` against
the full set of auto-wrapped System members, the issue's exact `Heating` +
`Protective` reproduction, and the depth-2 wire-shape invariant.

## 7. Campaign summary (P6)

See [`issue-134/phase-06-finalisation.md`](issue-134/phase-06-finalisation.md).

`_systems` aligned with the SysML `System` substitution-group (18 members);
86 tests across 4 test classes; 100% coverage on `Organizers.cs`; no public
API change; behaviour change is precisely the bug fix.
