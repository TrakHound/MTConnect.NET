# Phase 04 — regression pins

## Executed

Added `AvailabilityTopicRegressionTests.cs` to the paired test
project. The fixture pins the corrected behavior with two guards:

- `Topic_never_contains_probe_segment_for_any_inputs` walks a
  parametric matrix of five `topicPrefix` values and five
  `agentUuid` values (twenty-five total combinations, including
  adversarial inputs where the prefix or the uuid is literally the
  string `"Probe"`) and asserts that the produced topic always
  ends in
  `/{AvailabilityTopic.AgentSegment}/{uuid}/{AvailabilityTopic.AvailableSegment}`.
  A future refactor that hard-codes `"/Probe/"` into the
  availability publish surface fails this test on every input.
- `Agent_segment_is_distinct_from_probe_constant` pins the
  `AgentSegment` constant value (`"Agent"`) and asserts it is
  distinct from the literal `"Probe"`. Catches a refactor that
  re-routes the constant back at the Probe topic constant.

## Test state

```
Passed! - Failed: 0, Passed: 10, Skipped: 0, Total: 10
```

The test project now ships eight unit tests plus two regression
tests, all green. The regression tests run as part of the default
solution build with no special category or filter.

## Source references

The fixture cites the same public sources as the unit tests:

- `https://github.com/TrakHound/MTConnect.NET/issues/135`
- `https://github.com/mtconnect/cppagent`

## Metrics delta

- Tests under `tests/MTConnect.NET-AgentModule-MqttRelay-Tests/`:
  +2 (10 total).
- `AvailabilityTopic.cs` coverage: unchanged (already 100 percent).

## Deviations from plan

- The plan's `05-regression-pins.md` cited two compliance-suite
  files (`tests/Compliance/MTConnect-Compliance-Tests/L5_Regressions/`
  and `tests/Compliance/MTConnect-Compliance-E2E/L5_Regressions/`).
  Those compliance trees do not exist on `upstream/master` (they
  are deliverables of the dedicated tests-overhaul plan that has
  not landed). The regression pin therefore lives in the paired
  `MTConnect.NET-AgentModule-MqttRelay-Tests/` project where it is
  immediately runnable.
- The plan's `05-regression-pins.md` cited a
  `docs(tests): migrate issue-135 regression out of compliance-gate
  plan` commit. The compliance-gate plan it refers to is a
  separate (not yet landed) test-overhaul effort and has no
  tracked counterpart on this branch's surface, so the migration
  commit is dropped.

## Follow-ups

- Migration of this regression pin into the compliance-suite
  L5_Regressions tree once that surface lands upstream.
