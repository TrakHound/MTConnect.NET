# Phase 4 — Regression pins

## Executed

`tests/MTConnect.NET-Common-Tests/Devices/OrganizersSystemsRegressionTests.cs`
adds two regression guards under
`[Category("OrganizersSystemsRegressionGuard")]`:

1. **`Organizers_Systems_matches_pinned_System_substitution_group`** — equates
   `Organizers.Systems` to a literal pinned list of every `TypeId` that this
   branch validates as a System member. Drift in either direction
   (added/removed/renamed) breaks the test, forcing the maintainer of a
   regeneration to update both sides together.

2. **`Every_System_described_Component_subclass_is_in_Organizers_Systems`** —
   walks the `MTConnect.NET-Common` assembly via reflection, finds every
   non-abstract `IComponent` whose generated `DescriptionText` starts with
   "System that " or "System composed " (the SysML descriptive convention for
   `System` substitution-group members), and asserts the corresponding `TypeId`
   appears in `Organizers.Systems`. Catches regeneration that adds a System
   member without bumping `_systems`.

## Source citations

Each test class header cites:

- The SysML model URL (XMI): https://github.com/mtconnect/mtconnect_sysml_model.
- The Devices Information Model prose:
  https://docs.mtconnect.org/Part_3.0_DevicesInformationModel.

per CONVENTIONS §15.

## Project location

The plan's draft P4 placed the regression at
`tests/Compliance/MTConnect-Compliance-Tests/L5_Regressions/Issue134_OrganizersSystemsUnionTests.cs`,
but that test project does not exist on `upstream/master` — it is part of
the `00-bootstrap` / compliance-harness deliverable folded into
`feat/issue-133`. To stay independent of bootstrap landing, the regression
guard ships in `tests/MTConnect.NET-Common-Tests/Devices/`, paired with the
production code it pins. When the compliance harness lands, this test can
move there as part of the tests-overhaul plan (`11-tests/`).

## Test results

- `dotnet test`: `Failed: 0, Passed: 83`.
- `Organizers.cs` coverage remains at 100% line + 100% branch + 100% method.

## Deviations from plan

- Regression file path moved from a compliance-tests folder
  (which doesn't exist on `upstream/master` yet) to the existing
  `MTConnect.NET-Common-Tests` project.
- Two regression tests instead of three: the literal pin is strictly stronger
  than a per-member parametric variant because it catches both addition and
  removal in one assertion, so the parametric variant is redundant.

## DoD

Regression guard live; coverage holds at 100%; test green.
