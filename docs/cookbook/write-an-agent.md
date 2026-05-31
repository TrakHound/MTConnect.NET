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
device.AddDataItem(new AvailabilityDataItem(device.Id));

// Attach a Controller with CONTROLLER_MODE and EXECUTION data items.
var controller = new ControllerComponent { Id = "ctrl" };
controller.AddDataItem(new ControllerModeDataItem(controller.Id));
controller.AddDataItem(new ExecutionDataItem(controller.Id));
device.AddComponent(controller);
```

`AddComponent` and `AddDataItem` set the parent-pointers and the ID-format templates automatically — you do not need to populate `Container` or `Parent` by hand.

## 3. Create the agent

The agent is a [`MTConnectAgentBroker`](/api/MTConnect.Agents/MTConnectAgentBroker) instance — the broker subclass of [`MTConnectAgent`](/api/MTConnect.Agents/MTConnectAgent) that adds an observation buffer and an asset buffer:

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
using MTConnect.Observations;

agent.AddObservation(device.Uuid, new EventValueObservation
{
    DataItemId = "mill-01-avail",
    Timestamp = DateTime.UtcNow,
    CDATA = "AVAILABLE",
});

agent.AddObservation(device.Uuid, new EventValueObservation
{
    DataItemId = "ctrl-mode",
    Timestamp = DateTime.UtcNow,
    CDATA = "AUTOMATIC",
});

agent.AddObservation(device.Uuid, new EventValueObservation
{
    DataItemId = "ctrl-exec",
    Timestamp = DateTime.UtcNow,
    CDATA = "ACTIVE",
});
```

The agent assigns a monotonically increasing `Sequence` to each observation as it enters the buffer.

## 5. Start the HTTP server

The HTTP server module hosts the MTConnect endpoints. Add a reference to `MTConnect.NET-AgentModule-HttpServer` (already transitive through `MTConnect.NET`) and start it:

```csharp
using MTConnect.Configurations;
using MTConnect.Agents.Modules;

var httpConfig = new HttpServerModuleConfiguration
{
    Hostname = "0.0.0.0",
    Port = 5000,
    IndentOutput = true,
    DocumentFormat = "xml",
};

var httpModule = new HttpServerModule(agent, httpConfig);
await httpModule.StartAsync();

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
        <DataItem category="EVENT" id="mill-01-avail" type="AVAILABILITY"/>
      </DataItems>
      <Components>
        <Controllers id="ctrl-org">
          <Controller id="ctrl">
            <DataItems>
              <DataItem category="EVENT" id="ctrl-mode" type="CONTROLLER_MODE"/>
              <DataItem category="EVENT" id="ctrl-exec" type="EXECUTION"/>
            </DataItems>
          </Controller>
        </Controllers>
      </Components>
    </Device>
  </Devices>
</MTConnectDevices>
```

Note that the `Controllers` Organizer wrapped the Controller automatically — `Device.AddComponent` walks [`MTConnect.Devices.Organizers`](/api/MTConnect.Devices/Organizers) on insert and creates the Organizer where one is required.

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

## Where to next

- **Add an adapter**: see [Cookbook: Write an adapter](/cookbook/write-an-adapter) for the ingestion side.
- **Switch to JSON output**: set `documentFormat: "json-cppAgent"` in the HTTP config and `Accept: application/mtconnect+json` on the request. See [Wire formats: JSON-CPPAGENT](/wire-formats/json-cppagent).
- **Persist the buffer**: pass an [`IAgentConfiguration`](/api/MTConnect.Configurations/IAgentConfiguration) with `Durable = true` to enable on-disk durability.
- **Run it via YAML**: see [Configure an agent](/configure/agent-config) for the configuration-driven path that skips writing C# entirely.
- **Add an MQTT relay**: see [Cookbook: Configure MQTT relay](/cookbook/configure-mqtt-relay).
