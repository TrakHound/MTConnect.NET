# Embedded agent

- **Source path** — `examples/MTConnect.NET-Agent-Embedded/`
- **Project type** — .NET console application
- **NuGet package** — [`MTConnect.NET-Applications-Agents`](https://www.nuget.org/packages/MTConnect.NET-Applications-Agents)
- **Configuration** — `examples/MTConnect.NET-Agent-Embedded/agent.config.yaml`

## Purpose

Hosts an MTConnect agent inside the same .NET process as the data source, so the data source writes observations into the agent directly. This is the integration shape to pick when:

- the data source is .NET code you control (a PLC poller, a database tail, another MTConnect agent), and
- a separate SHDR adapter and a TCP hop between adapter and agent would add latency or operational surface without a corresponding benefit.

The example builds a synthetic CNC device model in code (a controller, a path, three linear axes) and emits a fresh observation set on each read tick.

## Build and run

```
dotnet run --project examples/MTConnect.NET-Agent-Embedded
```

The agent loads `agent.config.yaml` from the working directory, starts the HTTP server on port `5000`, and begins emitting observations on the `readInterval` cadence configured for the `datasource` module. Probe the running agent with:

```
curl http://localhost:5000/probe
curl http://localhost:5000/current
```

## How the example is structured

`Program.cs` is a single file with three pieces: the entry point, a custom configuration class, and the module that hosts the device model and the read loop.

### Entry point

```csharp
var app = new MTConnectAgentApplication();
app.Run(args, true);
```

`MTConnectAgentApplication` is the same host the standalone `mtconnect-agent` CLI uses. Calling `Run(args, true)` starts the agent, reads `agent.config.yaml`, loads any modules registered in that file, and blocks until shutdown.

### Custom module configuration

```csharp
public class ModuleConfiguration
{
    public string DeviceUuid { get; set; }
    public string DeviceName { get; set; }
    public string SerialNumber { get; set; }
}
```

Each module can declare its own configuration shape. The agent picks the matching YAML section out of `agent.config.yaml` based on `ConfigurationTypeId` and deserializes it into the class.

### Agent module

```csharp
public class Module : MTConnectInputAgentModule
{
    public const string ConfigurationTypeId = "datasource";

    public Module(IMTConnectAgentBroker agent, object configuration) : base(agent)
    {
        _configuration = AgentApplicationConfiguration.GetConfiguration<ModuleConfiguration>(configuration);
    }
}
```

`MTConnectInputAgentModule` is the base class for modules that produce observations. The agent calls two virtual hooks at start-up and on every read tick:

- `OnAddDevice()` — return an `IDevice` once at start-up; the agent registers it with the probe response.
- `OnRead()` — called every `readInterval` milliseconds; emit observations with `AddValueObservation` and `AddConditionObservation`.

### Building the device model in code

```csharp
protected override IDevice OnAddDevice()
{
    var device = new Device();
    device.Uuid = _configuration.DeviceUuid;
    device.Name = _configuration.DeviceName;
    device.AddDataItem<AvailabilityDataItem>();

    AddController(device);
    AddAxes(device);

    return device;
}
```

Constructing the model from code (rather than loading a static `devices.xml`) is the right path when the shape of the device depends on what the data source reports — variable axis counts, optional sensors, asset serial numbers that change at boot.

### Emitting observations

```csharp
protected override void OnRead()
{
    AddValueObservation<AvailabilityDataItem>(Availability.AVAILABLE);
    AddValueObservation<ControllerComponent, EmergencyStopDataItem>(EmergencyStop.ARMED);
    AddValueObservation<LinearComponent, PositionDataItem>(0.0002, "X", PositionDataItem.SubTypes.ACTUAL);
    AddConditionObservation<PathComponent, SystemDataItem>(
        MTConnect.Observations.ConditionLevel.WARNING, "404", "This is an Alarm");
}
```

`AddValueObservation` accepts the DataItem type and an optional component name, so the same code can target multiple instances of the same component (the three linear axes here share the call shape and differ only in the axis name).

## Configuring the read cadence and device identity

```yaml
modules:

- datasource:
    deviceUuid: 7E647B2D-C6A3-40BF-9CE9-FB09834850C9
    deviceName: dev-001
    serialNumber: 123456
    readInterval: 1000

- http-server:
    port: 5000
```

The `datasource` key matches `ConfigurationTypeId = "datasource"`; the fields below it map onto `ModuleConfiguration` and onto the inherited `readInterval` property.

## See also

- [Cookbook: write an agent](/cookbook/write-an-agent) — the same pattern, recast as a step-by-step recipe.
- [Cookbook: write a module](/cookbook/write-a-module) — how to build a reusable module that ships as a NuGet package.
- [HTTP server module](/modules/http-server) — the agent module that serves `/probe`, `/current`, and `/sample`.
- [Agent configuration](/configure/agent-config) — every key the agent reads from `agent.config.yaml`.
