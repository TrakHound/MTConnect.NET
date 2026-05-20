# SHDR

SHDR — Simple Hierarchical Data Representation — is the line-oriented adapter protocol that feeds the MTConnect agent from upstream equipment (PLCs, machine controllers, sensors). It is the only wire format on this page that flows *into* the agent rather than out of it; the agent surfaces the resulting observations through the HTTP wire formats (XML, JSON-CPPAGENT v2). The protocol is line-delimited ASCII over a TCP socket: one observation (or one observation batch) per newline-terminated line.

The MTConnect Standard documents SHDR as the canonical adapter protocol in [Part 7.0 — Protocol Information Model](https://docs.mtconnect.org/). The library's [`MTConnect.NET-SHDR`](https://github.com/TrakHound/MTConnect.NET/blob/master/libraries/MTConnect.NET-SHDR/README.md) package implements both sides — adapter (TCP server that publishes lines) and agent ingress client (TCP client that consumes lines).

## Line shape

Every SHDR line carries an optional ISO 8601 UTC timestamp followed by pipe-delimited fields:

```
<timestamp>|<key>|<value>[|<key>|<value>…]
```

- **Timestamp** — optional. When the adapter omits it, the agent stamps the line with its receive time.
- **Key** — the DataItem ID (or the device-scoped `<device>:<dataItemId>` form when the adapter speaks for multiple devices on the same socket).
- **Value** — the observation value, encoded per the representation kind (`VALUE`, `DATA_SET`, `TABLE`, `TIME_SERIES`, `CONDITION`).

The protocol layers four control idioms on top of that base:

- **Asset push** — `<timestamp>|@ASSET@|<assetId>|<assetType>|<payload>` (with `--multiline--<token>` framing when the asset is multi-line XML).
- **Asset removal** — `<timestamp>|@REMOVE_ASSET@|<assetId>` and `<timestamp>|@REMOVE_ALL_ASSETS@|<assetType>`.
- **Device push** — `<timestamp>|@DEVICE@|<payload>` for adapters that ship Devices.xml fragments inline.
- **Commands** — `* <command>: <value>` (e.g. `* deviceModel: <xml>`, `* heartbeat: 10000`) for connection-level negotiation.

## Codec classes

| Class | Role |
|---|---|
| [`MTConnect.Shdr.ShdrLine`](/api/mtconnect-shdr/ShdrLine) | Low-level line tokenizer. Splits a raw byte sequence on `\n`, separates the optional timestamp, and yields field tokens. |
| [`MTConnect.Shdr.ShdrDataItem`](/api/mtconnect-shdr/ShdrDataItem) | Round-trip codec for the `VALUE` representation (Events + Samples). `ToString()` emits SHDR; `FromString()` parses. |
| [`MTConnect.Shdr.ShdrCondition`](/api/mtconnect-shdr/ShdrCondition) | Codec for Conditions. Each fault state serializes as `<level>\|<nativeCode>\|<nativeSeverity>\|<qualifier>\|<message>`. |
| [`MTConnect.Shdr.ShdrTimeSeries`](/api/mtconnect-shdr/ShdrTimeSeries) | Codec for the `TIME_SERIES` representation. Emits `<sampleCount>\|<sampleRate>\|<v1> <v2> … <vN>`. |
| [`MTConnect.Shdr.ShdrDataSet`](/api/mtconnect-shdr/ShdrDataSet) | Codec for the `DATA_SET` representation. Emits space-separated `<key>=<value>` entries. |
| [`MTConnect.Shdr.ShdrTable`](/api/mtconnect-shdr/ShdrTable) | Codec for the `TABLE` representation. Emits space-separated `<key>={<cellKey>=<cellValue>…}` entries. |
| [`MTConnect.Shdr.ShdrAsset`](/api/mtconnect-shdr/ShdrAsset) | Codec for `@ASSET@` push lines, including the `--multiline--<token>` envelope for XML assets. |
| [`MTConnect.Shdr.ShdrMessage`](/api/mtconnect-shdr/ShdrMessage) | Codec for `MESSAGE`-representation Events (which carry a native code alongside the value). |
| [`MTConnect.Shdr.ShdrFaultState`](/api/mtconnect-shdr/ShdrFaultState) | Per-fault DTO used by `ShdrCondition`. |
| [`MTConnect.Adapters.Shdr.ShdrAdapter`](/api/mtconnect-adapters-shdr/ShdrAdapter) | Adapter-side TCP server that serializes queued observations and publishes lines on connect / on change. |
| [`MTConnect.Adapters.Shdr.ShdrIntervalAdapter`](/api/mtconnect-adapters-shdr/ShdrIntervalAdapter) | Adapter variant that flushes the most-recent value of each DataItem on a fixed interval. |
| [`MTConnect.Adapters.Shdr.ShdrQueueAdapter`](/api/mtconnect-adapters-shdr/ShdrQueueAdapter) | Adapter variant that publishes every queued observation on `SendBuffer()`. |
| [`MTConnect.Adapters.Shdr.ShdrIntervalQueueAdapter`](/api/mtconnect-adapters-shdr/ShdrIntervalQueueAdapter) | Adapter variant that flushes the full queue on a fixed interval. |
| [`MTConnect.Shdr.ShdrClient`](/api/mtconnect-shdr/ShdrClient) | Agent-side TCP client that reads from an `ShdrAdapter`, parses each line, and pushes observations into an `IMTConnectAgent`. |

## Sample lines

A simple Event observation, with the adapter emitting timestamps:

```
2023-01-26T16:48:17.0206852Z|L2estop|ARMED
```

A device-scoped observation (multi-device adapter on a single socket):

```
2023-01-26T20:54:34.1694626Z|OKUMA-Lathe:L2estop|ARMED
```

A batch of observations on one line (separator between key/value pairs is `|`):

```
2023-01-26T20:50:53.6161001Z|L2p1execution|READY|L2p1Fovr|100|L2p1partcount|15|L2p1Fact|250
```

A Condition with native diagnostics:

```
2022-02-01T13:55:11.8460000Z|L2p1system|FAULT|404|100|LOW|Testing from new adapter
```

A `TIME_SERIES` sample, six readings at 100 Hz:

```
2023-01-26T20:39:28.1540686Z|L2p1Sensor|6|100|12 15 14 18 25 30
```

A `DATA_SET` of named scalars:

```
2023-01-26T20:40:30.6718334Z|L2p1Variables|V1=5 V2=205
```

A `TABLE` of tool measurements:

```
2023-01-26T20:40:55.8702675Z|L2p1ToolTable|T1={LENGTH=7.123 DIAMETER=0.494 TOOL_LIFE=0.35} T2={LENGTH=10.456 DIAMETER=0.125 TOOL_LIFE=1}
```

A multi-line CuttingTool asset push:

```
2023-01-26T17:56:59.9694353Z|@ASSET@|5.12|CuttingTool|--multiline--W5XZBJ2QZV
<CuttingTool assetId="5.12" timestamp="2023-01-26T12:56:59.4778578-05:00" toolId="12">
  <CuttingToolLifeCycle>
    <Location type="SPINDLE">0</Location>
    <Measurements>
      <FunctionalLength units="MILLIMETER" code="LF">7.6543</FunctionalLength>
    </Measurements>
  </CuttingToolLifeCycle>
</CuttingTool>
--multiline--W5XZBJ2QZV
```

An asset removal:

```
2023-01-26T18:21:57.8208518Z|@REMOVE_ASSET@|file.test
```

Every fixture above is reproduced in [`libraries/MTConnect.NET-SHDR/README.md`](https://github.com/TrakHound/MTConnect.NET/blob/master/libraries/MTConnect.NET-SHDR/README.md) with the codec call that produced it.

## Spec-version compatibility

SHDR's line shape is stable across the MTConnect Standard's lifetime — the protocol was specified before the MTConnect v2.x type-system additions and has not gained new line forms. The only version-driven evolution is in the value space: new representations (`DATA_SET` in v1.4, `TABLE` in v1.5, `TIME_SERIES` carrying ≥ 1 sample) extended what the existing line shapes can carry without changing the framing.

| Spec version | SHDR coverage in this library |
|---|---|
| v1.0 – v1.3 | `VALUE` representation (Events + Samples), Conditions, asset push, asset removal. |
| v1.4 | Adds `DATA_SET` representation. |
| v1.5 | Adds `TABLE` representation. |
| v1.6 – v2.5 | No new SHDR line forms; observation values follow the typed-DataItem rules of the target version. |

For all versions, the canonical authority on SHDR is Part 7.0 of the MTConnect Standard at [docs.mtconnect.org](https://docs.mtconnect.org/). The reference adapter implementations in [`mtconnect/cppagent`](https://github.com/mtconnect/cppagent) under `simulator/` and `lib/` carry working SHDR samples cross-validated against the same protocol.

## Wire-flow sequence

```mermaid
sequenceDiagram
  participant PLC as PLC / equipment reader
  participant Adapter as ShdrAdapter (TCP server)
  participant Client as ShdrClient (TCP client, agent side)
  participant Agent as IMTConnectAgent

  PLC->>Adapter: Push DataItem value (in-process API)
  Note over Adapter: Queue observation; format via ShdrDataItem.ToString()
  Client->>Adapter: TCP connect (heartbeat negotiation)
  Adapter-->>Client: "* heartbeat: <ms>"
  Client-->>Adapter: "* PING"
  Adapter-->>Client: "* PONG <ms>"
  loop Per observation
    Adapter-->>Client: <timestamp>|<key>|<value>\n
    Client->>Client: ShdrLine.FromString → ShdrDataItem / ShdrCondition / …
    Client->>Agent: AddObservation(deviceUuid, dataItemId, value, timestamp)
  end
  PLC->>Adapter: Push asset (CuttingTool / File / …)
  Adapter-->>Client: <timestamp>|@ASSET@|<id>|<type>|--multiline--<token>\n<xml>\n--multiline--<token>\n
  Client->>Client: ShdrAsset.FromString → IAsset
  Client->>Agent: AddAsset(asset)
```

The heartbeat exchange (`* heartbeat`, `* PING`, `* PONG`) is the protocol's connection-keepalive contract; the agent disconnects an adapter that stops PONGing within twice the negotiated heartbeat. The library's [`ShdrClient`](/api/mtconnect-shdr/ShdrClient) handles the keepalive on the agent side; the [`ShdrAdapter`](/api/mtconnect-adapters-shdr/ShdrAdapter) family handles it on the equipment side.

## Caveats and known divergences

- **Timestamps are optional but recommended.** When the adapter omits a leading timestamp, the agent stamps the observation at its receive time — which may be milliseconds-to-seconds later than the actual physical event, depending on adapter throughput and TCP buffering. The `ShdrAdapter.OutputTimestamps` knob (default `true`) controls this.
- **Pipe is reserved.** Values that include `|` must be escaped or the line will misparse. The library's `ShdrDataItem.ToString()` does not currently escape; producers passing pipe-bearing strings must encode them out-of-band (URL-encoding is the cppagent convention).
- **Newline is reserved.** SHDR is strictly line-delimited; multi-line values use the `--multiline--<token>` framing exclusively for assets. Embedding a literal `\n` in a non-asset value will desynchronize the parser; producers must encode or strip them.
- **`@ASSET@` payloads are XML even on JSON-CPPAGENT deployments.** The asset push payload is an XML fragment regardless of which HTTP wire format the agent serves. The agent re-serializes into JSON-CPPAGENT v2 (or XML, or both) on the egress side.
- **Single-line vs multi-line assets is adapter-configured.** Setting `MultilineAssets = false` on the adapter inlines short asset XML on the same line; setting it to `true` (the default) uses the `--multiline--<token>` framing. The token is randomly generated per asset and must not appear inside the asset XML; the [`ShdrAsset`](/api/mtconnect-shdr/ShdrAsset) codec regenerates the token until it is unique against the payload.
- **`@REMOVE_ALL_ASSETS@` targets a type, not a device.** The line `<timestamp>\|@REMOVE_ALL_ASSETS@\|File` removes every File asset on the device the adapter speaks for. There is no device-spanning variant; a multi-device adapter sends one removal per device-scoped key.
- **Reconnect drops in-flight buffer.** When the TCP connection drops mid-stream, observations queued in the adapter's send buffer are lost unless the adapter is an [`ShdrQueueAdapter`](/api/mtconnect-adapters-shdr/ShdrQueueAdapter) variant. The agent does not back-fill; consumers who require zero loss should use the queue-adapter family or a transport with persistent delivery (e.g. the [MqttAdapter module](/modules/mqtt-adapter)).
- **No native TLS.** SHDR is plaintext TCP. Deployments that require encryption tunnel through SSH, stunnel, a service mesh, or a TLS-terminating proxy. The library does not negotiate TLS on the SHDR socket directly.
- **Adapter-side device push (`@DEVICE@`) is opt-in.** Most adapters keep Devices.xml on the agent side and let the agent's probe response describe the model. Adapters that ship the device model inline use `@DEVICE@`; consumers should confirm the deployment's source-of-truth before assuming either path.

## See also

- [`MTConnect.NET-SHDR` library README](https://github.com/TrakHound/MTConnect.NET/blob/master/libraries/MTConnect.NET-SHDR/README.md) — adapter setup, configuration knobs, and per-representation usage examples.
- [XML](./xml), [JSON-CPPAGENT v2](./json-v2-cppagent), [JSON-CPPAGENT-MQTT](./json-v2-cppagent-mqtt) — the egress wire formats the agent surfaces once SHDR observations land in its buffer.
- [Configure an adapter](/configure/) — operator-side guidance on bringing up an adapter against a PLC or equipment reader.
- [cppagent SHDR simulator](https://github.com/mtconnect/cppagent/tree/main/simulator) — reference adapter the library's SHDR codec cross-validates against.
