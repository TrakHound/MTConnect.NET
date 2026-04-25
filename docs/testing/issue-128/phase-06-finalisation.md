# Phase 06 — Finalisation

## DoD cross-check

| Phase | Item                                                            | Status |
|-------|-----------------------------------------------------------------|--------|
| P0    | Foundation + draft PR opened                                    | Done — PR #145.   |
| P1    | Defect-scoping writeup + segment-count decision                 | Done — phase-01.  |
| P2    | Red cases (28 effective + 2 v2.0 coincidental matches)          | Done — phase-02.  |
| P3    | Fix lands; reds → green; both files derive from document.Version | Done — phase-03. |
| P4    | Regression file + hardcode-literal guard green                  | Done — phase-04.  |
| P5    | E2E scenarios green                                             | Deferred — phase-05 explains. |
| P6    | Campaign summary recorded; PR remains draft for maintainer review | Done — this writeup. |

## Pre-close verification (limited)

```
$ NUGET_PACKAGES=/tmp/nuget-fix-issue-128 \
    dotnet build libraries/MTConnect.NET-JSON-cppagent/MTConnect.NET-JSON-cppagent.csproj -c Debug
... 0 Error(s)
$ NUGET_PACKAGES=/tmp/nuget-fix-issue-128 \
    dotnet test tests/MTConnect.NET-JSON-cppagent-Tests/MTConnect.NET-JSON-cppagent-Tests.csproj -c Debug
Passed!  - Failed: 0, Passed: 63, Skipped: 0, Total: 63
$ git status
On branch fix/issue-128
nothing to commit, working tree clean
```

`./tools/test.sh` and `MTCONNECT_E2E_DOCKER=true ./tools/test.sh`
deferred until `feat/issue-133` lands (the bootstrap script doesn't
exist on `upstream/master`).

## What does NOT happen in this dispatch

Per the dispatch instructions:
- No `git rebase upstream/master` — history rewrite is the human
  reviewer's call after they read the draft PR.
- No `gh pr ready` — PR stays draft.
- No `gh pr edit --add-reviewer PatrickRitchie` — same reason.

## Closing notes

The draft PR is ready for the maintainer's review. Re-dispatched
prompts (or the human author) can flip it to ready after:
1. `feat/issue-133` lands; rebase drops the duplicated test-project
   scaffolding commit.
2. The deferred E2E commit lands on top.
3. `gh pr ready` + reviewer assignment.
