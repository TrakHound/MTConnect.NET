# Phase 05 — E2E validation (deferred)

## Status

Deferred to a follow-up commit on this PR after `feat/issue-133`
merges upstream and rebase pulls in the Docker-gated test
infrastructure.

## Rationale

The plan calls for an MQTT round-trip:
1. Spin up a `testcontainers` mosquitto container.
2. Boot an in-process `MTConnectAgent` configured with
   `defaultVersion: 2.5`.
3. Subscribe via MQTTnet, capture published Streams + Devices envelopes.
4. Assert `.MTConnectStreams.schemaVersion == "2.5"` and
   `.MTConnectDevices.schemaVersion == "2.5"`.

Required infrastructure that does NOT exist on `upstream/master`:
- `Testcontainers.Mosquitto` (or generic `Testcontainers` + image
  pinning) — would be a new package reference on
  `tests/IntegrationTests/IntegrationTests.csproj`.
- `MQTTnet` subscriber wiring — same csproj edit.
- `[Trait("Category","RequiresDocker")]` filter glue — bootstrap
  contributes the runner script + CI matrix that respects it.
- `MTCONNECT_E2E_DOCKER` gating — same.

Per CONVENTIONS §18.4 (Subagent obligation — flag out-of-scope work)
and §18.3 (split rather than stretch), authoring the E2E here would
either silently expand the PR's surface across `tests/IntegrationTests/`
infrastructure or duplicate `feat/issue-133`'s deliverables. Both
violate scope discipline.

## What this PR ships in lieu of E2E

The unit-level coverage exhaustively asserts the contract:

- 28 parametric cases across both envelopes × 14 versions show
  `SchemaVersion` mirrors the configured value.
- Guard test refuses re-introduction of a hardcoded literal.

The codepath under test in P3 is the same one a Docker E2E would
exercise — the formatter chain runs synchronously inside the agent's
emit pipeline; the JSON envelope ctor receives the response document
that the agent's emit pipeline builds with the configured version.
There is no async / network state machine between the configured
version and the wire output that a Docker test would discover.

## Follow-up after #133 merges

After rebase:
1. Add `Testcontainers` + `MQTTnet` package refs to
   `tests/IntegrationTests/IntegrationTests.csproj`.
2. Author `tests/IntegrationTests/Regressions/Issue128MqttE2ETests.cs`
   with three scenarios per the plan's §"Scenarios" block.
3. Tag `[Trait("Category","RequiresDocker")]` and run under
   `MTCONNECT_E2E_DOCKER=true` on Ubuntu CI.
4. Capture wire samples in this writeup.
