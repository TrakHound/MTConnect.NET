# Phase 0 — Foundation

## Executed

- Seeded `docs/testing/issue-138.md` with the section skeleton (Defect + scope; per-phase placeholders).
- Created `docs/testing/issue-138/` for per-phase writeups.
- Confirmed the defect persists at branch-cut SHA `3d6321ab`:
  - `libraries/MTConnect.NET-JSON-cppagent/Devices/JsonDataItem.cs:87` — `Name = dataItem.Name;` unconditional copy.
  - `libraries/MTConnect.NET-JSON/Devices/JsonDataItem.cs:87` — same pattern in the base JSON formatter.
  - `libraries/MTConnect.NET-XML/Devices/XmlDataItem.cs:198` — already guards with `IsNullOrEmpty`; reference shape for the fix.
- `dotnet build libraries/MTConnect.NET-JSON-cppagent/MTConnect.NET-JSON-cppagent.csproj -c Debug` green (20 pre-existing warnings, 0 errors).

## Metrics delta

No production code touched in this phase; build / test counts unchanged.

## Deviations from plan

The branch was cut from `upstream/master` (SHA `3d6321ab`); the bootstrap test infrastructure (shared `tests/coverlet.runsettings`, `tools/test.sh`, scaffolded JSON-cppagent test project) is **not** present at this base. Per the plan briefing, the coverage gate runs against the inline workflow once the test project is scaffolded later in this plan; the deviation is recorded for the campaign-level tracker.

## Follow-ups

- None at P0 — skeleton only.
