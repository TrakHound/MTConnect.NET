# Phase 02 — Red tests

## Executed

- Scaffolded a new paired test project at
  `tests/MTConnect.NET-JSON-cppagent-Tests/` (single-TFM `net8.0`,
  NUnit 3.13.x, coverlet.collector). Layout matches the existing
  `MTConnect.NET-Common-Tests/` convention. Sentinel test
  `SanityTests.Passes_sanity_check` first lands in its own commit per
  the bootstrap pattern (commit `c5e6706d`).
- Wrote red tests in
  `tests/MTConnect.NET-JSON-cppagent-Tests/Streams/JsonSampleValueNumericTokenTests.cs`
  asserting:
  - numeric-string Sample values (`"0"`, `"42.5"`, `"-17.0"`,
    `"1586.66"`, `"0.000001"`, `"1e6"`) serialize as JSON number tokens;
  - boxed `double` and boxed `int` Sample values stay as number tokens;
  - the three-space `"1.5 -2.5 3.5"` literal stays a JSON string token
    (the whole value is non-numeric);
  - the `Observation.Unavailable` sentinel stays a JSON string token;
  - `null` Value is omitted by `JsonIgnoreCondition.WhenWritingDefault`.
- Category label: `NumericSampleAsJsonNumber` (CONVENTIONS-§14
  compliant — describes the assertion, not the bookkeeping number).

## Source references

In-test header comment cites:

- XSD: https://schemas.mtconnect.org/schemas/MTConnectStreams_2.7.xsd
  `FloatSampleValueType` simpleType, union of `xs:float` and the
  `"UNAVAILABLE"` enumeration member.
- Prose: https://docs.mtconnect.org Part_2.0 "Sample Value Types".
- Reference: https://github.com/mtconnect/cppagent v2.7.0.7
  (`JsonPrinter` Sample-value formatting).

## Red confirmation

`dotnet test tests/MTConnect.NET-JSON-cppagent-Tests/...`:

```
Failed!  - Failed:     6, Passed:    10, Skipped:     0, Total:    16
```

Six failures, all of the form
`Expected: Number / But was: String` for the
`Numeric_string_sample_value_emits_number_token` cases — which is the
defect under test. The `Double_sample_value_*` and
`Integer_sample_value_*` cases pass even pre-fix because `System.Text.Json`
emits the boxed numeric primitives as number tokens already; the
sentinel and three-space cases also pass — they are no-regression
guards.

## CI job

Per CONVENTIONS §1.7 (per-issue PRs do not modify
`.github/workflows/`), no CI workflow change is made. Reviewers run the
test locally with
`dotnet test tests/MTConnect.NET-JSON-cppagent-Tests/MTConnect.NET-JSON-cppagent-Tests.csproj`.

## DoD

Six red cases failing for the right reason; sentinel + edge cases are
no-regression guards; test project scaffold pushed; category label
self-contained.

## Deviations from plan

- Category label changed from the plan's earlier `Issue129Red` /
  `NumericSampleAsJsonNumber` placeholder to a CONVENTIONS-§14
  compliant `NumericSampleAsJsonNumber` (the plan's `00-overview.md`
  was updated alongside, 2026-04-25, to match).
- Plan's `IntegerSampleValueType` and `ThreeSpaceSampleValueType` are
  not separate JSON carriers — both flow through the scalar
  `JsonSampleValue` path (DataSet, Table, and TimeSeries
  representations have their own carriers and converters that already
  emit numbers). The single converter covers all numeric cases; tests
  assert on the unified surface.
