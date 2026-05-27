---
title: Agent validation events
description: How MTConnectAgent surfaces validation failures via the Invalid*Added event family — consumer wire-up and contributor extension.
---

# Agent validation events

[`MTConnectAgent`](/api/MTConnect.Agents/MTConnectAgent) raises a small, uniformly-shaped family of events whenever input rejected by its validation pipeline is added — invalid Components, Compositions, DataItems, Observations, and Assets. The agent does **not** throw; it raises an event, lets you log or react, and continues serving requests. Consumers wire one or more handlers; contributors extending the agent add a new entry to the family by following the same naming and call-site pattern.

## The event family

The agent exposes one event per validatable element kind:

| Event | Delegate | When it fires |
| --- | --- | --- |
| `InvalidComponentAdded` | [`MTConnectComponentValidationHandler`](/api/MTConnect/MTConnectComponentValidationHandler) | A generic `Component` (unknown `Type`) is encountered while initialising a Device. |
| `InvalidCompositionAdded` | [`MTConnectCompositionValidationHandler`](/api/MTConnect/MTConnectCompositionValidationHandler) | A generic `Composition` is encountered while initialising a Device. |
| `InvalidDataItemAdded` | [`MTConnectDataItemValidationHandler`](/api/MTConnect/MTConnectDataItemValidationHandler) | A generic `DataItem` (unknown `Type`) is encountered while initialising a Device. |
| `InvalidObservationAdded` | [`MTConnectObservationValidationHandler`](/api/MTConnect/MTConnectObservationValidationHandler) | An observation value fails per-DataItem validation, or its `DataItemKey` is unknown. |
| `InvalidAssetAdded` | [`MTConnectAssetValidationHandler`](/api/MTConnect/MTConnectAssetValidationHandler) | An asset fails its `Process(...)` validation pipeline. |

A sixth member, **`InvalidDeviceAdded`** (with delegate `MTConnectDeviceValidationHandler`), is in flight via [TrakHound/MTConnect.NET#169](https://github.com/TrakHound/MTConnect.NET/pull/169) and will fire when `AddDevice(IDevice)` rejects a device whose own validation does not pass — making the family closed across the five Device-tree element kinds plus the top-level Device itself.

```mermaid
flowchart TB
  Input["Add input (Device / Observation / Asset)"] --> Validate{"Validate"}
  Validate -- pass --> Apply["Apply to agent state"]
  Validate -- fail --> Decide{"InputValidationLevel"}
  Decide -- Ignore --> Apply
  Decide -- Warn --> RaiseW["Raise Invalid*Added"]
  Decide -- Remove --> RaiseR["Raise Invalid*Added + remove subtree"]
  Decide -- Strict --> RaiseS["Raise Invalid*Added + reject whole Device"]
  RaiseW --> Apply
  RaiseR --> Apply
```

## Consumer POV

### Why events, not exceptions

`MTConnectAgent` is a long-running service. Throwing on the first invalid Component or stray Observation would crash the host process and take the rest of the agent down with it. The event-based contract lets callers:

- log the failure (with the rich [`ValidationResult`](/api/MTConnect.Devices/ValidationResult) payload) and keep serving valid data;
- decide centrally how strict to be — set [`InputValidationLevel`](/api/MTConnect.Configurations/InputValidationLevel) to `Ignore`, `Warn`, `Remove`, or `Strict` on the agent configuration, and the same handler runs across every level above `Ignore`;
- attribute the failure to the source `deviceUuid` so multi-device hosts can route the diagnostic appropriately.

### Wire-up

Attach one handler per event you care about; you can also attach the same handler to every event by adapting the signatures:

```csharp
using MTConnect;
using MTConnect.Agents;
using MTConnect.Assets;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Observations;

var agent = new MTConnectAgent(new AgentConfiguration
{
    InputValidationLevel = InputValidationLevel.Warn,
});

agent.InvalidComponentAdded += (deviceUuid, component, result) =>
    Console.Error.WriteLine($"[{deviceUuid}] invalid Component {component.Type}: {result.Message}");

agent.InvalidCompositionAdded += (deviceUuid, composition, result) =>
    Console.Error.WriteLine($"[{deviceUuid}] invalid Composition {composition.Type}: {result.Message}");

agent.InvalidDataItemAdded += (deviceUuid, dataItem, result) =>
    Console.Error.WriteLine($"[{deviceUuid}] invalid DataItem {dataItem.Type}: {result.Message}");

agent.InvalidObservationAdded += (deviceUuid, dataItemKey, result) =>
    Console.Error.WriteLine($"[{deviceUuid}] invalid observation for {dataItemKey}: {result.Message}");

agent.InvalidAssetAdded += (asset, result) =>
    Console.Error.WriteLine($"invalid Asset {asset.AssetId}: {result.Message}");
```

### Handler signatures

The delegates are all defined in [`MTConnect.Delegates`](/api/MTConnect/Delegates) and follow a uniform shape — the offending element, plus a `ValidationResult` (or `AssetValidationResult` for assets) describing what failed:

```csharp
public delegate void MTConnectComponentValidationHandler(string deviceUuid, IComponent component, ValidationResult validationResults);
public delegate void MTConnectCompositionValidationHandler(string deviceUuid, IComposition composition, ValidationResult validationResults);
public delegate void MTConnectDataItemValidationHandler(string deviceUuid, IDataItem dataItem, ValidationResult validationResults);
public delegate void MTConnectObservationValidationHandler(string deviceUuid, string dataItemKey, ValidationResult validationResults);
public delegate void MTConnectAssetValidationHandler(IAsset asset, AssetValidationResult validationResults);
```

`InvalidObservationAdded` carries the `DataItemKey` (a string) rather than an `IDataItem`, because the failure mode includes the case where the key did not resolve to a DataItem at all.

### What happens to the rejected input

The handler runs first; what the agent does next depends on `InputValidationLevel`:

- **`Ignore`** — the event does not fire, and the input is kept. Useful only for debugging.
- **`Warn`** — the event fires; the input is kept.
- **`Remove`** — the event fires; the offending node is pruned from its parent (e.g. `device.RemoveDataItem(id)`).
- **`Strict`** — the event fires; the entire Device is rejected (the `AddDevice` call returns `false` and no part of the tree is added).

## Contributor POV

The event family is designed to grow. When a new element class becomes validatable, follow the established pattern so consumers can wire it the same way they wire every other entry.

### Naming convention

- Event name: `Invalid<Noun>Added`, e.g. `InvalidComponentAdded`, `InvalidDeviceAdded`.
- Delegate name: `MTConnect<Noun>ValidationHandler`, e.g. `MTConnectComponentValidationHandler`, `MTConnectDeviceValidationHandler`.
- The validation result carries the description of *why* the input is invalid; the noun carries the *what*.

### Wiring a new entry

1. Add the delegate to [`libraries/MTConnect.NET-Common/Delegates.cs`](https://github.com/TrakHound/MTConnect.NET/blob/master/libraries/MTConnect.NET-Common/Delegates.cs). The first parameter is normally the `deviceUuid`; the second is the offending element; the third is the `ValidationResult`. (`InvalidAssetAdded` is the documented exception — assets are not tied to a single device, so the asset itself stands in for the device UUID.)
2. Add the event to `MTConnectAgent` next to the existing five, with an XML doc-comment that mirrors the others (`/// <summary>Raised when an Invalid <Noun> is Added</summary>`).
3. At the Add* call site, raise the event when the validation result fails and `_configuration.InputValidationLevel > InputValidationLevel.Ignore`:

   ```csharp
   if (!validationResult.IsValid)
   {
       if (_configuration.InputValidationLevel > InputValidationLevel.Ignore)
       {
           InvalidDeviceModelAdded?.Invoke(deviceUuid, deviceModel, validationResult);
       }
       if (_configuration.InputValidationLevel == InputValidationLevel.Strict) return null;
   }
   ```

4. Add a test that exercises both halves of the contract:

   ```csharp
   [Test]
   public void InvalidDeviceModelAdded_fires_and_the_device_is_not_added()
   {
       var agent = new MTConnectAgent(new AgentConfiguration { InputValidationLevel = InputValidationLevel.Strict });
       var fired = false;
       agent.InvalidDeviceModelAdded += (_, _, _) => fired = true;

       var ok = agent.AddDevice(BrokenDeviceModelFixture());

       Assert.That(fired, Is.True);
       Assert.That(ok,    Is.False);
       Assert.That(agent.GetDevices(), Is.Empty);
   }
   ```

### Cross-reference

- [TrakHound/MTConnect.NET#169](https://github.com/TrakHound/MTConnect.NET/pull/169) — adds `InvalidDeviceAdded` and is the canonical worked example of extending the family.
- [`MTConnectAgent`](/api/MTConnect.Agents/MTConnectAgent) — the surface where every entry lives.
- [`InputValidationLevel`](/api/MTConnect.Configurations/InputValidationLevel) — the agent-wide knob that gates whether the family fires at all.
