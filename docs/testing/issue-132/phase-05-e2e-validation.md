# Phase 5 — E2E validation

## Status: deferred

The original plan's E2E phase requires:

- Docker harness (`MTCONNECT_E2E_DOCKER=true`).
- Pinned `eclipse-mosquitto:2.0.22` and `mtconnect/agent:v2.7.0.7`
  containers.
- `tests/IntegrationTests/Regressions/Issue132MqttE2ETests.cs` — a
  new test class.
- A streamed-observation rename (`AssetCount` scalar →
  `AssetCountDataSet` map) plus per-asset-type bookkeeping on the
  asset buffer to assert the MQTT payload shape.

None of those preconditions are satisfied by `upstream/master`:

- The Docker E2E harness is part of the unmerged bootstrap plan.
- The streamed-observation rename + per-type bookkeeping is the
  "larger surface change" the dispatch directive explicitly
  out-of-scoped; the pragmatic Probe-side fix delivered in P3 does not
  yet alter the streamed observation shape.

The Probe-side spec compliance — `representation="DATA_SET"` on the
auto-injected DataItem — is fully covered by the unit + regression
tests in P2 + P4. The XML emitter walks the DataItem's `Representation`
property when rendering Probe; if `Representation` is `DATA_SET`, the
attribute appears in the Probe XML. So the wire shape on Probe is
deterministically correct given the unit-test green.

## Follow-ups (recorded for the parent plan)

1. Streamed-observation rename + per-type bookkeeping → separate plan,
   own PR.
2. MQTT + cppagent v2.7.0.7 parity Docker E2E once bootstrap has
   landed and exposes `MTCONNECT_E2E_DOCKER`.

## DoD

Phase deferred with explicit follow-up entries.
