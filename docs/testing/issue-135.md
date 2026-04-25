# Issue #135 — MqttRelay availability topic emission

Tracking writeup for the fix to [TrakHound/MTConnect.NET#135](https://github.com/TrakHound/MTConnect.NET/issues/135).

## 1. Defect and scope

The `MTConnect.NET-AgentModule-MqttRelay` agent module publishes the
Agent Availability state on `{TopicPrefix}/Probe/{AgentUuid}/Available`
as both the MQTT Last Will and Testament (LWT) and as a retained
`AVAILABLE` / `UNAVAILABLE` UTF-8 string. This breaks the contract
that every topic under `{TopicPrefix}/Probe/#` carries a JSON document
envelope: a subscriber wildcarding on `Probe/#` and parsing every
payload as JSON throws on the raw availability string.

Scope of this writeup is the `agent/Modules/MTConnect.NET-AgentModule-MqttRelay/`
project only. No other module, library, or test project is in scope.

## 2. Investigation

See `docs/testing/issue-135/phase-01-defect-scoping.md`.

## 3. Red tests

See `docs/testing/issue-135/phase-02-red-tests.md`.

## 4. Library fix

See `docs/testing/issue-135/phase-03-library-fix.md`.

## 5. Regression pins

See `docs/testing/issue-135/phase-04-regression-pins.md`.

## 6. End-to-end validation

See `docs/testing/issue-135/phase-05-e2e-validation.md`.

## 7. Summary

See `docs/testing/issue-135/phase-06-summary.md`.

## Operator migration

After this fix, operators that previously subscribed to
`{TopicPrefix}/Probe/{AgentUuid}/Available` for the raw-string
availability state must move their subscription to
`{TopicPrefix}/Agent/{AgentUuid}/Available`. Subscribers of the
`{TopicPrefix}/Probe/#` wildcard will no longer receive the
availability message and can rely on the wildcard delivering only
JSON document envelopes (Probe).
