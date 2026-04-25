# Phase 1 — Defect scoping

## Current `_systems` list (HEAD `3d6321ab`)

`libraries/MTConnect.NET-Common/Devices/Organizers.cs` lines 83-98:

| # | TypeId | `*Component.g.cs` | MinimumVersion |
|---|--------|-------------------|----------------|
| 1 | `Controller`     | `ControllerComponent.g.cs`     | v1.1 |
| 2 | `Coolant`        | `CoolantComponent.g.cs`        | v1.1 |
| 3 | `Dielectric`     | `DielectricComponent.g.cs`     | v1.4 |
| 4 | `Electric`       | `ElectricComponent.g.cs`       | v1.1 |
| 5 | `Enclosure`      | `EnclosureComponent.g.cs`      | v1.7 |
| 6 | `EndEffector`    | `EndEffectorComponent.g.cs`    | v1.7 |
| 7 | `Feeder`         | `FeederComponent.g.cs`         | v1.1 |
| 8 | `Hydraulic`      | `HydraulicComponent.g.cs`      | v1.1 |
| 9 | `Lubrication`    | `LubricationComponent.g.cs`    | v1.1 |
| 10 | `Pneumatic`     | `PneumaticComponent.g.cs`     | v1.1 |
| 11 | `ProcessPower`  | `ProcessPowerComponent.g.cs`  | v1.4 |
| 12 | `Protective`    | `ProtectiveComponent.g.cs`    | v1.4 |
| 13 | `WorkEnvelope`  | `WorkEnvelopeComponent.g.cs`  | v1.7 |

## System substitution-group enumeration source

The repo does not (yet, on `upstream/master`) carry the per-version XSD bundles
(those land via the planned `00-bootstrap` / compliance harness). The
substitution-group enumeration falls back to the second authoritative source per
CONVENTIONS §15: the SysML model, as it is materialized on this branch in the
`*.g.cs` files under `libraries/MTConnect.NET-Common/Devices/Components/`.

A `Component` subclass is a `System` substitution-group member iff its SysML
summary description begins with the canonical phrase pattern used for `System`
descendants — "System that ..." or "System composed of ...". The `SystemComponent`
abstract base itself is described as "Abstract Component that is permanently
integrated into the piece of equipment" and is not added (it is the parent, not
a member of the group).

Reproduction:

```bash
grep -B 1 'summary>$' libraries/MTConnect.NET-Common/Devices/Components/*.g.cs \
  | grep -E 'System (that|composed|used|which)' \
  | sed -E 's|.*Components/||; s|\.g\.cs.*||' \
  | sort -u
```

## Enumerated System substitution-group members on this revision

| TypeId | `*Component.g.cs` | MinimumVersion | In current `_systems`? |
|--------|-------------------|----------------|-------------------------|
| `AirHandler`    | `AirHandlerComponent.g.cs`    | v2.3 | NO |
| `Controller`    | `ControllerComponent.g.cs`    | v1.1 | yes |
| `Coolant`       | `CoolantComponent.g.cs`       | v1.1 | yes |
| `Cooling`       | `CoolingComponent.g.cs`       | v1.7 | NO |
| `Dielectric`    | `DielectricComponent.g.cs`    | v1.4 | yes |
| `Electric`      | `ElectricComponent.g.cs`      | v1.1 | yes |
| `Enclosure`     | `EnclosureComponent.g.cs`     | v1.7 | yes |
| `EndEffector`   | `EndEffectorComponent.g.cs`   | v1.7 | yes |
| `Feeder`        | `FeederComponent.g.cs`        | v1.1 | yes |
| `Heating`       | `HeatingComponent.g.cs`       | v1.7 | NO (issue #134) |
| `Hydraulic`     | `HydraulicComponent.g.cs`     | v1.1 | yes |
| `Lubrication`   | `LubricationComponent.g.cs`   | v1.1 | yes |
| `Pneumatic`     | `PneumaticComponent.g.cs`     | v1.1 | yes |
| `Pressure`      | `PressureComponent.g.cs`      | v1.1 | NO |
| `ProcessPower`  | `ProcessPowerComponent.g.cs`  | v1.4 | yes |
| `Protective`    | `ProtectiveComponent.g.cs`    | v1.4 | yes |
| `Vacuum`        | `VacuumComponent.g.cs`        | v1.7 | NO |
| `WorkEnvelope`  | `WorkEnvelopeComponent.g.cs`  | v1.7 | yes |

## Diff

- **Add** (5 members): `AirHandler`, `Cooling`, `Heating`, `Pressure`, `Vacuum`.
- **Remove** (0 members): every existing entry has a SysML summary that places
  it in the System substitution-group.
- **Reorder**: alphabetical by `TypeId` for stability and grep-friendly diffs.

## Consumer audit

```
$ grep -rn '_systems\|Organizers\.Systems' libraries/MTConnect.NET-Common/
libraries/MTConnect.NET-Common/Devices/Organizers.cs:83:        private static readonly IEnumerable<string> _systems = new List<string>
libraries/MTConnect.NET-Common/Devices/Organizers.cs:160:        public static IEnumerable<string> Systems => _systems;
libraries/MTConnect.NET-Common/Devices/Organizers.cs:181:                else if (_systems.Contains(componentType)) return SystemsComponent.TypeId;
```

The only consumer is `Organizers.GetOrganizerType()`, which is in turn called by
`Device.AddComponent()` (`libraries/MTConnect.NET-Common/Devices/Device.cs:336`)
and used solely to drive the auto-wrap-under-`<Systems>` decision. There are no
serialization or wire-shape consumers, so widening the list is a behavioural
change for `Device.AddComponent()` only and does not alter the Probe XML schema.

## Strategy decision

**Strategy A — hand-edit the list to the union of System substitution-group
members across every library-declared version.**

Rationale: the current `*.g.cs` files do not carry a per-class
substitution-group attribute or marker, so generator-backed lookup (Strategy B)
would require new generator infrastructure outside this plan's scope. The union
is small (18 members) and the cost of a future regenerator extending the list
is one-line additions per new System member, which the regression guard test
catches.

## Per-version implication

- `_systems` is consumed at runtime by `Device.AddComponent()`; the version
  observed by the agent process is the DataItem-level `MinimumVersion`, not the
  organiser membership decision.
- A consumer who builds a `HeatingComponent` (v1.7+) on top of an agent
  configured at v1.5 still gets the correct `<Systems>` wrap; if the agent then
  emits Probe at v1.5 the `<Heating>` element itself is filtered out by the
  emit-time version gate, not by `_systems` membership.

## DoD

List diff final, strategy A confirmed, consumer audit complete.
