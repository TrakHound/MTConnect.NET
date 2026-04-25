# Phase 0 — Foundation

## Executed

- Cut branch `fix/issue-134` from `upstream/master` (3d6321ab).
- Seeded `docs/testing/issue-134.md` skeleton plus this writeup folder.
- Drafted the draft-PR description skeleton (per CONVENTIONS §1.3 / §1.8).

## Validation

- `dotnet build tests/MTConnect.NET-Common-Tests/MTConnect.NET-Common-Tests.csproj -c Debug` green on net8.0.
- `dotnet test tests/MTConnect.NET-Common-Tests/MTConnect.NET-Common-Tests.csproj -c Debug` green: `Passed: 1, Failed: 0`.
- `git status` clean post-commit.

## Bootstrap dependency

The cross-cutting test infrastructure (shared coverlet runsettings, the
cross-OS `tools/test.{sh,ps1}` harness, the CI workflow rewrite) is part of
a separate in-flight branch and has not yet merged on `upstream/master`. This
phase therefore consumes the `tests/MTConnect.NET-Common-Tests` project as it
exists on `upstream/master` and falls back to direct `dotnet test` invocations
for local validation.

## Deviations from plan

- Phase 0 commit subject uses scope `testing` per CONVENTIONS §5.3 / §5.4 rule 5
  rather than an issue-bookkeeping scope.

## Follow-ups

None at this phase.
