# Phase 2 — Red tests

## Executed

Added `tests/MTConnect.NET-Common-Tests/Devices/OrganizersSystemsTests.cs` —
58 NUnit cases tagged `[Category("OrganizersSystemsSubstitutionGroup")]`,
sourced from `KnownSystemMembers` / `AutoWrappedSystemMemberTypes` /
`EqualDepthPeerPairs`:

| Asserted behaviour                                                                                | Cases | Outcome on HEAD |
|---------------------------------------------------------------------------------------------------|-------|-----------------|
| Every System substitution-group member is listed in `Organizers.Systems`.                         | 18    | 5 RED, 13 PASS  |
| `Organizers.GetOrganizerType` resolves every System member to `Systems`.                          | 18    | 5 RED, 13 PASS  |
| Every auto-wrapped System member lands under the `<Systems>` organizer (Controller excluded).     | 17    | 5 RED, 12 PASS  |
| Peer System pairs sit at equal tree depth after `Device.AddComponent()`.                          | 5     | 3 RED, 2 PASS   |

Red causes (cases failing for the right reason — missing list members):

- `AirHandler`, `Cooling`, `Heating`, `Pressure`, `Vacuum` are not in
  `Organizers.Systems` on HEAD; the assertion message names each member and
  cites the SysML-source rationale.
- The peer-depth fail cases are exactly the pairs the issue calls out
  (`Heating` vs `Protective`, `Heating` vs `Cooling`, `AirHandler` vs `Enclosure`).
- The remaining peer-depth cases (`Electric` vs `Hydraulic`, `Pressure` vs
  `Vacuum`) — `Pressure` vs `Vacuum` actually fails today because both members
  are missing; `Electric` vs `Hydraulic` passes because both already in list.
- The auto-wrap and GetOrganizerType variants fail with the same root cause for
  each missing member.

## Category naming

Per CONVENTIONS §14, the category is named `OrganizersSystemsSubstitutionGroup`
(describes what the tests assert) rather than `Issue134Red` (a bookkeeping
label). The plan-file's drafted `Issue134Red` would leak the internal plan
number into a public artefact and is rewritten here.

## CI job — not added

Per CONVENTIONS §1.7, per-issue PRs do not modify `.github/workflows/`.
Red-state confirmation runs locally:

```bash
dotnet test tests/MTConnect.NET-Common-Tests/MTConnect.NET-Common-Tests.csproj \
    -c Debug --filter 'TestCategory=OrganizersSystemsSubstitutionGroup'
```

The plan-file's drafted P2 commit `ci(tests): add issue-134-red confirmation
job` is dropped.

## Validation

- `dotnet build -c Debug` green on net8.0.
- `dotnet test --filter 'TestCategory=OrganizersSystemsSubstitutionGroup'`:
  `Failed: 18, Passed: 40, Total: 58` — every failure is for the SysML-stated
  reason (missing list member or asymmetric tree depth), no NRE / fixture bug.
- Tests outside the new category remain green.

## Deviations from plan

- Category renamed `Issue134Red` → `OrganizersSystemsSubstitutionGroup`
  (CONVENTIONS §14).
- CI-job commit dropped (CONVENTIONS §1.7).
- The plan-file P2 commits 2 + 3 reduce to one combined `test(common-tests)`
  commit because the CI job was dropped and the docs of the red-test matrix go
  inline in this writeup rather than as a separate `docs(issue-134)` commit.

## DoD

Reds live and failing for the right reason; existing tests untouched.
