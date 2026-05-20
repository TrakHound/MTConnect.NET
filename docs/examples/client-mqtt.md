# MQTT client

- **Source path** — `examples/MTConnect.NET-Client-MQTT/`
- **Project type** — .NET console application
- **NuGet package** — [`MTConnect.NET-MQTT`](https://www.nuget.org/packages/MTConnect.NET-MQTT)

## Purpose

Demonstrates the consumer side of an MTConnect-over-MQTT topic tree. The example subscribes to a broker that an agent publishes to (typically through the `mqtt-relay` or `mqtt-broker` agent modules) and prints each device and observation it receives.

## Build and run

Stand up an agent that publishes to MQTT — the shortest path is enabling the `mqtt-relay` module in `agent.config.yaml` and pointing it at a local Mosquitto or HiveMQ broker. Then:

```
dotnet run --project examples/MTConnect.NET-Client-MQTT
```

The example is hard-coded to `localhost:1883` with `MTConnect` as the topic prefix. Adjust those in `Program.cs` for any other broker.

## How the example is structured

`Program.cs` exposes two client shapes: a document client (default) and an entity client (commented out). Pick one depending on whether the publisher is a relay (Document mode, default) or a broker emitting per-entity messages (Entity mode).

### Document client

```csharp
var config = new MTConnectMqttClientConfiguration();
config.Server = "localhost";
config.Port = 1883;
config.TopicPrefix = "MTConnect";

var client = new MTConnectMqttClient(config);
client.ClientStarted += (s, args) => Console.WriteLine("Client Started");
client.DeviceReceived += (topic, device) =>
    Console.WriteLine($"Device Received : {device.Uuid} : {device.Name}");
client.ObservationReceived += (topic, observation) =>
    Console.WriteLine($"Observation Received : {observation.DataItemId} : {string.Join(';', observation.Values.Select(o => o.Value))}");

client.Start();
```

`MTConnectMqttClient` subscribes to the broker, walks the topic tree under `TopicPrefix`, parses each payload with the JSON-CPPAGENT-MQTT decoder, and dispatches the result through typed event hooks (`DeviceReceived`, `ObservationReceived`, `AssetReceived`).

### Document topic tree

A relay publishes whole envelopes under topics like:

```
MTConnect/Probe/<deviceUuid>
MTConnect/Current/<deviceUuid>
MTConnect/Sample/<deviceUuid>/<sequence>
MTConnect/Asset/<assetId>
```

A broker publishes per-observation messages instead — the topic tree is finer-grained, one topic per DataItem.

### Entity client

Commented out in the example, but useful when the publisher is `mqtt-broker` and emits one MQTT message per observation:

```csharp
var client = new MTConnectMqttClient("localhost", 1883);
client.ObservationReceived += (s, observation) => Console.WriteLine(observation.Uuid);
client.AssetReceived += (s, asset) => Console.WriteLine(asset.Uuid);
client.Start();
```

## When to pick this client

Choose `MTConnectMqttClient` when:

- the agent already publishes to an MQTT broker (cheaper for many-consumer fan-out than each consumer pulling `/sample`),
- the deployment topology has the consumer behind a firewall that allows outbound MQTT but not inbound HTTP, or
- the consumer wants push delivery without long-polling overhead.

## See also

- [MQTT relay module](/modules/mqtt-relay) — publishes whole envelopes (document mode).
- [MQTT broker module](/modules/mqtt-broker) — publishes per-observation messages (entity mode).
- [MQTT protocol overview](/configure/integrations/mqtt-protocol) — the canonical topic tree and message-shape catalogue.
- [JSON-CPPAGENT-MQTT wire format](/wire-formats/json-v2-cppagent-mqtt) — the on-the-wire payload shape.
- [Cookbook: write a JSON-MQTT consumer](/cookbook/write-a-json-mqtt-consumer) — the same flow as a recipe with multi-language samples.
