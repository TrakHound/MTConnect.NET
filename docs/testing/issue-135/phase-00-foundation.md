# Phase 00 — foundation

## Executed

- Cut branch `fix/issue-135` from `upstream/master` at `3d6321ab Updated ReadMe`.
- Seeded `docs/testing/issue-135.md` with the writeup index and
  per-phase placeholders.
- Created `docs/testing/issue-135/` for per-phase writeups.

## Bootstrap dependency

The plan author intended to consume bootstrap outputs (shared
`tests/coverlet.runsettings`, `tools/test.sh`, scaffolded test
projects) that have not yet landed on `upstream/master`. This branch
proceeds without those outputs:

- Validation runs `dotnet build` and `dotnet test` directly against
  the touched projects rather than `tools/test.sh`.
- Coverage is measured locally with `coverlet.collector` (already
  present in every existing test project's csproj) and
  `dotnet test --collect:"XPlat Code Coverage"`.
- The new `tests/MTConnect.NET-AgentModule-MqttRelay-Tests/` project
  is created as part of this branch (P2) rather than consumed from
  bootstrap.

## Metrics delta

None at this phase (documentation only).

## Deviations from plan

- The plan's `01-foundation.md` cites a `docs(issue-135): seed
  writeup skeleton` commit subject. `issue-135` is not a recognised
  scope under CONVENTIONS.md §5.3; the correct documentation scope
  for `docs/testing/<plan>/` is `testing`. This phase commits as
  `docs(testing): seed issue-135 writeup skeleton`.
- The plan assumed bootstrap had landed. Since it has not, the
  branch creates the paired test project itself in P2 instead of
  consuming a bootstrap-scaffolded stub.

## Follow-ups

- None.
