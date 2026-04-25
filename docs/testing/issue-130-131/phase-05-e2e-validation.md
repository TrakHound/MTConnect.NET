# Phase 5 — E2E validation

## Executed

Authored `tests/MTConnect.NET-JSON-cppagent-Tests/JsonHeaderWireShapeE2ETests.cs`,
which pins the post-fix wire shape across the three envelope DTOs and the three
schema versions (`2.0`, `2.3`, `2.5`) the plan called out as spot-checks. Each
test:

1. Constructs an `MTConnect*Header` with `SchemaVersion` set to the target version.
2. Wraps it in the matching `Json{Streams,Devices,Assets}Header` DTO.
3. Serializes through `System.Text.Json.JsonSerializer.Serialize`.
4. Asserts the resulting JSON exposes both `schemaVersion` (string-equal to the
   configured value) and `testIndicator` (boolean false by default).

Plus three round-trip cases (`Source -> Json -> Source`) that exercise the
reverse mappers' `SchemaVersion` and `TestIndicator` lines together.

## Validation

`dotnet test tests/MTConnect.NET-JSON-cppagent-Tests/MTConnect.NET-JSON-cppagent-Tests.csproj -c Debug --nologo` —
`Passed: 36, Failed: 0`. The test project covers:

- 18 P2 unit pins (six per envelope).
- 6 P4 reflection-guard pins (three DTOs * two fields).
- 12 P5 wire-shape e2e cases (three envelopes * three schema versions, plus
  three round-trip cases).

## Coverage

The new fixture is test-only; production coverage on the touched files is
unchanged from P3 (still 100%).

## Deviations from plan

The plan called for an MQTT Docker E2E scenario (`tests/IntegrationTests/Regressions/Issue130MqttE2ETests.cs`,
`MTCONNECT_E2E_DOCKER=true`, mosquitto fixtures). The Docker MQTT fixtures and
the bootstrap shim that gates them on `MTCONNECT_E2E_DOCKER` ship with
`feat/issue-133` and are not on `upstream/master`. Per CONVENTIONS §17.8 row dated
2026-04-25 ("Bootstrap precondition unmet on upstream/master"), the E2E coverage
is reframed at the unit-integration boundary inside the JSON-cppagent test
project. The wire-shape assertions are equivalent — they exercise the same DTO
serialization path the MQTT publisher uses on the agent side. Once #133 lands,
a follow-up PR can lift these into the IntegrationTests project with live
mosquitto coverage.

## Follow-ups

- Once #133 merges, lift the `JsonHeaderWireShapeE2ETests` cases into the
  Compliance / IntegrationTests E2E harness with live MQTT capture.
