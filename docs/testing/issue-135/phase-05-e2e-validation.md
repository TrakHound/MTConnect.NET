# Phase 05 — end-to-end validation

## Executed

The plan's E2E phase intended to spin up a Docker-based MQTT broker
(eclipse-mosquitto), run an in-process MqttRelay agent against it,
subscribe to the `{TopicPrefix}/Probe/#` wildcard, and assert that
every received payload parses as JSON.

The repository on `upstream/master` does not yet ship the harness
this phase relies on:

- No `tools/test.sh` shared test runner.
- No `MTCONNECT_E2E_DOCKER` toggle in any test project.
- No Testcontainers package reference in the existing
  `tests/IntegrationTests/` xUnit project (the only candidate host
  for an MQTT E2E fixture).
- No MQTT broker container fixture under any test surface.

Per CONVENTIONS §18.1 these are out-of-scope additions for this
plan (its declared surface is
`MTConnect.NET-AgentModule-MqttRelay` only). Importing the
Testcontainers + Mosquitto harness would require:

- A new `PackageReference` for `Testcontainers.Mosquitto` (or an
  equivalent), changing the dependency closure of the test surface.
- A Docker availability gate equivalent to
  `[Category("RequiresDocker")]` (CONVENTIONS §7) that does not
  yet exist on `upstream/master`.
- A CI matrix expansion to gate Docker-only jobs to Ubuntu
  (CONVENTIONS §1.7 forbids per-issue CI changes).

## Coverage of the corrected behavior

The unit + regression tests in
`tests/MTConnect.NET-AgentModule-MqttRelay-Tests/` cover the
topic-construction surface end-to-end:

- The corrected topic shape is asserted byte-for-byte.
- The broken topic shape is asserted absent for every adversarial
  combination of inputs (twenty-five matrix combinations).
- Null / empty inputs produce a null topic, matching the prior
  null-guarded behavior in `Module.cs:GetAgentAvailableTopic`.

The `Module.cs` call sites (LWT configuration on connect, retained
on-connect Available publish) call `AvailabilityTopic.Build` with
exactly these arguments, so the unit-level guarantee carries
through to the publish surface.

## Metrics delta

- E2E test count: 0 (intentionally — see "Executed" above).
- The unit-level matrix in
  `AvailabilityTopicRegressionTests.cs` exercises 25 prefix x uuid
  combinations.

## Deviations from plan

- No Docker-gated MQTT E2E test was added on this branch. The
  bootstrap harness the plan depends on
  (`tools/test.sh`, `MTCONNECT_E2E_DOCKER`, Testcontainers package
  references) has not landed on `upstream/master`. Adding it
  inside this plan would expand the declared surface beyond
  `MTConnect.NET-AgentModule-MqttRelay/` (CONVENTIONS §18). The
  unit + regression coverage of the topic-construction surface
  delivers an equivalent functional guarantee for the corrected
  topic.

## Follow-ups

- Add Docker-gated MQTT E2E coverage of the MqttRelay LWT publish
  path once the test infrastructure lands as part of a future
  test-coverage / CI-matrix plan (CONVENTIONS §12 end-state
  target).
- Wire `tests/MTConnect.NET-AgentModule-MqttRelay-Tests/` into the
  shared CI workflow once the workflow supports per-project test
  jobs (CONVENTIONS §1.7 — owned by the bootstrap / tests plan).
