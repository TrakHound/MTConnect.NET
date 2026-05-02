# DataItems

A **DataItem** is the primitive observable point on a [Component](/concepts/components) or [Composition](/concepts/components#compositions). It does not carry a value — it declares the shape of values that the agent expects. An `AVAILABILITY` EVENT DataItem says "this point reports `AVAILABLE` or `UNAVAILABLE` over time"; a `POSITION` SAMPLE DataItem says "this point reports a numeric position over time with a configured unit". The runtime stream of values associated with a DataItem is described by [Observations](/concepts/observations).

`MTConnect.NET` represents a DataItem with the [`MTConnect.Devices.DataItem`](/api/MTConnect.Devices/DataItem) class implementing `IDataItem`, and ships one concrete subclass per spec-defined DataItem type. The concrete classes live in `libraries/MTConnect.NET-Common/Devices/DataItems/` and are generated from the MTConnect SysML model: `AvailabilityDataItem`, `PositionDataItem`, `EmergencyStopDataItem`, `AssetCountDataItem`, etc.

## Three categories

Every DataItem belongs to exactly one of three categories, defined in [`MTConnect.Devices.DataItemCategory`](/api/MTConnect.Devices/DataItemCategory):

- **`EVENT`** — a discrete-state observation. Values are strings (`READY`, `INTERRUPTED`, `AVAILABLE`) or numerics that do not interpolate. Examples: `AVAILABILITY`, `EMERGENCY_STOP`, `CONTROLLER_MODE`, `EXECUTION`. Source: MTConnect Standard `Part_2.0` Streams §3 ([docs.mtconnect.org](https://docs.mtconnect.org/)).
- **`SAMPLE`** — a continuous-time numeric observation. Values are scalars or vectors that have a unit and can be interpolated. Examples: `POSITION`, `TEMPERATURE`, `LOAD`, `SPINDLE_SPEED`. SAMPLE DataItems carry a `Units` attribute, an optional `NativeUnits` attribute, and an optional `SignificantDigits` attribute.
- **`CONDITION`** — an alarm-style observation that aggregates into a level: `NORMAL`, `WARNING`, `FAULT`, or `UNAVAILABLE`. Conditions carry an optional `nativeCode`, an optional `qualifier` (`HIGH` / `LOW`), and a human-readable message. Examples: `SYSTEM`, `LOGIC_PROGRAM`, `MOTION_PROGRAM`. Conditions deserve their own treatment — see [Observations: Conditions](/concepts/observations#conditions).

The category drives serialization shape (a `<Samples>` element wraps SAMPLE observations on the wire; a `<Events>` element wraps EVENT observations; a `<Condition>` element wraps CONDITION observations) and it drives which Observation subclass the runtime constructs.

## Identity attributes

```csharp
public partial class DataItem : IDataItem
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string SubType { get; set; }
    public DataItemCategory Category { get; set; }

    public string Units { get; set; }
    public string NativeUnits { get; set; }
    public int? SignificantDigits { get; set; }

    public DataItemRepresentation Representation { get; set; }
    public DataItemStatistic Statistic { get; set; }
    public DataItemCoordinateSystem CoordinateSystem { get; set; }

    public IContainer Container { get; set; }
    public IDevice Device { get; set; }

    public virtual Version MinimumVersion { get; }
    public virtual Version MaximumVersion { get; }
}
```

- **`Id`** — locally unique within the Device. Required by the spec on the wire.
- **`Name`** — optional. Where present, the adapter line `LATHE.AXIS.X|<value>` resolves to a DataItem with `Name == "LATHE.AXIS.X"`. The SHDR adapter prefers `Name` lookup; the JSON consumer prefers `Id`.
- **`Type`** — the canonical type identifier in `UNDERSCORE_UPPER` form (`"AVAILABILITY"`, `"POSITION"`, `"EMERGENCY_STOP"`). The constant lives on the concrete class's `TypeId` field.
- **`SubType`** — refines the type. `POSITION` has subtypes `ACTUAL`, `COMMANDED`, `PROGRAMMED`, `TARGET`. `TEMPERATURE` has subtypes `ACTUAL`, `CALCULATED`.
- **`Category`** — EVENT / SAMPLE / CONDITION.
- **`Units`** — for SAMPLE DataItems. Canonical units are listed in the SysML model's `UnitsEnum` ([`mtconnect/mtconnect_sysml_model`](https://github.com/mtconnect/mtconnect_sysml_model)).
- **`Representation`** — `VALUE` (default), `DATA_SET`, `TABLE`, or `TIME_SERIES`. See [Representations](#representations) below.
- **`Container`** / **`Device`** — back-pointers to the Component / Device that owns the DataItem.

## Representations

`DataItemRepresentation` controls the shape of every observation that the DataItem produces:

- **`VALUE`** (default) — one scalar per observation. The wire shape is `<DataItem ...>scalar-value</DataItem>`.
- **`DATA_SET`** — a set of key-value entries per observation. Keys are spec-defined for some types (`PART_COUNT` discriminates `accepted`, `rejected`, `bad`) or adapter-defined for others. Wire shape: `<DataItem ...><Entry key="k1">v1</Entry><Entry key="k2">v2</Entry></DataItem>`.
- **`TABLE`** — entries-of-entries. Each top-level entry holds a sub-map. Used by `WORK_OFFSET_TABLE` and similar table-shaped DataItems.
- **`TIME_SERIES`** — a sample of values at a fixed cadence. Wire shape: `<DataItem ... sampleCount="N" sampleRate="freq">v1 v2 v3 ...</DataItem>`.

Representations are version-gated: `DATA_SET` enters the spec at v1.5, `TABLE` at v1.6, and the wire-shape rules for each are codified in `MTConnectStreams_<version>.xsd` ([schemas.mtconnect.org](https://schemas.mtconnect.org/)). The library's [Streams envelope](/wire-formats/) honours the gating when serializing a DataItem whose `Representation` is more recent than the requested response version.

## Constructing DataItems

Three idiomatic constructors:

```csharp
using MTConnect.Devices;
using MTConnect.Devices.DataItems;

// 1. Typed — the recommended path. The DataItem subclass carries the
// canonical Type / Category / default Units, so the call site is succinct.
var avail = new AvailabilityDataItem("ctrl");

// 2. Untyped via the base DataItem constructor. The constructor takes
// category + type + subType + id; suitable for adapter-side dynamic models.
var custom = new DataItem(
    DataItemCategory.SAMPLE,
    "POSITION",
    "ACTUAL",
    "x-pos-actual");

// 3. The container shortcut — attach a typed DataItem to a Component or
// Composition by generic.
component.AddDataItem<TemperatureDataItem>();
```

Construction through a typed class is preferable because the class also carries the `DescriptionText`, `MinimumVersion`, and any default `Units` value from the SysML model. The shortcut form leaves those nulls and pushes the burden of correctness onto the caller.

## Filters, Constraints, Definitions

A DataItem can carry side metadata that refines what the agent emits:

- **`Filters`** — server-side filters that suppress observations. The two spec-defined filter types are `MINIMUM_DELTA` (only emit if `|new - last| > delta`) and `PERIOD` (emit at most once per period). Class: [`DataItemFilter`](/api/MTConnect.Devices/DataItemFilter); enum: [`DataItemFilterType`](/api/MTConnect.Devices/DataItemFilterType).
- **`Constraints`** — declared bounds and enumerated allowed values. `MAXIMUM`, `MINIMUM`, `VALUE` lists. Constraints are advisory in the spec — the agent emits values that violate the constraints, but consumers may flag them as alarms.
- **`Definition`** — for `DATA_SET` / `TABLE` representations, the structural definition of the entries and the cells. Class: [`DataItemDefinition`](/api/MTConnect.Devices/DataItemDefinition).
- **`ResetTrigger`** — for accumulators (`PART_COUNT`, `ACCUMULATED_TIME`), the lifecycle event that zeroes the accumulator: `SHIFT`, `DAY`, `MAINTENANCE`. Enum: [`DataItemResetTrigger`](/api/MTConnect.Devices/DataItemResetTrigger).
- **`Source`** — the SHDR / adapter key that maps incoming lines to this DataItem. Class: [`DataItemSource`](/api/MTConnect.Devices/DataItemSource).

## DataItem hashing and change detection

`DataItem.GenerateHash()` produces a SHA-1 hash over identity, category, type/subType, units, representation, statistic, coordinate-system, filter list, constraints, definition, and source. A change to any of those values changes the hash. The Device's hash includes every DataItem hash, so a single DataItem edit propagates up to the Device's `Hash` attribute on `/probe` — consumers detect that a model edit happened without re-parsing the full Devices document.

## Version gating

Like Components, every DataItem class carries `MinimumVersion` and `MaximumVersion`. `DataItem.Process(IDataItem, Version)` is the version-aware serializer; it strips properties added after the target version (for instance, `Statistic` is v1.2+; serializing for v1.1 elides it) and rejects DataItems whose `MinimumVersion` is above the target. Source: the SysML model's per-class introduction version, mirrored into the generated `.g.cs` files. Source-of-truth note: when the SysML model and the XSD disagree about a DataItem's introduction version, `MTConnect.NET` follows the SysML model — see [Compliance: Known divergences](/compliance/known-divergences) for the precedent.

## Where to next

- [Observations](/concepts/observations) — the runtime values flowing through DataItems.
- [Relationships](/concepts/relationships) — DataItem-to-DataItem references.
- [`IDataItem` API reference](/api/MTConnect.Devices/IDataItem).
- [Cookbook: Write an agent](/cookbook/write-an-agent).
