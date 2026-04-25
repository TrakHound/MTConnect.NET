# Phase 2 — Red tests

## Executed

- Scaffolded NUnit 3.13.3 test projects for both JSON formatters (the bootstrap PR has not landed upstream, so the paired test projects do not yet exist on the branch-cut tree):
  - `tests/MTConnect.NET-JSON-cppagent-Tests/`
  - `tests/MTConnect.NET-JSON-Tests/`
- Added the projects to `MTConnect.NET.sln`.
- Authored `Devices/JsonDataItemEmptyNameOmissionTests.cs` in each test project covering four scenarios:
  1. Source `Name = null` → `name` key absent. (Already passes — `JsonIgnoreCondition.WhenWritingDefault` drops `null`.)
  2. Source `Name = string.Empty` → `name` key absent. (Red — current code emits `"name": ""`.)
  3. Source `Name = "temp"` → `name` key present with value `temp`. (Already passes.)
  4. Typed `TemperatureDataItem` with `Name = string.Empty` cleared from the type-default `"temp"` → `name` key absent. (Red.)
- Each test fixture carries a §15 source-reference comment block citing the XSD (`MTConnectDevices_2.5.xsd`, `DataItem/@name` `use="optional"`), the in-tree XML reference shape (`XmlDataItem.cs:198`), and the public defect tracker URL.
- NUnit category `DataItemNameOmissionWhenUnsetOrEmpty` is descriptive (per §14: a category that names what the test asserts rather than the bookkeeping number; the public issue URL lives in the fixture comment block).

## Validation (red)

- `dotnet build tests/MTConnect.NET-JSON-cppagent-Tests/MTConnect.NET-JSON-cppagent-Tests.csproj -c Debug` → 0 errors.
- `dotnet test tests/MTConnect.NET-JSON-cppagent-Tests/MTConnect.NET-JSON-cppagent-Tests.csproj -c Debug --no-build` → **2 failed, 2 passed** (empty + typed-cleared cases red).
- `dotnet build tests/MTConnect.NET-JSON-Tests/MTConnect.NET-JSON-Tests.csproj -c Debug` → 0 errors.
- `dotnet test tests/MTConnect.NET-JSON-Tests/MTConnect.NET-JSON-Tests.csproj -c Debug --no-build` → **2 failed, 2 passed** (same shape).

The reds fail for the right reason: the source `Name = ""` flows through the unconditional constructor copy and is emitted because `""` is not the type default (which is `null`).

## Metrics delta

- New NUnit test projects: 2.
- New test files: 2.
- New `[Test]` cases: 8 (4 per formatter), of which 4 are red on HEAD.

## Deviations from plan

- The plan named NUnit category `Issue138Red`. Replaced with `DataItemNameOmissionWhenUnsetOrEmpty` per CONVENTIONS §14 (descriptive labels rather than bookkeeping-number labels; the public issue URL lives in the fixture's source-reference block).
- The plan named a separate `ci(tests): add issue-138-red confirmation job` commit. Skipped per CONVENTIONS §1.7 ("Per-issue PRs do NOT modify `.github/workflows/`"). Red confirmation runs locally via `dotnet test` against the worktree tip prior to the green commit; reviewers can re-run the same command on this PR's parent commit (the test commit lands red).

## Follow-ups

- None.
