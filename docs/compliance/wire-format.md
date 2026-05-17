# Wire-format compliance

`MTConnect.NET` ships five wire formats. Each ones' on-the-wire shape is documented and validated against an authoritative source: an XSD for XML, the cppagent reference for the v2 JSON codec, and the spec prose for the SHDR adapter protocol. This page states each format's compliance posture — byte-for-byte parity with the reference, partial parity, or intentional divergence — and points at the test fixtures and source files that pin the claim.

## Summary

| Wire format | Reference | Posture | Codec class |
|---|---|---|---|
| XML | `MTConnect<Envelope>_<version>.xsd` from [schemas.mtconnect.org](https://schemas.mtconnect.org/) | XSD-validated, every envelope, every version v1.0 — v2.7 | [`MTConnect.Formatters.Xml.XmlFormatter`](/api/MTConnect.Formatters.Xml/XmlFormatter) |
| JSON v1 | Library-defined (no normative JSON Schema exists) | Stable on the library, mirrors XML's object structure | [`MTConnect.Formatters.Json.JsonFormatter`](/api/MTConnect.Formatters.Json/JsonFormatter) |
| JSON-CPPAGENT (v2) | cppagent's `application/mtconnect+json` output | Byte-for-byte cppagent-parity (array-of-wrappers canonical shape) | [`MTConnect.Formatters.JsonCppAgent.JsonCppAgentFormatter`](/api/MTConnect.Formatters.JsonCppAgent/JsonCppAgentFormatter) |
| JSON-CPPAGENT-MQTT | cppagent's MQTT topic broker output | Byte-for-byte cppagent-parity (per-DataItem topic shape) | [`MTConnect.Formatters.JsonCppAgentMqtt.JsonCppAgentMqttFormatter`](/api/MTConnect.Formatters.JsonCppAgentMqtt/JsonCppAgentMqttFormatter) |
| SHDR | `Part_5.0` Network spec prose | Spec-compliant; both as adapter producer and as adapter consumer | [`MTConnect.Shdr.ShdrAdapter`](/api/MTConnect.Shdr/ShdrAdapter) |

## XML

The XML output of every Streams / Devices / Assets / Error envelope validates against the matching `MTConnect<Envelope>_<version>.xsd` schema from [schemas.mtconnect.org](https://schemas.mtconnect.org/). The compliance test harness loads each shipped XSD as an embedded resource and validates a battery of golden-file fixtures against it (see [Test harness](/compliance/test-harness)). The XSDs are vendored unchanged into the test project, byte-identical to the upstream copies. Formatter pass: [`MTConnect.Formatters.Xml.XmlFormatter`](/api/MTConnect.Formatters.Xml/XmlFormatter) emits with `XmlWriterSettings.Indent = true` by default, configurable through the `IndentOutput` flag on each Module's configuration.

The library's XSD validator deliberately uses a permissive parser configuration to remain cross-platform. The .NET BCL ships an XSD 1.0 validator; a small set of XSD 1.1-only assertions in the published schemas (such as `xs:assert` for the `representation` enum) are not enforced at parse time, but the library injects the equivalent runtime checks. See [Troubleshooting: XSD validation failures](/troubleshooting/xsd-validation-failures) for the diagnostic pattern when a model fails validation locally that succeeded against an XSD 1.1 validator.

## JSON v1

JSON v1 is `MTConnect.NET`'s in-house JSON shape, predating the MTConnect Standard's adoption of a JSON wire format. It mirrors the XML object structure one-to-one — `<Streams>` becomes `Streams`, every attribute becomes a field, every nested element becomes a nested object. The format is documented at [Wire formats: JSON v1](/wire-formats/json-v1) and remains a supported output for consumers that depend on it. The MTConnect Standard does not declare a normative JSON Schema (the JSON wire format in `Part_5` is descriptive prose, not a normative schema — see [Redmine #3889](https://projects.mtconnect.org/issues/3889)); the library's JSON v1 codec is the canonical shape against itself and is regression-tested with golden fixtures.

## JSON-CPPAGENT (v2)

The JSON-CPPAGENT codec is the library's byte-for-byte parity target with the [cppagent](https://github.com/mtconnect/cppagent) reference implementation's `application/mtconnect+json` output. The canonical shape is an array of single-key wrappers — each observation is wrapped in `{"<type>": {...}}` — which the cppagent v2 codec adopted in 2023.

Round-trip parity test fixtures live in `tests/MTConnect.NET-Common-Tests/CppAgentParity/` (golden files captured from a published cppagent build, byte-compared against `MTConnect.NET`'s output). A successful run is the gate that any consumer parsing cppagent's output can swap in `MTConnect.NET`'s agent and observe no change.

Documented at [Wire formats: JSON-CPPAGENT](/wire-formats/json-cppagent).

## JSON-CPPAGENT-MQTT

The JSON-CPPAGENT-MQTT codec is the per-DataItem MQTT topic broker shape: every DataItem publishes to a topic of the form `<prefix>/Device/<deviceUuid>/<dataItemId>` with a JSON-CPPAGENT-formatted payload. The cppagent reference's MQTT broker (introduced in cppagent v2) defines the topic template; `MTConnect.NET`'s [MqttRelay](/modules/) and [MqttBroker](/modules/) modules both speak this format.

Compliance is enforced by golden-file fixtures of cppagent's published MQTT output and round-trip parity tests. Documented at [Wire formats: JSON-CPPAGENT-MQTT](/wire-formats/json-cppagent-mqtt).

## SHDR

The SHDR protocol is the legacy text-line adapter protocol described in `Part_5.0` Network of the MTConnect Standard ([docs.mtconnect.org](https://docs.mtconnect.org/)). A line is `timestamp|dataItemKey|value` with subsequent `|key|value` repeats for compound observations. `MTConnect.NET` implements SHDR both as an adapter (the agent reads SHDR lines from a TCP socket) and as a producer (the standalone adapter app emits SHDR lines from a custom data source).

Compliance is enforced by:

- A round-trip parser test against every example line in `Part_5.0`'s prose (each test fixture cites the spec section it pins).
- A heartbeat-and-reconnect state-machine test against the `Part_5.0` mandated SHDR connection lifecycle (PING / PONG, asterisk heartbeat).

Documented at [Wire formats: SHDR](/wire-formats/shdr).

## Variants of "compliant"

Three claim levels in this section:

- **XSD-validated** — for XML envelopes. The output is fed through the matching XSD; a parse-clean result is sufficient.
- **Byte-for-byte parity** — for the JSON-CPPAGENT and JSON-CPPAGENT-MQTT codecs. The output is byte-compared against a captured cppagent reference output of the same model.
- **Spec-compliant prose** — for SHDR. The behavior is exercised against test fixtures that pin the spec's stated examples and state machines.

Each claim is auditable: the test fixtures cite the source ([Compliance: Spec cross-references](/compliance/spec-cross-references)), and the test harness records pass / fail per claim level.

## Cross-implementation matrix

A consumer asks "will this work against MTConnect.NET the same way it works against cppagent?" The answer:

| Wire format | cppagent v2.5+ | MTConnect.NET | Round-trip parity? |
|---|---|---|---|
| XML | yes | yes | yes (XSD-validated both sides) |
| JSON v1 | no (cppagent never shipped JSON v1) | yes | n/a |
| JSON-CPPAGENT v2 | yes | yes | yes (golden-file byte-parity) |
| JSON-CPPAGENT-MQTT | yes | yes | yes (golden-file byte-parity) |
| SHDR | yes | yes | yes (per `Part_5.0` prose) |

## Where to next

- [Per-version matrix](/compliance/per-version-matrix) — which MTConnect spec versions each wire format supports.
- [Test harness](/compliance/test-harness) — how to run the compliance tier locally.
- [Wire formats: index](/wire-formats/) — per-format envelope shapes and sample payloads.
