# Phase 1 — Defect scoping

## Surface located

`grep -rn 'AssetCountDataItem' libraries/MTConnect.NET-Common/Agents/`
returns three hits at lines 1152, 1154, 1157 in
`libraries/MTConnect.NET-Common/Agents/MTConnectAgent.cs`. The
auto-generator runs inside the private `NormalizeDevice(IDevice)` method
and is symmetric with the existing `Availability`, `AssetChanged`, and
`AssetRemoved` injection blocks immediately above it.

```csharp
// libraries/MTConnect.NET-Common/Agents/MTConnectAgent.cs (1151-1161)
// Add Required AssetCount DataItem
if (obj.DataItems.IsNullOrEmpty() || !obj.DataItems.Any(o => o.Type == AssetCountDataItem.TypeId))
{
    var assetcount = new AssetCountDataItem(obj.Id);
    assetcount.Device = obj;
    assetcount.Container = obj;
    assetcount.Name = AssetCountDataItem.NameId;
    var x = obj.DataItems.ToList();
    x.Add(assetcount);
    obj.DataItems = x;
}
```

## Generated DataItem class

`libraries/MTConnect.NET-Common/Devices/DataItems/AssetCountDataItem.g.cs`
declares:

```csharp
public const DataItemRepresentation DefaultRepresentation = DataItemRepresentation.VALUE;
```

Both the parameterless and the `(string deviceId)` constructors set
`Representation = DefaultRepresentation`, so any caller that doesn't
override it gets `VALUE`. The injection block above is one such caller.

## Spec authority

The MTConnect Standard, Part 2 — Devices Information Model, defines
`ASSET_COUNT` (UML element ID
`_19_0_3_68e0225_1640602520420_217627_44`, captured in the
`.g.cs` file's banner comment) as a `DATA_SET` representation: a map of
`<asset type, count>` pairs. The generator output is the discrepancy —
it should be `DATA_SET`, not `VALUE`. This is a SysML-importer bug
tracked separately; the present PR is the user-visible-side workaround.

## Decision: pragmatic override at the injection site

The dispatch directive narrows scope to a one-line override at the
injection site:

```csharp
assetcount.Representation = DataItemRepresentation.DATA_SET;
```

This intentionally does NOT touch `AssetCountDataItem.g.cs`; if a user
declares an `ASSET_COUNT` DataItem in `devices.xml`, it goes through
`NormalizeDataItems` instead of the auto-generator block, so its
declared `representation` attribute is preserved.

## What still needs follow-up

- Generator-side fix to flip `DefaultRepresentation` to `DATA_SET` so
  every code path that constructs an `AssetCountDataItem` is correct
  (sysml-importer plan).
- Per-asset-type bookkeeping on the asset buffer
  (`GetCountsByType()`-style helper) and emission of the corresponding
  `AssetCountDataSet` observation entries on Current / Sample. The
  Probe-side spec compliance is what this PR delivers; the streamed
  observation shape is a separate, larger surface change.

## Validation

Read-only inspection. No code changed in this phase.

## DoD

Decision recorded: pragmatic override at injection site in P3; out-of-scope
follow-ups noted explicitly.
