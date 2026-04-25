# Phase 0 — Foundation

## Executed

- Cut branch `fix/issue-134` from `upstream/master` (3d6321ab) into a dedicated worktree.
- Seeded `docs/testing/issue-134.md` skeleton plus this writeup folder.
- Drafted PR body skeleton at `extra-files.user/plans/02-issue-134-organizers-systems-stale/pr-body.md`.

## Validation

- `dotnet build tests/MTConnect.NET-Common-Tests/MTConnect.NET-Common-Tests.csproj -c Debug` green on net8.0.
- `dotnet test tests/MTConnect.NET-Common-Tests/MTConnect.NET-Common-Tests.csproj -c Debug` green: `Passed: 1, Failed: 0`.
- `git status` clean post-commit.

## Bootstrap dependency

The plan declares a soft dependency on `00-bootstrap/` (per `01-foundation.md`),
which per `landing-coordination.md` was folded into `feat/issue-133` and has not
yet merged. This phase therefore consumes the `tests/MTConnect.NET-Common-Tests`
project as it exists on `upstream/master` rather than any bootstrap-scaffolded
test harness. Coverlet runsettings, `tools/test.sh`, and the cross-OS CI workflow
fix all live behind that bootstrap gate; this branch falls back to the local
`dotnet test` invocation for validation.

## Deviations from plan

- Phase 0 commit subject uses scope `testing` (per CONVENTIONS §5.3 / §5.4 rule 5)
  rather than the plan's drafted `docs(issue-134): ...`. The plan-file's drafted
  scope `issue-134` is not a real scope under §5.3.

## Follow-ups

None at this phase.
