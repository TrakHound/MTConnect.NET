# Issue #128 — JSON-cppagent schemaVersion hardcoded

## 1. Defect + scope

`MTConnectStreams.schemaVersion` and `MTConnectDevices.schemaVersion` were
hardcoded to the literal `"2.0"` in both ctors of
`JsonMTConnectStreams` and `JsonMTConnectDevices`, regardless of the
agent's configured `DefaultVersion`. The cppagent JSON-MQTT format
contract requires the configured release to flow through to the wire.

Surface (two production files):

- `libraries/MTConnect.NET-JSON-cppagent/Streams/JsonMTConnectStreams.cs`
- `libraries/MTConnect.NET-JSON-cppagent/Devices/JsonMTConnectDevices.cs`

`JsonMTConnectAssets.cs` does not expose `SchemaVersion` and is unaffected.

## 2. Investigation (P1)

## 3. Red tests (P2)

## 4. Library fix (P3)

## 5. Regression pins (P4)

## 6. E2E validation (P5)

## 7. Campaign summary (P6)
