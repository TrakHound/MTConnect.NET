# Per-version compliance matrix

`MTConnect.NET` is built against the MTConnect Standard from v1.0 through v2.7. Every shipped envelope — Devices, Streams, Assets, Error — validates against the corresponding XSD at every version listed below. Every spec-defined type that appears at a given version is present in the library at that version, gated by the type's `MinimumVersion` / `MaximumVersion` metadata.

## Spec versions supported

The library tracks every published MTConnect spec version. The version constants live in [`MTConnect.MTConnectVersions`](/api/MTConnect/MTConnectVersions), and `MTConnectVersions.Max` advertises the latest one the library can serialize for.

| MTConnect version | Released by MTConnect | Library constant | Streams XSD | Devices XSD | Assets XSD | Error XSD |
|---|---|---|---|---|---|---|
| v1.0 | 2008 | `MTConnectVersions.Version10` | yes | yes | n/a | yes |
| v1.1 | 2010 | `MTConnectVersions.Version11` | yes | yes | n/a | yes |
| v1.2 | 2012 | `MTConnectVersions.Version12` | yes | yes | yes | yes |
| v1.3 | 2014 | `MTConnectVersions.Version13` | yes | yes | yes | yes |
| v1.4 | 2017 | `MTConnectVersions.Version14` | yes | yes | yes | yes |
| v1.5 | 2018 | `MTConnectVersions.Version15` | yes | yes | yes | yes |
| v1.6 | 2019 | `MTConnectVersions.Version16` | yes | yes | yes | yes |
| v1.7 | 2020 | `MTConnectVersions.Version17` | yes | yes | yes | yes |
| v1.8 | 2020 | `MTConnectVersions.Version18` | yes | yes | yes | yes |
| v2.0 | 2021 | `MTConnectVersions.Version20` | yes | yes | yes | yes |
| v2.1 | 2022 | `MTConnectVersions.Version21` | yes | yes | yes | yes |
| v2.2 | 2023 | `MTConnectVersions.Version22` | yes | yes | yes | yes |
| v2.3 | 2024 | `MTConnectVersions.Version23` | yes | yes | yes | yes |
| v2.4 | 2024 | `MTConnectVersions.Version24` | yes | yes | yes | yes |
| v2.5 | 2025 | `MTConnectVersions.Version25` | yes | yes | yes | yes |
| v2.6 | 2025 | `MTConnectVersions.Version26` | yes | yes | yes | yes |
| v2.7 | 2025 | `MTConnectVersions.Version27` | yes | yes | yes | yes |

Assets are not part of the v1.0 / v1.1 envelope set; the Assets envelope enters at v1.2 per the spec's XSD release history ([schemas.mtconnect.org](https://schemas.mtconnect.org/)).

Note: v1.9 was never released — the MTConnect Standard's version numbering jumped from v1.8 to v2.0. The library matches: no `Version19` constant exists.

## Per-envelope semantics

Each cell in the matrix above means: the agent can be asked to emit the named envelope at the named spec version, and the output validates against the matching `MTConnect<Envelope>_<version>.xsd` schema published by the standard body. The version-aware serializers ([`Device.Process`](/api/MTConnect.Devices/Device#Process), [`DataItem.Process`](/api/MTConnect.Devices/DataItem#Process), [`Composition.Process`](/api/MTConnect.Devices/Composition#Process), and the per-envelope JSON / XML codecs) prune properties, DataItems, Components, and Assets whose `MinimumVersion` is above the target version, and prune any whose `MaximumVersion` is below it.

## Type introduction inventory

A subset of spec-version-introduced types the matrix tracks (the full inventory lives on each type's [API reference page](/api/), generated from the SysML `introducedAtVersion` tag):

- **v1.2**: Assets envelope; `Composition` introduced; `Description.Model` introduced.
- **v1.3**: `References` element on Components; `Configuration` element under Components.
- **v1.4**: `Composition` ships as a first-class element under Components; `Specification` element; `Constraints` on DataItems; `Filter` element on DataItems.
- **v1.5**: `DATA_SET` representation introduced; `DataItemRelationship` introduced; `ComponentRelationship` introduced.
- **v1.6**: `TABLE` representation introduced; `CoordinateSystem` element introduced (deprecating the `coordinateSystem` enum); `Reset` element.
- **v1.7**: Agent self-describing `Agent` Device introduced; `SpecificationRelationship`; `PEER` `ComponentRelationship` type; `MTConnectVersion` attribute on Device.
- **v1.8**: `coordinateSystemIdRef` attribute on DataItem.
- **v2.0**: `DATA_SET` becomes the canonical representation for several types (`PART_COUNT`, `ASSET_COUNT`); MQTT topic templates introduced in the spec's `Part_5` (Network).
- **v2.1**: extended `Quality` enum entries.
- **v2.2**: `Hash` attribute on Header and on Device; `ComponentConfigurationParameters` Asset.
- **v2.3**: `Quality` field on Observation made first-class.
- **v2.4**: Rich-template Pallet measurements (`HeightMeasurement`, `WidthMeasurement`, `LoadedHeightMeasurement`, etc.) replace free-form `Measurement`.
- **v2.5**: spec-internal cleanup; no new wire-format-visible types.
- **v2.6**: per the spec's `Part_3` cleanup; v2.6 SysML XMI is byte-identical to v2.7 per the spec's own publication (see [Compliance: Known divergences](/compliance/known-divergences)).
- **v2.7**: spec cleanup; multiple minor enum and attribute additions.

Every introduction is auditable through the generated `.g.cs` files under `libraries/MTConnect.NET-Common/`, which carry the per-type `MinimumVersion` value sourced from the SysML model's `introducedAtVersion` ([`mtconnect/mtconnect_sysml_model`](https://github.com/mtconnect/mtconnect_sysml_model)).

## Header fields per version

The `Header` element gains attributes over time. The library populates them based on the target version; the [`MTConnectAgentInformation`](/api/MTConnect.Agents/MTConnectAgentInformation) class is the source.

| Attribute | Introduced at | Notes |
|---|---|---|
| `creationTime` | v1.0 | UTC ISO 8601. |
| `sender` | v1.0 | agent hostname or configured value. |
| `instanceId` | v1.0 | agent's `InstanceId`; resets on a buffer wipe. |
| `version` | v1.0 | agent's software version (not the MTConnect spec version). |
| `assetBufferSize` | v1.2 | matches the agent's `assetBufferSize` config. |
| `assetCount` | v1.2 | live count of assets in the buffer. |
| `bufferSize` | v1.0 | matches the agent's `observationBufferSize` config. |
| `nextSequence` | v1.0 | next sequence the agent will assign. |
| `firstSequence` | v1.0 | lowest sequence still in the buffer. |
| `lastSequence` | v1.0 | highest sequence currently in the buffer. |
| `schemaVersion` | v1.4 | the MTConnect spec version of the response (e.g. `"2.5"`). |
| `validation` | v1.7 | true if the agent validates incoming Devices against the XSD. |
| `Hash` | v2.2 | SHA-1 over the model; consumers detect a model change without re-parsing. |
| `deviceModelChangeTime` | v1.7 | timestamp of the last model change. |

Source: `MTConnectStreams_<version>.xsd` element `MTConnectStreamsType/Header` ([schemas.mtconnect.org](https://schemas.mtconnect.org/)); SysML `Header` class ([`mtconnect/mtconnect_sysml_model`](https://github.com/mtconnect/mtconnect_sysml_model)).

## How the agent picks a version

When a consumer queries `/probe`, `/current`, `/sample`, or `/asset`, the agent picks the response version as follows:

1. If the request URL carries a `?version=` parameter, the agent uses that.
2. Otherwise the agent uses the configured `defaultVersion` from `agent.config.yaml`.
3. If neither is set, the agent uses `MTConnectVersions.Max`.

Requesting a version older than `MTConnectVersions.Version10` returns an Error envelope; requesting a version newer than `Max` returns an Error envelope as well.

## Where to next

- [Wire-format compliance](/compliance/wire-format) — which on-the-wire codecs are byte-for-byte cppagent-parity.
- [Spec cross-references](/compliance/spec-cross-references) — how every claim above is grounded in an XMI / XSD / prose citation.
- [Known divergences](/compliance/known-divergences) — where the standard contradicts itself and how the library handles each case.
- [Test harness](/compliance/test-harness) — how to run the compliance tier locally.
