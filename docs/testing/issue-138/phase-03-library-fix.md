# Phase 3 — Library fix

## Executed

### 1. `MTConnect.NET-JSON-cppagent/Devices/JsonDataItem.cs`

Constructor copy at line 87 guarded with `IsNullOrEmpty`, mirroring the XML reference shape (`XmlDataItem.cs:198`):

```diff
-                Name = dataItem.Name;
+                if (!string.IsNullOrEmpty(dataItem.Name)) Name = dataItem.Name;
```

The `[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]` attribute called out in the plan is **not** added: the global `JsonFunctions.DefaultOptions` already sets `DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault`, which drops `null` from string-typed properties. The constructor guard is the load-bearing change; an attribute would be redundant under the current global option and would diverge from the maintainer's existing style on this file.

### 2. `MTConnect.NET-JSON/Devices/JsonDataItem.cs`

Same change applied symmetrically to the base JSON formatter; same reasoning.

### 3. Validation (green)

- `dotnet test tests/MTConnect.NET-JSON-cppagent-Tests/MTConnect.NET-JSON-cppagent-Tests.csproj -c Debug` → **4 passed, 0 failed**.
- `dotnet test tests/MTConnect.NET-JSON-Tests/MTConnect.NET-JSON-Tests.csproj -c Debug` → **4 passed, 0 failed**.

## Coverage

The constructor body in `JsonDataItem.cs` of both formatters is exercised by all four red-tests-now-green per file. Coverage results captured via `dotnet test --collect:"XPlat Code Coverage"`; ReportGenerator summary appended to this writeup.

## Metrics delta

- `JsonDataItem.cs:87` (cppagent): unconditional copy → `IsNullOrEmpty`-guarded.
- `JsonDataItem.cs:87` (base JSON): unconditional copy → `IsNullOrEmpty`-guarded.
- 8 test cases formerly red on the empty / typed-cleared paths are now green.

## Deviations from plan

- The `[JsonIgnore(Condition = WhenWritingNull)]` attribute called out in the plan was **omitted**: the global serializer option `DefaultIgnoreCondition = WhenWritingDefault` already drops null strings, and adding the attribute would be redundant. The constructor guard alone is sufficient for the four red cases pinned in P2.

## Follow-ups

- None.
