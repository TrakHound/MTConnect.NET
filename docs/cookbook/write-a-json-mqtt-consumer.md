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

The relay publishes JSON-CPPAGENT-MQTT (a JSON-CPPAGENT envelope wrapped for MQTT delivery). The library's [`JsonCppAgentFormatter`](/api/MTConnect.Formatters.JsonCppAgent/JsonCppAgentFormatter) parses it into typed `IStreamsResponseDocument`, `IDevicesResponseDocument`, and `IAssetsResponseDocument` instances:

```csharp
using MTConnect.Formatters.JsonCppAgent;

client.ApplicationMessageReceivedAsync += async e =>
{
    var payload = e.ApplicationMessage.PayloadSegment.ToArray();
    var topic = e.ApplicationMessage.Topic;

    if (topic.StartsWith("MTConnect/Document/Sample/") ||
        topic.StartsWith("MTConnect/Document/Current/"))
    {
        var streams = new JsonCppAgentFormatter().DeserializeStreamsResponse(payload);
        if (streams is null) return;

        foreach (var deviceStream in streams.Streams.DeviceStreams)
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
    var assets = new JsonCppAgentFormatter().DeserializeAssetsResponse(payload);
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

For one specific DataItem in Entity-structured mode:

```text
MTConnect/Entity/Observations/mill-01/x-pos-actual
```

A consumer that wants only spindle observations from any mill subscribes:

```text
MTConnect/Entity/Observations/mill-+/spindle-+
```

The broker handles the wildcard expansion; the consumer sees only matching topics.

## 6. Reconnect handling

MQTT clients drop connections on network blips. MQTTnet handles reconnect with a managed client wrapper:

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
    var streams = new JsonCppAgentFormatter().DeserializeStreamsResponse(e.ApplicationMessage.PayloadSegment.ToArray());
    foreach (var ds in streams.Streams.DeviceStreams)
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

For InfluxDB specifically, see the [InfluxDB integration page](/configure/integrations/influxdb) for the line-protocol mapping.

## Where to next

- [Cookbook: Configure MQTT relay](/cookbook/configure-mqtt-relay) — the agent side that publishes the messages this consumer reads.
- [Wire formats: JSON-CPPAGENT-MQTT](/wire-formats/json-cppagent-mqtt) — payload shape reference.
- [`JsonCppAgentFormatter` API reference](/api/MTConnect.Formatters.JsonCppAgent/JsonCppAgentFormatter).
- [Configure & Use: integrations: InfluxDB](/configure/integrations/influxdb) — turn this consumer into a bridge.
