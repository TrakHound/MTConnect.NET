# Phase 00 — Foundation

## Executed

- Branch `fix/issue-129` cut from `upstream/master` at SHA `3d6321ab`
  ("Updated ReadMe").
- Worktree at `.claude/worktrees/fix-issue-129/`.
- Seeded `docs/testing/issue-129.md` with the defect summary + spec
  references and the per-phase section skeleton.
- Created `docs/testing/issue-129/` directory for per-phase writeups;
  this phase writeup is the first entry.

## Bootstrap-dependency note

The plan's `01-foundation.md` references tooling expected to land via the
upstream bootstrap PR (`tests/coverlet.runsettings`, `tools/test.sh`,
fixed `.github/workflows/dotnet.yml`). At branch-cut time those artefacts
are not on `upstream/master`. Per CONVENTIONS §1.5a (concurrent plan
execution by default), this plan proceeds in parallel and uses
`dotnet test` / `dotnet build` directly for its own validation gate,
rebasing on the bootstrap PR once it merges.

## Validation

- `git status` clean.
- No production code changed in this commit; existing build is unaffected.

## Deviations from plan

None. The bootstrap-dependency note above is a CONVENTIONS §1.5a-allowed
parallel execution, not a scope deviation.

## Follow-ups

None at this phase.
