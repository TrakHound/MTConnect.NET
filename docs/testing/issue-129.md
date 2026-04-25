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

`tests/MTConnect.NET-JSON-cppagent-Tests/Streams/JsonSampleValueNumericTokenTests.cs`
pins six numeric-string cases (`0`, `42.5`, `-17.0`, `1586.66`,
`0.000001`, `1e6`), three boxed-`double` cases, three boxed-`int` cases,
the three-space string-token case, the `UNAVAILABLE` sentinel
string-token case, and a null-omission case. Pre-fix red: 6 of 16
fail with `Expected: Number / But was: String`. See
`docs/testing/issue-129/phase-02-red-tests.md`.

## 4. Library fix (P3)

New `JsonSampleValueConverter` at
`libraries/MTConnect.NET-JSON-cppagent/Streams/`. Applied as
`[JsonConverter(typeof(JsonSampleValueConverter))]` on
`JsonSampleValue.Value`. Branches: `null` -> null-token; sentinel -> string;
numeric primitive or numeric-parseable string -> number; anything else
-> string fallback. See
`docs/testing/issue-129/phase-03-library-fix.md`.

## 5. Regression pins (P4)

`tests/MTConnect.NET-JSON-cppagent-Tests/Regressions/Issue129_NumericSampleNumberTokenTests.cs`
pins the JSON-number-token contract for representative numeric inputs,
preserves the `UNAVAILABLE` sentinel as a string token, locks the
three-space carrier as a string token, and pins
invariant-culture parsing under `de-DE` thread culture.
`SampleValueWriteStringValueGuardTests` is a file-level grep guard
that prohibits inline `WriteStringValue` in `JsonSampleValue.cs` and
pins the `[JsonConverter]` attribute. See
`docs/testing/issue-129/phase-04-regression-pins.md`.

## 6. E2E validation (P5)

`tests/MTConnect.NET-JSON-cppagent-Tests/E2E/SampleValueWireFormatE2ETests.cs`
exercises the `IObservation -> JsonSampleValue -> JsonSerializer`
path end-to-end and pins the wire-format tokens. Docker-gated MQTT
round-trip scenarios deferred to plan 11's compliance E2E project per
the plan's local-fallback clause. See
`docs/testing/issue-129/phase-05-e2e-validation.md`.

## 7. Campaign summary (P6)

- **Issue**: TrakHound/MTConnect.NET#129 — JSON-cppagent emitted
  numeric Sample values as JSON string tokens, breaking parity with
  the cppagent reference (v2.7.0.7) and violating the XSD
  `FloatSampleValueType` union (xs:float | "UNAVAILABLE").
- **Root cause**: `JsonSampleValue.Value` (an `object` property) had
  no custom converter; default `System.Text.Json` `object`-property
  serialization wrote the underlying `string` value as a JSON string
  token regardless of whether it represented a number.
- **Fix**: New `JsonSampleValueConverter` writes a JSON number token
  for numeric primitives and numeric-parseable strings; preserves
  `Observation.Unavailable` and any non-numeric string as a JSON string
  token. Applied via `[JsonConverter(...)]` on the property.
- **Tests**: 33 cases under `MTConnect.NET-JSON-cppagent-Tests/`
  covering numeric strings, boxed numerics, sentinel preservation,
  three-space carriers, null omission, invariant-culture parsing,
  the `[JsonConverter]` attribute pin, and the
  inline-`WriteStringValue` grep guard.
- **Coverage**: 100% on `JsonSampleValueConverter.cs` and the
  modified line in `JsonSampleValue.cs` (the `[JsonConverter]`
  attribute). See `docs/testing/issue-129/phase-06-finalisation.md`.
- **No public API changes**.
