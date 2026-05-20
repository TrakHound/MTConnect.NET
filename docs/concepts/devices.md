# Devices

A **Device** is the top-level addressable entity that an MTConnect Agent emits observations for. Conceptually it is "one machine" — a CNC mill, a robot cell, a coordinate-measuring machine, a vibration sensor cluster — and in `MTConnect.NET` it maps directly to the [`MTConnect.Devices.Device`](/api/MTConnect.Devices/Device) class implementing `IDevice`. Every Device carries a UUID, a human-readable name, a type identifier, and a tree of [Components](/concepts/components), [Compositions](/concepts/components#compositions), and [DataItems](/concepts/data-items) underneath it.

## Identity and structure

A Device has three identity attributes, all required by the MTConnect Standard:

- **`Uuid`** — globally unique, set once when the model is authored and never re-generated. Consumers correlate observations across agent restarts on this field.
- **`Id`** — locally unique within the model tree. Used to address the Device in the path-query syntax (`/probe/<deviceUuid>` resolves the same node as `/probe?path=...//Device[@id='<id>']`).
- **`Name`** — display name. Free-form; not required to be unique across an installation.

The class signature is straightforward:

```csharp
public partial class Device : IDevice
{
    public const string TypeId = "Device";

    public string Uuid { get; set; }
    public string Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }

    public IEnumerable<IComponent> Components { get; set; }
    public IEnumerable<IComposition> Compositions { get; set; }
    public IEnumerable<IDataItem> DataItems { get; set; }
}
```

The `Type` string defaults to `"Device"` and matches the `TypeId` constant the class exposes. Subtypes (such as the Agent's self-describing Device, where `Type == "Agent"`) carry their own `TypeId` and resolve to a distinct concrete class. The spec defines `Device` and `Agent` as the two concrete Device types in `Part_3.0` of the MTConnect Standard ([docs.mtconnect.org](https://docs.mtconnect.org/)); `MTConnect.NET` adds no third Device subtype beyond those.

## The Agent Device

When the agent boots it auto-registers a self-describing Device of type `"Agent"` — a `MTConnect.Devices.Agent` instance — that emits agent-introspection observations (`AVAILABILITY` for the agent itself, `ASSET_CHANGED`, `ASSET_REMOVED`, `MTCONNECT_VERSION`, and adapter / module health DataItems). Disable this with `enableAgentDevice: false` in `agent.config.yaml` when a consumer is strict about device counts. The Agent Device first appears in v1.7 of the Standard — the library emits no Agent Device when serializing for a version below `MTConnectVersions.Version17`, mirroring the version-gating in `Device.Process(IDevice, Version)`.

## Authoring a Device in code

The minimal in-code constructor produces a Device with a random `Id`, a random `Uuid`, and an empty tree:

```csharp
using MTConnect.Devices;
using MTConnect.Devices.Components;
using MTConnect.Devices.DataItems;

var device = new Device
{
    Id = "mill-01",
    Uuid = "1234-5678-9abc-def0",
    Name = "Mill #1",
    Type = Device.TypeId,
};

// Attach a Controller component and an AVAILABILITY data item.
var controller = new ControllerComponent { Id = "ctrl" };
controller.AddDataItem(new AvailabilityDataItem(controller.Id));
device.AddComponent(controller);
```

`AddComponent` is not just a list append. It walks the [Organizers](/api/MTConnect.Devices/Organizers) table to decide whether the child Component needs to nest under an Organizer Component (an `Axes` Organizer for an `Axis`, a `Controllers` Organizer for a `Controller`, etc.) and creates the Organizer on demand. The same logic runs for `AddDataItem`, which assigns the data item's `Container` to the Device and re-runs the `DataItemIdFormat` template (`Component._defaultDataItemIdFormat`) to derive a deterministic Id.

## Authoring a Device in `Devices.xml`

For deployments that ship a static model alongside the agent, the canonical authoring path is an XML file that the agent reads at startup. The schema is the `MTConnectDevices_<version>.xsd` envelope from [schemas.mtconnect.org](https://schemas.mtconnect.org/). A minimal example:

```xml
<MTConnectDevices xmlns="urn:mtconnect.org:MTConnectDevices:2.5"
                  xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
                  xsi:schemaLocation="urn:mtconnect.org:MTConnectDevices:2.5
                                      https://schemas.mtconnect.org/schemas/MTConnectDevices_2.5.xsd">
  <Header creationTime="2025-01-01T00:00:00Z" sender="agent-01"
          instanceId="1737504000" version="2.5.0.0" assetBufferSize="1024"
          assetCount="0" bufferSize="131072"/>
  <Devices>
    <Device id="mill-01" uuid="1234-5678-9abc-def0" name="Mill #1">
      <DataItems>
        <DataItem category="EVENT" id="mill-01-avail" type="AVAILABILITY"/>
      </DataItems>
      <Components>
        <Controller id="ctrl">
          <DataItems>
            <DataItem category="EVENT" id="ctrl-mode" type="CONTROLLER_MODE"/>
          </DataItems>
        </Controller>
      </Components>
    </Device>
  </Devices>
</MTConnectDevices>
```

The agent loads `Devices.xml` through the XML codec in `MTConnect.NET-XML` and validates it against the appropriate XSD. Validation failures are not silent — see [Troubleshooting: XSD validation failures](/troubleshooting/xsd-validation-failures) for the diagnostic flow.

## Device-tree traversal

`Device` exposes recursive traversal helpers so a consumer can ask "every Component under me, regardless of depth" or "every DataItem matching this Type" without writing the recursion itself:

- `GetComponents()` — flat enumeration of every Component in the tree.
- `GetCompositions()` — flat enumeration of every Composition.
- `GetDataItems()` — flat enumeration of every DataItem across the Device, its Components, and its Compositions.
- `GetDataItemByKey(key)` — resolves a DataItem by `Id`, then `Name`, then `Source.DataItemId`, then `Source.Value` — the same precedence the SHDR adapter uses when matching an incoming line to a target DataItem.
- `GetDataItemByType(typeId, subType?)` — type-first lookup.

The recursive traversal respects Component nesting (an `Axis` under an `Axes` Organizer under a `Path` under the root Device is still reachable in a single `GetDataItems()` call).

## Device hashing

`Device.GenerateHash()` produces an SHA-1 hash over the Device's identity properties plus the hashes of every DataItem, Composition, and Component beneath it. Two Devices with identical models hash to identical strings; that hash drives the `deviceModelChangeTime` value that the agent advertises on `/probe`, and the v2.2+ Header's `Hash` attribute. Consumers detect a model change by comparing the cached hash against the live one, without diffing the full XML.

```csharp
var hashV1 = device.GenerateHash();
device.AddDataItem(new EmergencyStopDataItem("ctrl"));
var hashV2 = device.GenerateHash();
// hashV1 != hashV2 — the DataItem addition changed the device's identity.
```

## Where to next

- [Components](/concepts/components) — the recursively-nested machine pieces.
- [DataItems](/concepts/data-items) — the primitive observable points.
- [Observations](/concepts/observations) — the time-stamped values flowing through DataItems.
- [`IDevice` API reference](/api/MTConnect.Devices/IDevice) — every property, every method.
- [Cookbook: Write an agent](/cookbook/write-an-agent) — a hands-on walk-through.
