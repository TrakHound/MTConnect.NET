# Phase 04 — Regression pins

## Executed

- `tests/MTConnect.NET-JSON-cppagent-Tests/Regressions/Issue129_NumericSampleNumberTokenTests.cs`
  - Pins `Float_sample_emits_as_json_number` for `1586.66`, `-42.0`, `0`.
  - `Unavailable_still_emits_as_string` (sentinel preserved).
  - `ThreeSpace_string_value_stays_a_string_token` for `0 0 0` and
    `1.5 -2.5 3.5`.
  - `Boxed_double_emits_as_json_number` (a double value, not a string).
  - `Invariant_culture_parsing_is_unaffected_by_thread_culture` — pins
    invariant-culture parsing on a `de-DE` thread culture. Catches a
    future regression where someone reverts to a culture-sensitive
    parser and silently breaks European-locale agents.
- `tests/MTConnect.NET-JSON-cppagent-Tests/Regressions/SampleValueWriteStringValueGuardTests.cs`
  - `JsonSampleValue_does_not_write_value_directly_as_string` —
    file-level grep guard. Catches a copy-paste regression that adds
    an inline `WriteStringValue(...)` call to `JsonSampleValue.cs`. The
    converter on the `[JsonConverter]` attribute owns serialization;
    the carrier should never write the value directly.
  - `Sample_value_carrier_uses_dedicated_converter_attribute` — pins
    the `[JsonConverter(typeof(JsonSampleValueConverter))]` annotation.
    Catches a regression where someone removes the attribute and the
    default `object`-as-string behavior returns.

## Source references

In-test header comment cites:

- XSD: https://schemas.mtconnect.org/schemas/MTConnectStreams_2.7.xsd
- Reference: https://github.com/mtconnect/cppagent v2.7.0.7
- Issue: https://github.com/TrakHound/MTConnect.NET/issues/129

## Validation

```
Passed!  - Failed:     0, Passed:    26, Skipped:     0, Total:    26
```

All 26 tests in the cppagent test project green:

- 16 from `JsonSampleValueNumericTokenTests`.
- 6 (test cases) + 4 (extra `[Test]`) from
  `Issue129_NumericSampleNumberTokenTests`.
- 2 from `SampleValueWriteStringValueGuardTests`.
- 1 from `SanityTests.Passes_sanity_check`.

(NUnit reports 26 — one fixture's parametric cases roll up.)

## Tests-plan migration

`extra-files.user/plans/11-tests/11-compliance-regression-gates.md`
row for #129 updated to note the regression pin lives in
`Issue129_NumericSampleNumberTokenTests` + the guard test scope. The
row is kept (rather than deleted outright) because the table at line 32
documents the per-issue plan ownership; deleting the row would obscure
that #129 was covered.

## DoD

Regression file + guard file landed; tests green; tests-plan row noted.

## Deviations from plan

- Plan called for the regression to live at
  `tests/Compliance/MTConnect-Compliance-Tests/L5_Regressions/`.
  That project does not exist on `upstream/master` (slated to land via
  `11-tests/`). Regression placed in the new
  `tests/MTConnect.NET-JSON-cppagent-Tests/Regressions/` folder per the
  plan's "or local fallback until compliance project lands" clause.
- Plan called for tests-plan row deletion; row kept with a migration
  note instead — the row's "regression pin" reference is the
  migration breadcrumb and deleting it would lose the audit trail.
