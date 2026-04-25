# Phase 02 — red tests

## Executed

- Created the paired test project
  `tests/MTConnect.NET-AgentModule-MqttRelay-Tests/` (NUnit 3.13.x,
  matching the existing `MTConnect.NET-SHDR-Tests` pattern). The
  project references the agent module project directly so the
  topic-construction helper extracted in phase 01 is unit-testable.
- Added `AvailabilityTopicTests.cs` with eight tests pinning the
  corrected topic shape:
  - `Build_returns_topic_outside_probe_wildcard` — guards against the
    `Probe/#` wildcard contract violation.
  - `Build_uses_dedicated_agent_segment` — pins the exact corrected
    topic `MTConnect/Agent/agent-uuid-1/Available`.
  - `Build_preserves_multi_segment_topic_prefix` — covers a typical
    multi-segment `TopicPrefix` such as `MTConnect/Document`.
  - Four null / empty input cases plus the
    `AvailableSegment_constant_pins_trailing_topic_segment` constant
    test.
- Registered the new project in `MTConnect.NET.sln` so the
  solution-level build picks it up.

## Red state

Three of the eight tests failed against the prior commit (the
extracted helper still emitted the broken topic shape):

```
Failed Build_returns_topic_outside_probe_wildcard
Failed Build_uses_dedicated_agent_segment
Failed Build_preserves_multi_segment_topic_prefix
```

The five remaining tests passed (null / empty inputs and the
`AvailableSegment` constant) because those branches are unaffected
by the broken topic shape. The fix in phase 03 makes the three red
tests green without changing the five already-passing ones.

## Source references

The fixture cites the public sources required by CONVENTIONS §15:

- `https://github.com/TrakHound/MTConnect.NET/issues/135` — the
  defect report, motivation for the fix.
- `https://github.com/mtconnect/cppagent` — the reference
  implementation for the canonical Probe/# wildcard contract
  (JSON-only payloads).

## Metrics delta

- Test project count: +1.
- Tests under `tests/MTConnect.NET-AgentModule-MqttRelay-Tests/`: +8.

## Deviations from plan

- The plan's `03-red-tests.md` cited an `Issue135Red` NUnit category
  + an `issue-135-red` inverted-exit-code CI job. CONVENTIONS §1.7
  forbids per-issue CI changes (CI surface is owned by the bootstrap
  prelude PR + the tests plan). CONVENTIONS §14 also forbids
  internal-only category labels of the
  `Issue<NNN>Red` shape. The category / CI surface is therefore
  dropped; the tests are uncategorised and run as part of the
  default solution build. The behavior of the gate is preserved:
  the three previously-red tests now appear in the standard test
  run and surface the failure to any reviewer.
- The plan's `03-red-tests.md` cites a Testcontainers-Mosquitto
  end-to-end fixture. The MqttFixture / Testcontainers harness it
  references is a bootstrap-plan deliverable that has not landed
  on `upstream/master`. The unit-level helper test produces an
  equivalent red signal without the infrastructure dependency.

## Follow-ups

- E2E coverage of the LWT publish path on a real MQTT broker
  remains a follow-up for a future Docker-gated test plan
  (CONVENTIONS §12). The unit tests cover the topic-construction
  surface only.
