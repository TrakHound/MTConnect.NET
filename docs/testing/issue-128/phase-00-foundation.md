# Phase 00 — Foundation

## Branch

Cut from `upstream/master` at HEAD `3d6321ab`. Branch: `fix/issue-128`.

## Bootstrap dependency

This plan ordinarily depends on the bootstrap deliverables that live on
`feat/issue-133` (paired test project, `tools/test.sh`, coverlet
runsettings). At the time of this dispatch `feat/issue-133` had not yet
merged upstream, so the cut-point is `upstream/master`.

Per CONVENTIONS §17.8 (row 2026-04-25, "Silent scope expansion"), the
paired test project `tests/MTConnect.NET-JSON-cppagent-Tests/` is
scaffolded on this branch as a sanctioned workaround. The scaffolding
mirrors the structure C-138 (PR #140) and C-135 (PR #142) used. When
`feat/issue-133` merges upstream this PR rebases and the scaffolding
commit will be dropped during §1.5 history rewrite.

## Skeleton commit

`docs/testing/issue-128.md` skeleton + `docs/testing/issue-128/`
phase-writeup folder seeded.

## Validation

- Worktree at `.claude/worktrees/fix-issue-128/`.
- `git status` clean after first commit.
- Draft PR opened against `TrakHound/MTConnect.NET` master.

## Deviations from plan

The plan's `01-foundation.md` calls for `./tools/test.sh` as a validation
step. That script lives on `feat/issue-133` and is not present on
`upstream/master`; this phase substitutes `dotnet build` + `dotnet test`
for the equivalent gate, scoped to the JSON-cppagent + paired test
project.
