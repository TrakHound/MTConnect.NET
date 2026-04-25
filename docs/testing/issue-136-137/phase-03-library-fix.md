# Phase 03 — Library fix

## Executed

### `Device` default constructor

Removed three lines from
`libraries/MTConnect.NET-Common/Devices/Device.cs`:

```diff
 public Device()
 {
-    Id = StringFunctions.RandomString(10);
-    Name = "dev";
-    Uuid = Guid.NewGuid().ToString();
     Type = TypeId;
     DataItems = new List<IDataItem>();
     Components = new List<IComponent>();
     Compositions = new List<IComposition>();

     DataItemIdFormat = Component._defaultDataItemIdFormat;
     CompositionIdFormat = Component._defaultCompositionIdFormat;
     ComponentIdFormat = Component._defaultComponentIdFormat;
 }
```

`Type`, the collection initialisations, and the `IdFormat` defaults
remain untouched — they are infrastructure for the object's later use,
not identity / naming.

### Generated `*Component.g.cs` files

Removed `Name = NameId;` from the default constructor body of every
generated component subclass under
`libraries/MTConnect.NET-Common/Devices/Components/` (115 files):

```diff
 public AxesComponent()
 {
     Type = TypeId;
-    Name = NameId;
 }
```

Verified: `grep -l "Name = NameId" libraries/MTConnect.NET-Common/Devices/Components/*.g.cs`
returns 0 files after the edit.

### Scriban template

Removed the matching line from the SysML-import generator template at
`build/MTConnect.NET-SysML-Import/CSharp/Templates/Devices.ComponentType.scriban`,
so any future regeneration produces the new shape rather than
re-introducing the back-fill:

```diff
 public {{name}}()
 {
     Type = TypeId;
-    Name = NameId;
 }
```

The `NameId` constant itself remains declared on every subclass —
consumers reading the constant for non-default-construction purposes
are unaffected.

### Existing tests update

None required — the only pre-existing common-tests file is
`tests/MTConnect.NET-Common-Tests/Devices/Device.cs` carrying a
trivial `Test1` that always passes; it does not assert any
constructor default.

## Validation

- `dotnet build MTConnect.NET.sln -c Debug` — green; 0 errors, 563
  warnings (all pre-existing on `upstream/master`).
- `dotnet test tests/MTConnect.NET-Common-Tests/ --filter
  'TestCategory=DeviceComponentDefaultsRemoved'` — 16 passed, 0
  failed.
- `dotnet test tests/MTConnect.NET-Common-Tests/` — 17 passed, 0
  failed (16 reds + the pre-existing `Test1`).
- `dotnet test tests/MTConnect.NET-XML-Tests/` — 4 passed.
- `dotnet test tests/MTConnect.NET-SHDR-Tests/` — 27 passed.
- `dotnet test tests/IntegrationTests/` — 2 passed.
- `tests/MTConnect.NET-HTTP-Tests/` and
  `tests/MTConnect.NET-Tests-Agents/` target `net6.0` but the
  libraries target `net8.0`; restore fails. This is pre-existing on
  `upstream/master`, unrelated to this campaign. Logged in
  follow-ups.

## Deviations from plan

- The plan's `04-library-fix.md` §"Commits" lists three commits:
  `fix(common): remove auto-generated random uuid from device ctor`,
  `test(common-tests): remove red category and ci job`, and
  `docs(testing): document library fix and breaking change`. Two
  scope-shape changes apply:
  - The single `fix(common)` commit describes the removal of three
    lines (Id, Name, Uuid) plus 115 generated `*Component.g.cs`
    edits — all under the `common` library scope per §5.3 — so the
    commit subject is `fix(common): drop default values from device
    and component constructors`.
  - The Scriban template under `build/MTConnect.NET-SysML-Import/`
    is a separate scope per §5.3 (`sysml-import`); it lands in its
    own commit `fix(sysml-import): drop name back-fill from
    component template`, per §5.4 rule 1 (production code wins) and
    §1.5 atomic-commit rule.
  - The `test(common-tests): remove red category and ci job` commit
    is dropped: per CONVENTIONS §1.7 there is no per-issue CI job
    to remove, and the red category is deliberately retained on the
    fixtures so the regression-pin filter (P4) can be invoked
    against the same name.

## Coverage impact

`libraries/MTConnect.NET-Common/Devices/Device.cs` lost three lines
of identity assignment; the remaining ctor body is exercised by
`DeviceCtorDefaultsTests` (constructs a `Device`, walks `Type`,
collections). `libraries/MTConnect.NET-Common/Devices/Components/*.g.cs`
each has one less line in the ctor, with the remaining `Type =
TypeId;` exercised by the reflection walker test.

## Documentation

This file. Campaign index `docs/testing/issue-136-137.md` §4 links
to it.

## Follow-ups

- `tests/MTConnect.NET-HTTP-Tests/` and
  `tests/MTConnect.NET-Tests-Agents/` target `net6.0` while the
  libraries target `net8.0`. Restore fails on those test projects on
  `upstream/master`. Not in scope for this campaign — record under
  `todo.md §F` for follow-up.
