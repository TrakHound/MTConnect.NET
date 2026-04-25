# Phase 05 — E2E validation

## Executed

`tests/MTConnect.NET-JSON-cppagent-Tests/E2E/SampleValueWireFormatE2ETests.cs`
exercises the full
`IObservation -> JsonSampleValue(observation) -> JsonSerializer` path
used by `JsonHttpResponseDocumentFormatter` /
`JsonHttpEntityFormatter` / `JsonMqttEntityFormatter`. Cases:

- `Numeric_observation_serializes_as_json_number_token` for
  `(temp, 863.7060)`, `(temp, 0.0001)`, `(rotary-velocity, 12345)`,
  `(amperage, -5.5)`. Asserts `JsonValueKind.Number`.
- `Unavailable_observation_serializes_as_json_string_token` — sentinel
  preserved.
- `Three_space_observation_preserves_string_token` —
  `"1.5 -2.5 3.5"` stays a JSON string token (cppagent parity).
- `Numeric_value_round_trips_to_observation` — number-token roundtrips
  back to a boxed `double` via the converter's `Read`.

## Plan-vs-executed scope reduction

The plan's P5 calls for Docker-gated MQTT scenarios with
`mtconnect/agent:v2.7.0.7` and `eclipse-mosquitto:2.0.22`. Per
CONVENTIONS section 12, Docker-gated E2E scenarios live under
`tests/Compliance/MTConnect-Compliance-E2E/L4_CrossImpl/` and that
project does not exist on `upstream/master` at branch-cut time
(slated to land via plan 11). Per the plan's "or local fallback until
compliance project lands" clause (mirrored from the regression-pin
phase), the E2E here is the wire-format equivalent — the assertions
match what the Docker scenario would assert (`type == "number"` for
numeric Sample values, `type == "string"` for the sentinel) without
requiring Docker. When the compliance E2E project lands, this plan's
scenarios migrate to it as Docker-gated SHDR -> agent -> MQTT
round-trips.

## cppagent parity

The cppagent v2.7.0.7 reference emits a JSON number for numeric
Sample values and a JSON string for `UNAVAILABLE`. The MT.NET wire
output now matches; representative captures:

```json
{ "value": 863.706, "dataItemId": "temp", "type": "Temperature" }
{ "value": "UNAVAILABLE", "dataItemId": "temp", "type": "Temperature" }
```

## Validation

```
Passed!  - Failed:     0, Passed:    33, Skipped:     0, Total:    33
```

## Workflow catalogue

`docs/testing/workflows.md` does not exist on `upstream/master`
(slated for plan 11 / 00-bootstrap). Once it lands, this plan's
JSON-cppagent Sample-value workflow row reads:

| ID | Input | Output | Owning test class |
|---|---|---|---|
| ws-cppagent-sample-numeric | `IObservation` with numeric Result | JSON number token | `SampleValueWireFormatE2ETests` |
| ws-cppagent-sample-unavailable | `IObservation` with `UNAVAILABLE` Result | JSON string token | `SampleValueWireFormatE2ETests` |

## DoD

E2E green; cross-impl wire shape captured; no regression on existing
tests in the test project.

## Deviations from plan

- Docker-gated MQTT round-trip deferred to plan 11's compliance E2E
  project per the plan's local-fallback clause. Captured under
  follow-ups for the main agent's todo.md.
