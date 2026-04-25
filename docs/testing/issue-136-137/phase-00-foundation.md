# Phase 00 — Foundation

## Executed

- Cut branch `fix/issue-136-137` from `upstream/master` at SHA
  `3d6321ab` (the upstream HEAD at branch-cut time).
- Seeded the campaign writeup skeleton at
  `docs/testing/issue-136-137.md` plus the per-phase folder
  `docs/testing/issue-136-137/`.
- Confirmed the production surfaces the campaign will modify:
  - `libraries/MTConnect.NET-Common/Devices/Device.cs` — default
    constructor assigns `Id`, `Name`, and `Uuid`.
  - `libraries/MTConnect.NET-Common/Devices/Components/*.g.cs` — 115
    generated component classes whose default constructor assigns
    `Name = NameId`.
  - `build/MTConnect.NET-SysML-Import/CSharp/Templates/Devices.ComponentType.scriban`
    — the Scriban template that emits the `Name = NameId` line into
    every generated component.
- Confirmed the test infrastructure consumed by the campaign exists on
  `upstream/master`:
  - `tests/MTConnect.NET-Common-Tests/MTConnect.NET-Common-Tests.csproj`
    — NUnit 3.13.2, references
    `libraries/MTConnect.NET-Common/MTConnect.NET-Common.csproj`.
- Opened a draft pull request against `TrakHound/MTConnect.NET` from
  `ottobolyos:fix/issue-136-137`.

## Metrics delta

- Files added: `docs/testing/issue-136-137.md`,
  `docs/testing/issue-136-137/phase-00-foundation.md`,
  `extra-files.user/plans/09-issue-136-137-device-component-ctor-defaults/pr-body.md`
  (the last is gitignored under `extra-files.user/`; not in the commit).
- No production code touched in P0.

## Deviations from plan

- The plan's `01-foundation.md` §"Validation" assumes the bootstrap
  prelude PR has landed, exposing `tools/test.sh` and the per-project
  paired test scaffolding. Bootstrap has not landed upstream, but the
  surfaces this campaign needs (`tests/MTConnect.NET-Common-Tests/`)
  exist on `upstream/master` directly, so the campaign cuts from
  `upstream/master` per the dispatch envelope's explicit guard-rail.
- The plan's first commit subject was `docs(issue-136-137): seed
  writeup skeleton`. That scope is invented; the §5.3 scope for files
  under `docs/testing/<plan>/` is `testing`. This phase commits as
  `docs(testing): seed issue-136-137 writeup skeleton` instead. Same
  pattern applied across every other commit in the campaign whose
  plan-file subject used an invented scope.

## Follow-ups

None for this phase.
