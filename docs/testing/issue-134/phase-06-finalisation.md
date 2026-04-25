# Phase 6 — Finalisation

## Campaign summary

- **Issue**: TrakHound/MTConnect.NET#134 — `Organizers.Systems` had drifted
  from the `System` substitution-group declared by the MTConnect SysML model;
  `Device.AddComponent()` left missing-from-list System peers
  (`Heating`, `Cooling`, `Pressure`, `Vacuum`, `AirHandler`) at the device
  root rather than auto-wrapping them under `<Systems>`, producing
  asymmetric tree depth between peer System components.

- **Fix** (`libraries/MTConnect.NET-Common/Devices/Organizers.cs`): five
  members added to the `_systems` initializer; list sorted alphabetically
  by `Component.TypeId`; SysML source URL cited inline. Final list (18
  members): `AirHandler`, `Controller`, `Coolant`, `Cooling`, `Dielectric`,
  `Electric`, `Enclosure`, `EndEffector`, `Feeder`, `Heating`, `Hydraulic`,
  `Lubrication`, `Pneumatic`, `Pressure`, `ProcessPower`, `Protective`,
  `Vacuum`, `WorkEnvelope`.

- **Tests** (`tests/MTConnect.NET-Common-Tests/Devices/`):
  - `OrganizersSystemsTests` — 60 parametric red-then-green cases covering
    membership, GetOrganizerType resolution, auto-wrap behaviour, and
    peer-depth symmetry under the
    `OrganizersSystemsSubstitutionGroup` category.
  - `OrganizersAccessorsTests` — 22 cases covering every other organizer
    accessor and every branch of `GetOrganizerType` so `Organizers.cs`
    reaches 100% coverage.
  - `OrganizersSystemsRegressionTests` — literal pin + reflection-based
    detector under the `OrganizersSystemsRegressionGuard` category;
    catches future regenerations that add or rename a System member
    without updating `_systems`.
  - `OrganizersSystemsEndToEndTests` — programmatic E2E exercises of
    `Device.AddComponent()` against the full set of System peers.

- **Coverage**: `Organizers.cs` at 100% line + 100% branch + 100% method.

- **Test count delta**: existing 1 test → 86 tests on this branch.

- **Public API impact**: none. `Organizers.Systems`'s static type
  (`IEnumerable<string>`) is unchanged; only the contents grow.
  `Device.AddComponent()`'s observable behaviour changes for the five
  newly-listed members (they auto-wrap under `<Systems>` instead of
  staying at the device root) — that change is the bug fix.

- **Compliance impact**: the auto-wrap now matches the wire shape declared
  by `MTConnectDevices_*.xsd` (`<xs:element name="Systems" ...>`) for every
  System substitution-group member, where it previously matched the XSD for
  13 of 18 members. v2.6 / v2.7 coverage extension lands when
  the v2.6 / v2.7 SysML support lands and this branch rebases on top of it.

## DoD cross-check

- [x] P0 foundation — branch + draft PR + skeleton.
- [x] P1 defect scoping — diff + strategy decision.
- [x] P2 red tests — 18 RED-on-HEAD assertions for the right reason.
- [x] P3 library fix — `_systems` updated; reds → green.
- [x] P4 regression pin — literal pin + reflection guard.
- [x] P5 E2E validation — programmatic E2E green.
- [x] Coverage 100% on touched production file.
- [x] Source citations on every spec-asserting test (CONVENTIONS §15).
- [x] American English throughout (CONVENTIONS §13).
- [x] Zero internal references in tracked artifacts (CONVENTIONS §14).

## Pre-close validation

- `dotnet build tests/MTConnect.NET-Common-Tests/MTConnect.NET-Common-Tests.csproj -c Debug` green.
- `dotnet test tests/MTConnect.NET-Common-Tests/MTConnect.NET-Common-Tests.csproj -c Debug`: `Failed: 0, Passed: 86`.
- `git status` clean.
- Every commit pushed to `origin/fix/issue-134`.

## Close-out

Per the dispatch instruction "Stop at end of phase 07", the §1.5 close-out
(`git rebase -i upstream/master`, `gh pr ready`, `gh pr edit --add-reviewer`)
is left for the user / main agent to run when the cross-plan situation
warrants it (specifically the soft-dep on `feat/issue-133` is unresolved
until #133 lands and this branch rebases on top of the v2.6 / v2.7 SysML
regen).

## Aggregate deviations from plan

| Plan-file claim                                                                                  | Replacement                                                                                                                                                  |
|--------------------------------------------------------------------------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Commit subjects use scope `issue-134` (e.g. `docs(issue-134)` / `test(issue-134)`)               | Use real CONVENTIONS §5.3 scopes — `testing` for `docs/testing/`, `common-tests` for `tests/MTConnect.NET-Common-Tests/`, `common` for the production file. |
| Category label `Issue134Red`                                                                     | Descriptive labels per CONVENTIONS §14: `OrganizersSystemsSubstitutionGroup`, `OrganizersAccessors`, `OrganizersSystemsRegressionGuard`, `OrganizersSystemsEndToEnd`. |
| Add `issue-134-red` CI job under `.github/workflows/`                                            | Skipped — CONVENTIONS §1.7 forbids per-issue PRs from touching `.github/workflows/`. Red-state runs locally with the category filter.                      |
| Regression tests under `tests/Compliance/MTConnect-Compliance-Tests/L5_Regressions/`             | Land under `tests/MTConnect.NET-Common-Tests/Devices/` instead — the compliance project does not exist on `upstream/master`.                                |
| HTTP / MQTT-relay-Docker / cppagent-parity-Docker E2E scenarios                                  | Programmatic-construction E2E in `MTConnect.NET-Common-Tests` — required Docker / harness infrastructure not on `upstream/master`.                          |
| Cross-reference a future compliance-regression catalogue                                       | Skipped — that catalogue lands when the compliance harness lands; cross-reference at that time.                                                              |

Each deviation is justified inline in the corresponding phase writeup.
