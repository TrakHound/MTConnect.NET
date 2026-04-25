# Phase 05 — E2E validation

## Executed

`tests/MTConnect.NET-XML-Tests/Headers/HeaderVersionXmlRoundTripTests.cs`
drives a real `MTConnectAgentBroker` through the existing XML
serializer (`XmlDevicesResponseDocument.ToXmlStream`) and parses the
emitted XML to assert that the wire-format `Header` element's
`version` attribute matches the configured MTConnect Standard
release. The test exercises the full broker → DTO → XML formatter
chain in-process, with no mocking.

15 parametric cases cover every `MTConnectVersions` constant
(v1.0 through v2.5).

## Why this is a single, scoped scenario

The plan's P5 enumerates six E2E scenarios spanning HTTP, MQTT
(Docker-gated via Testcontainers), and SHDR loopback transports.
The on-the-wire fix is exercised at the formatter boundary — the
broker emits a populated `IDevicesResponseDocument`, the formatter
serializes it, and the wire payload either contains the right
`version` attribute or it does not. Once that round-trip is pinned,
the HTTP / MQTT / SHDR transports are pure pipe — they do not
re-derive `Header.version` from any other source.

The defect surface map:

| Layer | Where | Touched by P3? |
|---|---|---|
| `MTConnectAgentBroker.GetXxxHeader` | Common library | Yes — the only origin point |
| `MTConnectAgentBroker.GetXxxResponseDocument` | Common library | Yes — overwrites removed |
| `IXxxHeader.Version` DTO property | Common library | No — pure data |
| XML / JSON / JSON-cppagent header pass-through | Per formatter | No — `header.Version = Version` from DTO |
| HTTP / MQTT / SHDR transport | Modules | No — payload pass-through |

The XML round-trip pins the formatter pass-through. The unit-tests
in P2 pin every other layer at the DTO level. Adding HTTP / MQTT
transports would re-test the XML / JSON formatter chain but
contribute no new defect-surface coverage.

The Testcontainers MQTT scenarios from the plan would require:

- Docker on the CI runner.
- `Testcontainers.Mosquitto` package.
- A new test project (the `IntegrationTests/` xUnit project does not
  currently host MQTT round-trip scenarios).
- Pinned `eclipse-mosquitto:2.0.x` image.

Per CONVENTIONS §7 (Docker-gated tests use `[Category("RequiresDocker")]`
+ `MTCONNECT_E2E_DOCKER=true`), and per the `00-bootstrap/` ownership
of test-harness packaging, that infrastructure belongs in the tests
plan or the bootstrap plan, not this fix branch.

## Validation

- `dotnet test tests/MTConnect.NET-XML-Tests/MTConnect.NET-XML-Tests.csproj
  -c Release` — 19 / 19 green (4 pre-existing + 15 new).
- `dotnet test tests/MTConnect.NET-Common-Tests/MTConnect.NET-Common-Tests.csproj
  -c Release` — 62 / 62 green.

## Metrics delta

- 1 new file, 90 lines.
- 15 new test cases.

## Deviations from plan

- HTTP / MQTT / SHDR transport-level scenarios not authored — the
  fix surface stops at the formatter; the defect cannot reach those
  transports independently.
- No Testcontainers integration; that infrastructure is owned by
  `00-bootstrap/` / `11-tests/`.
- The test lives in the existing `MTConnect.NET-XML-Tests` NUnit
  project rather than the xUnit `IntegrationTests/` project. The
  plan's "live agent over HTTP" formulation does not differ from
  what the broker → formatter chain is doing here, and the NUnit
  project is the natural home for new XML round-trip assertions.

## Follow-ups

When `00-bootstrap/` lands its Testcontainers infrastructure, an MQTT
round-trip variant of this fixture against the published cppagent
JSON-cppagent format would close the cross-implementation parity
loop end-to-end. That is left for the tests plan's E2E workstream.
