# Migrate between MTConnect versions

`MTConnect.NET` supports every published MTConnect spec version from v1.0 through v2.7. A consumer or agent moving between versions encounters changes in the wire shape, the type system, and the available DataItems / Assets. This recipe is the migration playbook — what to expect at each version step, what code-level changes follow, and how to keep two ends in sync during the transition.

## When to migrate

Three triggers for a version bump:

1. **A spec-version-introduced type is needed** — the consumer wants `ComponentConfigurationParameters` Assets (v2.2+), rich Pallet measurements (v2.4+), or the `Quality` Observation field (v2.3+).
2. **A consumer requires a specific version** — a downstream historian validates against v2.5 XSDs and rejects v2.0 envelopes.
3. **A regulatory or organizational mandate** — the customer's compliance posture pins them to v2.x.

The wire format does not change in a way that forces a version bump on its own; consumers reading XML against the schema's namespace already pick up the right schema URL.

## Migration matrix

The high-frequency-of-impact version transitions:

| From | To | Code-level impact | Wire-level impact |
|---|---|---|---|
| v1.6 → v1.7 | low | `Agent` Device introduced (auto-registered); `MTConnectVersion` attribute on Device | new top-level `Agent` element in `<Devices>` |
| v1.8 → v2.0 | low | `coordinateSystem` enum deprecated in favor of `coordinateSystemIdRef` | namespace change to `urn:mtconnect.org:MTConnect<X>:2.0` |
| v2.1 → v2.2 | medium | `Hash` attribute on `Header` and `Device`; `ComponentConfigurationParameters` Asset | new `hash` attribute on every Device |
| v2.2 → v2.3 | low | `Quality` first-class on Observations | new `quality` attribute on Observations |
| v2.3 → v2.4 | medium | rich-template Pallet measurements (typed classes replace free-form `Measurement`) | new Measurement element names in `MTConnectAssets` |
| v2.4 → v2.5 | low | spec-internal cleanup; no new wire-format-visible types | no new attributes |
| v2.5 → v2.6 | low | v2.6 XMI is byte-identical to v2.7 in the published spec | no wire-format change |
| v2.6 → v2.7 | low | spec cleanup; minor attribute additions | no breaking wire-format changes |

The "impact" column is the typical code-side surface area for a consumer / agent author. None of the transitions above are forced-rewrite; the library handles each through version-aware serialization.

## Step 1: pin the target

Set the agent's `defaultVersion` to the target:

```yaml
defaultVersion: 2.5
```

Or pin in code:

```csharp
using MTConnect;

var version = MTConnectVersions.Version25;
var device = Device.Process(myDevice, version);
```

`Device.Process(IDevice, Version)` is the version-aware serializer. It prunes properties that did not exist at `version`, strips Components and DataItems whose `MinimumVersion` is above `version`, and emits the rest. The same call shapes JSON-cppagent serialization through `JsonCppAgentFormatter`.

## Step 2: validate `Devices.xml` against the new XSD

Update the XML namespace and `schemaLocation`:

```xml
<MTConnectDevices xmlns="urn:mtconnect.org:MTConnectDevices:2.5"
                  xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
                  xsi:schemaLocation="urn:mtconnect.org:MTConnectDevices:2.5
                                      https://schemas.mtconnect.org/schemas/MTConnectDevices_2.5.xsd">
  ...
</MTConnectDevices>
```

Validate locally:

```sh
xmllint --noout --schema https://schemas.mtconnect.org/schemas/MTConnectDevices_2.5.xsd Devices.xml
```

The agent validates on startup; running the validator separately is faster when iterating on the model.

## Step 3: update consumer expectations

Consumers reading the agent's output need to handle the new attributes / elements without breaking. Specifically:

- **v2.2 `Hash` attribute** — consumers that ignored unknown attributes already handle this; consumers that did strict-schema validation pick up the new value automatically through the schema bump.
- **v2.3 `Quality` field** — consumers reading observations should default `Quality` to `VALID` when absent.
- **v2.4 Pallet measurements** — consumers parsing Asset measurements should accept the new typed element names (`HeightMeasurement`, `WidthMeasurement`, `LoadedHeightMeasurement`, etc.) in addition to the free-form `Measurement` element. The library handles both.

## Step 4: code-side updates

Where the consumer is using `MTConnect.NET` directly, the migration is a constant change:

```diff
- var version = MTConnectVersions.Version23;
+ var version = MTConnectVersions.Version25;
```

That single change re-shapes every serialization on the next request. No other code change is required for a non-breaking bump.

For breaking transitions (v1.8 → v2.0 around `coordinateSystem`, v2.3 → v2.4 around Pallet measurements), the library accepts the old shape on input and serializes the new shape on output — so reading a v1.8 `Devices.xml` and serving it as v2.0 works without rewriting the input file. To migrate the input file's authoring style:

```diff
- <DataItem id="x-pos" type="POSITION" coordinateSystem="MACHINE"/>
+ <DataItem id="x-pos" type="POSITION" coordinateSystemIdRef="machine-cs"/>
```

with a `CoordinateSystem` element declared under the Component's `Configuration`.

## Step 5: dual-version operation during the transition

A frequent migration pattern: run two agents at once, one on the old version and one on the new, with both fed by the same adapter. Consumers pinned to the old spec read from agent A; consumers pinned to the new spec read from agent B. The same SHDR adapter feeds both via separate `shdr-adapter` module instances on each agent.

Simplification once the consumer base is fully migrated: shut down agent A and keep agent B.

A single-agent dual-version pattern works too, since the agent serializes to the requested version per request:

```sh
# Old consumer hits the agent with ?version=1.8
curl 'http://agent:5000/current?version=1.8'

# New consumer hits the agent with ?version=2.5
curl 'http://agent:5000/current?version=2.5'
```

The agent prunes properties and elements per request; no agent-side switching is required.

## Step 6: validate after migration

A regression checklist:

- [ ] `curl 'http://agent:5000/probe?version=<new>'` returns a `Devices` envelope whose namespace matches `<new>`.
- [ ] `xmllint --noout --schema <newXsd>` against a captured `/probe` response succeeds.
- [ ] `Header.schemaVersion` matches the new version on every endpoint.
- [ ] Consumer logs show no parse errors for new-version-introduced attributes.
- [ ] If using MQTT relay, subscribers receive payloads that parse cleanly.

## Common migration pitfalls

### A v2.0+ Devices.xml served to a v1.x-pinned consumer

The agent prunes v2.0-introduced properties when serving v1.x, so this works. Where it fails: when the v2.0-only DataItems are referenced by ID elsewhere in the model (e.g. a `LIMIT` `DataItemRelationship` pointing at a v2.0-only DataItem), the v1.x output ends up with a dangling `idRef`. The fix: author Relationships only against DataItems present at the lowest version your consumers query.

### Pallet measurements still showing as free-form on v2.4

If your `Devices.xml` was authored at v2.3 with `<Measurement code="HEIGHT" ...>`, the library accepts it but does not auto-promote it to the v2.4 typed `HeightMeasurement` class. To migrate the authoring style, rewrite the `Devices.xml` to use the typed element names. The library serializes both shapes; choose the v2.4 typed shape for new models.

### Schema version mismatch at startup

The agent rejects a `Devices.xml` whose namespace declares a version higher than the agent's `MTConnectVersions.Max`. Bump the library before bumping the `Devices.xml` namespace; do not bump the `Devices.xml` to v3.0 if the library shipped only through v2.7. See [Troubleshooting: Schema version mismatches](/troubleshooting/schema-version-mismatches).

## Where to next

- [Compliance: Per-version matrix](/compliance/per-version-matrix) — the full feature inventory per version.
- [Configure an agent](/configure/agent-config) — where `defaultVersion` lives.
- [Troubleshooting: Schema version mismatches](/troubleshooting/schema-version-mismatches) — the diagnostic flow when the agent and consumer disagree.
- [Troubleshooting: XSD validation failures](/troubleshooting/xsd-validation-failures) — when the new `Devices.xml` fails validation.
