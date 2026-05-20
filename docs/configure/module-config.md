# Configure modules

Modules are the agent's pluggable extension points. Each module attaches a capability — HTTP serving, MQTT publishing, SHDR ingestion — and a single agent typically runs several modules at once. This page documents every shipped module's configuration keys.

The module set:

- [HTTP server](#http-server) — `/probe`, `/current`, `/sample`, `/asset` endpoints.
- [MQTT broker](#mqtt-broker) — embedded MQTT broker.
- [MQTT relay](#mqtt-relay) — outbound publisher to an external MQTT broker.
- [MQTT adapter](#mqtt-adapter) — inbound consumer from an external MQTT broker.
- [SHDR adapter](#shdr-adapter) — inbound TCP-SHDR adapter.
- [HTTP adapter](#http-adapter) — inbound HTTP-poll adapter.

Every module is declared in `agent.config.yaml` under `modules:` as a single-key map. The key is the module's name; the value is the module's config object.

## HTTP server

Hosts the MTConnect endpoints: `/probe`, `/current`, `/sample`, `/asset`, plus optional file-serving for stylesheets and schemas. Module key: `http-server`. Implementation: `MTConnect.NET-AgentModule-HttpServer`.

### Keys

```yaml
- http-server:
    hostname: 0.0.0.0          # interface to bind. Default: all interfaces.
    port: 5000                 # listen port. Default: 5000.
    allowPut: false            # whether /put is allowed for ingest. Default: false.
    indentOutput: true         # pretty-print responses. Default: false.
    documentFormat: xml        # default response format. xml | json | json-cppAgent
    accept:                    # MIME-to-format dispatch.
      text/xml: xml
      application/json: json
      application/mtconnect+json: json-cppAgent
    responseCompression:       # which compression encodings to advertise.
    - gzip
    - br
    files:                     # static-file mounts under /styles, /schemas, etc.
    - path: schemas
      location: schemas
    - path: styles
      location: styles
    tls:                       # optional TLS settings.
      pem:
        certificateAuthority: /path/to/rootCA.crt
        certificatePath: /path/to/server.crt
        privateKeyPath: /path/to/server.key
        privateKeyPassword: <secret>
```

Multiple `http-server` instances are supported and common — one on port `5000` plain, another on port `5001` with TLS, and a third on `5002` with `allowPut: true` for an internal ingest endpoint.

### `tls`

The `tls:` block configures TLS termination. Two shapes are supported:

- `pem:` — PEM-encoded certificate chain and private key.
- `pfx:` — PKCS#12 bundle.

When TLS is enabled, the HTTP server accepts only HTTPS connections. The `tlsConfiguration` class is shared with the MQTT modules — see [`MTConnect.Tls.TlsConfiguration`](/api/MTConnect.Tls/TlsConfiguration).

## MQTT broker

Hosts an embedded MQTT broker that publishes one topic per Device / DataItem, formatted as JSON-CPPAGENT-MQTT. Consumers connect with any MQTT client. Module key: `mqtt-broker`. Implementation: `MTConnect.NET-AgentModule-MqttBroker`.

```yaml
- mqtt-broker:
    port: 1883
    currentInterval: 10000
    sampleInterval: 500
    documentFormat: JSON-CPPAGENT
    topicPrefix: MTConnect/Document
    topicStructure: Document    # Document | Entity
    tls:
      pem:
        certificateAuthority: /path/to/rootCA.crt
        certificatePath: /path/to/server.crt
        privateKeyPath: /path/to/server.key
        privateKeyPassword: <secret>
```

### Topic structures

- **`Document`** — one topic per response document. Topics are `<prefix>/Probe/<deviceUuid>`, `<prefix>/Current/<deviceUuid>`, `<prefix>/Sample/<deviceUuid>`, `<prefix>/Asset/<deviceUuid>/<assetId>`. The payload is the full envelope.
- **`Entity`** — one topic per Observation / Asset / Device. Topics are `<prefix>/Devices/<deviceUuid>`, `<prefix>/Observations/<deviceUuid>/<dataItemId>`, `<prefix>/Assets/<deviceUuid>/<assetId>`. The payload is a single entity.

The `Document` structure is the cppagent-MQTT parity shape; the `Entity` structure is the library's per-observation alternative for consumers that prefer one-topic-per-observation.

### Intervals

- **`currentInterval`** — milliseconds between successive `Current` publishes. Default: `10000` (10 s).
- **`sampleInterval`** — milliseconds between successive `Sample` publishes. Default: `500`.

## MQTT relay

Publishes outbound MQTT messages to an external broker. The agent acts as a client. Module key: `mqtt-relay`. Implementation: `MTConnect.NET-AgentModule-MqttRelay`. The configuration object is [`MqttRelayModuleConfiguration`](/api/MTConnect.Configurations/MqttRelayModuleConfiguration).

```yaml
- mqtt-relay:
    server: broker.example.com   # MQTT broker hostname.
    port: 1883                   # broker port.
    timeout: 5000                # connect / I/O timeout (ms).
    reconnectInterval: 10000     # delay between reconnect attempts (ms).

    username: agent-01           # optional broker auth.
    password: <secret>
    clientId: agent-01           # MQTT client id.
    cleanSession: true           # MQTT clean-session flag.
    qos: 1                       # 0 | 1 | 2.

    useTls: false                # whether to use TLS.
    tls:
      pem:
        certificateAuthority: /path/to/rootCA.crt
        certificatePath: /path/to/agent.crt
        privateKeyPath: /path/to/agent.key
        privateKeyPassword: <secret>

    topicPrefix: MTConnect        # topic prefix.
    topicStructure: Document      # Document | Entity (see MQTT broker).
    documentFormat: JSON-CPPAGENT-MQTT
    indentOutput: false

    currentInterval: 5000         # millis between Current publishes.
    sampleInterval: 500           # millis between Sample publishes.
    durableRelay: true            # buffer + replay on reconnect.
```

### `durableRelay`

When true, the relay buffers outbound messages while disconnected and replays them on reconnect, preserving ordering. Set false to drop in-flight messages and resume only the live stream.

### Default values

The shipped defaults match what the constructor sets:

| Key | Default |
|---|---|
| `server` | `localhost` |
| `port` | `1883` |
| `timeout` | `5000` |
| `reconnectInterval` | `10000` |
| `topicPrefix` | `MTConnect` |
| `topicStructure` | `Document` |
| `documentFormat` | `json-cppAgent` |
| `currentInterval` | `5000` |
| `sampleInterval` | `500` |
| `durableRelay` | `false` |

## MQTT adapter

Subscribes to an external MQTT broker and ingests Observations from incoming messages. Module key: `mqtt-adapter`. Implementation: `MTConnect.NET-AgentModule-MqttAdapter`.

```yaml
- mqtt-adapter:
    deviceKey: mill-01            # the Device this adapter feeds.
    server: broker.example.com
    port: 1883
    topic: MTConnect/Adapter/mill-01    # subscribe topic (supports + and # wildcards).
    username: agent-01
    password: <secret>
    useTls: false
    tls: { ... }                  # same TlsConfiguration shape as above.
```

The adapter expects each MQTT message to carry a JSON-CPPAGENT or JSON-v1 Observation payload (or an SHDR-formatted text line). The `documentFormat` field auto-detects when omitted.

## SHDR adapter

Connects to an SHDR-output adapter over TCP and ingests SHDR lines. Module key: `shdr-adapter`. Implementation: `MTConnect.NET-AgentModule-ShdrAdapter`.

```yaml
- shdr-adapter:
    deviceKey: mill-01             # the Device this adapter feeds.
    hostname: localhost            # remote SHDR adapter host.
    port: 7878                     # remote SHDR adapter port.
    heartbeat: 1000                # heartbeat interval (ms).
    reconnectInterval: 1000        # delay between reconnect attempts (ms).
    connectionTimeout: 1000        # connection timeout (ms).
```

Multiple instances are typical when more than one upstream adapter feeds the agent — one per Device, each on its own port.

## HTTP adapter

Polls an HTTP endpoint that returns MTConnect-shaped responses. Module key: `http-adapter`. Implementation: `MTConnect.NET-AgentModule-HttpAdapter`.

```yaml
- http-adapter:
    deviceKey: mill-01
    url: http://upstream-agent.example.com:5000/sample
    interval: 1000                 # poll interval (ms).
    documentFormat: xml            # expected response format.
```

The HTTP adapter is most often used to chain agents — a regional aggregator agent polling per-cell agents — but works against any HTTP endpoint that emits the right envelope.

## Configuration objects

Each module's YAML keys deserialize into a configuration class:

| Module | Configuration class |
|---|---|
| `http-server` | [`MTConnect.Configurations.HttpServerModuleConfiguration`](/api/MTConnect.Configurations/HttpServerModuleConfiguration) |
| `mqtt-broker` | [`MTConnect.Configurations.MqttBrokerModuleConfiguration`](/api/MTConnect.Configurations/MqttBrokerModuleConfiguration) |
| `mqtt-relay` | [`MTConnect.Configurations.MqttRelayModuleConfiguration`](/api/MTConnect.Configurations/MqttRelayModuleConfiguration) |
| `mqtt-adapter` | [`MTConnect.Configurations.MqttAdapterModuleConfiguration`](/api/MTConnect.Configurations/MqttAdapterModuleConfiguration) |
| `shdr-adapter` | [`MTConnect.Configurations.ShdrAdapterModuleConfiguration`](/api/MTConnect.Configurations/ShdrAdapterModuleConfiguration) |
| `http-adapter` | [`MTConnect.Configurations.HttpAdapterModuleConfiguration`](/api/MTConnect.Configurations/HttpAdapterModuleConfiguration) |

Programmatic configuration uses the class directly:

```csharp
using MTConnect.Configurations;

var relay = new MqttRelayModuleConfiguration
{
    Server = "broker.example.com",
    Port = 8883,
    UseTls = true,
    DocumentFormat = "JSON-CPPAGENT-MQTT",
    TopicPrefix = "MTConnect/Document",
    CurrentInterval = 5000,
    SampleInterval = 500,
    DurableRelay = true,
};
```

## Where to next

- [Configure an agent](/configure/agent-config) — the agent-wide settings the modules live under.
- [Cookbook: Configure MQTT relay](/cookbook/configure-mqtt-relay) — a full MQTT-relay walk-through.
- [Cookbook: Write a JSON-MQTT consumer](/cookbook/write-a-json-mqtt-consumer) — the consumer side.
- [Troubleshooting: MQTT TLS handshake](/troubleshooting/mqtt-tls-handshake) — when TLS connections fail.
