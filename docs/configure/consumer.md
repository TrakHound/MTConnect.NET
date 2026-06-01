# Connect a consumer

A running MTConnect agent exposes its device model, observations, and assets over two transports: HTTP (the canonical MTConnect REST protocol) and MQTT (the cppagent-parity broker / relay shape). This page covers both — the endpoints + topics a consumer connects to, the polling vs streaming patterns, and the parsing primitives the library ships for .NET consumers.

The previous page, [Run](./run), assumes the agent is up and serving. This page picks up at "the agent is running; how do I read from it?"

## HTTP — the MTConnect REST protocol

The HTTP server module (`http-server`) exposes four canonical endpoints under the configured port (default `5000`).

| Path | Purpose | Typical use |
|---|---|---|
| `/probe` | Returns `<MTConnectDevices>` — the full device model the agent loaded. | Discovery; the consumer reads this once at startup to learn the available DataItems. |
| `/current` | Returns `<MTConnectStreams>` with the most-recent observation per DataItem. | Snapshot of the live state. |
| `/sample` | Returns `<MTConnectStreams>` with every observation in a sequence range. | Time-series replay; pagination via the `nextSequence` cursor in the response Header. |
| `/asset/<assetId>` | Returns `<MTConnectAssets>` carrying one asset. | Asset retrieval by ID. The `/assets` endpoint (no ID) returns every active asset. |

Each path also accepts a device-scoped prefix: `/MyDevice/probe`, `/MyDevice/current`, `/MyDevice/sample`. The prefix narrows the response to one device's tree.

### Polling

A polling consumer hits `/current` on its own schedule:

```sh
curl -s 'http://agent.local:5000/Mill-1/current'
```

The response is `<MTConnectStreams>`; each `<ComponentStream>` carries the most-recent observation per DataItem. Polling is simplest and tolerates network drops gracefully — every poll is independent.

### Streaming

The `/sample` endpoint supports HTTP long-polling: a request with `interval=<ms>&heartbeat=<ms>` keeps the connection open and pushes a fresh `<MTConnectStreams>` chunk every `interval` milliseconds while the agent has new observations to send.

```sh
curl -N 'http://agent.local:5000/sample?from=12345&interval=100&heartbeat=10000'
```

The server emits an HTTP `multipart/x-mixed-replace` stream; each part is one `<MTConnectStreams>` envelope. Disconnects are detected via the heartbeat (the agent sends a heartbeat chunk every `heartbeat` ms when no observations are pending); a consumer that misses a heartbeat reconnects from the last seen `nextSequence`.

The MTConnect Standard caps the maximum `interval` to keep slow consumers from monopolising the connection. The current agent honours the spec minimum / maximum at `parts/2.0/HttpProtocol.md` (see [Compliance: Wire format](/compliance/wire-format)).

### Spec-version negotiation

A `version=` query parameter pins the envelope shape:

```sh
curl -s 'http://agent.local:5000/probe?version=2.5'
```

Without `version=`, the agent serves its `defaultVersion` (from `agent.config.yaml`, defaulting to `MTConnectVersions.Max`). The version pin determines which DataItems / Asset types / Component types are pruned out of the response — the library walks each entity's `MinimumVersion` / `MaximumVersion` and emits only what the requested version supports.

### Content-type negotiation

The default content type is `application/xml`. The agent also serves JSON when the consumer requests it:

| `Accept` header | Wire format | Codec |
|---|---|---|
| `application/xml` (or no `Accept`) | XML | [`MTConnect.Formatters.Xml.XmlResponseDocumentFormatter`](/api/MTConnect.Formatters.Xml.XmlResponseDocumentFormatter) |
| `application/json` | Legacy JSON v1 | [`MTConnect.Formatters.JsonResponseDocumentFormatter`](/api/MTConnect.Formatters.JsonResponseDocumentFormatter) |

The JSON v2 (cppagent-parity) codec is selected via the `documentFormat:` key on the HTTP server module config rather than through HTTP content negotiation; see [Module configuration: HTTP server](/configure/module-config#http-server) for the per-module `accept:` mapping.

### .NET HTTP client

For a .NET consumer, the [`MTConnectHttpClient`](/api/MTConnect.Clients.MTConnectHttpClient) family handles polling, long-polling, sequence-cursor management, and codec dispatch:

```csharp
using MTConnect.Clients;

var client = new MTConnectHttpClient("http://agent.local:5000");
client.CurrentReceived += (sender, doc) =>
{
    foreach (var device in doc.Streams)
    foreach (var component in device.ComponentStreams)
    foreach (var observation in component.Observations)
        Console.WriteLine($"{component.Name}.{observation.DataItemId} = {observation.Result}");
};

client.Start();
```

The client handles reconnects on the consumer's behalf; the `ConnectionError` event fires on transport failures and the client backs off and retries. Parsing or dispatch failures surface through `InternalError`.

## MQTT — the cppagent-parity broker / relay tree

When the agent runs an `mqtt-broker` module or relays through `mqtt-relay`, every observation is published to an MQTT topic in the [cppagent JSON v2 MQTT](/wire-formats/json-v2-cppagent-mqtt) shape. Two topic layouts are published in parallel: the document-server layout (whole-envelope publishes keyed by device) from [`MTConnect.MTConnectMqttDocumentServer`](/api/MTConnect.md), and the entity-server layout (per-data-item, per-asset publishes) from [`MTConnect.MTConnectMqttEntityServer`](/api/MTConnect.md).

Document-server topics (envelope payloads, one publish per device):

| Topic | Payload | Retained |
|---|---|---|
| `MTConnect/Probe/<deviceUuid>` | `<MTConnectDevices>` envelope for one device | Yes — late subscribers immediately receive the device model. |
| `MTConnect/Current/<deviceUuid>` | `<MTConnectStreams>` snapshot for one device | Yes — late subscribers receive the most-recent state. |
| `MTConnect/Sample/<deviceUuid>` | `<MTConnectStreams>` delta since the last publish | No — pure event stream. |
| `MTConnect/Asset/<deviceUuid>/<assetId>` | Asset payload for one asset | Yes — late subscribers receive every active asset. |

Entity-server topics (per-data-item, per-asset payloads):

| Topic | Payload |
|---|---|
| `MTConnect/Devices/<deviceUuid>/Device` | Single-device document payload. |
| `MTConnect/Devices/<deviceUuid>/Observations/<dataItemId>` | One observation in JSON v2 shape. |
| `MTConnect/Devices/<deviceUuid>/Assets/<assetId>` | One asset in JSON v2 shape. |

The `<prefix>/` segment (`MTConnect/` above) is configurable via the `mqtt-broker` / `mqtt-relay` `topicPrefix:` key.

### Subscribe with mosquitto_sub

```sh
mosquitto_sub -h broker.local -t 'MTConnect/#' -v
```

Every message the agent publishes flows through. For a single device's per-data-item stream:

```sh
mosquitto_sub -h broker.local -t 'MTConnect/Devices/Mill-1/Observations/#' -v
```

### .NET MQTT consumer

The [`MTConnectMqttClient`](/api/MTConnect.Clients.MTConnectMqttClient) class is the consumer-side counterpart of the broker module. It subscribes to the topic tree, parses each message through the JSON v2 codec, and surfaces typed observations via events:

```csharp
using MTConnect.Clients;

var client = new MTConnectMqttClient("broker.local", 1883);
client.ObservationReceived += (sender, observation) =>
{
    Console.WriteLine($"{observation.DeviceUuid}/{observation.DataItemId} = {observation.Result}");
};

client.Start();
```

See [Cookbook: Write a JSON-MQTT consumer](/cookbook/write-a-json-mqtt-consumer) for an end-to-end consumer including reconnect handling.

### Python MQTT consumer

For a non-.NET consumer, the JSON v2 payload is plain JSON that parses with any standard library:

```python
import json
import paho.mqtt.client as mqtt

def on_message(client, userdata, msg):
    payload = json.loads(msg.payload)
    # MTConnect/Devices/+/Observations/<dataItemId> delivers one observation per message.
    # The topic tail names the data-item; the payload carries the JSON v2 observation body.
    data_item_id = msg.topic.rsplit('/', 1)[-1]
    print(data_item_id, payload)

client = mqtt.Client()
client.on_message = on_message
client.connect("broker.local", 1883)
client.subscribe("MTConnect/Devices/+/Observations/#")
client.loop_forever()
```

The [JSON v2 cppagent MQTT page](/wire-formats/json-v2-cppagent-mqtt) documents the exact payload shape for each topic.

## Picking polling vs streaming vs MQTT

The three patterns trade off in predictable ways:

- **Polling `/current` on a timer**: simplest. Loses observations between polls. Use when a snapshot every N seconds is enough.
- **Long-poll `/sample`**: lossless across polls (the `nextSequence` cursor advances through every observation in the agent's buffer). Holds one TCP connection per consumer; good for one or two consumers, costly for many.
- **MQTT subscription**: lossless and fan-out-friendly. The broker handles the connection multiplexing. Adds an MQTT broker to the deployment. Use when several consumers need the same stream, or when the consumer is firewall-restricted from reaching the agent directly.

## Spec-source-of-record

The HTTP REST protocol is documented at [`Part_1.0` REST API](https://docs.mtconnect.org/) (the MTConnect Standard's normative prose). The library's compliance posture is tracked under [Compliance: Wire format](/compliance/wire-format). The JSON v2 MQTT shape is the cppagent reference implementation's broker output — see the [Compliance: Known divergences](/compliance/known-divergences) page for current parity gaps.

## See also

- [Configure an agent](./agent-config) — the agent's HTTP / MQTT module configuration.
- [Run](./run) — starting the agent the consumer connects to.
- [Operate](./operate) — observability and operational signals on the agent side.
- [Cookbook: Write a JSON-MQTT consumer](/cookbook/write-a-json-mqtt-consumer) — end-to-end MQTT consumer walkthrough.
- [Wire formats: XML](/wire-formats/xml) — the canonical wire format.
- [Wire formats: JSON-CPPAGENT (v2)](/wire-formats/json-v2-cppagent) — the JSON v2 wire format.
- [Wire formats: JSON-CPPAGENT-MQTT](/wire-formats/json-v2-cppagent-mqtt) — the MQTT topic + payload shape.
