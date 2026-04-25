# Phase 03 — library fix

## Executed

- Added the `AvailabilityTopic.AgentSegment` constant
  (value `"Agent"`).
- Changed `AvailabilityTopic.Build` to interpolate `AgentSegment`
  into the produced topic instead of
  `MTConnectMqttDocumentServer.ProbeTopic`. The helper now returns
  `{TopicPrefix}/Agent/{AgentUuid}/Available` for any non-empty
  inputs, satisfying the corrected topic contract.

`Module.cs:GetAgentAvailableTopic` did not require further edits in
this phase — its phase-01 refactor delegated all topic construction
to `AvailabilityTopic.Build`, so flipping the helper's output
automatically corrects every call site (the LWT configuration on
connect plus the on-connect retained Available message publish).

## Test state

```
Passed!  - Failed: 0, Passed: 8, Skipped: 0, Total: 8
```

All eight `AvailabilityTopicTests` are green. No other test project
is touched by this fix.

## Coverage

`coverlet.collector` reports 100 percent line and 100 percent
branch coverage on `agent/Modules/MTConnect.NET-AgentModule-MqttRelay/AvailabilityTopic.cs`:

```
class name="MTConnect.AvailabilityTopic"
filename="agent/Modules/MTConnect.NET-AgentModule-MqttRelay/AvailabilityTopic.cs"
line-rate="1" branch-rate="1" complexity="4"
```

`Module.cs:GetAgentAvailableTopic` itself is a four-line guarded
delegation:

```csharp
private string GetAgentAvailableTopic()
{
    if (Agent == null || _configuration == null) return null;
    return AvailabilityTopic.Build(_configuration.TopicPrefix, Agent.Uuid);
}
```

The function depends on `IMTConnectAgentBroker`, on the MQTT
factory, and on `MqttRelayModuleConfiguration` instances that only
exist inside a running agent process. Unit-level coverage of the
function in isolation is therefore not achievable without
materialising agent infrastructure that does not exist on
`upstream/master` (no integration / Testcontainers harness, no
shared agent mock). The function's body is structurally identical
to the pre-fix body apart from the delegation to the now-tested
helper, so the testable surface (topic construction) is fully
covered.

## Operator migration

After this fix:

- Operators that subscribed to
  `{TopicPrefix}/Probe/{AgentUuid}/Available` for the raw
  `AVAILABLE` / `UNAVAILABLE` UTF-8 string must move their
  subscription to `{TopicPrefix}/Agent/{AgentUuid}/Available`.
- Operators that subscribed to the `{TopicPrefix}/Probe/#` wildcard
  for JSON document envelopes can rely on the wildcard delivering
  only JSON envelopes with this fix.

The migration is a one-line subscription change on the operator
side. The publish-side payload format is unchanged
(`AVAILABLE` / `UNAVAILABLE` UTF-8 string, retained, QoS 1).

## Metrics delta

- `AvailabilityTopic.cs` coverage: 0 percent (file new in phase 01)
  -> 100 percent line / 100 percent branch.
- Number of red tests: 3 -> 0.
- Public API additions: `AvailabilityTopic.AgentSegment` constant.

## Deviations from plan

- The plan's `04-library-fix.md` cites a `fix(mqtt-relay): ...`
  commit subject. `mqtt-relay` is not a recognised scope under
  CONVENTIONS §5.3; the correct scope for production code under
  `agent/Modules/<NAME>/` is `agent-module` with the module name
  carried in the subject body (CONVENTIONS §5.5). This phase
  commits as
  `fix(agent-module): correct availability topic emission in MqttRelay`.
- The plan's `04-library-fix.md` also lists removing the
  `Issue135Red` category and an `issue-135-red` CI job. Phase 02's
  deviation note already documents that those artefacts were never
  introduced; nothing to remove.

## Follow-ups

- `Module.cs`'s broader uncovered surface (the entire 700-line file
  on `upstream/master`) is out of scope for this fix and remains a
  follow-up for the dedicated test-coverage plan (CONVENTIONS §10
  end-state target).
