# Issue #129 — JSON-cppagent emits numeric Sample value as string token

Reference: https://github.com/TrakHound/MTConnect.NET/issues/129

## 1. Defect + scope

Under the `JSON-cppagent` formatter (HTTP and MQTT variants), numeric Sample
observations serialize their `value` property as a JSON **string** token:

```json
"Temperature": [ { "value": "863.7060", "dataItemId": "temp" } ]
```

The cppagent reference implementation (v2.7.0.7) emits a JSON **number** token
for the same SHDR feed:

```json
"Temperature": [ { "value": 863.7060, "dataItemId": "temp" } ]
```

The MTConnect XSD `FloatSampleValueType` is a union of `xs:float` and the
literal `"UNAVAILABLE"` sentinel. A numeric-value-as-string token matches
neither member of the union and breaks downstream parity with cppagent.

Source references:

- XSD: https://schemas.mtconnect.org/schemas/MTConnectStreams_2.7.xsd —
  `FloatSampleValueType` simpleType, union of `xs:float` and `"UNAVAILABLE"`.
- cppagent reference: https://github.com/mtconnect/cppagent v2.7.0.7,
  `JsonPrinter` Sample-value formatting.

## 2. Investigation (P1)

Defect surface narrows to `JsonSampleValue.Value` (an untyped `object`
property) under `libraries/MTConnect.NET-JSON-cppagent/Streams/`. The
DataSet, Table, and TimeSeries representation carriers already branch
between numeric and string output via their dedicated converters; the
scalar Sample case has no converter and falls through to the default
`System.Text.Json` `object`-property handling, which emits the
underlying `string` value as a JSON string token. See
`docs/testing/issue-129/phase-01-defect-scoping.md` for the full
inventory + spec references.

## 3. Red tests (P2)

(Filled in by phase 02.)

## 4. Library fix (P3)

(Filled in by phase 03.)

## 5. Regression pins (P4)

(Filled in by phase 04.)

## 6. E2E validation (P5)

(Filled in by phase 05.)

## 7. Campaign summary (P6)

(Filled in by phase 06.)
