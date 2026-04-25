# Phase 06 — Docs and finalisation

## Executed

- `docs/testing/issue-129.md` filled out — defect summary, sources,
  per-phase pointers, and the campaign summary at section 7.
- All five per-phase writeups under `docs/testing/issue-129/`
  committed.
- PR body at `extra-files.user/plans/05-issue-129-numeric-sample-string-token/pr-body.md`
  drafted in P0; remains accurate at close-out (no scope changes
  beyond the audited section 18.2 expansion).

## DoD cross-check

- [x] P0 — foundation skeleton committed (`07b570e1`).
- [x] P1 — defect scoping documented (`5163e6f1`).
- [x] P2 — red tests landed (`c5e6706d` scaffold, `45f54dbf` red
      tests, `1c4aea5e` category alignment).
- [x] P3 — library fix landed (`98f3a19a`); 16 red tests now green.
- [x] P4 — regression pin (`9c3eebb6`) + guard (`1e3ed5c5`) landed;
      tests-plan row updated.
- [x] P5 — wire-format E2E landed (`12df6318`); Docker scenarios
      deferred to plan 11 per local-fallback clause.
- [x] P6 — docs + summary committed.

## Pre-close verification

- `dotnet build libraries/MTConnect.NET-JSON-cppagent/MTConnect.NET-JSON-cppagent.csproj`
  green (Debug net8.0, 0 errors, 20 unrelated warnings inherited from
  the `MTConnect.NET-Common` project).
- `dotnet test tests/MTConnect.NET-JSON-cppagent-Tests/MTConnect.NET-JSON-cppagent-Tests.csproj`
  green: 38/38 passed.
- `git status` clean.

## Coverage

`coverlet.collector` (the project's existing coverage adapter) failed
to produce output in this sandbox due to a `Broken pipe` error in the
data-collector socket layer — a tooling-level environment issue
unrelated to the plan. `coverlet.msbuild` was added temporarily and
attempted but also did not produce a cobertura.xml inside the time
budget. The csproj change was reverted before finalisation; the
shipped test project relies on the existing `coverlet.collector`.

**Coverage is asserted by an explicit branch-to-test mapping:**

`libraries/MTConnect.NET-JSON-cppagent/Streams/JsonSampleValueConverter.cs`
(new, 84 lines, 9 branches in `Write` + 3 branches in `Read`):

| Code path | Test |
|---|---|
| `Read` Number branch | `Read_returns_double_for_number_token` |
| `Read` String branch | `Read_returns_string_for_string_token` |
| `Read` default branch | `Read_returns_null_for_unsupported_token` |
| `Write` null branch | `Boxed_null_is_written_as_null_token_when_emitted` |
| `Write` sentinel string | `Unavailable_sample_value_emits_string_token` |
| `Write` numeric-string | `Numeric_string_sample_value_emits_number_token` (6 cases) |
| `Write` non-numeric string fallback | `Three_space_string_value_stays_a_string_token` |
| `Write` `double` branch | `Double_sample_value_emits_number_token` |
| `Write` `int` branch | `Integer_sample_value_emits_number_token` |
| `Write` other-numeric branches (`float`, `decimal`, `long`, `uint`, `ulong`, `short`, `ushort`, `byte`, `sbyte`) | mechanical equivalents to the `double`/`int` cases — `WriteNumberValue` overloads with the same contract; documented as defensive cover for callers that pass non-default numeric types. |
| `Write` non-numeric non-string fallback | `Bool_sample_value_emits_string_token_via_invariant_format` |

`libraries/MTConnect.NET-JSON-cppagent/Streams/JsonSampleValue.cs` —
two-line modification (added `using` + `[JsonConverter]` attribute);
no behavioral lines added; existing constructors and `ToObservation`
unchanged.

The "other-numeric branches" row above is the only line of the new
converter not directly exercised by a unique test. Each branch is a
single-token type-pattern fall-through to `WriteNumberValue(<value>)`;
.NET's pattern-match switch on the boxed type guarantees the correct
overload at runtime, and the surface is structurally equivalent to
the explicitly-tested `double` and `int` cases.

## Follow-ups

- File a `todo.md §F` pattern: add a coverage-collection sanity check
  to the bootstrap plan's CI workflow so future per-issue PRs catch
  the data-collector-broken-pipe symptom before phase close.
- Optional simplification: collapse the eleven explicit numeric
  primitive branches in `JsonSampleValueConverter.Write` into a
  single `IsNumeric()`-based path that calls
  `WriteNumberValue(((object)value).ToDouble())`. Trades runtime
  precision (long => double has ~15 decimal digits) for surface
  simplicity. Defer to a maintainer review before changing.

## Close-out

Stops at end of phase 07 per dispatch instructions. Draft PR remains
draft; user reviews before flipping to ready / requesting maintainer
review.
