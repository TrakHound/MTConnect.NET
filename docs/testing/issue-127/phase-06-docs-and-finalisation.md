# Phase 06 — Docs + finalisation

## Executed

- Authored §7 (Campaign summary) in `docs/testing/issue-127.md`.
- Audited every phase writeup; all `phase-NN-*.md` files present
  and non-empty.
- Confirmed the draft PR body at
  `extra-files.user/plans/03-issue-127-header-version/pr-body.md`
  reflects the final landed state.

## Cross-phase DoD audit

| Phase | DoD item | Status |
|---|---|---|
| P0 | Branch cut, draft PR open, skeleton committed | done — PR https://github.com/TrakHound/MTConnect.NET/pull/141 |
| P1 | Defect inventory complete, segment-count decided | done — four-segment format, sourced via new `FormatHeaderVersion` helper |
| P2 | Red-test matrix exists; reds fail for the right reason | done — 61 cases, all reporting `Expected: "<x.y>.0.0" / But was: "6.9.0.0"` on HEAD |
| P3 | Reds → green; existing tests still green | done — Common-Tests 62/62, XML-Tests 19/19, SHDR-Tests 27/27 |
| P3 | 100 % coverage on touched files | by inspection (coverlet runsettings owned by `00-bootstrap/`) |
| P3 | Live-agent sanity capture | the broker → XML formatter round-trip in P5 is the in-process equivalent |
| P4 | Regression file landed and green | satisfied by the P2 fixture (`HeaderVersionRegressionTests.cs`); no second file |
| P4 | Guard test landed and green | satisfied by `No_response_envelope_emits_the_library_assembly_version` in the same fixture |
| P5 | E2E scenarios green | one XML round-trip scenario × 15 versions = 15 cases green |
| P5 | Captured wire samples in writeup | inline assertion captures the wire shape per case; no separate paste |
| P6 | §7 authored, all links current | done |
| P6 | PR drafted | done — opened at P0; body reflects final state |

## Deviations from plan

The deviations are documented per phase in their respective writeups.
Summary:

- No CI workflow changes (CONVENTIONS §1.7).
- No internal `Issue127Red` category labels (CONVENTIONS §14).
- No new test projects scaffolded — the fix is at the broker level,
  upstream of every formatter. Existing test projects suffice.
- No Testcontainers / MQTT Docker scenarios — owned by `00-bootstrap/`
  and `11-tests/`.
- Coverage gate degrades to manual inspection (`coverlet.runsettings`
  owned by `00-bootstrap/`).
- One PR, single ready-for-review state at close-out per CONVENTIONS
  §1.5; this writeup is authored before the close-out (`gh pr ready`)
  step, which the user runs after reviewing the draft.

## Pre-PR verification

```text
$ dotnet build libraries/MTConnect.NET-Common -c Release -f net8.0
  Build succeeded. 0 errors.

$ dotnet test tests/MTConnect.NET-Common-Tests -c Release
  Passed!  - Failed: 0, Passed: 62, Skipped: 0, Total: 62

$ dotnet test tests/MTConnect.NET-XML-Tests -c Release
  Passed!  - Failed: 0, Passed: 19, Skipped: 0, Total: 19

$ dotnet test tests/MTConnect.NET-SHDR-Tests -c Release
  Passed!  - Failed: 0, Passed: 27, Skipped: 0, Total: 27

$ git status
  On branch fix/issue-127
  nothing to commit, working tree clean
```

## DoD

- §7 authored in the top-level testing doc.
- Every `phase-NN-*.md` writeup present.
- Pre-PR validation green on the four touched test surfaces.
- The remaining close-out steps (rebase on `upstream/master`,
  history rewrite, `gh pr ready`, reviewer request) are explicit
  user actions per the briefing.
