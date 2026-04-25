# Phase 0 — Foundation

## Branch + draft PR

- Branch: `fix/issue-132`, cut from `upstream/master` at HEAD `3d6321ab`.
- Draft PR opened on `TrakHound/MTConnect.NET` from
  `ottobolyos:fix/issue-132`.

## Bootstrap dependency

This phase originally required bootstrap deliverables
(`tests/coverlet.runsettings`, `tools/test.sh`, `.github/workflows/dotnet.yml`
rewrite). Bootstrap has not landed upstream. The narrowed pragmatic scope
of this PR uses only what already exists on `upstream/master`:

- `tests/MTConnect.NET-Common-Tests/` — paired test project, NUnit,
  references `MTConnect.NET-Common`.

The pragmatic fix does not need any of the bootstrap-specific harness;
the existing `dotnet test` invocation suffices.

## Skeleton seeded

- `docs/testing/issue-132.md` — campaign index.
- `docs/testing/issue-132/` — per-phase writeups folder.
- `docs/testing/issue-132/phase-00-foundation.md` — this file.

## DoD

- Skeleton committed.
- Bootstrap dependency narrowed to "uses existing
  `MTConnect.NET-Common-Tests/` only — no bootstrap deliverables
  consumed."
