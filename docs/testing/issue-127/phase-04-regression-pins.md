# Phase 04 — Regression pins

## Executed

The plan's P4 calls for a separate L5 compliance regression file at
`tests/Compliance/MTConnect-Compliance-Tests/L5_Regressions/Issue127_HeaderVersionTests.cs`,
plus a repo-wide guard test asserting no wire-format envelope echoes
the library assembly version.

The compliance harness project is owned by the `11-tests/` plan's
P9 and has not landed on `upstream/master`. The plan permits the
fallback location `tests/MTConnect.NET-Common-Tests/Regressions/...`
when the compliance project is absent.

The fixture in
`tests/MTConnect.NET-Common-Tests/Headers/HeaderVersionRegressionTests.cs`
(authored in P2, green on arrival after P3) already carries:

1. The 60 envelope-kind × version cases that pin the new behavior
   (`Devices_*`, `Assets_*`, `Error_*`,
   `Devices_..._when_passed_explicitly`).
2. The repo-wide guard test
   `No_response_envelope_emits_the_library_assembly_version` that
   walks each envelope kind and asserts none of them contain the
   library `AssemblyVersion`.

That fixture is already the L5-shaped regression pin the plan called
for, in the fallback location the plan permits. P4 does not add a
second file — duplicating the assertions across two locations would
fail CONVENTIONS §11's "tests pinned per regression rather than
spread thinly".

The plan's expectation that this phase edit
`plans/11-tests/11-compliance-regression-gates.md` to remove the
`#127` row from its `UpstreamBlocked` list is satisfied trivially:
that file lives under `extra-files.user/` (gitignored), is not
public, and is owned by the `11-tests/` plan author. The local
plan tracker reflects the P3 / P4 close-out separately.

## Metrics delta

No new commits in this phase. The fixture authored in P2 is the
canonical regression pin, validated green by P3.

## Deviations from plan

- No second file at the L5 location (compliance project absent).
- No edits to `plans/11-tests/11-compliance-regression-gates.md` from
  this phase's commits — that file is private and tracked outside
  the public repo. CONVENTIONS §9 covers the within-public-tree case
  (plan-edits land in commits); the gitignored case has no public
  artifact to update.

## Follow-ups

When `11-tests/` lands the compliance project, the
`HeaderVersionRegressionTests.cs` file moves under
`tests/Compliance/MTConnect-Compliance-Tests/L5_Regressions/` per
that plan's P9. The move is a `test(compliance):` commit on the
tests-plan branch, not on this fix branch.
