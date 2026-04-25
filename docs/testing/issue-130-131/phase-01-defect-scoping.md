# Phase 1 — Defect scoping

## Executed

Confirmed at `upstream/master` SHA `3d6321ab`:

1. **`SchemaVersion` absent from JSON-cppagent header DTOs**:

   - `libraries/MTConnect.NET-JSON-cppagent/Streams/JsonStreamsHeader.cs` — no `SchemaVersion` member.
   - `libraries/MTConnect.NET-JSON-cppagent/Assets/JsonAssetsHeader.cs` — no `SchemaVersion` member.
   - `libraries/MTConnect.NET-JSON-cppagent/Devices/JsonDevicesHeader.cs` — `SchemaVersion` declared (line 19) but the constructor accepting `IMTConnectDevicesHeader` (lines 45-59) never assigns it; the reverse mapper `ToDevicesHeader()` (lines 62-75) never copies it back.

2. **`SchemaVersion` absent from source DTOs**:

   - `libraries/MTConnect.NET-Common/Headers/IMTConnectStreamsHeader.cs` — declares `Version` (line 23) but no `SchemaVersion`.
   - `libraries/MTConnect.NET-Common/Headers/IMTConnectDevicesHeader.cs` — same shape.
   - `libraries/MTConnect.NET-Common/Headers/IMTConnectAssestsHeader.cs` — same shape.
   - The `MTConnect*Header` implementations follow the interfaces — no `SchemaVersion` to map.

   The plan as authored assumed `IMTConnect*Header.SchemaVersion` already existed. It does not. The fix surface is widened to add `SchemaVersion` to the three interfaces and their implementations as additive (`enh`) members.

3. **`TestIndicator` already present everywhere — issue #131 already addressed**:

   - All three JSON-cppagent header DTOs declare `[JsonPropertyName("testIndicator")] public bool TestIndicator { get; set; }`.
   - Each constructor assigns `TestIndicator = header.TestIndicator;` from the source DTO.
   - Each reverse mapper (`ToStreamsHeader()`, `ToDevicesHeader()`, `ToAssetsHeader()`) copies the field back.
   - Source DTOs `IMTConnect*Header.TestIndicator` and their `MTConnect*Header.TestIndicator` impls are wired.
   - No production code change required for #131; tests still pin the wire shape so a future regression cannot silently drop the field.

4. **cppagent reference** — cppagent v2.7.0.7 emits `Header.schemaVersion` and `Header.testIndicator` on every envelope (Streams, Devices, Assets). The wire shape this PR converges on:

   ```json
   {
     "MTConnectStreams": {
       "Header": {
         "schemaVersion": "2.5",
         "testIndicator": false,
         "...": "..."
       }
     }
   }
   ```

## Metrics delta

Read-only investigation; no production change.

## Deviations from plan

The P1 plan assumed `IMTConnect*Header.SchemaVersion` already existed and that #131 (`testIndicator`) was an open defect. Both assumptions are wrong on `upstream/master`. Corrections recorded above; the broader plan widening is documented in `phase-00-foundation.md` "Deviations from plan" §1 + §2.

## Follow-ups

None — scope is final.
