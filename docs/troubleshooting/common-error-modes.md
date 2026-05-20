# Common error modes

A field guide to error scenarios that come up in production deployments of `MTConnect.NET`. Each entry covers the symptom, the cause, and the fix. The four scenarios with dedicated pages — [XSD validation failures](/troubleshooting/xsd-validation-failures), [Schema version mismatches](/troubleshooting/schema-version-mismatches), and [MQTT TLS handshake](/troubleshooting/mqtt-tls-handshake) — are linked from here; this page indexes the rest.

## Empty `name` on Probe DataItems

**Symptom**: `/probe` returns DataItems whose `name` attribute is empty:

```xml
<DataItem id="x-pos-actual" name="" category="SAMPLE" type="POSITION"/>
```

**Cause**: the DataItem was constructed with an explicit empty `Name` value (rather than a null `Name`, which the XML codec would omit). The serializer emits an attribute when the property is non-null, and the empty string passes the non-null check.

**Fix**: leave `Name` null when no name is needed. The library treats null Name as "no name attribute on the wire"; only set Name when the SHDR adapter or another lookup pathway uses it.

```csharp
// Correct
var di = new PositionDataItem("ctrl") { SubType = "ACTUAL" };

// Wrong — emits name="" on the wire.
var di = new PositionDataItem("ctrl") { SubType = "ACTUAL", Name = "" };
```

## ASSET_COUNT emitted as a scalar EVENT instead of a DATA_SET

**Symptom**: a cppagent-compatible consumer expects `representation="DATA_SET"` on the `ASSET_COUNT` DataItem and rejects the response when the library emits it as a scalar.

**Cause**: this is the documented divergence between the spec's normative sources (SysML XMI + XSD) and the cppagent reference. The library follows the normative sources; see [Compliance: Known divergences](/compliance/known-divergences) and [Redmine #3890](https://projects.mtconnect.org/issues/3890).

**Fix**: author the `ASSET_COUNT` DataItem with `representation="DATA_SET"` explicitly:

```xml
<DataItem category="EVENT" id="agent-asset-count"
          type="ASSET_COUNT" representation="DATA_SET"/>
```

The library serializes the explicit representation as authored.

## Pallet measurement constructors not present

**Symptom**: a consumer expects to construct `HeightMeasurement`, `WidthMeasurement`, or `LoadedHeightMeasurement` and gets a "type not found" compile error.

**Cause**: the library version pre-dates the v2.4 rich-template Pallet measurements. Earlier versions used the free-form `Measurement` class for every pallet measurement.

**Fix**: upgrade `MTConnect.NET-Common` to a version that ships the v2.4 typed measurements. Alternatively, construct a free-form `Measurement` with the appropriate `Code`:

```csharp
var m = new Measurement
{
    Code = "HEIGHT",
    Value = 200m,
    Units = "MILLIMETER",
};
```

## Devices.xml validation surprises after a v2.x bump

**Symptom**: a `Devices.xml` that validated under v2.3 fails under v2.5, even though no edits were made to the model itself.

**Cause**: the v2.5 XSD tightened or added constraints (typically around attribute occurrence) that the v2.3 model did not satisfy. The library re-validates on every startup against the namespace declared in the document.

**Fix**: read the validation error (it identifies the failing element), consult the [Per-version compliance matrix](/compliance/per-version-matrix) for the introduction version of any new constraint, and fix the model. See [Troubleshooting: XSD validation failures](/troubleshooting/xsd-validation-failures) for the category-by-category fix flow.

## SHDR adapter disconnects every few minutes

**Symptom**: the agent's `shdr-adapter` module log shows repeated reconnect cycles with `[WRN] SHDR connection lost; reconnecting in 1000ms`.

**Causes**:

1. The adapter's heartbeat interval is shorter than the agent's `heartbeat:` configuration. The agent expects a PING at least every `heartbeat` ms; if the adapter sends them less often, the agent times out.
2. A network device (firewall, load balancer) drops the TCP connection after an idle timeout. Long-running SHDR connections are vulnerable to NAT timeouts on cellular and VPN paths.
3. The adapter process is being recycled (memory pressure, scheduled restart) more often than the reconnect loop accommodates.

**Fix**:

- Match the agent's `heartbeat:` to the adapter's emit interval (e.g. `heartbeat: 2000` if the adapter pings every 2 s).
- Set the keepalive TCP socket option on the adapter side, or shorten the agent's `heartbeat:` so the dead connection is detected and re-established faster than the NAT timeout.
- If the adapter is unstable, run it as a systemd unit with `Restart=always`.

## Agent serves stale observations after a buffer wipe

**Symptom**: the agent's `/current` returns observations whose `Timestamp` is older than the agent's `creationTime`. A consumer reads "stale data" warnings.

**Cause**: a durable-buffer restore loaded historical observations from the on-disk buffer. The observations are not stale per se — they are the most-recent values the agent has — but they look stale to a consumer that compares `Timestamp` against `now`.

**Fix**: this is the intended behavior. To force a fresh start, set `durable: false` in `agent.config.yaml` or delete the `buffer/` directory before restarting.

## MQTT relay's `availability` topic shows UNAVAILABLE even when connected

**Symptom**: a subscriber to `<topicPrefix>/Probe/Availability` reads `UNAVAILABLE` even after the agent has reconnected to the broker.

**Cause**: the broker's retained-message replay sent the consumer the previous LWT (Last Will and Testament) message before the relay's reconnect handler pushed the new `AVAILABLE` value. The retained-message protocol means subscribers see whatever was last published as retained, and the publish of `AVAILABLE` happens slightly after the client connects.

**Fix**: wait for the relay's connect handler to fire; the `AVAILABLE` retained message will replace the stale `UNAVAILABLE`. For consumers that depend on immediate accuracy, subscribe to the topic with `qos: 1` and the broker re-delivers the latest retained value after the agent's connect handler fires.

## HTTP /probe response is 503 or hangs

**Symptom**: a curl against `/probe` hangs or returns `503 Service Unavailable`.

**Causes**:

1. The agent has not yet finished startup — the HTTP server module starts before the device-load step completes, and requests during that window return 503.
2. A long-running compaction or buffer-write operation is blocking the response path.
3. The agent is paused waiting for an adapter connection that never arrives.

**Fix**:

- For startup-window 503s, wait for the agent's log to print the "Devices loaded" line before sending requests.
- For sustained 503s, check the agent's metrics (`/probe?path=//Agent` returns the Agent Device's introspection observations). Adapter health is visible there.

## Observations missing the `sequence` attribute

**Symptom**: an XML observation envelope omits the `sequence` attribute, and a consumer paginating on it fails.

**Cause**: a custom serializer was used that did not write `sequence`. The library always sets `Sequence` on every observation as it enters the buffer.

**Fix**: use the library's `XmlFormatter`, `JsonFormatter`, or `JsonCppAgentFormatter` rather than a hand-rolled serializer. Each shipped formatter writes `sequence` on every observation as required by the XSD.

## JSON-CPPAGENT response missing the array-of-wrappers shape

**Symptom**: a v2-pinned consumer rejects the response because the observations are not wrapped in `{"<type>": {...}}` envelopes.

**Cause**: the response is being serialized through the JSON v1 codec rather than the JSON-CPPAGENT codec. The v1 codec mirrors the XML object structure one-to-one; the v2 codec uses the array-of-wrappers shape cppagent established.

**Fix**: set `documentFormat: "json-cppAgent"` on the HTTP server module, or pass `Accept: application/mtconnect+json` on the request. The HTTP module dispatches by `Accept` header when the configuration's `accept:` map declares it.

## Where to next

- [Troubleshooting: XSD validation failures](/troubleshooting/xsd-validation-failures) — model-validation specifics.
- [Troubleshooting: Schema version mismatches](/troubleshooting/schema-version-mismatches) — version-pin specifics.
- [Troubleshooting: MQTT TLS handshake](/troubleshooting/mqtt-tls-handshake) — TLS specifics.
- [Compliance: Known divergences](/compliance/known-divergences) — where the library and the spec disagree.
- [Configure an agent](/configure/agent-config) — the agent-wide configuration.
