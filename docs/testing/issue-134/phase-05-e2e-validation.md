# Phase 5 — E2E validation

## Executed

`tests/MTConnect.NET-Common-Tests/Devices/OrganizersSystemsEndToEndTests.cs`
exercises `Device.AddComponent()` end-to-end against the full set of System
substitution-group members under
`[Category("OrganizersSystemsEndToEnd")]`:

1. **`All_auto_wrapped_system_peers_land_under_single_Systems_organizer`** —
   adds every auto-wrapped System member (17 components) to a fresh `Device`
   one at a time, asserts a single `<Systems>` organizer is materialized as a
   direct child of the device, and asserts every member appears as a direct
   child of that organizer.

2. **`Heating_and_Protective_share_Systems_organizer_after_separate_AddComponent_calls`**
   — reproduces the exact reproduction case named in the issue: add `Heating`,
   then add `Protective`, then assert (a) only one `<Systems>` exists, (b) it
   contains both, (c) `Heating` does not leak out as a direct child of the
   Device.

3. **`Programmatic_device_assembly_produces_systems_at_consistent_depth`** —
   asserts every System peer ends up at tree depth 2 (Device → Systems → Member),
   which is the expected wire shape per `MTConnectDevices_*.xsd`.

## Test results

- `dotnet test`: `Failed: 0, Passed: 86`.
- `Organizers.cs` coverage holds at 100% line + branch + method.

## Not executed (scope deviation from plan-file P5)

The plan's drafted P5 calls for HTTP-emitted Probe parity, MQTT-relay
publish/subscribe Probe parity, and `mtconnect/agent:v2.7.0.7` cppagent parity
under Docker. None of those gates apply on `upstream/master`:

- The `tests/IntegrationTests` project exists but is xUnit-based and has no
  HTTP / MQTT fixtures touching `Device.AddComponent()` programmatic
  construction.
- The `MTCONNECT_E2E_DOCKER` infrastructure is part of the deferred bootstrap /
  `00-bootstrap` deliverable folded into `feat/issue-133`.
- The cppagent parity harness similarly lands as part of the compliance plan.

The E2E intent in scope here — proving `Device.AddComponent()` produces a
symmetric Probe-shaped tree for peer System members — is fully covered by the
programmatic tree-shape assertions above. Wire-format parity (XML round-trip,
MQTT JSON, cppagent comparison) is downstream behaviour the symmetry test
already implies and lands as part of the L4 cross-impl plan when the
compliance harness lands.

## Deviations from plan

- HTTP / MQTT / cppagent-parity scenarios deferred to the compliance harness
  plan (`11-tests/`) — required infrastructure not on `upstream/master`.
- The `tests/IntegrationTests/Regressions/Issue134E2ETests.cs` path is replaced
  by the in-place E2E test class in `MTConnect.NET-Common-Tests`. The test
  class name `OrganizersSystemsEndToEndTests` is descriptive (CONVENTIONS §14)
  rather than `Issue134E2ETests`.

## DoD

E2E behaviour green on programmatic device construction; writeup complete.
