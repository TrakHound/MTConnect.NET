# Write a JSON-MQTT consumer

This recipe builds a consumer that subscribes to an MTConnect agent's MQTT relay output, parses the JSON-CPPAGENT-MQTT payloads, and surfaces the observations. By the end you have:

- A dotnet console application connecting to an MQTT broker.
- Subscription to the relay's per-Device topics.
- Parsing of the JSON envelope into typed `IObservation` instances.
- A Python alternative for non-dotnet consumers.

## 1. Set up the dotnet consumer

```sh
dotnet new console -o MyConsumer
cd MyConsumer
dotnet add package MQTTnet
dotnet add package MTConnect.NET-JSON-cppagent
```

The `MQTTnet` package is the MQTT client; `MTConnect.NET-JSON-cppagent` is the codec that parses JSON-CPPAGENT envelopes into the library's type system.

## 2. Connect and subscribe

```csharp
using MQTTnet;
using MQTTnet.Client;
using System.Text;

var factory = new MqttFactory();
var client = factory.CreateMqttClient();

var options = new MqttClientOptionsBuilder()
    .WithTcpServer("broker.example.com", 1883)
    .WithCredentials("consumer-01", "<secret>")
    .WithClientId("consumer-01")
    .WithCleanSession(false)
    .Build();

await client.ConnectAsync(options);

// Subscribe to every Sample envelope from every Device.
await client.SubscribeAsync(
    new MqttTopicFilterBuilder()
        .WithTopic("MTConnect/Document/Sample/+")
        .WithAtLeastOnceQoS()
        .Build());

// Also subscribe to Current envelopes for the bootstrap snapshot.
await client.SubscribeAsync(
    new MqttTopicFilterBuilder()
        .WithTopic("MTConnect/Document/Current/+")
        .WithAtLeastOnceQoS()
        .Build());

Console.WriteLine("Connected; awaiting payloads. Ctrl-C to exit.");
```

## 3. Parse the payload

The relay publishes JSON-CPPAGENT-MQTT (a JSON-CPPAGENT envelope wrapped for MQTT delivery). The library's [`JsonMqttResponseDocumentFormatter`](/api/MTConnect.Formatters.JsonMqttResponseDocumentFormatter) parses it into typed `IStreamsResponseDocument`, `IDevicesResponseDocument`, and `IAssetsResponseDocument` instances:

```csharp
using MTConnect.Formatters;
using MTConnect.Observations;
using System.IO;

var formatter = new JsonMqttResponseDocumentFormatter();

client.ApplicationMessageReceivedAsync += async e =>
{
    var payload = e.ApplicationMessage.PayloadSegment.ToArray();
    var topic = e.ApplicationMessage.Topic;

    if (topic.StartsWith("MTConnect/Document/Sample/") ||
        topic.StartsWith("MTConnect/Document/Current/"))
    {
        using var stream = new MemoryStream(payload);
        var result = formatter.CreateStreamsResponseDocument(stream);
        var streams = result.Content;
        if (streams is null) return;

        foreach (var deviceStream in streams.Streams)
        {
            foreach (var componentStream in deviceStream.ComponentStreams)
            {
                foreach (var obs in componentStream.Observations)
                {
                    Console.WriteLine(
                        $"{deviceStream.Uuid} {obs.DataItemId} = {obs.GetValue(ValueKeys.Result)} @ {obs.Timestamp:O}");
                }
            }
        }
    }

    await Task.CompletedTask;
};
```

A `Sample` payload contains the delta since the last publish — observations that happened in the last `sampleInterval` ms. A `Current` payload contains the most-recent observation per DataItem at publish time.

## 4. Watch for asset changes

Assets arrive on a separate topic. Subscribe and parse:

```csharp
await client.SubscribeAsync(
    new MqttTopicFilterBuilder()
        .WithTopic("MTConnect/Document/Asset/+/+")
        .Build());

// In the message handler:
if (topic.StartsWith("MTConnect/Document/Asset/"))
{
    using var stream = new MemoryStream(payload);
    var assets = formatter.CreateAssetsResponseDocument(stream).Content;
    foreach (var asset in assets.Assets)
    {
        Console.WriteLine($"Asset {asset.AssetId} type={asset.Type} removed={asset.Removed}");
    }
}
```

## 5. Filter by Device or DataItem

The single-plus wildcards in the topic patterns scope subscription. For one specific Device:

```text
MTConnect/Document/Sample/mill-01
```

For one specific DataItem in Entity-structured mode (`topicStructure: Entity`, which publishes under `<topicPrefix>/Devices/<deviceUuid>/Observations/<dataItemId>`):

```text
MTConnect/Devices/mill-01/Observations/x-pos-actual
```

A consumer that wants every observation from every Device subscribes:

```text
MTConnect/Devices/+/Observations/#
```

The broker handles the wildcard expansion; the consumer sees only matching topics. MQTT wildcards match whole topic levels only (`mill-+` is not a valid filter), so narrower matching on part of a Device or DataItem ID belongs in the message handler.

## 6. Reconnect handling

MQTT clients drop connections on network blips. MQTTnet handles reconnect with a managed client wrapper, which ships in the separate `MQTTnet.Extensions.ManagedClient` package (`dotnet add package MQTTnet.Extensions.ManagedClient`):

```csharp
var managed = factory.CreateManagedMqttClient();

await managed.StartAsync(new ManagedMqttClientOptionsBuilder()
    .WithClientOptions(options)
    .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
    .Build());

await managed.SubscribeAsync(/* topics ... */);
```

The managed client reconnects automatically and re-establishes subscriptions. Pair it with `cleanSession: false` on the relay so the broker delivers messages that were published while the consumer was disconnected.

## 7. Python consumer

For non-dotnet integrations, the same payloads parse cleanly in Python:

```python
import paho.mqtt.client as mqtt
import json

def on_connect(client, userdata, flags, rc):
    print(f"Connected; rc={rc}")
    client.subscribe("MTConnect/Document/Sample/+")
    client.subscribe("MTConnect/Document/Current/+")

def on_message(client, userdata, msg):
    payload = json.loads(msg.payload)
    streams = payload.get("MTConnectStreams", {})
    for device_stream in streams.get("Streams", {}).get("DeviceStream", []):
        for component_stream in device_stream.get("ComponentStream", []):
            for category in ("Samples", "Events", "Condition"):
                items = component_stream.get(category, [])
                for obs in items:
                    # obs is a single-key wrapper: {"AvailabilityType": {...}}
                    for type_name, body in obs.items():
                        data_item_id = body.get("dataItemId")
                        value = body.get("Value") or body.get("value")
                        ts = body.get("timestamp")
                        print(f"{device_stream['uuid']} {data_item_id} = {value} @ {ts}")

client = mqtt.Client(client_id="consumer-py")
client.username_pw_set("consumer-01", "<secret>")
client.on_connect = on_connect
client.on_message = on_message
client.connect("broker.example.com", 1883, 60)
client.loop_forever()
```

The JSON-CPPAGENT shape — each observation wrapped in `{"<type>": {...}}` — is unambiguous to parse; the outer key names the spec type, the inner body carries `dataItemId`, `timestamp`, `value` / `Value`, and any DATA_SET / TABLE entries.

## 8. Bridging into a database

A consumer that bridges observations into a time-series database queues them and bulk-inserts:

```csharp
using System.Threading.Channels;

var queue = Channel.CreateBounded<IObservation>(10_000);

client.ApplicationMessageReceivedAsync += async e =>
{
    using var stream = new MemoryStream(e.ApplicationMessage.PayloadSegment.ToArray());
    var streams = formatter.CreateStreamsResponseDocument(stream).Content;
    foreach (var ds in streams.Streams)
        foreach (var cs in ds.ComponentStreams)
            foreach (var obs in cs.Observations)
                await queue.Writer.WriteAsync(obs);
};

_ = Task.Run(async () =>
{
    var batch = new List<IObservation>(500);
    await foreach (var obs in queue.Reader.ReadAllAsync())
    {
        batch.Add(obs);
        if (batch.Count >= 500)
        {
            await InsertBatch(batch);
            batch.Clear();
        }
    }
});
```

For InfluxDB specifically, see the [InfluxDB integration page](/configure/integrations/#influxdb) for the line-protocol mapping.

## Use the built-in MQTT client

Steps 2–6 wire MQTTnet and the formatter together by hand. The `MTConnect.NET-MQTT` package ships [`MTConnectMqttClient`](/api/MTConnect.Clients.MTConnectMqttClient), which does the subscribing, topic dispatch, and JSON-CPPAGENT-MQTT parsing for you and raises typed events. The runnable sample in `examples/MTConnect.NET-Client-MQTT/` uses it:

```sh
dotnet add package MTConnect.NET-MQTT
```

```csharp
using MTConnect.Clients;
using MTConnect.Configurations;

var config = new MTConnectMqttClientConfiguration();
config.Server = "localhost";
config.Port = 1883;
config.TopicPrefix = "MTConnect/Document"; // must match the relay's topicPrefix

var client = new MTConnectMqttClient(config);
client.ClientStarted += (s, args) => Console.WriteLine("Client Started");
client.DeviceReceived += (s, device) =>
    Console.WriteLine($"Device Received : {device.Uuid} : {device.Name}");
client.ObservationReceived += (s, observation) =>
    Console.WriteLine($"Observation Received : {observation.DataItemId} : {string.Join(";", observation.Values.Select(o => o.Value))}");
client.AssetReceived += (s, asset) =>
    Console.WriteLine($"Asset Received : {asset.AssetId}");

client.Start();
```

There is also a shorthand constructor, `new MTConnectMqttClient("localhost", 1883, topicPrefix: "MTConnect/Document")`. Besides the per-entity events, the client raises whole-document events (`ProbeReceived`, `CurrentReceived`, `SampleReceived`, `AssetsReceived`) for consumers that want the envelope rather than individual observations.

The client subscribes to the Document-structured topic tree under `TopicPrefix`:

```text
<topicPrefix>/Probe/<deviceUuid>
<topicPrefix>/Current/<deviceUuid>
<topicPrefix>/Sample/<deviceUuid>
<topicPrefix>/Asset/<deviceUuid>/<assetId>
```

Set `config.DeviceUuid` to scope the subscription to one Device; leave it empty to receive every Device. Two things to line up with the publishing agent:

- **Topic structure** — `MTConnectMqttClient` only understands `topicStructure: Document` (the `mqtt-relay` and `mqtt-broker` modules both default to it). For `topicStructure: Entity`, use the raw MQTTnet approach from steps 2–5, or `MTConnectMqttExpandedClient`, which reads the per-entity `MTConnect/Devices/...` tree.
- **Document format** — the client decodes payloads as `json-cppagent-mqtt` by default, so set `documentFormat: JSON-CPPAGENT-MQTT` on the publishing module.

Pick the MQTT client over polling the agent's HTTP `/sample` endpoint when the agent already publishes to a broker (cheaper fan-out to many consumers), when a firewall allows outbound MQTT but not inbound HTTP to the agent, or when you want push delivery without long-polling.

## Where to next

- [Cookbook: Configure MQTT relay](/cookbook/configure-mqtt-relay) — the agent side that publishes the messages this consumer reads.
- [Wire formats: JSON-CPPAGENT-MQTT](/wire-formats/json-v2-cppagent-mqtt) — payload shape reference.
- [MQTT relay module](/modules/mqtt-relay) and [MQTT broker module](/modules/mqtt-broker) — the two agent modules that publish to MQTT.
- [MQTT protocol overview](/wire-formats/mqtt) — the canonical topic tree and message-shape catalogue.
- [`JsonMqttResponseDocumentFormatter` API reference](/api/MTConnect.Formatters.JsonMqttResponseDocumentFormatter).
- [Configure & Use: integrations: InfluxDB](/configure/integrations/#influxdb) — turn this consumer into a bridge.
