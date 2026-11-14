# Write an agent

This recipe walks through building an MTConnect agent in code, end to end. By the end you have a running process that:

- Holds a Device with a Controller component and a few DataItems.
- Accepts observations.
- Serves `/probe`, `/current`, and `/sample` over HTTP on port 5000.

The complete code is small: about 60 lines of C# plus a `*.csproj` reference.

## 1. Set up the project

Create a new console project and add the meta package:

```sh
dotnet new console -o MyAgent
cd MyAgent
dotnet add package MTConnect.NET
```

The `MTConnect.NET` meta package transitively pulls every shipped library, including `MTConnect.NET-Common`, `MTConnect.NET-HTTP`, and `MTConnect.NET-XML`.

## 2. Build the Device

In `Program.cs`:

```csharp
using MTConnect.Devices;
using MTConnect.Devices.Components;
using MTConnect.Devices.DataItems;

// Create a Device with a Controller and an Availability data item on each.
var device = new Device
{
    Id = "mill-01",
    Uuid = "1234-5678-9abc-def0",
    Name = "Mill #1",
    Type = Device.TypeId,
};

// Attach an AVAILABILITY data item on the Device itself.
var availability = new AvailabilityDataItem(device.Id);   // id "mill-01_availability"
device.AddDataItem(availability);

// Attach a Controller with CONTROLLER_MODE and EXECUTION data items.
var controller = new ControllerComponent { Id = "ctrl" };
var mode = new ControllerModeDataItem(controller.Id);      // id "ctrl_mode"
var execution = new ExecutionDataItem(controller.Id);      // id "ctrl_exec"
controller.AddDataItem(mode);
controller.AddDataItem(execution);
device.AddComponent(controller);
```

`AddComponent` and `AddDataItem` set the parent-pointers automatically — you do not need to populate `Container` or `Parent` by hand. The `(parentId)` constructors build each DataItem ID as `<parentId>_<name>`.

## 3. Create the agent

The agent is a [`MTConnectAgentBroker`](/api/MTConnect.Agents.MTConnectAgentBroker) instance — the broker subclass of [`MTConnectAgent`](/api/MTConnect.Agents.MTConnectAgent) that adds an observation buffer and an asset buffer:

```csharp
using MTConnect.Agents;

var agent = new MTConnectAgentBroker(
    uuid: "agent-uuid-01",
    instanceId: (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
    deviceModelChangeTime: DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
    initializeAgentDevice: true);

agent.AddDevice(device);
```

The `initializeAgentDevice: true` argument auto-registers the self-describing Agent Device that emits agent-introspection observations.

## 4. Push observations

A single observation lifecycle:

```csharp
agent.AddObservation(device.Uuid, availability.Id, "AVAILABLE", DateTime.UtcNow);
agent.AddObservation(device.Uuid, mode.Id, "AUTOMATIC", DateTime.UtcNow);
agent.AddObservation(device.Uuid, execution.Id, "ACTIVE", DateTime.UtcNow);
```

The second argument is a DataItem key: the DataItem's `id`, `name`, or `source` all resolve. Omit the timestamp to stamp the observation with the current time.

The agent assigns a monotonically increasing `Sequence` to each observation as it enters the buffer.

## 5. Start the HTTP server

[`MTConnectHttpServer`](/api/MTConnect.Servers.Http.MTConnectHttpServer) from `MTConnect.NET-HTTP` (already transitive through `MTConnect.NET`) hosts the MTConnect endpoints for an agent broker:

```csharp
using MTConnect.Configurations;
using MTConnect.Servers.Http;

var httpConfig = new HttpServerConfiguration
{
    Port = 5000,          // leave Server unset to listen on every interface
    IndentOutput = true,
    DocumentFormat = "xml",
};

var httpServer = new MTConnectHttpServer(httpConfig, agent);
httpServer.Start();

Console.WriteLine("Agent running at http://localhost:5000/probe");
Console.WriteLine("Press Ctrl-C to exit.");
await Task.Delay(Timeout.Infinite);
```

## 6. Hit the endpoints

In another terminal:

```sh
curl -s http://localhost:5000/probe | head -30
curl -s http://localhost:5000/current | head -30
curl -s http://localhost:5000/sample?count=10 | head -30
```

`/probe` returns the Device model — Components, Compositions, DataItems — wrapped in a `MTConnectDevices` envelope. `/current` returns the most-recent observation per DataItem. `/sample` returns the observation history.

A `/probe` response looks like:

```xml
<MTConnectDevices xmlns="urn:mtconnect.org:MTConnectDevices:2.5">
  <Header creationTime="2025-01-01T12:34:56Z" sender="agent-01"
          instanceId="1735734896" version="2.5.0.0" assetBufferSize="1000"
          assetCount="0" bufferSize="150000"/>
  <Devices>
    <Device id="mill-01" uuid="1234-5678-9abc-def0" name="Mill #1">
      <DataItems>
        <DataItem category="EVENT" id="mill-01_availability" name="availability" type="AVAILABILITY"/>
      </DataItems>
      <Components>
        <Controller id="ctrl">
          <DataItems>
            <DataItem category="EVENT" id="ctrl_mode" name="mode" type="CONTROLLER_MODE"/>
            <DataItem category="EVENT" id="ctrl_exec" name="exec" type="EXECUTION"/>
          </DataItems>
        </Controller>
      </Components>
    </Device>
  </Devices>
</MTConnectDevices>
```

`Device.AddComponent` consults [`MTConnect.Devices.Organizers`](/api/MTConnect.Devices.Organizers) on insert and creates the Organizer a Component type requires (for example `Axes` around a `Linear`). `Controller` is the exception: it is added directly under `Components`, without a `Controllers` wrapper.

## 7. Test it

A minimal smoke test using `dotnet`'s built-in HTTP client:

```csharp
using System.Net.Http;

var http = new HttpClient { BaseAddress = new Uri("http://localhost:5000") };
var probe = await http.GetStringAsync("/probe");
Console.WriteLine(probe);
```

The HTTP client returns the same XML envelope the agent serves to curl.

## What you have now

- A running agent with one Device, one Controller, and three EVENT DataItems.
- Observations entering the buffer with monotonic sequence numbers.
- The four MTConnect endpoints (`/probe`, `/current`, `/sample`, `/asset`) served over HTTP.

## Embedded agent

Steps 2–5 wire the broker and the HTTP server by hand. The alternative is to host the full agent application inside your process and plug your data source in as an agent module. The agent then reads `agent.config.yaml`, loads the `http-server` module (and any others) from it, and your module writes observations into the agent directly — no separate SHDR adapter and no TCP hop between adapter and agent. Pick this shape when the data source is .NET code you control (a PLC poller, a database tail) and you want the standalone agent's configuration, logging, and module set without writing them yourself.

The runnable version lives in `examples/MTConnect.NET-Agent-Embedded/`:

```sh
dotnet run --project examples/MTConnect.NET-Agent-Embedded
```

In your own project, reference the agent host package:

```sh
dotnet add package MTConnect.NET-Applications-Agents
```

### Entry point

```csharp
using MTConnect.Applications;

var app = new MTConnectAgentApplication();
app.Run(args, true);
```

`MTConnectAgentApplication` is the same host the standalone agent uses. `Run(args, true)` reads `agent.config.yaml` from the working directory, loads the modules listed in it — including module classes in your own assembly — and blocks until shutdown.

### The data-source module

```csharp
using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Devices.Components;
using MTConnect.Devices.DataItems;
using MTConnect.Observations.Events;

public class ModuleConfiguration
{
    public string DeviceUuid { get; set; }
    public string DeviceName { get; set; }
    public string SerialNumber { get; set; }
}

public class Module : MTConnectInputAgentModule
{
    public const string ConfigurationTypeId = "datasource"; // matches the key in agent.config.yaml
    private readonly ModuleConfiguration _configuration;

    public Module(IMTConnectAgentBroker agent, object configuration) : base(agent)
    {
        Id = "DataSource Module"; // used for logging
        _configuration = AgentApplicationConfiguration.GetConfiguration<ModuleConfiguration>(configuration);
    }

    // Called once at start-up: return the Device to register with the agent.
    protected override IDevice OnAddDevice()
    {
        var device = new Device();
        device.Uuid = _configuration.DeviceUuid;
        device.Id = _configuration.DeviceName;
        device.Name = _configuration.DeviceName;
        device.AddDataItem<AvailabilityDataItem>();

        var controller = new ControllerComponent { Name = ControllerComponent.NameId };
        controller.AddDataItem<EmergencyStopDataItem>();

        var path = new PathComponent { Name = PathComponent.NameId };
        path.AddDataItem<ExecutionDataItem>();
        path.AddDataItem<SystemDataItem>();
        controller.AddComponent(path);
        device.AddComponent(controller);

        var axes = new AxesComponent { Name = AxesComponent.NameId };
        foreach (var name in new[] { "X", "Y", "Z" })
        {
            var axis = new LinearComponent { Name = name };
            axis.AddDataItem<PositionDataItem>(PositionDataItem.SubTypes.ACTUAL);
            axis.AddDataItem<LoadDataItem>();
            axes.AddComponent(axis);
        }
        device.AddComponent(axes);

        return device;
    }

    // Called every readInterval milliseconds: read the data source and emit observations.
    protected override void OnRead()
    {
        AddValueObservation<AvailabilityDataItem>(Availability.AVAILABLE);
        AddValueObservation<ControllerComponent, EmergencyStopDataItem>(EmergencyStop.ARMED);
        AddValueObservation<LinearComponent, PositionDataItem>(0.0002, "X", PositionDataItem.SubTypes.ACTUAL);
        AddValueObservation<LinearComponent, LoadDataItem>(2, "X");
        AddConditionObservation<PathComponent, SystemDataItem>(
            MTConnect.Observations.ConditionLevel.WARNING, "404", "This is an Alarm");
    }
}
```

[`MTConnectInputAgentModule`](/api/MTConnect.Agents.MTConnectInputAgentModule) is the base class for modules that produce observations. It has two hooks:

- `OnAddDevice()` — runs once at start-up; the returned `IDevice` is added to the agent and appears in `/probe`. Building the model in code (rather than loading a static `devices.xml`) suits devices whose shape depends on what the data source reports — variable axis counts, optional sensors, serial numbers read at boot.
- `OnRead()` — runs every `readInterval` milliseconds. `AddValueObservation<TComponent, TDataItem>(value, componentName, subType)` finds the DataItem by Component type, Component name, and DataItem type, so the three axes share one call shape and differ only in the axis name. `AddConditionObservation` does the same for CONDITION DataItems.

`AgentApplicationConfiguration.GetConfiguration<T>` deserializes the module's YAML section into your configuration class.

### The configuration file

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

The `datasource` key matches `ConfigurationTypeId`; the fields under it map onto `ModuleConfiguration`, plus the inherited `readInterval`. Copy the file to the build output (`<None Update="agent.config.yaml"><CopyToOutputDirectory>Always</CopyToOutputDirectory></None>` in the `*.csproj`) so the agent finds it at run time, then check the result with `curl http://localhost:5000/probe` and `curl http://localhost:5000/current`.

To publish to MQTT as well, add an `mqtt-relay` section (see [Cookbook: Configure MQTT relay](/cookbook/configure-mqtt-relay)). To package a module as a reusable NuGet package, see [Cookbook: Write a module](/cookbook/write-a-module).

## Where to next

- **Add an adapter**: see [Cookbook: Write an adapter](/cookbook/write-an-adapter) for the ingestion side.
- **Switch to JSON output**: set `documentFormat: "json-cppAgent"` in the HTTP config and `Accept: application/mtconnect+json` on the request. See [Wire formats: JSON-CPPAGENT](/wire-formats/json-v2-cppagent).
- **Persist the buffer**: set `durable: true` in `agent.config.yaml` (the [`IAgentApplicationConfiguration.Durable`](/api/MTConnect.Configurations.IAgentApplicationConfiguration) setting) when the agent runs under `MTConnectAgentApplication`, as in [Embedded agent](#embedded-agent).
- **Run it via YAML**: see [Configure an agent](/configure/agent-config) for the configuration-driven path that skips writing C# entirely.
- **Add an MQTT relay**: see [Cookbook: Configure MQTT relay](/cookbook/configure-mqtt-relay).
