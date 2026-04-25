# Phase 05 — E2E validation

## Executed

Added a wire-shape smoke fixture at
`tests/MTConnect.NET-XML-Tests/Devices/DeviceCtorDefaultsWireShapeTests.cs`
covering three scenarios:

- `Default_constructed_Device_emits_empty_identity_attributes` — a
  freshly-constructed `Device` serialised to XML via
  `XmlDevice.ToXml(...)` produces `id=""`, `name=""`, and `uuid=""`
  (empty attribute values) rather than placeholder strings. The test
  also explicitly negates `name="dev"` to pin the previous-behaviour
  removal.
- `Caller_set_identity_round_trips_through_XML` — explicit
  object-initialiser identity round-trips through the XML emitter,
  proving the campaign hasn't broken the happy path.
- `Sequential_default_Devices_emit_identical_empty_uuid_attributes` —
  the original #136 symptom (GUID drift across constructions) pinned
  at the wire-shape layer; two sequential `new Device()` serialisations
  both carry `uuid=""`.

These scenarios are the closest available approximation to the plan's
HTTP+MQTT Docker E2E.

## Deviations from plan

- The plan's `06-e2e-validation.md` calls for HTTP and MQTT-over-Docker
  scenarios using `tests/IntegrationTests/Regressions/Issue136E2ETests.cs`
  with `MTCONNECT_E2E_DOCKER=true` and `eclipse-mosquitto:2.0.22`. The
  per-issue paired Docker-E2E infrastructure is part of the bootstrap
  / tests-plan deliverables that have not yet landed on
  `upstream/master`. Authoring the HTTP+MQTT scenarios on this branch
  would silently re-scaffold integration-test infrastructure — exactly
  the silent-scope-expansion failure mode flagged in CONVENTIONS §17.8
  row 5.
- Per CONVENTIONS §17.3 / §18.4 the deviation is reported here rather
  than blindly scaffolded. The wire-shape smoke covers the key
  contract (empty attribute values on the wire after default
  construction; explicit setter still round-trips) at the level the
  current `tests/MTConnect.NET-XML-Tests/` infrastructure supports.
- Once the bootstrap and tests-plan deliverables land, the HTTP+MQTT
  agent-restart scenarios can be added in a follow-up commit on the
  same branch (or a follow-up PR if the branch is already merged).
  Tracked in `todo.md §F`.

## Validation

- `dotnet test tests/MTConnect.NET-XML-Tests/` — 7 passed, 0 failed
  (4 pre-existing + 3 new wire-shape scenarios).

## Documentation

This file. Campaign index `docs/testing/issue-136-137.md` §6 links
to it.

## Follow-ups

- Add HTTP+MQTT-over-Docker E2E scenarios once the bootstrap test
  infrastructure lands. Track in `todo.md §F`.
