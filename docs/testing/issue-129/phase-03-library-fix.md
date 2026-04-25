# Phase 03 — Library fix

## Executed

Added a custom `JsonConverter<object>` at
`libraries/MTConnect.NET-JSON-cppagent/Streams/JsonSampleValueConverter.cs`
and applied it to `JsonSampleValue.Value` via `[JsonConverter(...)]`.

## Converter contract

`Write(Utf8JsonWriter, object, JsonSerializerOptions)`:

- `null` -> `WriteNullValue`. (In practice `JsonSampleValue` omits
  `null` Values via `JsonIgnoreCondition.WhenWritingDefault`, but the
  branch is kept defensive.)
- `string` matching `Observation.Unavailable` (`"UNAVAILABLE"`) ->
  `WriteStringValue` (preserves the XSD `FloatSampleValueType` union
  member 2 and round-trips with cppagent).
- `string` parseable as `double` under `CultureInfo.InvariantCulture`
  with `NumberStyles.Float` -> `WriteNumberValue`.
- `string` non-numeric, non-sentinel -> `WriteStringValue` (e.g. a
  three-space sample like `"1.5 -2.5 3.5"`).
- Numeric primitive (`double`, `float`, `decimal`, `int`, `long`,
  `uint`, `ulong`, `short`, `ushort`, `byte`, `sbyte`) ->
  `WriteNumberValue` of the appropriate overload.
- Anything else -> `WriteStringValue` after `string.Format(invariant,
  "{0}", value)`.

`Read(ref Utf8JsonReader, ...)`:

- `Number` token -> `GetDouble()`.
- `String` token -> `GetString()`.
- Otherwise -> `TrySkip()`; return `null`.

## Why a single converter, not three

Plan-anticipated `IntegerSampleValueType` and
`ThreeSpaceSampleValueType` are not separate JSON carriers in this
codebase — both reach the JSON layer through `JsonSampleValue.Value`
(scalar Sample). The DataSet, Table, and TimeSeries representations
have their own converters that already emit JSON number tokens
(`JsonDataSetEntries.Converter`, `JsonTableEntries.Converter`,
`JsonTimeSeriesSamples.Converter`). One converter on the scalar carrier
is the minimal correct surface.

## Sentinel handling

Verified by `Unavailable_sample_value_emits_string_token` — token kind
is `JsonValueKind.String`, content is `"UNAVAILABLE"`.

## Existing fixtures

`grep -rn '"value":\s*"[0-9]' tests/` returns no hits in the upstream
tree at branch-cut time — no existing test asserts the buggy behavior,
so the fix introduces no regression.

## Validation

`dotnet test tests/MTConnect.NET-JSON-cppagent-Tests/...`:

```
Passed!  - Failed:     0, Passed:    16, Skipped:     0, Total:    16
```

All 16 cases green: 6 previously-red `Numeric_string_*` cases now
emit number tokens; 3 `Double_*` + 3 `Integer_*` + 1 three-space + 1
sentinel + 1 null-omission cases still green (no regression).

## DoD

Red tests green; sentinel + non-numeric + null behavior preserved;
single atomic commit per CONVENTIONS §1.5.

## Deviations from plan

- Plan called for one commit per converter (Float / Integer /
  ThreeSpace) — collapsed to a single commit because the single
  converter covers all three (rationale above).
- Plan said "category + CI job removed" — never created a CI job per
  CONVENTIONS §1.7; the `NumericSampleAsJsonNumber` category persists
  through to phase 04 as a self-contained label.
