# HTTP client

- **Source path** — `examples/MTConnect.NET-Client-HTTP/`
- **Project type** — .NET console application
- **NuGet package** — [`MTConnect.NET-HTTP`](https://www.nuget.org/packages/MTConnect.NET-HTTP)

## Purpose

Demonstrates the consumer side of the MTConnect HTTP protocol: connect to a running agent, fetch the device model from `/probe`, then subscribe to a long-polling sample stream and surface each observation to application code via events.

## Build and run

Start an agent (the [embedded-agent example](/examples/agent-embedded) is the shortest path), then in a second shell:

```
dotnet run --project examples/MTConnect.NET-Client-HTTP
```

The example prompts for the hostname and port of the agent on standard input. With the embedded example running:

```
Enter Hostname:
localhost
Enter Port:
5000
Connecting to (localhost:5000)..
Client Started
Device Received : 7E647B2D-C6A3-40BF-9CE9-FB09834850C9 : dev-001 : 2.7
Observation Received : availability : AVAILABLE @ 2026-05-15T11:08:43.0000000Z
...
```

## How the example is structured

`Program.cs` exposes two client shapes: a document-level client (default) and an entity-level client (commented out). The document client mirrors what most consumers want — read full `Devices`, `Streams`, and `Assets` envelopes; subscribe to event streams.

### Document client

```csharp
var client = new MTConnectHttpClient(hostname, port);
client.Interval = 100;

client.ClientStarted += (s, args) => Console.WriteLine("Client Started");
client.ClientStopped += (s, args) => Console.WriteLine("Client Stopped");
client.FormatError   += (s, args) => Console.WriteLine($"Format Error : {args.ContentType.Name} : {args.Messages?.FirstOrDefault()}");

client.ProbeReceived   += (s, response) => { /* iterate response.Devices */ };
client.CurrentReceived += (s, response) => { /* iterate response.Streams */ };

client.Start();
```

`MTConnectHttpClient` wraps the agent's HTTP endpoints (`/probe`, `/current`, `/sample`, `/asset`) and exposes them as a long-polling subscription with event hooks. The `Interval` property is the consumer-side sample interval—the client requests `/sample?interval=100` and receives each sequence batch through `CurrentReceived` (or `SampleReceived` if you wire that hook up instead).

### Subscribing to the wired device model

The client maintains a wired snapshot of every device it sees from `/probe` responses, keyed by UUID. Subscribe to `DeviceReceived` to react per-device while the snapshot is filling, and read the `Devices` accessor at any point to consult the post-probe model:

```csharp
client.DeviceReceived += (s, device) =>
{
    Console.WriteLine($"Device Received : {device.Uuid} : {device.Name}");
    foreach (var dataItem in device.GetDataItems())
    {
        // Container + Device back-pointers are wired in by the parser.
        Console.WriteLine($"  {dataItem.Id} : {dataItem.Type} : container={dataItem.Container?.Id}");
    }
};

client.Start();

// At any point after the first probe completes:
foreach (var (uuid, device) in client.Devices)
{
    Console.WriteLine($"Cached device : {uuid} : {device.Name}");
}
```

`DeviceReceived` fires once per parsed device for every Probe response, carrying the fully wired `IDevice` instance—the same one the `Devices` snapshot accessor returns for that UUID—with the agent's `InstanceId` stamped on each DataItem. A handler that throws is isolated by the client: the exception is forwarded through `InternalError` and the cache fill continues. `Devices` returns a fresh read-only snapshot on every read; cache the reference if you read repeatedly between probes. Devices that disappear between probes are evicted at the start of the next probe, so the snapshot never carries entries the agent no longer advertises.

The `Devices` accessor and the `DeviceReceived` fan-out were added per [issue #176](https://github.com/TrakHound/MTConnect.NET/issues/176) (approved 2026-06-01 by @PatrickRitchie).

### Per-observation validation

The example shows how to validate each observation against its DataItem constraints:

```csharp
var validationResult = observation.Validate();
Console.WriteLine($"Observation Validation : {observation.DataItemId} : {validationResult.IsValid} : {validationResult.Message}");
```

`Observation.Validate()` checks the value against the DataItem's controlled vocabulary (for events with enumerated subtypes), the representation (`VALUE`, `DATA_SET`, `TABLE`, `TIME_SERIES`), and the per-version constraints carried in the SysML model.

## When to pick this client

Choose `MTConnectHttpClient` when you want:

- the full envelope structure (devices, components, observations grouped by component-stream), not just flattened observations,
- automatic long-polling so the consumer reacts to new data without burning a poll loop,
- a single event-driven API surface that mirrors the REST shape one-to-one.

Choose `MTConnectMqttClient` (see the [MQTT client example](/examples/client-mqtt)) instead when the agent publishes through `mqtt-relay` or `mqtt-broker` and you want push delivery without the long-polling overhead.

## See also

- [HTTP server module](/modules/http-server) — the server side of this conversation.
- [Wire formats — XML](/wire-formats/xml) and [JSON v1](/wire-formats/json-v1) / [JSON-CPPAGENT v2](/wire-formats/json-v2-cppagent) — what the envelopes look like on the wire.
- [API reference: `MTConnectHttpClient`](/api/) — every event and property the client exposes.
