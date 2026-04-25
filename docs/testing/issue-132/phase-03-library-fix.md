# Phase 3 — Library fix

## Diff

`libraries/MTConnect.NET-Common/Agents/MTConnectAgent.cs`,
`NormalizeDevice` (private), in the `Add Required AssetCount DataItem`
block:

```diff
                     var assetcount = new AssetCountDataItem(obj.Id);
                     assetcount.Device = obj;
                     assetcount.Container = obj;
                     assetcount.Name = AssetCountDataItem.NameId;
+                    // ASSET_COUNT is a DATA_SET representation per MTConnect Part 2
+                    // (UML _19_0_3_68e0225_1640602520420_217627_44). The generated
+                    // AssetCountDataItem still defaults Representation to VALUE; override
+                    // it here so the auto-injected DataItem matches the spec.
+                    assetcount.Representation = DataItemRepresentation.DATA_SET;
                     var x = obj.DataItems.ToList();
                     x.Add(assetcount);
                     obj.DataItems = x;
```

## Why this is the right place

- The auto-generator is exactly the path that emits the wrong shape. The
  `(string deviceId)` constructor on `AssetCountDataItem` sets
  `Representation = DefaultRepresentation` (which is `VALUE` per the
  generator), so any caller that doesn't override gets the wrong value.
- User-declared `ASSET_COUNT` DataItems in `devices.xml` flow through
  `NormalizeDataItems`, which returns them as-declared. Their
  `representation` attribute is preserved verbatim. Test
  `UserDeclared_AssetCount_Representation_Is_Preserved` exercises this
  invariant.
- The `AssetCountDataItem.g.cs` `DefaultRepresentation` constant is
  generator output (SysML model importer); fixing it there is a separate,
  upstream-of-this-repo concern. Tracked as a follow-up.

## Test outcome

```text
Passed AutoInjected_AssetCount_Has_DataSet_Representation [179 ms]
Passed UserDeclared_AssetCount_Representation_Is_Preserved [137 ms]

Total tests: 2
     Passed: 2
 Total time: 3.2057 Seconds
```

Whole `MTConnect.NET-Common-Tests` project after the fix:

```text
Passed!  - Failed: 0, Passed: 3, Skipped: 0, Total: 3, Duration: 437 ms
```

(The pre-existing `Test1` placeholder still passes; the two new tests
are the ones authored in P2.)

## Breaking-change impact

The Probe wire shape for the auto-injected `ASSET_COUNT` DataItem
changes from `<DataItem ... type="ASSET_COUNT" />` (with the implicit
default `representation="VALUE"` carried in the agent's internal state)
to `<DataItem ... type="ASSET_COUNT" representation="DATA_SET" />`.
Consumers branching on the absence of `representation=` for this
DataItem must accept the explicit attribute. Note the streamed
observation shape (`AssetCount` scalar) is **unchanged** by this PR;
the streamed-observation rename to `AssetCountDataSet` with a per-type
map is a separate, larger surface change tracked as a follow-up.

## Coverage

The only production line touched in this PR is the inserted
`assetcount.Representation = DataItemRepresentation.DATA_SET;`
assignment. Both red tests exercise the auto-generator path; the
assignment is unconditionally executed every time an `ASSET_COUNT` is
auto-injected, so line coverage on the new line is 100% via either
test.

## DoD

Reds → green; user-declared-preservation invariant green; no other
test in the project affected.
