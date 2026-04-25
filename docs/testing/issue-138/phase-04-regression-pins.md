# Phase 4 — Regression pins

## Executed

### 1. XML parity pin

`tests/MTConnect.NET-XML-Tests/Devices/XmlDataItemEmptyNameOmissionTests.cs` — three tests confirm `XmlDataItem.WriteXml` already omits the `name` attribute when source `Name` is null or empty, and emits it when set. This is the parity pin called out in the plan: it locks the reference shape that the JSON formatters now mirror, so a future regression on the XML side would surface alongside any fresh JSON regression.

### 2. Source-grep guard

`tests/MTConnect.NET-JSON-cppagent-Tests/Devices/JsonDataItemSourceGuardTests.cs` — single test that walks up from `TestContext.CurrentContext.TestDirectory` to find `MTConnect.NET.sln`, then reads each watched JSON formatter source file and asserts via regex that no line matches the unguarded `^\s*Name\s*=\s*dataItem\.Name\s*;\s*$` pattern. Watched files:

- `libraries/MTConnect.NET-JSON-cppagent/Devices/JsonDataItem.cs`
- `libraries/MTConnect.NET-JSON/Devices/JsonDataItem.cs`

Future contributors who reintroduce the unconditional copy on either file (e.g. a generator-output regen that loses the guard) hit a fail-fast unit-test.

### 3. Existing P2 tests act as the spec-level regression pin

The four P2 fixtures (`JsonDataItemEmptyNameOmissionTests` in both new test projects) now read as regression pins: each carries a §15 source-reference comment block citing the XSD `use="optional"` declaration on `DataItem/@name`, and each asserts the spec-conformant wire shape against the in-memory `JsonDataItem` constructor. No separate copy of the same assertions in a `L5_Regressions/` location was added — the compliance test project does not exist on the branch-cut tree (it lands with the tests plan), and duplicating the fixtures locally would create test-suite drift.

## Validation (green)

- `dotnet test tests/MTConnect.NET-JSON-cppagent-Tests/MTConnect.NET-JSON-cppagent-Tests.csproj -c Debug` → 5 passed (4 omission + 1 source guard).
- `dotnet test tests/MTConnect.NET-JSON-Tests/MTConnect.NET-JSON-Tests.csproj -c Debug` → 4 passed.
- `dotnet test tests/MTConnect.NET-XML-Tests/MTConnect.NET-XML-Tests.csproj -c Debug` → all green, with the 3 new XML parity tests added.

## Metrics delta

- New test files: 2 (`JsonDataItemSourceGuardTests.cs`, `XmlDataItemEmptyNameOmissionTests.cs`).
- New `[Test]` cases: 4 (1 source-grep guard + 3 XML parity).

## Deviations from plan

- The plan called for placing the regression at `tests/Compliance/MTConnect-Compliance-Tests/L5_Regressions/`. That project does not exist on `upstream/master` (it lands with `11-tests/`); the plan provided an explicit "or local fallback" allowance. Used the local fallback in the existing test projects.
- The plan named NUnit category `Issue138Red` for the source-guard test. Replaced with descriptive labels per CONVENTIONS §14; the public issue URL is in the fixture comment.
- The plan called for a separate `docs(tests): extend compliance-gate plan migration table with issue-138` commit. Skipped: the migration table referenced is in `extra-files.user/plans/11-tests/` (gitignored); editing it cross-plan is allowed by §9 but cannot land in this PR's tree because the plan files are not tracked. Bridged into the campaign tracker via the report summary.

## Follow-ups

- When `11-tests/` lands the compliance project, migrate `XmlDataItemEmptyNameOmissionTests` and `JsonDataItemSourceGuardTests` into `tests/Compliance/MTConnect-Compliance-Tests/L5_Regressions/`. Remove the local copies in the same migration commit.
