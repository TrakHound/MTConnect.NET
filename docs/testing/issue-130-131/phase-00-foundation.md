# Phase 0 — Foundation

## Executed

- Seeded `docs/testing/issue-130-131.md` with the section skeleton.
- Created `docs/testing/issue-130-131/` for per-phase writeups.
- Confirmed branch cut from `upstream/master` at SHA `3d6321ab Updated ReadMe`.
- Confirmed defect persists at branch-cut:
  - `libraries/MTConnect.NET-JSON-cppagent/Streams/JsonStreamsHeader.cs` — no `SchemaVersion` property declared.
  - `libraries/MTConnect.NET-JSON-cppagent/Assets/JsonAssetsHeader.cs` — no `SchemaVersion` property declared.
  - `libraries/MTConnect.NET-JSON-cppagent/Devices/JsonDevicesHeader.cs` — `SchemaVersion` property declared but never assigned in the `IMTConnectDevicesHeader` constructor and never copied back in `ToDevicesHeader()`.
  - Source DTOs `IMTConnectStreamsHeader` / `IMTConnectDevicesHeader` / `IMTConnectAssetsHeader` (and their implementations) do not expose `SchemaVersion` at all — the JSON-cppagent header DTOs cannot map a property that does not exist on the source surface.

## Metrics delta

No production code touched in this phase; build / test counts unchanged.

## Deviations from plan

1. Issue #131 (`testIndicator`) is **already addressed on `upstream/master`** — every JSON-cppagent header DTO declares `[JsonPropertyName("testIndicator")] public bool TestIndicator { get; set; }`, the constructor maps `TestIndicator = header.TestIndicator`, and the reverse mapper restores it. No fix required for #131; the plan's symmetric `schemaVersion + testIndicator` matrix collapses to a `schemaVersion`-only fix for the production path. Tests that pin the existing `testIndicator` shape are still authored to lock in the wire contract.
2. Plan originally assumed `IMTConnect*Header.SchemaVersion` already existed on the source DTOs and only the JSON-cppagent mapping was missing. That is not the case at branch cut: none of the three source DTOs expose `SchemaVersion`. The plan is widened to include adding `SchemaVersion` to the three `IMTConnect*Header` interfaces and their `MTConnect*Header` implementations. This is a strictly additive change to the public surface (`enh(common)` per CONVENTIONS §5.1) — it does not break existing consumers.
3. Bootstrap precondition: `tests/MTConnect.NET-JSON-cppagent-Tests/` is not on `upstream/master`. Per CONVENTIONS §17.8 row dated 2026-04-25 ("Silent scope expansion: bootstrap-test-project scaffolding"), the test project is scaffolded on this branch. The duplication will resolve via rebase once `feat/issue-133` (PR #133) merges upstream — the PR description carries the `Depends on #133` note.

## Follow-ups

- None at P0 — skeleton only.
