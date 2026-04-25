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

## 4. Library fix (P3)

## 5. Regression pins (P4)

## 6. E2E validation (P5)

## 7. Campaign summary (P6)
