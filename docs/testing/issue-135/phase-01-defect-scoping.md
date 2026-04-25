# Phase 01 — defect scoping

## Publish surface

`agent/Modules/MTConnect.NET-AgentModule-MqttRelay/Module.cs`
contains all topic-construction sites that produce the broken
availability publishes:

- Line 738-746 — `GetAgentAvailableTopic()` returns
  `{TopicPrefix}/{ProbeTopic}/{Agent.Uuid}/Available` where
  `ProbeTopic` resolves to the constant `Probe`. Result:
  `{TopicPrefix}/Probe/{AgentUuid}/Available`.
- Line 119-123 — `clientOptionsBuilder.WithWillTopic(GetAgentAvailableTopic())`
  configures the MQTT Last Will and Testament. Payload is
  `Availability.UNAVAILABLE.ToString()` (raw UTF-8 bytes), QoS 1,
  retained.
- Line 417-449 — when the `Agent` device's Probe document is
  published, the same `GetAgentAvailableTopic()` is republished as
  a retained message with payload `Availability.AVAILABLE.ToString()`
  (raw UTF-8 bytes), QoS 1, retained.

`MqttTopicStructure.cs` declares topic prefix constants but does
not contribute additional topic-construction sites.

## cppagent reference

The reference `mtconnect/cppagent` implementation does not publish a
dedicated raw-string availability topic under the Probe wildcard.
Agent availability is communicated via:

- The MQTT Last Will and Testament fires a transport-level
  notification on a topic outside the document wildcards (typical
  layout: a status / agent topic, never under `Probe/#`).
- The normal observation flow encodes the `AVAILABILITY` DataItem
  inside the JSON `Current` envelope when the agent is reachable.

A subscriber wildcarding on `{TopicPrefix}/Probe/#` consequently
receives only JSON document envelopes from the cppagent reference.

## Strategy decision

**Strategy A — relocate the topic out of the Probe wildcard.**

The fix relocates both the LWT and the retained message from
`{TopicPrefix}/Probe/{AgentUuid}/Available` to
`{TopicPrefix}/Agent/{AgentUuid}/Available`. The `Probe/#` wildcard
then carries only JSON document envelopes, matching the cppagent
contract.

Strategy A was selected over the alternatives because:

- Strategy B (JSON-wrap the payload in place) keeps the four-segment
  topic shape under `Probe/#` which still violates the cppagent
  three-segment Probe contract. Partial compliance only.
- Strategy C (eliminate the dedicated publish, rely on the
  `AVAILABILITY` DataItem in the Current envelope) loses the MQTT
  transport-level Last Will signal that operators rely on for fast
  detection of unclean disconnects.

## Breaking-change impact

Strategy A is a breaking change for any operator that subscribes
directly to `{TopicPrefix}/Probe/{AgentUuid}/Available`. The fix
ships with an operator migration note in `docs/testing/issue-135.md`
and a `Breaking:` bullet in the PR body.

A repo-wide grep for the legacy topic shape finds no in-repo
consumers (no examples, no fixtures, no documentation) — the
breaking surface is external to this repository.

## Sources

- `https://github.com/TrakHound/MTConnect.NET/issues/135` — defect
  report describing the failed `Probe/#` JSON parse.
- `https://github.com/mtconnect/cppagent` — reference implementation
  for the canonical MQTT topic layout under `Probe/#`.

## Metrics delta

None at this phase (documentation only).

## Deviations from plan

- The plan's `02-defect-scoping.md` cites a `docs(issue-135): scope
  mqtt-relay availability topic defect` commit subject. `issue-135`
  is not a recognised scope under CONVENTIONS.md §5.3; the correct
  documentation scope is `testing` (per §5.4 rule 5: docs about a
  feature use the feature's scope, but the writeups under
  `docs/testing/<plan>/` are plan-tracking artefacts and use the
  `testing` documentation scope).

## Follow-ups

- None.
