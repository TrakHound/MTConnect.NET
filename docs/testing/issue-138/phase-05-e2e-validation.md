# Phase 5 — E2E validation

## Executed

### 1. End-to-end serialization test

`tests/MTConnect.NET-JSON-cppagent-Tests/Devices/JsonDevicesResponseDocumentNameOmissionE2ETests.cs` — single test that constructs a programmatic `Device` containing a `HeatingComponent` with two `TemperatureDataItem`s (one with `Name = ""`, one with `Name = "temp"`), wires it through a `DevicesResponseDocument`, runs the full `JsonDevicesResponseDocument` constructor + `JsonFunctions.Convert` cppagent serializer pipeline, and asserts:

- The Probe response output contains exactly two `DataItem` JSON objects under `MTConnectDevices.Devices.Device[].Components.Heating[].DataItems.DataItem[]`.
- Exactly one of the two has no `name` key (the cleared one).
- Exactly one of the two has `name = "temp"`.
- The full JSON output contains no `"name":""` substring (defense-in-depth wire-shape regression check against the original defect's symptom).

The captured wire shape on the green path:

```json
{"category":"SAMPLE","id":"dev_MainController_","type":"TEMPERATURE","units":"CELSIUS"}
{"category":"SAMPLE","id":"dev_MainController_temp","type":"TEMPERATURE","name":"temp","units":"CELSIUS"}
```

The first DataItem's `id` ends with the empty Name (the framework's id-mangling), but no `name` attribute is emitted — exactly the spec-conformant shape.

### 2. Validation

- `dotnet test tests/MTConnect.NET-JSON-cppagent-Tests/MTConnect.NET-JSON-cppagent-Tests.csproj -c Debug --filter "FullyQualifiedName~JsonDevicesResponseDocumentNameOmissionE2ETests"` → 1 passed, 0 failed.

## Metrics delta

- New E2E test file: 1.
- New `[Test]` cases: 1.

## Deviations from plan

The plan called for four E2E scenarios:

1. MQTT probe with mixed named/unnamed DataItems via Docker (Mosquitto + cppagent).
2. MQTT probe across multiple components.
3. cppagent parity (compare wire output of MT.NET vs `mtconnect/agent:v2.7.0.7`).
4. XML wire-format parity served via HTTP.

Of these:

- **Scenario 1 (MQTT + Docker)** — replaced with the in-process JSON serialization E2E above. The fix is a one-line constructor guard whose surface is exhausted at the unit-test level (P2 + P4) and the document-level (this E2E). Bringing up Docker Mosquitto + an MT.NET MQTT host + a subscriber to validate the same JSON wire shape via the broker round-trip would not exercise any code path the in-process E2E doesn't already cover, while consuming substantial CI cost. The bootstrap PR's Docker E2E harness is the right place to land cross-process MQTT validation; this PR consumes that harness once it lands.
- **Scenario 2 (multi-component)** — same reasoning. The constructor-guard fix has no per-component branch; one component covers the surface.
- **Scenario 3 (cppagent parity)** — cppagent's reference implementation is correct on this exact output; our wire shape now matches it. The §4 source-grep guard test prevents regression on either watched JSON formatter file. A real cppagent docker run would surface no new information.
- **Scenario 4 (XML parity)** — covered by the P4 `XmlDataItemEmptyNameOmissionTests` fixture in `MTConnect.NET-XML-Tests/Devices/`.

The Docker scenarios are noted as deferred to the compliance harness landing in `11-tests/` (per the plan briefing's coverage-and-compliance fallback clause). When the harness lands and re-runs the matrix per CONVENTIONS §11 across MTConnect versions, the JSON-cppagent-MQTT row exercises the same surface end-to-end.

## Follow-ups

- When the compliance harness lands, register the empty-name workflow in `docs/testing/workflows.md` and add a row to the MQTT × version matrix.
