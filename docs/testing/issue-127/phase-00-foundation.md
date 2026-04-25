# Phase 00 — Foundation

## Executed

- Cut `fix/issue-127` from `upstream/master` per the branch-naming rule
  (issue-NNN identifier, no descriptive slug).
- Authored `docs/testing/issue-127.md` skeleton with placeholders for each
  later phase.
- Authored the draft PR body skeleton at
  `extra-files.user/plans/03-issue-127-header-version/pr-body.md`.

## Metrics delta

No production code touched. Documentation footprint:

- `docs/testing/issue-127.md` (new) — 49 lines, skeleton only.
- `docs/testing/issue-127/phase-00-foundation.md` (this file).

## Deviations from plan

- The `00-bootstrap/` plan has not landed on `upstream/master`. The plan's
  P0 specifies `./tools/test.sh` and `tests/coverlet.runsettings` as
  prerequisites; in their absence the validation gate in this branch falls
  back to direct `dotnet test` invocations against the relevant test
  project. Coverage measurement uses Coverlet's default collector
  (already wired into the existing `MTConnect.NET-Common-Tests` csproj).
- No `.github/workflows/` edits — per CONVENTIONS §1.7, per-issue PR
  branches do not modify CI; that work is owned by `00-bootstrap/` /
  `11-tests/`.

## Follow-ups

None for this phase.

## Validation

- `git status` clean after the foundation commit (only the new
  `docs/testing/issue-127*` files staged).
- Build green on the relevant projects via
  `dotnet build libraries/MTConnect.NET-Common/MTConnect.NET-Common.csproj
  -c Release -f net8.0` (no production change yet).
