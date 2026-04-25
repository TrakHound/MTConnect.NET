# Phase 01 — Defect scoping

## Constructor surface confirmation

### `Device` default constructor — `libraries/MTConnect.NET-Common/Devices/Device.cs`

At branch-cut SHA the relevant block is:

```csharp
public Device()
{
    Id = StringFunctions.RandomString(10);
    Name = "dev";
    Uuid = Guid.NewGuid().ToString();
    Type = TypeId;
    DataItems = new List<IDataItem>();
    Components = new List<IComponent>();
    Compositions = new List<IComposition>();

    DataItemIdFormat = Component._defaultDataItemIdFormat;
    CompositionIdFormat = Component._defaultCompositionIdFormat;
    ComponentIdFormat = Component._defaultComponentIdFormat;
}
```

The three lines this campaign removes are:

- `Id = StringFunctions.RandomString(10);` — produces a different
  random ten-character string per construction. The plan's narrative
  cites the older fixture string `"4A1GF40513"`; the upstream HEAD
  has already moved to `RandomString(10)`. Either form leaks a
  placeholder identity to consumers that do not override `Id`.
- `Name = "dev";` — placeholder string left from a development
  fixture.
- `Uuid = Guid.NewGuid().ToString();` — the #136 defect proper. A
  fresh GUID per construction silently violates the MTConnect XSD
  contract that `Device.uuid` is stable for the entity's lifetime.

The remaining lines (collection initialisation, `IdFormat` defaults,
`Type = TypeId`) are kept — they are infrastructure for the object's
later use, not identity / naming.

### `Component` subclass back-fill — generated `*.g.cs`

The base `Component` constructor in
`libraries/MTConnect.NET-Common/Devices/Component.cs` does not assign
`Name`. The back-fill lives in every concrete subclass under
`libraries/MTConnect.NET-Common/Devices/Components/`. Each generated
file follows the same shape:

```csharp
public AxesComponent()
{
    Type = TypeId;
    Name = NameId;   // <-- this line removed by the campaign
}
```

`grep -l "Name = NameId" libraries/MTConnect.NET-Common/Devices/Components/*.g.cs`
returns 115 files at branch-cut SHA. The line is identical in every
file (`Type = TypeId;` is preserved; only `Name = NameId;` is
removed).

The pattern's source is the SysML-import Scriban template at
`build/MTConnect.NET-SysML-Import/CSharp/Templates/Devices.ComponentType.scriban`,
which emits:

```text
public {{name}}()
{
    Type = TypeId;
    Name = NameId;
}
```

Without changing the template, every future regeneration would
re-introduce the back-fill. The fix removes the `Name = NameId;` line
from the template and from every already-generated file.

### Composition back-fill — none

`build/MTConnect.NET-SysML-Import/CSharp/Templates/Devices.CompositionType.scriban`
emits a constructor body of `{ Type = TypeId; }` — no `Name`
assignment. The 67 generated `Compositions/*.g.cs` files all match the
template; none assign `Name`. Compositions are out of scope for this
campaign.

### `Agent : Device` subclass — already clean

`libraries/MTConnect.NET-Common/Devices/Agent.cs` declares two
constructors:

- `public Agent()` — assigns only `Type = TypeId` plus collection
  initialisation; no `Id`, `Name`, or `Uuid` assignment. This ctor
  inherits `Device()`, so today it accepts the same identity leak via
  the base call. After the fix, `new Agent()` correctly leaves all
  identity fields `null`.
- `public Agent(MTConnectAgent agent)` — explicitly sets `Id`, `Name`,
  and `Uuid` from the agent argument. Out of scope.

## Caller audit — `new Device()` inside `libraries/`

```text
libraries/MTConnect.NET-Common/Devices/Device.cs        line 1007
libraries/MTConnect.NET-Common/Agents/MTConnectAgent.cs line 1092
libraries/MTConnect.NET-JSON/Devices/JsonDevice.cs       line 135
libraries/MTConnect.NET-JSON-cppagent/Devices/JsonDevice.cs line 112
libraries/MTConnect.NET-XML/Devices/XmlDevice.cs        line 74
```

In every one of these call sites the next several lines explicitly
reassign `Id`, `Name`, and `Uuid` from the source DTO. None depend on
the auto-generated values; the breaking change is invisible to
production code.

## Caller audit — `new Agent()` inside `libraries/`

`libraries/MTConnect.NET-XML/Devices/XmlAgent.cs` line 13. Same
pattern: the next lines assign `Id`, `Name`, and `Uuid` from the XML
input.

## Caller audit — `new <T>Component()`

The library does not call `new <T>Component()` (without arguments) and
then read `Name` without an explicit assignment. Consumers who do
construct components by reflection (e.g. the agent's
device-configuration loader) supply `Name` themselves from the loaded
configuration. Removing the `Name = NameId` back-fill simply makes
unset `Name` explicit instead of placeholder.

## Existing tests that may break

`tests/MTConnect.NET-Common-Tests/Devices/Device.cs` carries only a
trivial `Test1` that always passes. No existing test asserts the old
defaults; nothing in the in-tree test suite needs updating alongside
the library fix.

## Breaking-change scope

Public API surfaces affected:

- `MTConnect.Devices.Device.Id`, `.Name`, `.Uuid` — `null` after
  default-constructor invocation; previously a random ten-character
  string, the literal `"dev"`, and a fresh GUID respectively.
- `MTConnect.Devices.Components.<T>Component.Name` — `null` after
  default-constructor invocation for every concrete subclass;
  previously the lowercase type name (e.g. `"axes"`, `"linear"`).
- Subclasses inheriting from `Device` (today: `Agent`) inherit the
  cleaned-up base constructor; their parameterless construction now
  also leaves `Id`, `Name`, `Uuid` `null`.

## Validation

Read-only phase. `git status` clean before and after.

## Documentation

This file. The campaign index
(`docs/testing/issue-136-137.md` §2) links to it.

## Deviations from plan

None.

## Follow-ups

None for this phase.
