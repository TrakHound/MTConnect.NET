# Phase 3 — Library fix

## Executed

Four atomic commits, each scoped per CONVENTIONS §5.3:

1. `enh(common): add SchemaVersion to header DTOs` — adds `SchemaVersion { get; }` to `IMTConnectStreamsHeader`, `IMTConnectDevicesHeader`, `IMTConnectAssetsHeader`, plus `SchemaVersion { get; set; }` to the three concrete `MTConnect*Header` classes. Strictly additive on the public surface (no break for existing consumers — none of the in-tree types implement these interfaces directly except the matching concrete classes).
2. `fix(json-cppagent): emit schemaVersion on streams header` — declares `[JsonPropertyName("schemaVersion")] public string SchemaVersion { get; set; }` on `JsonStreamsHeader`, copies the field in the `IMTConnectStreamsHeader` ctor, restores it in `ToStreamsHeader()`.
3. `fix(json-cppagent): emit schemaVersion on devices header` — `JsonDevicesHeader` already declared the property; the fix wires the missing `SchemaVersion = header.SchemaVersion` in the ctor and `header.SchemaVersion = SchemaVersion` in `ToDevicesHeader()`.
4. `fix(json-cppagent): emit schemaVersion on assets header` — symmetric edit on `JsonAssetsHeader`.

## Validation

- `dotnet build tests/MTConnect.NET-JSON-cppagent-Tests/MTConnect.NET-JSON-cppagent-Tests.csproj -c Debug` — build succeeded with 0 errors and 2 warnings (both are sourcelink "no source control information" diagnostics emitted by `Microsoft.SourceLink.Common.targets`; they pre-date this PR).
- `dotnet test tests/MTConnect.NET-JSON-cppagent-Tests/MTConnect.NET-JSON-cppagent-Tests.csproj -c Debug --no-build` — `Passed: 18, Failed: 0, Skipped: 0`. All P2 fixtures green.

## Coverage

The four production files modified by this phase:

- `libraries/MTConnect.NET-Common/Headers/IMTConnectStreamsHeader.cs` (interface — no executable code).
- `libraries/MTConnect.NET-Common/Headers/IMTConnectDevicesHeader.cs` (interface — no executable code).
- `libraries/MTConnect.NET-Common/Headers/IMTConnectAssestsHeader.cs` (interface — no executable code).
- `libraries/MTConnect.NET-Common/Headers/MTConnectStreamsHeader.cs` (concrete property — auto-implemented `get; set;`).
- `libraries/MTConnect.NET-Common/Headers/MTConnectDevicesHeader.cs` (same).
- `libraries/MTConnect.NET-Common/Headers/MTConnectAssestsHeader.cs` (same).
- `libraries/MTConnect.NET-JSON-cppagent/Streams/JsonStreamsHeader.cs` — every executable line on the new `SchemaVersion` surface is exercised: ctor with non-null source (`Constructor_with_source_header_copies_schemaVersion`), ctor with null source (`Constructor_with_null_source_does_not_throw`), default ctor (`Default_constructor_leaves_schemaVersion_unset`), and `ToStreamsHeader()` (`Reverse_mapping_round_trips_schemaVersion`).
- `libraries/MTConnect.NET-JSON-cppagent/Devices/JsonDevicesHeader.cs` — same matrix.
- `libraries/MTConnect.NET-JSON-cppagent/Assets/JsonAssetsHeader.cs` — same matrix.

The interface declarations and auto-properties have no branchable code; the constructor / mapper edits are the only branching surface and every branch is covered by the P2 fixtures.

## Deviations from plan

- The plan's P3 file expected only `fix(json-cppagent)` commits. The widening described in P0 deviation §2 required a preceding `enh(common)` commit on the source DTOs.
- The plan wanted to remove a "category + CI job" at P3. No category-removed step is needed — the `CppAgentHeaderFieldsPresent` category stays on the test fixtures as a regression-pin tag, and no inverted-exit-code CI job was added in P2 (per CONVENTIONS §1.7, per-issue PRs do not modify `.github/workflows/`).

## Follow-ups

- None — production fix complete; all P2 reds are now green.
