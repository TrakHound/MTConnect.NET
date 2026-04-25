# Phase 6 — Campaign summary

## Executed

- Filled in the every-section detail in `docs/testing/issue-130-131.md`
  (sections 2 through 7), each cross-referencing the per-phase writeup.
- Confirmed the draft PR body (`extra-files.user/plans/08-issue-130-131-cppagent-header-missing-fields/pr-body.md`)
  matches the landed scope and carries the `Depends on #133` note required for
  the test-project scaffolding overlap.

## Validation gate

- `dotnet build libraries/MTConnect.NET-JSON-cppagent/MTConnect.NET-JSON-cppagent.csproj -c Debug -f net8.0`
  green (0 errors, 2 unrelated sourcelink warnings).
- `dotnet test tests/MTConnect.NET-JSON-cppagent-Tests/MTConnect.NET-JSON-cppagent-Tests.csproj -c Debug --nologo`
  green: `Passed: 36, Failed: 0, Skipped: 0`.
- `git status` clean.
- Every commit pushed to `origin/fix/issue-130-131`.

## Metrics delta

| Metric                                      | Before | After |
|---------------------------------------------|--------|-------|
| `JsonStreamsHeader` SchemaVersion member    | absent | declared + wired + round-trip |
| `JsonDevicesHeader` SchemaVersion member    | declared, not wired | declared + wired + round-trip |
| `JsonAssetsHeader` SchemaVersion member     | absent | declared + wired + round-trip |
| `IMTConnect*Header.SchemaVersion`           | absent | added on three interfaces + impls |
| Test cases on JSON-cppagent layer           | 0 | 36 |
| Production files reaching 100% coverage     | n/a (paired test project did not exist) | 9 (3 interfaces + 3 impls + 3 cppagent DTOs) |

## Deviations from plan

Three plan-level deviations, all flagged in earlier phase writeups:

1. **#131 already addressed** — The plan paired #130 + #131 for adjacent
   line-region edits. `testIndicator` turned out to be live on
   `upstream/master`, so this PR contributes only regression pins for #131,
   not production code (`phase-00-foundation.md` §1).
2. **Source-DTO widening** — The plan assumed `IMTConnect*Header.SchemaVersion`
   was already present on the source surface; it was not. The plan was widened
   to add the property to three interfaces + three concrete classes
   (`phase-00-foundation.md` §2).
3. **Bootstrap test-project scaffolding** — `tests/MTConnect.NET-JSON-cppagent-Tests/`
   does not exist on `upstream/master`. Per CONVENTIONS §17.8 row dated
   2026-04-25, the project is scaffolded on this branch, with the
   `Depends on #133` note in the PR description. Once #133 merges upstream the
   duplication resolves via rebase.
4. **E2E reframing** — The plan called for Docker-MQTT scenarios using the
   bootstrap E2E shim that ships with `feat/issue-133`. The unit-integration
   wire-shape pins exercise the same DTO serialization path the MQTT publisher
   uses; live MQTT capture is a follow-up after #133 lands
   (`phase-05-e2e-validation.md`).

## Follow-ups

- After #133 merges upstream: drop the duplicated test-project scaffolding
  commits during the close-out rebase, and lift the `JsonHeaderWireShapeE2ETests`
  cases into the IntegrationTests project with mosquitto fixtures.
- The `extra-files.user/plans/11-tests/11-compliance-regression-gates.md`
  migration row for #130 + #131 is a main-agent task tracked in `todo.md`;
  not a per-issue PR commit.

## Pre-close

```
git status   # clean
dotnet build libraries/MTConnect.NET-JSON-cppagent/MTConnect.NET-JSON-cppagent.csproj -c Debug -f net8.0
dotnet test  tests/MTConnect.NET-JSON-cppagent-Tests/MTConnect.NET-JSON-cppagent-Tests.csproj -c Debug --nologo
```

## §1.5 close-out

Not run by the implementing subagent. The user reviews the draft PR
(https://github.com/TrakHound/MTConnect.NET/pull/147) and decides when to
flip it from draft to ready, including the history rewrite + reviewer
assignment per CONVENTIONS §1.5.
