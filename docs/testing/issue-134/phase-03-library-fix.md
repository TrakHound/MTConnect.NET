# Phase 3 — Library fix

## Executed

`libraries/MTConnect.NET-Common/Devices/Organizers.cs` — `_systems` field
initializer extended with the five missing System substitution-group members:

```diff
+   AirHandlerComponent.TypeId,
    ControllerComponent.TypeId,
    CoolantComponent.TypeId,
+   CoolingComponent.TypeId,
    DielectricComponent.TypeId,
    ElectricComponent.TypeId,
    EnclosureComponent.TypeId,
    EndEffectorComponent.TypeId,
    FeederComponent.TypeId,
+   HeatingComponent.TypeId,
    HydraulicComponent.TypeId,
    LubricationComponent.TypeId,
    PneumaticComponent.TypeId,
+   PressureComponent.TypeId,
    ProcessPowerComponent.TypeId,
    ProtectiveComponent.TypeId,
+   VacuumComponent.TypeId,
    WorkEnvelopeComponent.TypeId
```

The list is now alphabetically sorted by `Component.TypeId`. A short comment
above the field cites the SysML model URL as the authoritative source.

## Test refinement landed alongside

`tests/MTConnect.NET-Common-Tests/Devices/OrganizersSystemsTests.cs` —
`GetOrganizerType_for_system_member_returns_Systems` was split into
`GetOrganizerType_for_auto_wrapped_system_member_returns_Systems`
(parametric over every System member except `Controller`) and
`GetOrganizerType_for_Controller_returns_Controllers_not_Systems`
(direct test for the carve-out). The original assertion contradicted
`Device.AddComponent()`'s deliberate behaviour where `Controller`
remains at the device root rather than auto-wrapping under `<Systems>`.

## Coverage

`tests/MTConnect.NET-Common-Tests/Devices/OrganizersAccessorsTests.cs`
backfills the remaining branches of `Organizers.cs` so the file reaches
100% line + 100% branch + 100% method coverage:

```
<class name="MTConnect.Devices.Organizers" filename="Devices/Organizers.cs"
       line-rate="1" branch-rate="1" complexity="37">
```

## Test results

- `dotnet build -c Debug` green on net8.0.
- `dotnet test`: `Failed: 0, Passed: 81` — 18-of-18 SystemsOrganizer pass plus
  17-of-17 AutoWrappedSystemMember pass plus 1-of-1 Controller carve-out plus
  5-of-5 peer-depth pass plus 17-of-17 auto-wrap pass plus 22-of-22 accessor
  cases plus the existing 1 placeholder test = 81 total.

## Strategy A confirmed

Strategy B (generator-backed lookup via reflection / attribute) is unavailable
on `upstream/master`'s `*.g.cs` files — they do not expose
substitution-group metadata. Adopting it would require generator changes
out-of-scope per `00-overview.md` §1 and per CONVENTIONS §18.

## Validation

- Reds → green.
- Tests outside the new categories remain green.
- `Organizers.cs` at 100% / 100% / 100%.
- `git status` clean post-commit.

## Deviations from plan

- The plan-file P3 commit 2 (`test(issue-134): remove red category and ci job`)
  is dropped: the category remains as `OrganizersSystemsSubstitutionGroup`
  (descriptive, not bookkeeping — CONVENTIONS §14), and no CI job was added
  in P2 to remove (CONVENTIONS §1.7).

## DoD

Reds green; `Organizers.cs` at 100% coverage; library fix landed.
