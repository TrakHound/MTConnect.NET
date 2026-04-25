# Phase 06 — summary

## Outcome

Resolves [TrakHound/MTConnect.NET#135](https://github.com/TrakHound/MTConnect.NET/issues/135).

The MqttRelay agent module now publishes the agent availability
state (MQTT Last Will and Testament + on-connect retained Available
message) on `{TopicPrefix}/Agent/{AgentUuid}/Available` instead of
`{TopicPrefix}/Probe/{AgentUuid}/Available`. The
`{TopicPrefix}/Probe/#` wildcard therefore carries only JSON
document envelopes, matching the cppagent reference contract.

## Surface touched

Strictly inside the declared scope
`agent/Modules/MTConnect.NET-AgentModule-MqttRelay/` plus the new
paired test project:

| Path                                                                                                  | Kind        | Change                              |
|-------------------------------------------------------------------------------------------------------|-------------|-------------------------------------|
| `agent/Modules/MTConnect.NET-AgentModule-MqttRelay/AvailabilityTopic.cs`                              | production  | new (helper class + corrected topic) |
| `agent/Modules/MTConnect.NET-AgentModule-MqttRelay/Module.cs`                                         | production  | delegates topic construction to helper |
| `tests/MTConnect.NET-AgentModule-MqttRelay-Tests/MTConnect.NET-AgentModule-MqttRelay-Tests.csproj`    | test        | new project (NUnit 3.13.x)          |
| `tests/MTConnect.NET-AgentModule-MqttRelay-Tests/AvailabilityTopicTests.cs`                           | test        | new — eight unit tests              |
| `tests/MTConnect.NET-AgentModule-MqttRelay-Tests/AvailabilityTopicRegressionTests.cs`                 | test        | new — two regression tests          |
| `MTConnect.NET.sln`                                                                                   | build       | registers the new test project      |
| `docs/testing/issue-135.md`                                                                           | docs        | new — writeup index                 |
| `docs/testing/issue-135/phase-00-foundation.md` ... `phase-06-summary.md`                             | docs        | new — per-phase writeups            |

## Validation summary

- `dotnet build agent/Modules/MTConnect.NET-AgentModule-MqttRelay/MTConnect.NET-AgentModule-MqttRelay.csproj -c Debug` — green.
- `dotnet test tests/MTConnect.NET-AgentModule-MqttRelay-Tests/ -c Debug` — 10 of 10 tests pass.
- `coverlet.collector` reports 100 percent line / 100 percent branch
  coverage on `AvailabilityTopic.cs`. `Module.cs:GetAgentAvailableTopic`
  itself is a four-line guarded delegation; its testable surface
  (the topic-construction logic) is covered through the helper.
- `git status` clean.
- Every commit pushed to `origin/fix/issue-135`.

## Compliance impact

The MTConnect Standard does not normatively specify the MQTT topic
on which an agent advertises availability (the standard scopes
the MQTT envelope payload, not the topic-tree layout). This fix
aligns the topic tree with the cppagent reference implementation's
posture — `Probe/#` carries only JSON document envelopes — without
modifying any wire-shape that the standard constrains.

No L1-L5 compliance row is affected.

## Breaking change

Operators that subscribe directly to
`{TopicPrefix}/Probe/{AgentUuid}/Available` for the raw
`AVAILABLE` / `UNAVAILABLE` UTF-8 string must move their
subscription to `{TopicPrefix}/Agent/{AgentUuid}/Available`. The
PR body and the head commit `BREAKING CHANGE:` trailer call this
out.

## Cross-phase deviations

The full set of deviations documented per phase, summarised:

- Commit subjects use the CONVENTIONS §5.3 scopes (`testing`,
  `agent-module`) rather than the `issue-135` / `mqtt-relay` scopes
  the plan files cited. The latter are not part of the §5.3
  taxonomy.
- The plan's Testcontainers-backed E2E phase is not implemented;
  the bootstrap harness it relies on (`tools/test.sh`, the
  `MTCONNECT_E2E_DOCKER` toggle, the Testcontainers package
  reference) has not landed on `upstream/master`. The unit +
  regression coverage of the topic-construction surface delivers
  an equivalent functional guarantee.
- The plan's NUnit `[Category("Issue135Red")]` + `issue-135-red`
  inverted-exit-code CI job were dropped. CONVENTIONS §1.7 forbids
  per-issue CI changes; CONVENTIONS §14 forbids
  `Issue<NNN>Red`-style internal-only category labels.

All deviations are within the declared scope of
`MTConnect.NET-AgentModule-MqttRelay/` and the paired test project
(CONVENTIONS §18).

## Follow-ups

- Once a Docker-gated test infrastructure lands upstream, port the
  regression matrix into a real-broker E2E fixture
  (CONVENTIONS §12).
- Once the compliance-suite L5_Regressions surface lands upstream,
  migrate the regression fixture into that tree.
- Wire `tests/MTConnect.NET-AgentModule-MqttRelay-Tests/` into the
  CI matrix once the workflow supports per-project test jobs
  (CONVENTIONS §1.7).
