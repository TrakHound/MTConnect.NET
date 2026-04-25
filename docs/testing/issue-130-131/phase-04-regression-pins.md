# Phase 4 — Regression pins

## Executed

Authored `tests/MTConnect.NET-JSON-cppagent-Tests/CppAgentHeaderFieldsRegressionGuardTests.cs`.
The fixture is a reflection guard that loops over every JSON-cppagent header DTO type
(`JsonStreamsHeader`, `JsonDevicesHeader`, `JsonAssetsHeader`) and asserts:

- `SchemaVersion` is declared as a public `string` property carrying
  `[JsonPropertyName("schemaVersion")]`.
- `TestIndicator` is declared as a public `bool` property carrying
  `[JsonPropertyName("testIndicator")]`.

If a future refactor introduces a new cppagent header DTO and forgets one of the
two fields, the guard fails fast on every CI run — not when an MQTT capture is
manually inspected. If a future refactor renames the JSON property (`schemaVersion`
to `schema_version`, say), the guard fails too — locking in cppagent v2 wire shape.

## Validation

`dotnet test tests/MTConnect.NET-JSON-cppagent-Tests/MTConnect.NET-JSON-cppagent-Tests.csproj -c Debug --nologo` —
`Passed: 24, Failed: 0`. Six guard cases (3 DTOs * 2 fields) plus the 18 fixture
cases from P2.

## Coverage

The guard test exercises pure reflection over the DTO types; it adds no new
production code, so coverage on touched files is unchanged from P3 (still 100%).

## Deviations from plan

The plan called for a separate `tests/Compliance/MTConnect-Compliance-Tests/L5_Regressions/`
home for the regression fixture. That directory does not exist on `upstream/master`
(it ships with `feat/issue-133`'s compliance harness). The fixture lands inside the
already-scaffolded `tests/MTConnect.NET-JSON-cppagent-Tests/` test project as the
nearest equivalent — co-located with the per-envelope unit tests it complements.

The plan also referenced removing `#130` from a migration table at
`plans/11-tests/11-compliance-regression-gates.md`. That file lives under
`extra-files.user/`, which is gitignored and does not appear inside the worktree
per CONVENTIONS §1.0. Plan-level migration is a main-agent task tracked in `todo.md`,
not a per-issue PR commit.

## Follow-ups

- None — regression pin landed on the only test project available at branch cut.
