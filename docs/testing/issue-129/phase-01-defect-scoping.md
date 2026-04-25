# Phase 01 — Defect scoping

## Investigation summary

The cppagent JSON formatter has four Sample-value carrier types under
`libraries/MTConnect.NET-JSON-cppagent/Streams/`:

| File | Carrier kind | Numeric path today |
|---|---|---|
| `JsonSampleValue.cs` | scalar Sample (Float / Integer) — `Value` is `object` | **Defect** — `Value` holds the raw `string` from `observation.GetValue(ValueKeys.Result)`; `System.Text.Json` emits a string token unconditionally. |
| `JsonSampleDataSet.cs` | `DATA_SET` representation — `Entries` typed `JsonDataSetEntries` | OK — `JsonDataSetEntries.JsonDataSetEntriesConverter` already branches on `entry.Value.IsNumeric()` and emits `WriteNumber` / `WriteString` per entry. |
| `JsonSampleTable.cs` | `TABLE` representation — `Entries` typed `JsonTableEntries` | OK — same branching as DataSet via `JsonTableEntriesConverter`. |
| `JsonSampleTimeSeries.cs` | `TIME_SERIES` representation — `Samples` typed `JsonTimeSeriesSamples` | OK — `JsonTimeSeriesSamplesConverter.Write` writes a JSON number for each sample (`writer.WriteNumberValue(sample)`); only the `UNAVAILABLE` sentinel is emitted as a string. |

`grep -rn 'WriteStringValue\|WriteNumberValue\|JsonConverter.*Sample'
libraries/MTConnect.NET-JSON-cppagent/` confirms the inventory:

```
Streams/JsonTableEntries.cs:100:                writer.WriteStringValue(Observation.Unavailable);
Streams/JsonDataSetEntries.cs:88:               writer.WriteStringValue(Observation.Unavailable);
Streams/JsonTimeSeriesSamples.cs:74:            writer.WriteNumberValue(sample);
Streams/JsonTimeSeriesSamples.cs:82:            writer.WriteStringValue(Observation.Unavailable);
```

No `JsonSampleValueConverter` exists today; that's the defect.

## Root cause

`JsonSampleValue.Value` is a plain `object` property (no
`[JsonConverter]`). Construction sites all do
`Value = observation.GetValue(ValueKeys.Result)` which always returns a
`string`. `System.Text.Json` serializes `object` properties using the
runtime type — a `string` value, even one containing only digits, is
written as a JSON string token.

The `IObservation.GetValue` API does not distinguish numeric Sample
results from non-numeric Sample results, so the fix lives at the JSON
boundary, not at the observation construction site.

## Fix surface

A custom `JsonConverter` for the `JsonSampleValue.Value` `object`
property that branches on the runtime payload:

- `Observation.Unavailable` ("UNAVAILABLE") -> `WriteStringValue`.
- Numeric primitive (`double`, `float`, `decimal`, `int`, `long`, etc.)
  -> `WriteNumberValue`.
- `string` that parses as a `double` (invariant culture) ->
  `WriteNumberValue`.
- Anything else (`null`, non-numeric string, custom payload) ->
  `WriteStringValue` fallback (preserves backward compatibility for any
  consumer relying on the string-fallback escape hatch).

The same branching pattern is already used by
`JsonDataSetEntriesConverter` and `JsonTableEntriesConverter`; the new
converter mirrors that contract for the simple Sample case.

## Sentinel handling

`Observation.Unavailable` (the literal `"UNAVAILABLE"`) MUST stay as a
string token per `FloatSampleValueType` union member 2. Verified in
existing fixtures: `JsonSampleValueTests` (none today) need a positive
sentinel test pinning the string-token expectation.

## Spec sources

- XSD: https://schemas.mtconnect.org/schemas/MTConnectStreams_2.7.xsd
  `FloatSampleValueType` simpleType (union of `xs:float` and
  `"UNAVAILABLE"`).
- Prose: https://docs.mtconnect.org/ Part_2.0 §6.5 "Sample Value Types".
- Reference: https://github.com/mtconnect/cppagent v2.7.0.7,
  `JsonPrinter::printSampleValue`.

## Existing-test impact

`grep -rn '"value":\s*"[0-9]' tests/` returns no hits — no existing test
asserts the buggy behavior; the fix will not break any current
assertions.

## DoD

Converter inventory final; `JsonSampleValue.cs` is the single defect
surface; sentinel-handling decision explicit (string token); fallback
decision explicit (preserve as string).

## Deviations from plan

None. Plan-anticipated `IntegerSampleValueType` and
`ThreeSpaceSampleValueType` are not separate XSD-mapped C# carriers in
this codebase — both flow through `JsonSampleValue` (scalar) for
non-DataSet / non-Table / non-TimeSeries Samples. The single converter
covers all numeric Sample cases.
