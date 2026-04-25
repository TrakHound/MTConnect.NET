# Phase 1 — Defect scoping

## Executed

### 1. Constructor copy + property attribute confirmed at HEAD

`libraries/MTConnect.NET-JSON-cppagent/Devices/JsonDataItem.cs`:

- Property declaration at lines 27-28:
  ```csharp
  [JsonPropertyName("name")]
  public string Name { get; set; }
  ```
  No `[JsonIgnore(...)]` attribute.
- Constructor copy at line 87:
  ```csharp
  Name = dataItem.Name;
  ```
  Unconditional.

### 2. Null-vs-empty source chain

`Name` on `IDataItem` is a plain `string` with no default value (`libraries/MTConnect.NET-Common/Devices/DataItem.g.cs:69`). The CLR default for `string` is `null`. The defect fires when the source `dataItem.Name` is `string.Empty`, which happens via:

- `Component.AddDataItem<T>(string name, ...)` (`libraries/MTConnect.NET-Common/Devices/Component.cs:964`) — assigns the supplied `name` parameter without filtering. A caller passing `""` propagates the empty string.
- Direct caller assignment (`dataItem.Name = ""`) — observed in the upstream issue's reproduction.
- Generated subclasses such as `TemperatureDataItem` set `Name = NameId` in their default constructor; a caller can clear it back to `""`.

When the source is `null`, the global serializer option `DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault` (see item 3) drops the property because `default(string)` is `null`. When the source is `""`, the property is emitted because `""` is not the type default.

### 3. Global serializer options

`libraries/MTConnect.NET-JSON-cppagent/JsonFunctions.cs:20` and the four ad-hoc `JsonSerializerOptions` constructions at lines 57, 84, 111 all set:

```csharp
DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
```

The base JSON formatter `libraries/MTConnect.NET-JSON/JsonFunctions.cs` mirrors the same option at lines 21, 38, 75, 102, 129. `WhenWritingDefault` skips `null` for reference types but not `""`. Consequence: a constructor-level `IsNullOrEmpty` guard is the load-bearing fix; an additional `[JsonIgnore(WhenWritingNull)]` attribute would be redundant under the current global option (the property is already null-skipped). To stay close to the maintainer's existing style and avoid touching attribute surface unnecessarily, the fix adds the constructor guard only.

### 4. Base `MTConnect.NET-JSON` parity

`libraries/MTConnect.NET-JSON/Devices/JsonDataItem.cs:87` carries the same unconditional copy:

```csharp
Name = dataItem.Name;
```

The base JSON formatter is in scope; the fix mirrors the cppagent change in a separate atomic commit.

### 5. XML reference confirmation

`libraries/MTConnect.NET-XML/Devices/XmlDataItem.cs:198`:

```csharp
if (!string.IsNullOrEmpty(dataItem.Name)) writer.WriteAttributeString("name", dataItem.Name);
```

This is the shape both JSON formatters mirror.

### 6. Adjacent unconditional copies in the same constructor

The `JsonDataItem(IDataItem)` constructor of the cppagent variant copies several other optional string properties unconditionally at lines 86-91, 138, 147, 162-165, 199, 202, 212. Examples:

- `Id = dataItem.Id;` — required per XSD, never empty in valid input.
- `Type = dataItem.Type;` — required per XSD, never empty in valid input.
- `SubType`, `NativeUnits`, `InitialValue`, `CoordinateSystemIdRef`, `Units` — all optional.

For the optional fields, the same defect pattern (`""` flowing through to the wire) is theoretically possible. The plan briefing instructs filing follow-up issues rather than widening this PR's scope, since #138's reproduction only covers `Name`. Audited and not expanded.

### 7. Existing tests asserting empty-string behavior

Searched `tests/` for `name.*=.*""|Name\s*=\s*""` patterns; no existing test depends on the empty-string emission. No test edits required to land the fix.

## Metrics delta

Read-only phase; no production touch.

## Deviations from plan

None.

## Follow-ups

- Other optional string properties in `JsonDataItem(IDataItem)` (`SubType`, `NativeUnits`, `InitialValue`, `CoordinateSystemIdRef`, `Units`, `Statistic`, `Representation`, `ResetTrigger`) may exhibit a parallel empty-string defect under specific caller patterns. Out of scope for this PR per the plan's guidance; flag for the campaign tracker.
